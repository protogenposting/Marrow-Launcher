﻿using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using Octokit;
using Microsoft.Win32;

GitHubClient client = new GitHubClient(new ProductHeaderValue("protogenposting"));

IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("protogenposting", "Marrow");

Release latest = releases[0];

string lastDownloadedRelease = "";

try{
    using (StreamReader reader = new StreamReader("LastVersion.txt"))
    {
        lastDownloadedRelease = reader.ReadLine();
    }
}
catch(Exception e)
{
    
}

Console.WriteLine("Last Release Downloaded: "+lastDownloadedRelease);

Console.WriteLine("CurrentRelease: "+latest.TagName);

if(lastDownloadedRelease.Equals("") || !latest.TagName.Equals(lastDownloadedRelease))
{
    using (StreamWriter outputFile = new StreamWriter("LastVersion.txt"))
    {
        outputFile.WriteLine(latest.TagName);
    }

    Console.WriteLine("Downloading Update!");

    var webClient = new WebClient();

    webClient.UseDefaultCredentials = true;

    webClient.DownloadFile("https://github.com/protogenposting/Marrow/releases/latest/download/Marrow.jar", "Marrow.jar");
}

Console.WriteLine("Download Check Done... Launching");

if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    Console.WriteLine("Wow... A Linux User... So Hot...");

    string result = ShellHelper.Bash("java -jar Marrow.jar");

    if(result.Contains("command not found"))
    {
        Console.WriteLine("No Java Version Found!");

        Console.WriteLine("Run This Command: sudo apt install openjdk-21-jdk");
    }
}
else
{
    string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");

    Console.WriteLine("Java Home At: "+javaHome);

    try{
        var processInfo = new ProcessStartInfo(javaHome + "/bin/java.exe", "-jar Marrow.jar")
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true
        };

        Process proc;
        
        if ((proc = Process.Start(processInfo)) == null)
        {
            throw new InvalidOperationException("??");
        }

        Console.WriteLine("Running From Home!");

        proc.WaitForExit();

        string error = proc.StandardError.ReadToEnd();

        if(error.Contains("has been compiled by a more recent version of the Java Runtime"))
        {
            Console.WriteLine("Java Is Outdated! Download java version 21 or higher here! https://www.openlogic.com/openjdk-downloads");
        }

        int exitCode = proc.ExitCode;
        
        proc.Close();
    }
    catch(Exception e)
    {
        try{
            var processInfo = new ProcessStartInfo("java.exe", "-jar Marrow.jar")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true
            };

            Process proc;
            
            if ((proc = Process.Start(processInfo)) == null)
            {
                throw new InvalidOperationException("??");
            }

            Console.WriteLine("Running From EXE!");

            proc.WaitForExit();

            string error = proc.StandardError.ReadToEnd();

            if(error.Contains("has been compiled by a more recent version of the Java Runtime"))
            {
                Console.WriteLine("Java Is Outdated! Download java version 21 or higher here! https://www.openlogic.com/openjdk-downloads");
            }

            int exitCode = proc.ExitCode;

            proc.Close();
        }
        catch(Exception e2)
        {
            Console.WriteLine("NO JAVA.EXE OR JAVA HOME FOUND");

            Console.WriteLine("Download java version 21 or higher here! https://www.openlogic.com/openjdk-downloads");

            Console.WriteLine("IF THAT DOESN'T WORK GO HERE TO FIGURE OUT JAVA_HOME https://confluence.atlassian.com/doc/setting-the-java_home-variable-in-windows-8895.html");
        }
    }
}

Console.WriteLine("Press Enter To Exit...");

Console.ReadLine();