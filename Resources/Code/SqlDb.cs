using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design.Serialization;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LifeDB.Resources.Code
{

    public static class SqlDb
    {
        public static int lastCount { get; private set; }


        //------------------------------//


        private static SQLiteConnection connection = Connect();


        public static SQLiteConnection Connect()
        {

            Boolean genTable;
            String exists;
            if (File.Exists("..//LifeDB//Resources//Files//Db//myDatabase.db"))
            {
                exists = "New = False";
                genTable = false;
            }
            else
            {
                exists = "New = True";
                genTable = true;
            }

           
            SQLiteConnection sqlite_conn = new SQLiteConnection("Data Source = ..//LifeDB//Resources//Files//Db//myDatabase.db;" +
                                                                 "SQLITE_USE_URI = 1" +
                                                                 "Version = 3;" +
                                                                 "UseUTF16Encoding = true; " +
                                                                 "DateTimeFormat = UnixEpoch;" +
                                                                 exists +
                                                                 "Compress = True;");


            try
            {
                sqlite_conn.Open();
                if (genTable == true) CreateDefaultTable(sqlite_conn);
                Console.WriteLine("Connected!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDb.Connect()");
            }



            return sqlite_conn;

        }


        public static void Reconnect(SQLiteConnection con)
        {

            try
            {
                con.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDb.Reconnect");
            }

        }


        //------------------------------//


        private static void CreateDefaultTable(SQLiteConnection con)
        {

            SQLiteCommand command;

            string statement = "CREATE TABLE myTable(" +
                "id INTEGER PRIMARY KEY NOT NULL, " +
                "item_name TEXT, " +
                "item_quantity INTEGER NULL, " +
                "item_category TEXT NULL, " +
                "added NUMERIC NULL, " +
                "expires NUMERIC NULL, " +
                "`limit` INTEGER NULL " +
                ")";


            command = con.CreateCommand();
            command.CommandText = statement;
            command.ExecuteNonQuery();

        }


        public static Boolean Add(SqlPacket sqlPacket)
        {

            //id, item_name, item_quantity, item_category, added, expires, limit

            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();

                command.CommandText = "INSERT INTO myTable (" + sqlPacket.GetKeyCSVString() + ") " +
                                                    "VALUES (" + sqlPacket.GetValueCSVString() + ")";

                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(SqlPacket sqlPacket)");
                return false;
            }

            return true;

        }


        public static Boolean Edit(SqlPacket sqlPacket)
        {
            
            ///var keys = sqlPacket.GetKeys();
            ///var values = sqlPacket.GetValues();

            var mappings = sqlPacket.GetMappings();

            Func<String, String, String> MergeIdAssigned = (s1, s2) => s1 + " = " + s2;
            Func<String, String, String> MergilizeStringLiteralAssignments = (s1, s2) => s1 + " = '" + s2 + "', ";
            Func<String, String, String> MergilizeNonStringLiteralAssignments = (s1, s2) => s1 + " = " + s2 + ", ";
            //Func<String, String, String> MergilizeStringLiteralAssignmentsWOTrailingComma = (s1, s2) => s1 + " = '" + s2 + "' ";
            //Func<String, String, String> MergilizeNonStringLiteralAssignmentsWOTrailingComma = (s1, s2) => s1 + " = " + s2 + " ";

            //--------------------------------//

            String ASSIGNED = "";
            StringBuilder ASSIGNMENTS = new();

            //--------------------------------//

            //int pairCount = ((keys.Count()-1 + values.Count()-1) / 2);//((keys.Count() - 1 + values.Count() - 1) / 2); //index adjustment
            for (int i = 0; i < mappings.Count; i++) //i < pairCount
            {
                
                //The Null Purge
                if (mappings[i].GetValue() == null | mappings[i].GetValue() == "" | mappings[i].GetValue() == " ")
                {
                    mappings.RemoveAt(i);
                    continue;
                }

                //The WHERE
                if (mappings[i].GetKey() == "id")
                {
                    ASSIGNED = MergeIdAssigned(mappings[i].GetKey(), mappings[i].GetValue());
                    continue;
                }

                //The Value Chain
                switch (mappings[i].GetKey())
                {
                    case "item_name":
                        ASSIGNMENTS.Append(MergilizeStringLiteralAssignments(mappings[i].GetKey(), mappings[i].GetValue()));
                        break;

                    case "item_quantity":
                        ASSIGNMENTS.Append(MergilizeNonStringLiteralAssignments(mappings[i].GetKey(), mappings[i].GetValue()));
                        break;

                    case "item_category": 
                        ASSIGNMENTS.Append(MergilizeStringLiteralAssignments(mappings[i].GetKey(), mappings[i].GetValue()));
                        break;

                    case "added" or "expires":
                        DateOnly ODate;
                        Boolean dateified = DateOnly.TryParse(mappings[i].GetValue(), out ODate);
                        if (dateified == true)
                        { 
                            String tmp = ODate.Year + "" + ODate.Month + "" + ODate.Day;
                            int numerified = Int32.Parse(tmp);
                            ASSIGNMENTS.Append(MergilizeNonStringLiteralAssignments(mappings[i].GetKey(), numerified.ToString()));
                            break;
                        }
                        else
                        {
                            mappings.RemoveAt(i);
                            break;
                        }

                    case "`limit`":
                        ASSIGNMENTS.Append(MergilizeNonStringLiteralAssignments(mappings[i].GetKey(), mappings[i].GetValue()));
                        break;


                }


            }


            //The Backup Where
            if (ASSIGNED == "" | ASSIGNED == " " | ASSIGNED == null)
            {
                //get the first assignment and use as backup where
                var tmp = ASSIGNMENTS.ToString().Split(',')[0];
                ASSIGNED = tmp.Remove(','); //just in case
            }

            
            //Remove Trailing Comma (but not space) /////////////////////////////////FIX ME! :: failure on using only id & limit.    
            if (true)
            {
                ASSIGNMENTS.Remove(ASSIGNMENTS.Length-2, 1);
            }


            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();
                // Count() starts at 1 :: Index[] starts at 0 
                // var k = Mergilizer(keys[0], values[0]);

                command.CommandText = "UPDATE myTable " + "SET " + ASSIGNMENTS + "WHERE " + ASSIGNED;

                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Edit(SqlPacket sqlPacket)");
                return false;
            }

            return true;
            
            
        }


        //DEFAULT IS REMOVE BY ID...
        //Should take every other value except expressions, but if written right...it may go through
        public static Boolean Remove(SqlPacket sqlPacket)
        {

            var mappings = sqlPacket.GetMappings(); //a type and a value

            Func<String, String, String> MergilizeStringLiteralAssignmentsWOTrailingComma = (s1, s2) => s1 + " = '" + s2 + "' ";
            Func<String, String, String> MergilizeNonStringLiteralAssignmentsWOTrailingComma = (s1, s2) => s1 + " = " + s2 + " ";

            String ASSIGNED = "";

            for (int i = 0; i < mappings.Count; i++) //i < pairCount
            {

                //The Null Purge
                if (mappings[i].GetValue() == null | mappings[i].GetValue() == "" | mappings[i].GetValue() == " ")
                {
                    throw new Exception("Invalid Date");
                }

                //The Value Chain
                switch (mappings[i].GetKey())
                {
                    case "id":
                        ASSIGNED = MergilizeNonStringLiteralAssignmentsWOTrailingComma(mappings[i].GetKey(), mappings[i].GetValue());
                        break;

                    case "item_name":
                        ASSIGNED = MergilizeStringLiteralAssignmentsWOTrailingComma(mappings[i].GetKey(), mappings[i].GetValue());
                        break;

                    case "item_quantity":
                        ASSIGNED = MergilizeNonStringLiteralAssignmentsWOTrailingComma(mappings[i].GetKey(), mappings[i].GetValue());
                        break;

                    case "item_category":
                        ASSIGNED = MergilizeStringLiteralAssignmentsWOTrailingComma(mappings[i].GetKey(), mappings[i].GetValue());
                        break;

                    case "added" or "expires":
                        DateOnly ODate;
                        Boolean dateified = DateOnly.TryParse(mappings[i].GetValue(), out ODate);
                        if (dateified == true)
                        {
                            String tmp = ODate.Year + "" + ODate.Month + "" + ODate.Day;
                            int numerified = Int32.Parse(tmp);
                            ASSIGNED = MergilizeNonStringLiteralAssignmentsWOTrailingComma(mappings[i].GetKey(), numerified.ToString());
                            break;
                        }
                        else
                        {
                            throw new Exception("Invalid Date");
                        }

                    case "`limit`":
                        ASSIGNED = MergilizeNonStringLiteralAssignmentsWOTrailingComma(mappings[i].GetKey(), mappings[i].GetValue());
                        break;


                }

            }
                    
            SQLiteCommand command;

            try
                {
                    //SQLiteCommand command;
                    command = SqlDb.connection.CreateCommand();
                    command.CommandText = "DELETE FROM myTable WHERE " + ASSIGNED;
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString() + " @SqlDB.Remove(SqlPacket sqlPacket)");
                    return false;
                }

                StockViewController.parseDelete(command.CommandText);
                return true;

        }


        //DANGER -- UNSANITIZED INPUT
        public static Boolean Run(String userCommand)
        {

            if (userCommand == null | userCommand == " ") { return false; }

            if(!userCommand.Contains("SELECT") | !userCommand.Contains("select"))  
            {
                try
                {
                    SQLiteCommand command;
                    command = SqlDb.connection.CreateCommand();

                    command.CommandText = userCommand;

                    command.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString() + " @SqlDB.Run(String userCommand)");
                    return false;
                }

                //need to update table!
                return true;

            }
            else 
            {

                try
                {
                    SQLiteCommand command;
                    command = SqlDb.connection.CreateCommand();

                    command.CommandText = userCommand;

                    command.ExecuteReader(); //I guess this will work...dunno?
                    

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString() + " @SqlDB.Run(String userCommand)");
                    return false;
                }

                //need to update table!
                return true;

            }


        }


        //Reserved for internal use
        public static SQLiteDataReader SelectAll()
        {
           
            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();
                command.CommandText = "Select * from myTable";
                SQLiteDataReader DataReader = command.ExecuteReader();
                return DataReader;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.SelectAll()");
            }

            return null;

        }


        public static void DBCount()
        {

            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand(); 
                command.CommandText = "Select COUNT(id) from myTable"; 
                SQLiteDataReader DataReader = command.ExecuteReader(); 
                DataReader.Read();  
                lastCount = (Int32)DataReader.GetInt64(0); 

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.DBCount()");
            }

        }


        public static SQLiteDataReader GetIdRange(int startId, int endId)
        {

            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();
                command.CommandText = "SELECT * from myTable WHERE id BETWEEN "+startId+" AND "+endId;
                SQLiteDataReader DataReader = command.ExecuteReader();
                return DataReader;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.GetIdRange(int startId, int endId)");
            }

            return null;

        }


        public static SQLiteDataReader GetVisibleIdRange()
        {

            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();
                command.CommandText = "SELECT * from myTable WHERE id BETWEEN " + 1 + " AND " + StockViewController.currentRow; //current row is last known written row.
                SQLiteDataReader DataReader = command.ExecuteReader();
                return DataReader;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.GetVisibleIdRange()");
            }

            return null;

        }


        //------------------------------//


        //Conv_Methods
        public static void Pump(params String[] kvps)
        {

            SqlDb.Add(new SqlPacket().Build(kvps));

        }


        public static void Pump(Enum Command, params String[] kvps)
        {
            if (Command.Equals(SqlDb.Command.add))
                SqlDb.Add(new SqlPacket().Build(kvps));

            if (Command.Equals(SqlDb.Command.edit))
                SqlDb.Edit(new SqlPacket().Build(kvps));

            if (Command.Equals(SqlDb.Command.remove))
                SqlDb.Remove(new SqlPacket().Build(kvps));

        }


        public enum Command {
            add,
            edit,
            remove   
        }


    }


    public class SqlPacket
    {
       

        IList<KVP<String, String>> Mappings = new List<KVP<String, String>>(); 
    

        public SqlPacket() 
        { 
            //SqlPacketId++; //Make this & class threadsafe
        }  


        public void Add(String key, String? value)
        {
            //pairs.Add(new KVP("key", "value"));
            Mappings.Add(new KVP<String, String>(key, value));
             
        }


        public SqlPacket Build(String[] pairs)
        {
            
            if(pairs.Length % 2 != 0)
            {
                throw new FormatException("Just contact VexCaliber on GitHub...");
            }

            /*
               a,b,c,d :: content
               0,1,2,3 :: index
               1,2,3,4 :: length
               k,v,k,v :: process
                                  */

            for(int k = 0, v = k+1; k <= pairs.Length-1; k += 2, v = k+1)
            {
                Add(pairs[k], pairs[v]);
                //Console.WriteLine(pairs[k], pairs[v]);
            }

            //Console.WriteLine("SqlPacket Build Successful! =)");

            return this;

        }


        public void RemoveKey(String key)
        {
            var copy = Copy(Mappings);

            foreach (KVP<String, String> pair in copy)
            {
                if(pair.GetKey() == key)
                {
                    Mappings.Remove(pair);
                    break;

                }
            }

        }


        //Removes Pair...fix this...
        public void RemoveValue(String value)
        {
            var copy = Copy(Mappings);

            foreach (KVP<String, String> pair in copy)
            {
                if (pair.GetValue() == value)
                {
                    Mappings.Remove(pair);
                    break;

                }
            }

        }


        public void RemovePair(String key)
        {
            RemoveKey(key);
        }


        public IList<KVP<String,String>> Copy(IList<KVP<String,String>> map) 
        {

            var copy = new List<KVP<String,String>>();

            foreach(KVP<String, String> pair in Mappings)
            {
                copy.Add(pair);
            }

            return copy;

        }


        public IList<String> GetKeys()
        {
            IList<String> KeyList = new List<String>();

            foreach(KVP<String, String> pair in Mappings)
            {
                KeyList.Add(pair.GetKey());
            }

            return KeyList;

        }


        public IList<String> GetValues()
        {
            IList<String> ValueList = new List<String>();
            //StringBuilder sb = new();

            foreach (KVP<String, String> pair in Mappings)
            {
                
                ValueList.Add(pair.GetValue());
                //sb.Append('\'');

                //if (pair.GetValue() == null)
                //    sb.Append(' ');
                //else
                   // sb.Append(pair.GetValue());
                
                //sb.Append('\'');

                //ValueList.Add(sb.ToString());
            }

            return ValueList;
        }


        public String GetKeyCSVString()
        {
            StringBuilder sb = new StringBuilder();
            int counter = 1;

            foreach (KVP<String, String> pair in Mappings)
            {
                sb.Append(pair.GetKey());
                if(counter < Mappings.Count()) sb.Append(',');           
                counter++;
            }

            return sb.ToString();

        }

       
        public String GetValueCSVString()
        {
            StringBuilder sb = new StringBuilder();
            int counter = 1;

            //Cascading SQLDB Type Conversion Recovery
            foreach (KVP<String, String> pair in Mappings)
            {

                ///Path A: value is int 

                //if null, check if it's in dates places correlated to counter, if true trydateonly, else apply null...if not null, fallthough to try int32
                if (pair.GetValue() == null | pair.GetValue() == "" | pair.GetValue() == " ")
                    {
                        
                        if (counter == 5 | counter == 6) goto TryDateOnly;
                        
                        sb.Append("NULL");
                        if (counter < Mappings.Count()) sb.Append(',');
                        counter++;
                        continue;

                    }
                    


                
       // Standard Int32 Option         
       TryInt32:try 
                {
                    
                    Int32 Oint;
                    Boolean intified = Int32.TryParse(pair.GetValue(), out Oint);

                    //if int, stringify it and continue, else trydateonly
                    if (intified == true)
                    {

                        sb.Append(Oint.ToString());

                        if (counter < Mappings.Count()) sb.Append(',');

                        counter++;

                        continue;
                    
                    }else goto TryDateOnly;

                }
                catch (Exception e)
                {
                    //SWALLOWED
                }


    // Standard DateOnly Option            
    TryDateOnly:try
                {
                   
                    //new default date setter for added -- always do things imperatively >.> never trust others to work right
                    if(counter == 5 && pair.GetValue() == null | pair.GetValue() == "")
                    {
                        var date = System.DateOnly.FromDateTime(DateTime.Now);
                        pair.SetValue(date.ToString());
                    }

                    DateOnly ODate;
                    Boolean dateified = DateOnly.TryParse(pair.GetValue(), out ODate);

                    //if a date, pass the format (note: format lost) else trystring
                    if (dateified == true)
                    {
                        //var fix = ODate.ToString(); 
                        //fix.Replace(@"/",@"//"); //ESCAPE FORWARD SLASH
                        ///Ok, so we're keeping dates numeric, and we'll pass the nums in as it needs
                        ///we'll then rebuild the date on the read
                        int numerified;
                        String tmp = "";

                        tmp += ODate.Year + "" + ODate.Month + "" + ODate.Day;
                        numerified = Int32.Parse(tmp);

                        ///MessageHandler.userConsole.Text += numerified + " ";
                        ///MessageHandler.userConsole.Text += tmp + " ";

                        sb.Append(numerified); //MessageHandler.userConsole.Text += " " + ODate.ToString() + " ";////////////////////////////////////////////////////////////////////

                        if (counter < Mappings.Count()) sb.Append(',');

                        counter++;

                        continue;

                    }
                    else goto TryString;


                }
                catch (Exception e2)
                {
                    //SWALLOWED
                }


      //Standard String Option (original base choice / the OG code)
      //Last ditch, use ' to surround string if not null, making a string literal, else, pass a space char for default
      TryString:

                sb.Append('\''); 

                if (pair.GetValue() == null)
                    sb.Append(' ');
                else
                    sb.Append(pair.GetValue());

                sb.Append('\'');

                if (counter < Mappings.Count()) sb.Append(',');
                counter++;
            }

            return sb.ToString();

        }


        public IList<KVP<String, String>> GetMappings()
        {
            return Mappings;
        }


    }


    public class KVP<K,V>
    {
        private K Key;
        private V? Value;

        public KVP(K k, V v)
        {
            Key = k;
            Value = v;
        }

        public K GetKey()
        {
            return Key;
        }

        public V GetValue()
        {
            return Value;
        }

        public void SetValue(V? newValue)
        {
            this.Value = newValue;
        }

    }

       

  

}


