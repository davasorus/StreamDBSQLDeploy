using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Reflection;

#region variables

string url1 = "https://go.microsoft.com/fwlink/?linkid=866658";
string url2 = "https://aka.ms/ssmsfullsetup";
string fileName1 = "SQL2019-SSEI-Expr.exe";
string fileName2 = "SSMS-Setup-ENU.exe";
string fileName3 = "SETUP.EXE";
string machineName = Environment.MachineName;
string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

//creates connection information to create db
SqlConnection myConn = new SqlConnection("server=" + machineName + @"\SQLEXPRESS;database=Master;integrated security=SSPI;");
string str;
str = "CREATE DATABASE STREAMINGDB";

//location of SQl Server 2019 installer
string sqlCmd1 = Path.Combine(@"C:\Temp\sqlsetup\SQLEXPR_2019", fileName3);
string sqlCmd2 = Path.Combine(@"C:\temp\sqlsetup", fileName2);

//config of sql server 2019
string sqlCmdScript1 = @" /Q /ACTION=""Install"" /ROLE=""AllFeatures_WithDefaults"" /IACCEPTROPENLICENSETERMS=""True"" /SUPPRESSPRIVACYSTATEMENTNOTICE=""True"" /ENU=""True""" +
    @" /QUIETSIMPLE=""False"" /USEMICROSOFTUPDATE=""True"" /SUPPRESSPAIDEDITIONNOTICE=""True"" /FEATURES=SQL /INDICATEPROGRESS=""True"" " +
    @" /INSTANCENAME=""SQLEXPRESS"" /INSTALLSHAREDDIR=""C:\Program Files\Microsoft SQL Server"" /INSTANCEID=""SQLEXPRESS"" /FILESTREAMLEVEL=""3"" /ADDCURRENTUSERASSQLADMIN=""True""" +
    @" /BROWSERSVCSTARTUPTYPE=""Automatic"" /IACCEPTSQLSERVERLICENSETERMS=""True"" /SQLTELSVCACCT=""NT AUTHORITY\NETWORK SERVICE"" /SQLTEMPDBLOGFILESIZE=""8"" " +
    @" /SQLSVCACCOUNT=""NT AUTHORITY\NETWORK SERVICE"" /SQLTEMPDBFILECOUNT=""1"" /SQLTEMPDBFILESIZE=""8"" /SQLTEMPDBFILEGROWTH=""64"" /SQLTEMPDBLOGFILEGROWTH=""64"" /FILESTREAMSHARENAME=""FILESTREAMSHARE"" ";

//config of SSMS
string sqlCmdScript2 = @"/install /quiet /restart";

#endregion

#region file related move/download/extracting

//creates directory for sql Server installer
Directory.CreateDirectory(@"C:\temp\sqlsetup");

logEntryWriter("downloading SQl Server 2019");

//downloads SQL Server 2019 for install
using (var client = new WebClient())
{
    client.DownloadFile(url1, fileName1);
    client.DownloadFile(url2, fileName2);
}

logEntryWriter("download of SQl Server 2019 is complete");

