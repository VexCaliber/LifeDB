using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

using FontFamily = System.Windows.Media.FontFamily;

namespace LifeDB.Resources.Code
{
    internal class StockViewController
    {

        private static System.Windows.Documents.Table table;


        public static int currentRow { get; private set; }


        //private static TableRow rowTemplateA = (TableRow)Application.Current.Resources["RowA"];
        //private static TableRow rowTemplateB = (TableRow)Application.Current.Resources["RowB"];


        //------------------------------//


        public static void Generate()
        {

            SQLiteDataReader reader = SqlDb.SelectAll();

            if (reader == null) throw new Exception("Failed to get data from table via Select All @TableViewController.Generate()");

            var columns = reader.FieldCount;

            List<String> values = new List<String>();

            while (reader.Read()) //REVIEW ME!
            {

                for (int i = 0; i < columns; i++)
                {

                    if (i == 4 | i == 5)
                    {
                        ///We're getting a number YYYY/MM/DD
                        ///Take the value and build a date  :: In 1/12/2020, becomes 2020/1/12; Out value -> String -> Date -> Add -> Table
                        //2020112  20200112  20112  200112
                        //0123456
                        var preform = reader.GetValue(i);
                        var d = preform.ToString();

                        values.Add(DateNormalizer(d));
                    }
                    else
                        values.Add(reader.GetValue(i).ToString());

                }

                GenerateRow(values);

                values.Clear();

            }


        }


        public static void Generate(SQLiteDataReader reader)
        {

            if (reader == null) return; //Break out and fail silently

            var columns = reader.FieldCount;

            List<String> values = new List<String>();

            while (reader.Read()) //REVIEW ME!
            {

                for (int i = 0; i < columns; i++)
                {

                    if (i == 4 | i == 5)
                    {

                        var preform = reader.GetValue(i);
                        var d = preform.ToString();
                        values.Add(DateNormalizer(d));

                    }
                    else
                        values.Add(reader.GetValue(i).ToString()); //reader.GetString(i) //DATES ARE WRONG IN THIS AND ABOVE...MAYBE IF/ELSE PARSE DATEONLY -> toString()

                }

                GenerateRow(values);

                values.Clear();

            }

        }


        public static void Agitate(SQLiteDataReader reader)
        {
            if (reader == null) return;
            var columns = reader.FieldCount;
            List<String> DBValues = new List<String>();

            int activeRow = 1;
            var cells = table.RowGroups[0].Rows[activeRow].Cells.ToList();

            while (reader.Read())
            {

                for (int i = 0; i < columns; i++)
                {
                    if (i == 4 | i == 5)
                    {

                        var preform = reader.GetValue(i);
                        var d = preform.ToString();
                        DBValues.Add(DateNormalizer(d));

                    }
                    else
                        DBValues.Add(reader.GetValue(i).ToString());

                }


                for (int i = 0; i < columns; i++)
                {
                    //an array of blocks
                    //row of cells (7), each cell has a block (paragraph), within that paragraph is a run, within the run is the text to change
                    if (!(DBValues[i].Equals(cells[i]               //get the cells in the active row
                        .Blocks                                     //get the array of parent blocks
                        .FirstBlock                                 //get the first parent block (paragragh)
                        .SiblingBlocks                              //get the array of sibling blocks of the first parent block
                        .FirstBlock                                 //get the first child block (paragraph(run("txt")))
                        .ContentStart                               //get the head (pointer) for the content of the block
                        .GetTextInRun(LogicalDirection.Forward))))  //get the text in this block, reading from left to right (String)
                                                                    //see what I mean...f-ing messy...I hope this is right, haven't tested yet

                    {
                        table.RowGroups[0].Rows[activeRow].Cells[i].Blocks.Clear(); //this can be modified using the above (precision), to reduce object allocations
                        table.RowGroups[0].Rows[activeRow].Cells[i].Blocks.Add(new Paragraph(new Run(DBValues[i])));
                    }

                }


                DBValues.Clear();
                activeRow++;

            }

        }

        /*
        public static void Regenerate()
        {
            List<TableRow> rows = new List<TableRow>();
            Boolean skippedRow = false;
            foreach(TableRow row in table.RowGroups[0].Rows)
            {
                if (skippedRow == false) { skippedRow = true; continue; }

                rows.Add(row);

            }

            foreach (TableRow row in rows)
            {
                table.RowGroups[0].Rows.Remove(row);
            }
            

            SQLiteDataReader reader = SqlDb.SelectAll();

            if (reader == null) throw new Exception("Failed to get data from table via Select All @TableViewController.Generate()");

            var columns = reader.FieldCount;

            List<String> values = new List<String>();

            while (reader.Read()) //REVIEW ME!
            {

                for (int i = 0; i < columns; i++)
                {

                    if (i == 4 | i == 5)
                    {
                        ///We're getting a number YYYY/MM/DD
                        ///Take the value and build a date  :: In 1/12/2020, becomes 2020/1/12; Out value -> String -> Date -> Add -> Table
                        //2020112  20200112  20112  200112
                        //0123456
                        var preform = reader.GetValue(i);
                        var d = preform.ToString();

                        values.Add(DateNormalizer(d));
                    }
                    else
                        values.Add(reader.GetValue(i).ToString());

                }

                GenerateRow(values);

                values.Clear();

            }


        }
        */

