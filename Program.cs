using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;

string url1 = "https://go.microsoft.com/fwlink/?linkid=866658";
string url2 = "https://aka.ms/ssmsfullsetup";
string fileName1 = "SQL2019-SSEI-Expr.exe";
string fileName2 = "SSMS-Setup-ENU.exe";
string fileName3 = "SETUP.EXE";
string machineName = Environment.MachineName;
string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

//location of SQl Server 2019 installer
string sqlCmd1 = Path.Combine(@"C:\Temp\sqlsetup\SQLEXPR_2019", fileName3);
string sqlCmd2 = Path.Combine(@"C:\temp\sqlsetup", fileName2);

//config of sql server 2019
string sqlCmdScript1 = @" /Q /ACTION=""Install"" /ROLE=""AllFeatures_WithDefaults"" /IACCEPTROPENLICENSETERMS=""True"" /SUPPRESSPRIVACYSTATEMENTNOTICE=""True"" /ENU=""True""" +
    @" /QUIETSIMPLE=""False"" /USEMICROSOFTUPDATE=""True"" /SUPPRESSPAIDEDITIONNOTICE=""True"" /FEATURES=SQL /INDICATEPROGRESS=""True"" " +
    @" /INSTANCENAME=""SQLEXPRESS"" /INSTALLSHAREDDIR=""C:\Program Files\Microsoft SQL Server"" /INSTANCEID=""SQLEXPRESS"" /FILESTREAMLEVEL=""3"" /ADDCURRENTUSERASSQLADMIN=""True""" +
    @" /BROWSERSVCSTARTUPTYPE=""Automatic"" /IACCEPTSQLSERVERLICENSETERMS=""True"" /SQLTELSVCACCT=""NT AUTHORITY\NETWORK SERVICE"" /SQLTEMPDBLOGFILESIZE=""8"" " +
    @" /SQLSVCACCOUNT=""NT AUTHORITY\NETWORK SERVICE"" /SQLTEMPDBFILECOUNT=""1"" /SQLTEMPDBFILESIZE=""8"" /SQLTEMPDBFILEGROWTH=""64"" /SQLTEMPDBLOGFILEGROWTH=""64"" /FILESTREAMSHARENAME=""FILESTREAMSHARE"" ";

string sqlCmdScript2 = @"/install /quiet /restart";

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

Process.Start("CMD.exe", @"/c C:\temp\sqlsetup\SQL2019-SSEI-Expr.exe /ACTION=Download MEDIAPATH=C:\temp\sqlsetup /MEDIATYPE=Core /QUIET");

Thread.Sleep(100000);

Process.Start("CMD.exe", @"/C C:\temp\sqlsetup\SQLEXPR_x64_ENU.exe /q /x:C:\temp\sqlsetup\SQLEXPR_2019");

Thread.Sleep(10000);

logEntryWriter("attempting to install SQl Server 2019");

scriptRun(sqlCmdScript1, sqlCmd1);

//code that does the installing of SQL Server
//Task task3 = Task.Factory.StartNew(() => scriptRun(sqlCmdScript1, sqlCmd1));

scriptRun(sqlCmdScript2, sqlCmd2);

//code that does the installing of SSMS
//Task task4 = Task.Factory.StartNew(() => scriptRun(sqlCmdScript2 , sqlCmd2));

//Task.WaitAll(task3, task4);

logEntryWriter("install of SQl Server Express 2019 Complete");

logEntryWriter(" account name: " + userName);

logEntryWriter("Password: P@ssW0rd");

logEntryWriter("Attempting to Deploy stream Data Base");

//creates connection information to create db
SqlConnection myConn = new SqlConnection("server=" + machineName + @"\SQLEXPRESS;database=Master;integrated security=SSPI;");

string str;
str = "CREATE DATABASE STREAMINGDB";

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

Process.Start("CMD>exe", "Restart /r");

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