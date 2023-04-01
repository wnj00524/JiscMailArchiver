﻿using HtmlAgilityPack;
using ScrapySharp.Network;
using System;
using System.Linq;

bool DEBUG = true;

Console.WriteLine("BrithArch Archiver v. 1");
Console.WriteLine("\n\nUse at your own risk. This softare may not work, may break your computer, get you banned from JiscMail or any number of bad things. Use at your own risk.");
/*
ScrapingBrowser myBrowser = new ScrapingBrowser();
BrithArchArchiver.scraping scraping = new BrithArchArchiver.scraping();
Dictionary<string, Dictionary<string, string>> yearlyLinks = new Dictionary<string, Dictionary<string, string>>();


Console.WriteLine("Loading front page...");
//First up, get the list of individual pages to download. 
var frontPageLinks = scraping.getLinks("https://www.jiscmail.ac.uk/cgi-bin/webadmin?A0=britarch", true);


Console.WriteLine("Loading individual pages...");
foreach(var link in frontPageLinks)
{
    Console.WriteLine("Loading for " + link.Key);
    string url = "https://www.jiscmail.ac.uk" + link.Value;
    Dictionary<string, string> yearLinks = scraping.getLinks(url, false,link.Key.Replace(" ","")+".bal");
    yearlyLinks[link.Key] = yearLinks;
}

foreach(var entry in yearlyLinks)
{
    Console.WriteLine(entry);
    Console.WriteLine("\n\nNew Year\n\n");
}
*/


List<string> URLS = new List<string>();
BrithArchArchiver.Downloader downloader = new BrithArchArchiver.Downloader();
using (var reader = new StreamReader("MEGA.bal"))
{
    string line;
    while ((line = reader.ReadLine()) != null)
    {
        URLS.Add(line);
    }
}
Console.WriteLine("We've got {0} URls to load!",URLS.Count);

int current_url_number = 1;
foreach(string URL in URLS)
{
    Console.Write("We are on {0} of {1}...\n", current_url_number, URLS.Count);
    if (DEBUG)
    {
        if (current_url_number > 3)
            break;
    }
    downloader.downLoadURL(URL);
    Console.Write("\nDone!\n");
    current_url_number++;
}









