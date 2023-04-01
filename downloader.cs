using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Network;

namespace BrithArchArchiver
{
    internal class Downloader
    {
        ScrapingBrowser myBrowser = new ScrapingBrowser();
        public void downLoadURL(string URL)
        {
            
            do_sleep();
            //Get the URl...
            WebPage webPage = myBrowser.NavigateToPage(new Uri(URL));
            //Get the bodyText nodes...
            var bodyText = webPage.Html.OwnerDocument.DocumentNode.SelectNodes("//pre");
            //Testing extractBodyText
            var body_text_value = extractBodyText(bodyText);
            //Get the fields nodes (i.e. P values)
            var fieldNodes = webPage.Html.OwnerDocument.DocumentNode.SelectNodes("//td//p");
            var subject = extract_field(fieldNodes, 40);
            var from = extract_field(fieldNodes, 42);
            var date = extract_field(fieldNodes, 46);

            //Put the info into a class for serialisation...
            BrithArchArchiver.entry entry = new entry();
            entry.Date = date;
            entry.From = from;
            entry.Subject = subject;
            entry.Message = body_text_value;

            //Then serialise!
            var json_string = JsonSerializer.Serialize(entry);
           // Console.WriteLine(json_string);

            //Save to file!            
            string fileName = GenFileName(entry);
            //Console.WriteLine("File name = " + fileName);
            //Check if the file exists...
            
            if (!File.Exists(fileName))
            {
                //If not, we save!
                File.WriteAllText(fileName, json_string);
            }
            else
            {
                //If it does...we increment the filenames until it finds a file that doens't exist. 
                int increment = 0;
                while (File.Exists(fileName))
                {
                    var firstHalf = fileName.Split(".")[0];
                    firstHalf = firstHalf + increment.ToString();
                    fileName = firstHalf + ".jsn";
                    increment = increment + 1;
                }
                File.WriteAllText(fileName, json_string);
            }
            
            

        }

        private string extractBodyText(HtmlNodeCollection bodyNodes)
        {
            if (bodyNodes != null)
            {
                foreach (var item in bodyNodes)
                {
                    return item.InnerText;
                }
            }
            return "";
        }

        private string extractMsgID(BrithArchArchiver.entry entry)
        {
            string r = "";
            var split_date = entry.Date;
            var date_element = "";
            foreach (var item in split_date)
            {
                date_element = date_element + item.ToString();
            }
            if (entry.Subject.Length < 7)
            {
                r = date_element + Clean_String(entry.Subject);
            }
            else
            {
                r = date_element + Clean_String(entry.Subject).Substring(0,4);
            }
            //If the string is long enough, take the year from the date and put it at the front of the file for easy sorting. 
            if (r.Length > 17)
            {
                var front = r.Substring(12, 4);
                r = r.Remove(12, 4);
                r = front + r;
            }
            
                      
            
            return Clean_String(r);
        }

        /// <summary>
        ///Clean up method for strings.
        /// </summary>
        /// <param name="to_clean"></param>
        /// <param name="all_to_lower"></param>
        /// <returns></returns>
        private string Clean_String(string to_clean, bool all_to_lower = false)
        {

            string r = to_clean;
            r = r.Replace(" ", "");
            r = r.Replace(",","");
            r = r.Replace("-", "");
            r = r.Replace(":", "");
            r = r.Replace(".", "");
            r = r.Replace("+", "");
            r = r.Replace("#", "");
            r = r.Replace("\\", "");
            r = r.Replace("/", "");
            if (all_to_lower)
                r = r.ToLower();
            return r;
        }

        private string GenFileName(BrithArchArchiver.entry entry)
        {
            
            string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string fileName = exePath + "\\Archive\\" +  extractMsgID(entry) + ".jsn";
            
            return fileName;
        }

        private string extract_field(HtmlNodeCollection fromNodes, int index)
        {
            if (index < fromNodes.Count && fromNodes.Count > 0)
            {
                return fromNodes[index].InnerText;
            }
            else
            {
                return "Out of Bounds Error";
            }
            
              
        }

       private void do_sleep()
        {
            Random random = new Random();
            int waitValue = random.Next(2000, 4000);
            Console.WriteLine("We'll pause here  for {0} seconds, to avoid getting blocked...", waitValue / 1000);
            Thread.Sleep(waitValue);
        }
    }
}
