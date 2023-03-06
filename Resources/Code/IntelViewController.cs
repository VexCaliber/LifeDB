using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LifeDB.Resources.Code
{
    internal class IntelViewController
    {

        private static StackPanel? snippetPanel;
        private static StackPanel? summaryPanel;

        public static void Init(StackPanel snippets, StackPanel summary)
        {
            snippetPanel = snippets;
            summaryPanel = summary;
        }

        //====================//

        private static void WriteToSnippets(String info)
        {

            Label x = new Label();
            x.Content = info;
            snippetPanel.Children.Add(x);

        }

        private static void WriteToSummary(String info)
        {

            Label x = new Label();
            x.Content = info;
            summaryPanel.Children.Add(x);

        }


        //====================//

        private static SQLiteDataReader getData()
        {
            return SqlDb.SelectAll();
        }

        //==========Command==========//

        /// THESE NEED TO GO TO A THREAD CAROUSEL FOR RESPONSIVENESS!
        public static void RunStats()
        {

            ScanTotalEntries(getData());
            ScanNames(getData()); //observation
            ScanEaches(getData());
            ScanDates(getData());
            ScanLimits(getData());

        }

        public static void Refresh()
        {
            ClearPanels();
            RunStats();
        }

        private static void ClearPanels()
        {
            snippetPanel.Children.Clear();
            summaryPanel.Children.Clear();
        }

        //==========Scanners==========//

        //|0 id |1 item_name |2 item_quantity |3 item_category |4 added |5 expires |6 limit |

        private static void ScanTotalEntries(SQLiteDataReader data)
        {
            
            int count = 0;

            while(data.Read())
            {
                count = (Int32)data.GetInt64(0);
            }

            WriteToSnippets("There are " + count + " entries in the table.");
            WriteToSummary("Entries("+count+")");
            
        }

        private static void ScanNames(SQLiteDataReader data)
        {
            
            List<string> snippets = new List<string>();
            List<string> summaries = new List<string>();

            List<string> names    = new List<string>();
            List<int> quants    = new List<int>();

            List<int> floats = new List<int>();

            int broadDuplicates = 0;
            int dupes           = 0;

            while (data.Read()) 
            { 

                names.Add(data.GetString(1));

                if (data.GetValue(2) == DBNull.Value) quants.Add(0);
                else quants.Add(data.GetInt32(2));

            }


            int ctrlNum = 0;
            int fl04t   = 0;
            
            List<string> prevEncountered = new List<string>();

            while (ctrlNum < names.Count) // arb controlNumber less than the sum of names
            {

                string tmp = names[ctrlNum]; // temp string = first instance of name
                fl04t      = quants[ctrlNum]; // first value in quants 1:1 corresponds to same position in names

                if (prevEncountered.Contains(tmp)) { fl04t = 0; tmp = ""; ctrlNum++; continue; } //prevents rebounding/recyling previous names...at one level

                for (int i = 0; i < names.Count; i++) // cycle through all names for duplicates
                {
                    if (tmp.Equals(names[i]) & i != ctrlNum) //if duplicate name encountered, but not the first of...
                    {
                        fl04t += quants[i]; // add the duplicates eaches quantity to the floating total
                        dupes++;            // tick the dupes
                        broadDuplicates++;  // tick the broad dupes
                        //NEED A WAY to BREAK OUT...THIS ALSO CAUSES REBOUNDING...
                        //it will continue cycling, but it won't break and get to fl04t = 0
                        //i think...idk, I'm a bit tired now...
                    }
                }

                //print the vals
                snippets.Add(tmp + " has " +dupes+ " duplicates, which combined amounts to a total of " +fl04t+ " eaches.");
                summaries.Add(fl04t + " " + tmp);

                //Prevent rebounding
                prevEncountered.Add(tmp);

                //reset important variables
                fl04t = 0;
                tmp  = "";
                ctrlNum++;

            }

            summaries.Insert(0, "Total Duplicates = " + broadDuplicates);

            foreach (string sn in snippets) WriteToSnippets(sn);
            //foreach (string su in summaries) WriteToSummary(su);

        }

        private static void ScanEaches(SQLiteDataReader data)
        {

            int eachesTotal = 0;

            while (data.Read())
            {
                //eachesTotal += data.GetInt32(2);
                if (data.GetValue(2) == DBNull.Value) continue;
                else eachesTotal += data.GetInt32(2);

            }

            WriteToSnippets("Total Amount of item eaches comes to a total of: "+eachesTotal);
            WriteToSummary("Total Eaches = "+eachesTotal);

        }

        //modify to offer order of which eaches to use, mindful of duplicates, based on earliest expiring to latest expiring...
        private static void ScanDates(SQLiteDataReader data)
        {
            int expiredEntries     = 0;
            int totalExpiredEaches = 0;

            while (data.Read())
            {

                if (ExpiresWithin30(data.GetString(5)))
                {

                    if (data.GetValue(2) == DBNull.Value) 
                        WriteToSnippets("At id: " + data.GetValue(0).ToString() + ", Item: " + data.GetString(1) + ", representing: " + 0 + " eaches is going to expire withing 30 days!");
                    else 
                        WriteToSnippets("At id: " + data.GetValue(0).ToString() + ", Item: " + data.GetString(1) + ", representing: " + data.GetValue(2).ToString() + " eaches is going to expire withing 30 days!");

                    WriteToSummary("id: " + data.GetValue(0).ToString() + "contains eaches nearing expiration");

                }
                
                if (HasExpired(data.GetString(5)))
                {

                    if (data.GetValue(2) == DBNull.Value)
                        WriteToSnippets("At id: " + data.GetValue(0).ToString() + ", Item: " + data.GetString(1) + ", representing: " + 0 + " eaches has expired!");
                    else
                        WriteToSnippets("At id: " + data.GetValue(0).ToString() + ", Item: " + data.GetString(1) + ", representing: " + data.GetValue(2).ToString() + " eaches has expired!");
                    
                    expiredEntries++;

                    if (data.GetValue(2) == DBNull.Value)
                        continue;
                    else
                        totalExpiredEaches += (Int32)data.GetValue(2);

              

                }
                

            }

            WriteToSummary(expiredEntries + " expired entries");
            WriteToSummary(totalExpiredEaches + " expired eaches");

        }

        //maybe modify to account for the duplicates
        //take advantage of bigbox api's (if available) to pull prices (with links) for resupply when under limit?  possibly dev a light scraper for this if needed.
        //if over limit, maybe scrape prices from online marketplaces for selling prompts
        private static void ScanLimits(SQLiteDataReader data)
        {

            int over = 0, at = 0, under = 0;

            while (data.Read())
            {

                if (data.GetValue(2) == DBNull.Value | data.GetValue(6) == DBNull.Value) continue; // well that was easy...god, tired brain is brutal >.>

                if ((Int32)data.GetValue(2) > (Int32)data.GetValue(6))
                {
                    int x = (Int32)data.GetValue(2) - (Int32)data.GetValue(6);
                    WriteToSnippets("id: " + data.GetValue(0).ToString() + " is over limit by " + x + " eaches!");
                    over++;
                }

                //Meh...I'm sure the following 2 will be more of a nuisance...
                if ((Int32)data.GetValue(2) == (Int32)data.GetValue(6))
                {
                    //WriteToSnippets("id: " + data.GetValue(0).ToString() + " is at the your set limit.");
                    at++;
                }

                if ((Int32)data.GetValue(2) < (Int32)data.GetValue(6))
                {
                    int x = (Int32)data.GetValue(6) - (Int32)data.GetValue(2);
                    //WriteToSnippets("id: " + data.GetValue(0).ToString() + " is under the your set limit by " + x + " eaches!");
                    under++;
                }

            }

            WriteToSummary(over  + " Entries over eaches limit");
            WriteToSummary(at    + " Entries at eaches limit");
            WriteToSummary(under + " Entries under eaches limit");

        }

        //==========Helpers==========//

        private static int Converted(String x) { return Int32.Parse(x); }
        
        private static String Converted(Int32 x) { return x.ToString(); }    
        
        private static Boolean ExpiresWithin30(String NormalizedExpiryDate)
        {

            if (NormalizedExpiryDate.Equals("-1")) return false;

            try
            {
                           
                //Dates are "day/month/year"
                //Split Index 0    1    2
                String[] split = NormalizedExpiryDate.Split("/");

                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
               
                DateOnly expirationDate = new DateOnly(day: Converted(split[0]), month: Converted(split[1]), year: Converted(split[2]));
            
                //Oh I hope this works...otherwise it's gonna get real autsy up in here...O.O
                if(currentDate.AddDays(30) >= expirationDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch(Exception e)
            {
                return false;
            }


        }

        private static Boolean HasExpired(String NormalizedExpiryDate)
        {

            if (NormalizedExpiryDate.Equals("-1")) return false;

            try
            {

                //Dates are "day/month/year"
                //Split Index 0    1    2
                String[] split = NormalizedExpiryDate.Split("/");

                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                DateOnly expirationDate = new DateOnly(day: Converted(split[0]), month: Converted(split[1]), year: Converted(split[2]));

                //Oh I hope this works...otherwise it's gonna get real autsy up in here...O.O
                if (currentDate > expirationDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }


        }


    }



}
