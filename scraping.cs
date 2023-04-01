using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Network;


namespace BrithArchArchiver
{
    internal class scraping
    {
        static int MaxVal = 99999999;
        ScrapingBrowser myBrowser = new ScrapingBrowser();
        string[] months =  new string[] { "january", "febuary", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
        int linkC = 0;
        public Dictionary<string, string> getLinks(string url, bool frontPage = false, string fileName = "")
        {
            //Get all the links on the page
            
            Dictionary<string, string> links = new Dictionary<string, string>();
            linkC = linkC + 1;
            if (linkC >= MaxVal)
            {
                Console.WriteLine("Reached maximum searches for speed purposes...returing what we have!");
                return links;
            }
            Random random = new Random();
            //Lets check to see if a proper filename was passed when getting page URLs...
            if (!frontPage && fileName == "")
            {
                Console.WriteLine("No filename passed when getting individual page indexes. Abort!");
                return links;
            }


            int waitValue = random.Next(2000, 6000);
            Console.WriteLine("We'll pause here  for {0} seconds, to avoid getting blocked...", waitValue/1000);
            Thread.Sleep(waitValue);
            WebPage webPage = myBrowser.NavigateToPage(new Uri(url));
            var rawLinks = webPage.Html.OwnerDocument.DocumentNode.SelectNodes("//a");
            
            //IF we're doing the front page we'll want to restrict our search to those with a month in there...

            if (rawLinks == null)
            {
                Console.WriteLine("Null rawLinks - stopping right now!");
                return links;
            }

            if (frontPage)
            {
                int c = 0;
               foreach (var link in rawLinks)
                {
                    
                        
                        foreach (string month in months)
                        {
                            c = c + 1;
                            if (link.InnerText.ToLower().Contains(month.ToLower()))
                            {
                                links[link.InnerText] = link.Attributes["href"].Value;
                            }
                        }
                    
                }
            }

            //If we're not doing a front page, then we'll need to identify those links that go to an email rather than other pages.
            if (!frontPage)
            {
                int c = 0;
                //We need to open up a file...
                TextWriter tw = new StreamWriter(fileName, true);
               
                
                    foreach (var link in rawLinks)
                    {
                        c = c + 1;
                        //Each link has "P=" in it, so we can search for that and save them to the links dictionary. 
                        if (link.Attributes["href"].Value.ToLower().Contains("p="))
                        {
                            //Ok it's here, but now we need to make sure there isn't already a link with the same name...
                            string keyValue = link.InnerText;
                            int inc = 0;
                            while (links.ContainsKey(keyValue))
                            {
                                inc++;
                                keyValue = keyValue + inc.ToString();
                            }
                        Console.WriteLine("Got a link called" + keyValue);
                        Console.WriteLine("The url is:" + link.Attributes["href"].Value);
                        
                            links[link.InnerText] = link.Attributes["href"].Value;
                        tw.WriteLine(link.Attributes["href"].Value);
                        }
                    }
                //close our filestream...
                tw.Close();
            }
            
            return links;
        }
    }
}