//moves sql server 2019 exe to install directory
File.Move(fileName1, Path.Combine(@"C:\temp\sqlsetup\", fileName1), true);
File.Move(fileName2, Path.Combine(@"C:\temp\sqlsetup\", fileName2), true);

logEntryWriter("Extracting install files");

embeddedResourceWork();

Process.Start("CMD.exe", @"/c C:\temp\sqlsetup\SQL2019-SSEI-Expr.exe /ACTION=Download MEDIAPATH=C:\temp\sqlsetup /MEDIATYPE=Core /QUIET");

Thread.Sleep(100000);

Process.Start("CMD.exe", @"/C C:\temp\sqlsetup\SQLEXPR_x64_ENU.exe /q /x:C:\temp\sqlsetup\SQLEXPR_2019");

Thread.Sleep(10000);

logEntryWriter("attempting to install SQl Server 2019");

#endregion

#region install SQL Server 2019 and SSMS

//code that does the installing of SQL Server
Task task3 = Task.Factory.StartNew(() => scriptRun(sqlCmdScript1, sqlCmd1));

//code that does the installing of SSMS
Task task4 = Task.Factory.StartNew(() => scriptRun(sqlCmdScript2, sqlCmd2));

Task.WaitAll(task3, task4);

#endregion

#region sqlcommands

logEntryWriter("install of SQl Server Express 2019 Complete");

logEntryWriter(" account name: " + userName);

logEntryWriter("Attempting to Deploy stream Data Base");

//sql command that creates the data base
SqlCommand myCommand = new SqlCommand(str, myConn);
try
{
    myConn.Open();

    logEntryWriter("Connection open");

    myCommand.ExecuteNonQuery();

    logEntryWriter("DataBase is Created Successfully");
}
catch (System.Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{
    if (myConn.State == ConnectionState.Open)
    {
        myConn.Close();
    }
}

//deployment scripts to create tables and stored proceedures

deploymentScriptRun(@"C:\Temp\sqlsetup\Scripts\CreateUserInformation.sql");
deploymentScriptRun(@"C:\Temp\sqlsetup\Scripts\CreateUserMessage.sql");
deploymentScriptRun(@"C:\Temp\sqlsetup\Scripts\CreateUserNameandMessage.sql");
deploymentScriptRun(@"C:\Temp\sqlsetup\Scripts\CreateInsertUserMessage.sql");
deploymentScriptRun(@"C:\Temp\sqlsetup\Scripts\CreateInsertUserName.sql");
deploymentScriptRun(@"C:\Temp\sqlsetup\Scripts\CreateInsertUserNameandMessage.sql");

#endregion

#region methods

//used to run file with the arguments of command
void scriptRun(string Command, string fileName)
{
    Process process = new Process();
    ProcessStartInfo startInfo = new ProcessStartInfo
    {
        WindowStyle = ProcessWindowStyle.Hidden,
        FileName = fileName,
        Arguments = Command
    };
    process.StartInfo = startInfo;
    process.Start();
    process.WaitForExit();
}

//code that executes sql db deployment sql scripts
void deploymentScriptRun(string fileName)
{
    string sqlConnectionString = @"server=" + machineName + @"\SQLEXPRESS;database=streamingDB;integrated security=SSPI;";

    string script = File.ReadAllText(fileName);

    SqlConnection conn = new SqlConnection(sqlConnectionString);

    conn = new SqlConnection(sqlConnectionString);
    conn.Open();

    try
    {
        SqlCommand cmd = new SqlCommand(script, conn);
        cmd.ExecuteNonQuery();
    }
    catch (Exception ex)
    {
        logEntryWriter(ex.ToString());
    }
    finally
    {
        conn.Close();
    }
}

//code to search through embedded resources and create local directories for local scripts
//   if the resource name contains "Lib" it goes into one location, if not another.
void embeddedResourceWork()
{
    createDir(@"C:\Temp\sqlsetup\Scripts\");

    string strPath = @"C:\Temp\sqlsetup\Scripts\";

    try
    {
        string[] assembly = Assembly.GetExecutingAssembly().GetManifestResourceNames();

        foreach (var assem in assembly)
        {
            string FullPath = Path.Combine(strPath, assem);

            var resourceName = assem;
            writeResourceToFile(resourceName, FullPath);
            scriptLabel(strPath, FullPath);
        }
    }
    catch
    {
        logEntryWriter("Error accessing resources!");
    }
}

//creates directories if needed
void createDir(string folder)
{
    try
    {
        if (Directory.Exists(folder))
        {
            //nothing
        }
        else
        {
            DirectoryInfo di = Directory.CreateDirectory(folder);

            string logEntry = DateTime.Now + " " + folder + " was created.";
            logEntryWriter(logEntry);
        }
    }
    catch (Exception ex)
    {
        string logEntry = DateTime.Now + " " + ex.ToString();

        logEntryWriter(logEntry);
    }
}

//this creates the resources from the application to a specified location
void writeResourceToFile(string resourceName, string fileName)
{
    using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
    {
        using (var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            resource.CopyTo(file);
            file.Close();

            string logEntry = DateTime.Now + " " + file + " successfully extracted.";
            logEntryWriter(logEntry);
        }
    }
}

//this will relabel all files in the found path that match any entry in the array
//  this works by pulling the full path in and separating it out into substrings
//   and then comparing those substrings to the script array.
//then the name of the file is changed to the text of the matching array entry with .ps1 appended at the end.
void scriptLabel(string strPath, string fullPath)
{
    string Script1 = "CreateInsertUserMessage";
    string Script2 = "CreateInsertUserName";
    string Script3 = "CreateInsertUserNameandMessage";
    string Script4 = "CreateUserInformation";
    string Script5 = "CreateUserMessage";
    string Script6 = "CreateUserNameandMessage";

    string s = fullPath;
    char[] separators = new char[] { '_', '\\', '.' };
    string[] subs = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    string[] scriptName = { Script1, Script2, Script3, Script4, Script5, Script6 };
    if (subs.Intersect(scriptName).Any())
    {
        try
        {
            foreach (var sub in subs)
            {
                foreach (var name in scriptName)
                {
                    if (sub.ToString() == name.ToString())
                    {
                        File.Move(fullPath, strPath + @"\" + sub + ".sql");

                        string logEntry1 = DateTime.Now + " " + strPath + @"\" + sub + " was relabeled to " + name;

                        logEntryWriter(logEntry1);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            string logEntry = DateTime.Now + " " + ex.ToString();

            logEntryWriter(logEntry);
        }
    }
    else
    {
    }
}

//This will write to a log file to keep between runs
void logEntryWriter(string LogEntry)
{
    try
    {
        Console.WriteLine(LogEntry);
    }
    catch (Exception e)
    {
        logEntryWriter(LogEntry);
    }
}

#endregion