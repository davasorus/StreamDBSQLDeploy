using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;

string url = "https://go.microsoft.com/fwlink/?linkid=866658";
string fileName = "SQL2019-SSEI-Expr.exe";
string machineName = Environment.MachineName;
string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

Directory.CreateDirectory(@"C:\temp\sqlsetup");

logEntryWriter("downloading SQl Server Express 2019");

using (var client = new WebClient())
{
    client.DownloadFile(url, fileName);
}

logEntryWriter("download of SQl Server Express 2019 Complete");

File.Move(fileName, Path.Combine(@"C:\temp\sqlsetup", fileName), true);

logEntryWriter("attempting to install SQl Server Express 2019 Complete");

string cmdprompt = ""+Path.Combine(@"C:\temp\sqlsetup", fileName)+ @"/Q /SUPPRESSPRIVACYSTATEMENTNOTICE /IACCEPTSQLSERVERLICENSETERMS" +
@"""/ACTION=""install""/ FEATURES = SQL, SSMS"+ " /INDICATEPROGRESS /INSTANCENAME=SQLEXPRESS/ SQLSVCACCOUNT = "
+ "" +userName + " /SQLSVCPASSWORD ="+ "P@ssW0rd" + "";


cmdScriptRun(cmdprompt);

logEntryWriter("install of SQl Server Express 2019 Complete");

logEntryWriter(" account name: " + machineName + @"\" + userName);

logEntryWriter("Password: P@ssW0rd");

logEntryWriter("Attempting to Deploy stream Data Base");

SqlConnection myConn = new SqlConnection("server="+ machineName+ @"\SQLEXPRESS;database=Master;integrated security=SSPI;");

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

//this will run command prompt scripts within the application.
void cmdScriptRun(string Command)
{
    Process process = new Process();
    ProcessStartInfo startInfo = new ProcessStartInfo
    {
        WindowStyle = ProcessWindowStyle.Hidden,
        FileName = "cmd.exe",
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