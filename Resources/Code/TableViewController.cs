using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace LifeDB.Resources.Code
{

    //add populate method for startup as a conv_method

    static class TableViewController
    {

        private static Table table;
        

        private static int rowCount;
        private static int columnCount = rowCount++;

        //pretend this doesn't exist yet...I've been down the rabbit hole going over every level in the intensely too deep bs for the table.
        //I could just be an idiot...could just do it array style...it is what it is...not set on strat yet...

        private static void AddRow()
        {
            if (rowCount % 2 == 0)
            {

                var x = table.RowGroups[0];
                var y = table.Columns[columnCount];
                var k = table.RowGroups[0].Rows[0];

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
            else
            {

            }
            

        }


        public static void Init(Table t)
        {
           
            table = t;
            rowCount = 2;
            
            AddRow();

        }

    }
}
