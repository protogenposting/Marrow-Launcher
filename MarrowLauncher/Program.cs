using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using Octokit;

GitHubClient client = new GitHubClient(new ProductHeaderValue("protogenposting"));

IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("protogenposting", "Marrow");

Release latest = releases[0];

var webClient = new WebClient();

webClient.UseDefaultCredentials = true;

webClient.DownloadFile("https://github.com/protogenposting/Marrow/releases/latest/download/Marrow.jar", "Marrow.jar");

if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    RunCommandWithBash("java Marrow.jar");
}
else
{
    string strCmdText;
    strCmdText= "java Marrow.jar";
    System.Diagnostics.Process.Start("CMD.exe",strCmdText);
}

string RunCommandWithBash(string command)
{
    var psi = new ProcessStartInfo();
    psi.FileName = "/bin/bash";
    psi.Arguments = command;
    psi.RedirectStandardOutput = true;
    psi.UseShellExecute = false;
    psi.CreateNoWindow = true;

    using var process = Process.Start(psi);

    process.WaitForExit();

    var output = process.StandardOutput.ReadToEnd();

    return output;
}