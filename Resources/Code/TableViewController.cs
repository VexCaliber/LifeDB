﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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


    public static class TableViewController
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
            
            while(reader.Read()) //REVIEW ME!
            {

                for (int i = 0; i < columns; i++)
                {

                    if(i == 4 | i == 5)
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


                for(int i = 0; i < columns; i++ )
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

        
        public static void Deletify()
        {

            //So...I'm getting the wall of rejections from Indeed r/n and honestly...I'm so fxcking done...
            //this is just gonna be an outline because fxck it

            //find the deleted row
            //delete it from the collection / the view
            //adjust the row colors below to fit the even/odd background color alternation (cascade)
            //rollback the current row (currentRow--)
            //maybe update the DBCount
            //burn the hr ladies at the stake
            //transition to improve chances at a fxcking job to make enough to live a normal fxcking life
            //whip myself for the "crime" of being a white male
            //continue telling myself that everything I've learned over the course of life will actually pay off one day~
            //while the country continues to burn at the behest of feral psychopaths...
            //tell myself that I'll finally get ahold of a rasp-pi 4 for testing
            //set up the asyncs for intel to monitor the table meta and generate suggestions and recommendations in a line by line format
            //ruthlessly pummel anybody who demands a college degree for a job an 8th grader can do.
            
            ///I'm tired of working hard to get nowhere, whilst also being expected to give more and more...
            ///...and that's it for today...enjoy! ;)


        }


        public static void Update(Boolean fetchNewRows)
        {

            if(fetchNewRows == true) {

                SqlDb.DBCount(); //REQUIRED
                int startingRow = currentRow + 1;

                if (currentRow < SqlDb.lastCount) 
                    Generate(SqlDb.GetIdRange(startingRow, SqlDb.lastCount)); //call and grab result set :: incl/incl & Generate
                
            }
            
            Agitate(SqlDb.GetVisibleIdRange());

            Deletify();

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
                for(int i = 0; i < cells.Count; i++) ///CHANGED -- From .count-1 to count
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
                case 5: return reverted.Append(RawDate.Substring(2,1))
                                        .Append("/")
                                        .Append(RawDate.Substring(3,2))
                                        .Append("/")
                                        .Append(RawDate.Substring(0,2))
                                        .ToString();



                case 6: return reverted.Append(RawDate.Substring(2,2))
                                        .Append("/")
                                        .Append(RawDate.Substring(4,2))
                                        .Append("/")
                                        .Append(RawDate.Substring(0,2))
                                        .ToString();



                case 7: return reverted.Append(RawDate.Substring(4, 1)) //day
                                        .Append("/")
                                        .Append(RawDate.Substring(5, 2)) //month
                                        .Append("/")
                                        .Append(RawDate.Substring(0, 4)) //year
                                        .ToString();
                                 
                    

                case 8: return reverted.Append(RawDate.Substring(4,2))
                                        .Append("/")
                                        .Append(RawDate.Substring(6,2))
                                        .Append("/")
                                        .Append(RawDate.Substring(0,4))
                                        .ToString();

                default: return "-1";
            }
               
        }

    }


}
