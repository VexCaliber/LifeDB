using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;

namespace LifeDB.Resources.Code
{

    //add populate method for startup as a conv_method

    static class TableViewController
    {

        private static Table table;

        //RowGroupCollection contains the rows[]
        private static TableRowGroupCollection rowGroups;

        //Contains the columns[]
        private static TableColumnCollection tableColumns;
        
        //+++We could get the table, and skip init for columns and header row (just below me) and go one by one...ignore the below.
        //---We need to init() the columns and header row and inject the values 1 by 1 into the document (edit: this edit will be required for extension:::
        //for making this extensible, you'll need to name the flow doc in the ui, pass that instead of the table,
        //and build out a new table based on either user input or a string of fields from an outside db file)

        // 0 = Header Row
        // 1 = First Row
        private static int currentRow;
        private static int currentColumn;

        //pretend this doesn't exist yet...I've been down the rabbit hole going over every level in the intensely too deep bs for the table.
        //I could just be an idiot...could just do it array style...it is what it is...not set on strat yet...
        

        private static void AddRow()
        {
            //var style = Application.Current.MainWindow.Resources["ResDef.xaml"];
            //var styles = Application.Current.Resources["ResDef.xaml"];
            //var styl = Application.Current.Resources.FindName("RowA");


            //var t = Application.Current.FindResource("ResDef.xaml");
            //var m = t.GetType().FindMembers["RowA"];
            //var rd = new System.Windows.ResourceDictionary();
            //var ke = rd.Source = new Uri("ResDef.xaml");
            //var kd[0];

            TableRow rowTemplateA = (TableRow)Application.Current.Resources["RowA"]; // YES!
            TableRow rowTemplateB = (TableRow)Application.Current.Resources["RowB"]; // Why the hell was this so hard? 

            // #5
            // var r = new TableRow();
            //r.FindResource(resourceKey: "RowA");
            //r.SetResourceReference(DependencyProperty.Register("RowA", Application.Current.Resources.FindName("RowA").GetType,r.GetType()),r);
            table.RowGroups[0].Rows.Add(new TableRow());//
            table.RowGroups[0].Rows.Add(rowTemplateA);
            table.RowGroups[0].Rows.Add(rowTemplateB);
            /*
             * 
             * Ok...so now we've left only the columns and the header row.
             * Now...there is no limit on the row length...it'll grow infinitely
             * We'll go loose on te columns, for future need in extension, and
             * then, all we have to do is get it to create the new row based on the template
             * 
             */

            if (currentRow % 2 == 0)
            {

                var l = table.RowGroups[0].Rows[currentRow];
                l.Cells.Add(new TableCell(new Paragraph(new Run("Product"))));
                l.Cells.Add(new TableCell(new Paragraph(new Run("Product2"))));
                l.Cells.Add(new TableCell(new Paragraph(new Run("Product3"))));

            }
            else
            {
                var l = table.RowGroups[0].Rows[currentRow];
                l.Cells.Add(new TableCell(new Paragraph(new Run("Product"))));
                l.Cells.Add(new TableCell(new Paragraph(new Run("Product2"))));
                l.Cells.Add(new TableCell(new Paragraph(new Run("Product3"))));
            }

            //Thread.Sleep(10000);
            //table = new Table();
            //For new tables, we need to unload any decl. in the xaml

        }

        private static void GenerateRow()
        {

            TableRow rowTemplateA = (TableRow)Application.Current.Resources["RowA"];
            TableRow rowTemplateB = (TableRow)Application.Current.Resources["RowB"];

            // var l = table.RowGroups[0].Rows[currentRow];
            // l.Cells.Add(new TableCell(new Paragraph(new Run("Product"))));

            //table.RowGroups[0].Rows.Add(new TableRow());
            //table.RowGroups[0].Rows.Add(rowTemplateA);
            //table.RowGroups[0].Rows.Add(rowTemplateB);

            if (currentRow % 2 == 0)
            {
                table.RowGroups[0].Rows.Add(rowTemplateA);
                table.RowGroups[0].Rows.Add(rowTemplateB);
                var x = table.RowGroups[5].Rows[currentRow].Cells.ToArray();
                var y = x[0];
                var z = y.Blocks;
                var a = z.FirstBlock;
                //var b = a.
                
                ///I swear...this is just unreasonable...i know it's for flow, but there really needs to be a better abstraction.
                ///on the bright side...we can add a row now!  and it's a row on command
                ///but I do wonder why the 2nd one won't come up...possibly just brainmelted, but
                ///it may be a name conflict with the xaml...but it's a key, so hopefully it's not that...idk though

                //table.RowGroups[0].Rows[currentRow].Cells.Add(new TableCell(new Paragraph(new Run("Product"))));
                //table.RowGroups[0].Rows[currentRow].Cells.Insert(2,new TableCell(new Paragraph(new Run("Product"))));
                //table.RowGroups[0].Rows[currentRow].Cells.Prepend(new TableCell(new Paragraph(new Run("Product"))));

            }
            else
            {
                table.RowGroups[0].Rows.Add(rowTemplateB);
                table.RowGroups[0].Rows[currentRow].Cells.Add(new TableCell(new Paragraph(new Run("Product"))));
            }

        }


        public static void Init(Table t)
        {
           
            table = t;
            rowGroups = t.RowGroups;
            tableColumns = t.Columns;

            currentRow = 1;
            currentColumn = 0;

            GenerateRow();
           

        }

    }

/*                var x = table.RowGroups[0];
                var y = table.Columns[columnCount];
                var k = table.RowGroups[0].Rows[0];

                var o = table.RowGroups[0].Rows[1];
                    o.Cells[1].Add(new TableCell(new Paragraph(new Run("TEST"))));

                var l = table.RowGroups[0].Rows[rowCount];
                l.Cells[rowCount].Add(new TableCell(new Paragraph(new Run("Product"))));*/

    // List<TableCell<Paragraph<Run>>> l = new List<TableCell<Paragraph<Run>>>();

    //var z = x.Rows[y].Add(new TableCell(new Paragraph(new Run("Product"))));
    // var z = x.Rows[y].Add(new TableCellCollection().Add((new TableCell(new Paragraph(new Run("Product"))))));
    // var z = x.Rows[y].Add(new TableCellCollection(l.All));

    //var l = table.RowGroups[0].Rows[rowCount];
    //l.Cells[rowCount].Add(new TableCell(new Paragraph(new Run("Product"))));

    //So I realize I'm missing something, but jesus christ...O.O 
    /*
     * So we have a table
     * that table has rows and columns
     * but those are apart of groups?
     * and you can iterate and pass data in bulk?
     * but it must be in TableCells
     * and in the tablecells a paragraph
     * and in the paragraph a "run"
     * 
     * I feel like this is absolutely tarded >.>
     * 
     * We can build the table and set properties via the backend or frontend
     * I have the ResDef, that I want to cycle between a & b.
     * 
     * Ideally...we pass a list<string> and it auto builds the row and adds it.
     * Wanna do it high level loose as much as possible with this...
     * 
     * this has eaten this whole chunk of time...but, atleast you got the Pump method & overload
     * 
     * 
     */



}
