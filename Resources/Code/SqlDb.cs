using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;

namespace LifeDB.Resources.Code
{

    public static class SqlDb
    {

        private static SQLiteConnection connection = Connect();

        public static SQLiteConnection Connect()
        {

            //new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True;");
            //Data Source = file://LifeDB//Resources//Files//Db//myDatabase.db

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

            //NOTE TO SELF: SET DEBUG EXECUTION TO THE LIFEDB directory --
            //And ..// seemed to work...hopefully that sticks, but idk...pathing is tarded sometimes :: Note...dropping //lifedb won't work
            SQLiteConnection sqlite_conn = new SQLiteConnection("Data Source = ..//LifeDB//Resources//Files//Db//myDatabase.db;" +
                                                                 "SQLITE_USE_URI = 1" +
                                                                 "Version = 3;" +
                                                                 "UseUTF16Encoding = true; " +
                                                                 "DateTimeFormat = UnixEpoch;" +
                                                                 exists +
                                                                 "Compress = True;");



            // Open the connection:
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

            //So...SQLite has a different (loose) type system...the below is a gen'd mySql, the original plan (mySql commands aren't 100% compatible O.~ it's always somethin' isn't it)
            /*
                string statement = "CREATE TABLE `myTable`(" +
                    "`id` INT UNSIGNED NOT NULL AUTO_INCREMENT," +
                    "`item_name` VARCHAR(256) NOT NULL," +
                    "`item_quantity` INT UNSIGNED NULL," +
                    "`item_category` VARCHAR(256) NULL," +
                    "`added` DATE NOT NULL DEFAULT (current_date())," +
                    "`expires` DATE NULL," +
                    "`limit` INT NULL," +
                    "PRIMARY KEY(`id`)," +
                    "UNIQUE INDEX `id_UNIQUE` (`id` ASC) VISIBLE)";
                                                                         */
            //id will auto-increment on missing/null vals...
            //No trailing commas :: keywords (eg. limit) are escaped w/ backticks (`)
            string statement = "CREATE TABLE myTable(" +
                "id INTEGER PRIMARY KEY NOT NULL, " +
                "item_name TEXT, " +
                "item_quantity INTEGER NULL, " +
                "item_category TEXT NULL, " +
                "added NUMERIC NOT NULL DEFAULT(CURRENT_DATE), " +
                "expires NUMERIC NULL, " +
                "`limit` INTEGER NULL " +
                ")";

            //Console.Beep();
            //Console.Beep();
            //Console.Beep();
            //Console.Beep();


            //id, item_name, item_quantity, item_category, added, expires, limit

            command = con.CreateCommand();
            command.CommandText = statement;
            command.ExecuteNonQuery();

        }


        public static Boolean Add(SqlPacket sqlPacket)
        {

            //item_name, item_quantity, item_category, added, expires, limit

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

            var keys = sqlPacket.GetKeys();
            var values = sqlPacket.GetValues();

            Func<String, String, String> MergeIdAssigned = (s1, s2) => s1 + " = " + s2;
            Func<String, String, String> MergilizeAssignments = (s1, s2) => s1 + " = '" + s2 + "', ";
            Func<String, String, String> MergilizeAssignmentsWOTrailingComma = (s1, s2) => s1 + " = '" + s2 + "' ";

            String ASSIGNED = "";
            StringBuilder ASSIGNMENTS = new();

            //Brain says this is ok...so I'm going with it...review me!
            int adjustedPairCount = ((keys.Count() - 1 + values.Count() - 1) / 2);
            for (int i = 0; i != adjustedPairCount; i++)
            {
                //SET ContactName = 'Alfred Schmidt', City = 'Frankfurt'

                if (i == 0)
                {
                    ASSIGNED = MergeIdAssigned(keys[i], values[i]);
                    continue;
                }

                if (i != adjustedPairCount - 1)
                    ASSIGNMENTS.Append(MergilizeAssignments(keys[i], values[i]));
                else
                    ASSIGNMENTS.Append(MergilizeAssignmentsWOTrailingComma(keys[i], values[i]));



            }

            //WHERE can take a number or a 'string', we need to test parse the first value...
            //we won't need it for the basic usage, but for future extentions that will use something else...we must 
            //but sql commands are strings...so it should parse it on its end...I'll skip and we'll see what happens

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

            var key = sqlPacket.GetKeys();
            var value = sqlPacket.GetValues();

            Func<String, String, String> MergeIdAssigned = (s1, s2) => s1 + " = " + s2;

            String ASSIGNED = MergeIdAssigned(key[0], value[0]);

            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();
                command.CommandText = "DELETE FROM myTable WHERE " + ASSIGNED;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Remove(SqlPacket sqlPacket)");
                return false;
            }

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
                    Console.WriteLine(e.ToString() + " @SqlDB.Run()");
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
                    Console.WriteLine(e.ToString() + " @SqlDB.Run()");
                    return false;
                }

                //need to update table!
                return true;

            }


        }

        //Reserved for testing & TableViewController
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
                Console.WriteLine(e.ToString() + " @SqlDB.Edit(SqlPacket sqlPacket)");
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



    //EXPERIMENTATION
    //Edit: Mmmmm, yeees...look at all my pokemon cards... (✿◕‿◕✿)
    public class SqlPacket
    {
       
        //need a higher order system for these arbitrary nums 
        //long SqlPacketId;
        //long SqlPacketVersion;

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
                throw new FormatException("You must provide values as k,v,k,v :: if a value doesn't exist, pass null w/o quotes");
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
                Console.WriteLine(pairs[k], pairs[v]);
            }

            Console.WriteLine("SqlPacket Build Successful! =)");

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


        // sb.Append('\''); values need their single quotes...REVIEW ME! -- Using if's, without quotes for nulls, it'll throw in Sql
        public IList<String> GetValues()
        {
            IList<String> ValueList = new List<String>();
            StringBuilder sb = new();

            foreach (KVP<String, String> pair in Mappings)
            {
                sb.Append('\'');

                if (pair.GetValue() == null)
                    sb.Append(' ');
                else
                    sb.Append(pair.GetValue());
                
                sb.Append('\'');

                ValueList.Add(sb.ToString());
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


        // sb.Append('\''); values need their single quotes...REVIEW ME! -- Using if's, without quotes for nulls, it'll throw in Sql
        public String GetValueCSVString()
        {
            StringBuilder sb = new StringBuilder();
            int counter = 1;

            foreach (KVP<String, String> pair in Mappings)
            {
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

        /*Duplikey Method? :: For sh_ts and giggles?
        //(Immediate implication: 1 key many vals, secondary...would serve as an inline multidimentional arraylistmap for building many sqlpackets on interation)
        //Would be good to have an abstraction above this...call it a handler or something, but regardless...it could hold an array of sqlpackets, duplikey or otherwise
        //it'd also get the responsibility of scheduling/(multithreading) the writes
        //[could possibly require null keys after the first to signify 1 key many vals]*/

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



    }

       

  

}


