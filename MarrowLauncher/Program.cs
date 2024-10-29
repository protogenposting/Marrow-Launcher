using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using Octokit;

GitHubClient client = new GitHubClient(new ProductHeaderValue("protogenposting"));

IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("protogenposting", "Marrow");

Release latest = releases[0];

string lastDownloadedRelease;

using (StreamReader reader = new StreamReader("LastVersion.txt"))
{
    lastDownloadedRelease = reader.ReadLine();
}

if(lastDownloadedRelease == null || latest.TagName != lastDownloadedRelease)
{
    using (StreamWriter outputFile = new StreamWriter("LastVersion.txt"))
    {
        outputFile.WriteLine(latest.TagName);
    }
    var webClient = new WebClient();

webClient.UseDefaultCredentials = true;

webClient.DownloadFile("https://github.com/protogenposting/Marrow/releases/latest/download/Marrow.jar", "Marrow.jar");
}

if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    string result = ShellHelper.Bash("java -jar Marrow.jar");

    if(result.Contains("command not found"))
    {
        Console.WriteLine("Installing OpenJDK Version 21...");
    }
}
else
{
    //fix this!!
    string strCmdText;
    strCmdText= "java -jar Marrow.jar";
    Process proc = Process.Start("CMD.exe",strCmdText);
}