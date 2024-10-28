using System;
using System.Net;
using Octokit;

GitHubClient client = new GitHubClient(new ProductHeaderValue("SomeName"));

IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("protogenposting", "Marrow");

Release latest = releases[0];

var webClient = new WebClient();

webClient.DownloadFile(new Uri(latest.ZipballUrl), "Marrow.zip");