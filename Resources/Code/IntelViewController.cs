using System;
using System.Collections.Generic;
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

        private static SQLiteDataReader? data;

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
            //ensures freshness
            data = SqlDb.SelectAll();
            return data; 
        }

        //==========Command==========//

        public static void RunStats()
        {

            getData();

            if (data == null) return;

            ScanTotalEntries();
            ScanNames();
            ScanEaches();

        }

        //==========Scanners==========//

        //|0 id |1 item_name |2 item_quantity |3 item_category |4 added |5 expires |6 limit |

        private static void ScanTotalEntries()
        {
            
            int count = 0;

            while(data.Read())
            {
                count = (Int32)data.GetInt64(0);
            }

            WriteToSnippets("There are " + count + " entries in the table.");
            WriteToSummary("Entries("+count+")");
            
        }

        private static void ScanNames()
        {
            
            List<String> snippets = new List<String>();
            List<String> summarys = new List<String>();

            List<String> names    = new List<String>();
            List<Int32> quants    = new List<Int32>();

            List<Int32> floats = new List<Int32>();

            int broadDuplicates = 0;
            int dupes           = 0;

            while (data.Read()) { names.Add(data.GetString(1));  quants.Add(data.GetInt32(2)); }

            int ctrlNum = 0;
            int fl04t   = 0;
            while (ctrlNum < names.Count)
            {
                String tmp = names[ctrlNum];
                fl04t      = quants[ctrlNum];

                for (int i = 0; i < names.Count; i++)
                {
                    if (tmp.Equals(names[i]) & i != ctrlNum)
                    {
                        fl04t += quants[i];
                        dupes++;
                        broadDuplicates++;
                    }
                }

                snippets.Add(tmp + " has " +dupes+ " duplicates, which combined amounts to a total of " +fl04t+ "eaches.");
                summarys.Add(fl04t + " " + tmp);

                fl04t = 0;
                tmp  = "";

            }

            summarys.Insert(0, "Total Duplicates = " + broadDuplicates);

        }

        private static void ScanEaches()
        {

            int eachesTotal = 0;

            while (data.Read()) eachesTotal += data.GetInt32(2);

            WriteToSnippets("Total Amount of item eaches comes to a total of: "+eachesTotal);
            WriteToSummary("Total Eaches = "+eachesTotal);

        }

        //shaky
        private static void ScanDates()
        {

            Func<String, String> normalize = (i) => { return TableViewController.DateNormalizer(i); };

            String xDate;
            String yDate;

            while (data.Read())
            {
                xDate = normalize(data.GetValue(4).ToString()); 
                yDate = normalize(data.GetValue(5).ToString());

                if (xDate.Length != yDate.Length) continue; //catch-all for non-comparables - remember, xDate will always be defaulted to current date whilst yDate will default to -1

                //kk, didn't do much today, but ksp2 came out officially early access so...considering I got turned down by yet another tech job I'm more than qual for...
                //I'ma go play rocket man ^.^ for the rest of the day.

                //var preform = reader.GetValue(i);
                //var d = preform.ToString();
                //values.Add(DateNormalizer(d));

            }

        }


    }



}