        //PENDING OUTSIDE REVIEW -- My brain just refuses to wanna fix this one...idk, but this'll have to wait a little...
        public static void Deletify(List<KVP<String, String>> actions)
        {

            //Regenerate();

            var headerCells = table.RowGroups[0].Rows[0].Cells;
            Boolean skippedRow = false;
            List<TableRow> forDeletion = new List<TableRow>();

            foreach (TableRow row in table.RowGroups[0].Rows)
            {

                if (skippedRow == false) { skippedRow = true; continue; }

                foreach (TableCell cell in row.Cells)
                {

                    //SET UP FOR MULTIPLE PAIRS, BUT NOT THE SYSTEM ITSELF YET...WILL CREATE ERRORS IN FUTURE
                    foreach (KVP<String, String> pair in actions)
                    {
                        foreach (TableCell headerCell in headerCells)
                        {
                            if (headerCell.Blocks.FirstBlock.SiblingBlocks.FirstBlock.ContentStart.GetTextInRun(LogicalDirection.Forward).ToString() == pair.GetKey())
                            {
                                if (cell.Blocks.FirstBlock.SiblingBlocks.FirstBlock.ContentStart.GetTextInRun(LogicalDirection.Forward).ToString() == pair.GetValue())
                                {

                                    forDeletion.Add(row);

                                }
                            }
                        }
                    }
                }
            }

            /*
            * Scan the Rows
            * Skip the Header Row
            * Scan the Cells
            * Per each action kvp
            * Scan the Header Cells
            * If the header text val equals the action key -> test value
            * If the cell value equals the action value -> add row to deletion list
            * Cycle through the rows to delete and remove them individually
            * -
            * Rollback last written row eg. currentRow
            */

            foreach (TableRow row in forDeletion)
            {
                table.RowGroups[0].Rows.Remove(row); //it will shift "up" or "left", the rows to fill the void, in the process,
                table.RowGroups[0].Rows.RemoveAt(currentRow); //it doesn't update the visual...so we do need to remove the last written row...?               
                --currentRow;
                --currentRow; //seems to have little to no effect, but currentRow is our critical pacemaker so idek what the hell is the deal here...
                ///--currentRow;
                Update(false);//and refresh

            }
            //SqlDb.DBCount();
            //Update(false);

            //table.RowGroups[0].Rows.RemoveAt(currentRow - 1);
            //currentRow--;

            //Update(true);


        }


        public static void parseDelete(String? commandText)
        {

            if (commandText == null
                | commandText == "DELETE FROM myTable WHERE " + ""
                | commandText == "DELETE FROM myTable WHERE " + " ") return;

            //" + ASSIGNED;
            String[] pieces = commandText.Split(' ');

            //"DELETE FROM myTable WHERE " + "id = 4"; ex/
            //"DELETE FROM myTable WHERE " + "item_name = rubberdicks"; ex/

            //int equalsCounter = 0;
            //foreach (String x in pieces)
            //    if (x == "=") equalsCounter += 1;

            List<KVP<String, String>> actions = new List<KVP<String, String>>();

            //if (equalsCounter == 1)
            //{
            for (int i = 0; i < pieces.Length; i++)
            {
                if (pieces[i] == "=")
                {
                    actions.Add(new KVP<String, String>(pieces[i - 1], pieces[i + 1]));
                }

            }
            //}

            Deletify(actions);

        }


        public static void Update(Boolean fetchNewRows)
        {

            if (fetchNewRows == true)
            {

                SqlDb.DBCount(); //REQUIRED
                int startingRow = currentRow + 1;

                if (currentRow < SqlDb.lastCount)
                    Generate(SqlDb.GetIdRange(startingRow, SqlDb.lastCount)); //call and grab result set :: incl/incl & Generate

            }

            Agitate(SqlDb.GetVisibleIdRange());

        }


        //------------------------------//


        public static void Init(System.Windows.Documents.Table t)
        {
            table = t;
        }


        //------------------------------//


