using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LifeDB.Resources.Code
{
    internal class IntelViewController
    {

        private static StackPanel snippetPanel;
        private static StackPanel summaryPanel;

        private static SQLiteDataReader data;

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


        //==========Scanners==========//

        //| id | item_name | item_quantity | item_category | added | expires | limit |

        private static void TotalItems()
        {
            
            int count = 0;

            while(data.Read())
            {
                count = (Int32)data.GetInt64(0);
            }

            WriteToSnippets("There are " + count + " items in the table.");
            WriteToSummary("Items("+count+")");
            
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

        }



    }



}