        private static void GenerateRow(List<String> values)
        {


            if (!(currentRow % 2 == 0))
            {

                table.RowGroups[0].Rows.Add(GenerateGenericRowA());
                currentRow++;
                var cells = table.RowGroups[0].Rows[currentRow].Cells.ToList();

                //clean ze row ///NO LONGER NEEDED HERE ::: MOVE LATER
                //for(int i = 0; i < cells.Count-1; i++) table.RowGroups[0].Rows[currentRow].Cells[i].Blocks.Clear();

                //assign the vals 
                for (int i = 0; i < cells.Count; i++) ///CHANGED -- From .count-1 to count
                {

                    table.RowGroups[0].Rows[currentRow].Cells[i].Blocks.Add(new Paragraph(new Run(values[i])));

                }




            }
            else
            {

                table.RowGroups[0].Rows.Add(GenerateGenericRowB());
                currentRow++;
                var cells = table.RowGroups[0].Rows[currentRow].Cells.ToList();

                for (int i = 0; i < cells.Count; i++) ///CHANGED -- From .count-1 to count
                {
                    //Found the answer...but god this is still awful...I'll do something about it...
                    //table.RowGroups[0].Rows[currentRow].Cells[i].Blocks.Clear();
                    table.RowGroups[0].Rows[currentRow].Cells[i].Blocks.Add(new Paragraph(new Run(values[i])));
                }

            }


        }


        //------------------------------//


        private static TableRow GenerateGenericRowA()
        {

            TableRow tr = new();

            //count - 1 changed to count :: we're starting from 1, not 0 for these, being collections.
            for (int i = 0; i < table.Columns.Count; i++)
            {
                TableCell tc = new TableCell();
                tc.TextAlignment = TextAlignment.Center;
                tr.Cells.Add(tc);
            }

            tr.Background = Brushes.White;
            tr.FontFamily = new FontFamily("consolas");
            tr.FontSize = 12;

            //ConsoleHandler.userConsole.Text += "row" + "; ";

            return tr;

        }


        private static TableRow GenerateGenericRowB()
        {

            TableRow tr = new();

            //count - 1 changed to count :: we're starting from 1, not 0 for these, being collections.
            for (int i = 0; i < table.Columns.Count; i++)
            {
                TableCell tc = new TableCell();
                tc.TextAlignment = TextAlignment.Center;
                tr.Cells.Add(tc);
            }

            tr.Background = Brushes.WhiteSmoke;
            tr.FontFamily = new FontFamily("consolas"); //THESE CAUSE AMBIG. ERRORS TRACK THE USINGS IN IMPORTS...they just decided to throw randomly O.o
            tr.FontSize = 12;

            //ConsoleHandler.userConsole.Text += "row" + "; ";

            return tr;

        }


        //------------------------------//

        //I NEED A BRILLIANT BIT OF WORK HERE!
        internal static String DateNormalizer(String RawDate)
        {
            //len = 7  len = 8   len=5  len=6
            //2020112  20200112  20112  200112
            //0123456  01234567  01234  012345

            //////// len=6   len=5   len=4
            //alt:// 202016  202016  2016
            //////// 012345  012345  0123

            /* CREATE IFS TO FIX VERY MALFORMED ALTS
            if(RawDate.Length == 6)
            {

                var currentYear = DateTime.Now.Year;
                var cloned = new String(RawDate);
                cloned.Insert(0, "00");

                // 6A         6B
                //len=6      len=6
                //200112     202016  --YYMMDD--YYYYMD-
                //012345     012345

                //    MMDD    YYMD
                //6A: 0112  B:2016
                var dropLeadYear = RawDate.Substring(2);

                //
                //6A: 00200112  6B: 00202016
                //        0112          2016

                var dropFullYear = cloned.Substring();

                
            }
            */

            StringBuilder reverted = new();

            switch (RawDate.Length)
            {
                case 5:
                    return reverted.Append(RawDate.Substring(2, 1))
                                        .Append("/")
                                        .Append(RawDate.Substring(3, 2))
                                        .Append("/")
                                        .Append(RawDate.Substring(0, 2))
                                        .ToString();



                case 6:
                    return reverted.Append(RawDate.Substring(2, 2))
                                        .Append("/")
                                        .Append(RawDate.Substring(4, 2))
                                        .Append("/")
                                        .Append(RawDate.Substring(0, 2))
                                        .ToString();



                case 7:
                    return reverted.Append(RawDate.Substring(4, 1)) //day
                                        .Append("/")
                                        .Append(RawDate.Substring(5, 2)) //month
                                        .Append("/")
                                        .Append(RawDate.Substring(0, 4)) //year
                                        .ToString();



                case 8:
                    return reverted.Append(RawDate.Substring(4, 2))
                                        .Append("/")
                                        .Append(RawDate.Substring(6, 2))
                                        .Append("/")
                                        .Append(RawDate.Substring(0, 4))
                                        .ToString();

                default: return "-1";
            }

        }

    }
}
