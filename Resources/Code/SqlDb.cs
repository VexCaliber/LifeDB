using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;

namespace LifeDB.Resources.Code
{ 



    public class SqlDb
    {

        private static SQLiteConnection connection;

        public static SQLiteConnection Connect()
        {

            //new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True;");
            //file://LifeDB//Resources//Files//Db//Base.db

            SQLiteConnection sqlite_conn =  new SQLiteConnection("Data Source=file://LifeDB//Resources//Files//Db//Base.db; " +
                                                                 "Version = 3; " +
                                                                 "UseUTF16Encoding = true; " +
                                                                 "DatTimeFormat = UnixEpoch;" +
                                                                 "New = True; " +
                                                                 "Compress = True;");
            
            // Open the connection:
            try
            {
                 sqlite_conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDb.Connect");
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

        ///

        static void CreateDefaultTable(SQLiteConnection con)
        {

            SQLiteCommand command;
            //string Createsql = "CREATE TABLE SampleTable(Col1 VARCHAR(20), Col2 INT)";
            //string Createsql1 = "CREATE TABLE SampleTable1(Col1 VARCHAR(20), Col2 INT)";
            //command = con.CreateCommand();
            //command.CommandText = Createsql;
            //command.ExecuteNonQuery();
            //command.CommandText = Createsql1;
            //command.ExecuteNonQuery();

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

            //id, item_name, item_quantity, item_category, added, expires, limit

            command = con.CreateCommand();
            command.CommandText = statement;
            command.ExecuteNonQuery();

        }

        /*
        static void InsertData(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            
            sqlite_cmd.CommandText = "INSERT INTO SampleTable (Col1, Col2) VALUES('Test Text ', 1); ";
            sqlite_cmd.ExecuteNonQuery();  
         
            sqlite_cmd.CommandText = "INSERT INTO SampleTable (Col1, Col2) VALUES('Test1 Text1 ', 2); ";
            sqlite_cmd.ExecuteNonQuery();   
         
            sqlite_cmd.CommandText = "INSERT INTO SampleTable (Col1, Col2) VALUES('Test2 Text2 ', 3); ";
            sqlite_cmd.ExecuteNonQuery();  
         
            sqlite_cmd.CommandText = "INSERT INTO SampleTable1 (Col1, Col2) VALUES('Test3 Text3 ', 3); ";
            sqlite_cmd.ExecuteNonQuery();

        }
        */


        public static Boolean Add(SqlPacket sqlPacket)  
        {
            
            //item_name, item_quantity, item_category, added, expires, limit


            //SqlString keys = new SqlString(new String("item_name,item_quantity,item_category,added,expires,limit"));
            //SqlString values = PacketValuesParser(sqlPacket);

            //tell me...i didn't need to convert all the string to sqlStrings >.< :: dude...
            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();

                command.CommandText = "INSERT INTO myTable ("+sqlPacket.GetKeyCSVString()+") " +
                                                    "VALUES ("+sqlPacket.GetValueCSVString()+")";

                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(SqlPacket sqlPacket)");
                return false;
            }

            return true;
            
        }

        //NOT DONE!
        public static Boolean Edit(SqlPacket sqlPacket)
        {

            var keys = sqlPacket.GetKeys();
            var values = sqlPacket.GetValues();

            Func<String, String, String> MergeIdAssigned = (s1,s2) => s1 + " = " + s2;
            Func<String, String, String> MergilizeAssignments = (s1, s2) => s1 + " = '" + s2 + "', ";
            Func<String, String, String> MergilizeAssignmentsWOTrailingComma = (s1, s2) => s1 + " = '" + s2 + "' ";

            String ASSIGNED = ""; 
            StringBuilder ASSIGNMENTS = new();

            //Brain says this is ok...so I'm going with it...review me!
            int adjustedPairCount = ((keys.Count()-1 + values.Count()-1) / 2);
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
                                    

                                    //id will be key 0, value 0 :: need substring of keys & values of 0-1 in/ex
                                    //we need a sneaky way back to avoiding going over the whole csv and splitting
                                    //OK...method instead of CSV...it'll +
                                    // 
                                    // Ok...We'll call Mergilizer on 1 in getKeys & get Values to get the WHERE 
                                    //
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(item_name)");
                return false;
            }

            return true;

        }















        /*

        //Apologies to anyone who sees this...I'm still getting used to working with sharp...O.O i realize an IList<<,>> would've been the better choice to build but >.> what can ya' do?
        public class SqlPacket
        {
            private string?         _item_name       = null;
            private int?            _item_quantity   = null;
            private string?         _item_category   = null;
            private DateOnly?       _added           = null;
            private DateOnly?       _expires         = null;
            private int?            _limit           = null;


            public string? item_name { get => _item_name; set => _item_name = value; }
            public int? item_quantity { get => _item_quantity; set => _item_quantity = value; }
            public string? item_category { get => _item_category; set => _item_category = value; }
            public DateOnly? added { get => _added; set => _added = value; }
            public DateOnly? expires { get => _expires; set => _expires = value; }
            public int? limit { get => _limit; set => _limit = value; }

        }

        
        public static SqlString PacketValuesParser(SqlPacket sqlPacket)
        {

            Object[] values = new object[] { sqlPacket.item_name, sqlPacket.item_quantity, sqlPacket.item_category, sqlPacket.added, sqlPacket.expires, sqlPacket.limit };

            //If any date is actually set (not null), transform it to sqlstring and then to string; place in array
            if (sqlPacket.added != null)
            {              
                values[3] = new SqlDateTime(sqlPacket.added.Value.Year, sqlPacket.added.Value.Month, sqlPacket.added.Value.Day).ToString();
            }

            if (sqlPacket.expires != null) {
                values[4] = new SqlDateTime(sqlPacket.expires.Value.Year, sqlPacket.expires.Value.Month, sqlPacket.expires.Value.Day).ToString();
            }


           

            
            StringBuilder sb = new();
            
            //Hopefully, it will append 'null' as needed, but we may have to return here >.> :: this is more complex than I thought...so much variability...
            //worst case scenario, i can just force the issue
            for (int i = 0; i < values.Length; i++)
            {
                sb.Append(values[i]);
                if (i != values.Length) sb.Append(',');
            }


            return new SqlString(sb.ToString());

        }
        


        /*Please ignore the concentrated autism below >.> I..uhm...tend to think a little too imperatively O.o*/
        //INSERT :: Settable>> item_name, item_quantity, item_category, added, expires, limit
        /*
        static Boolean Add(string item_name) 
        {
            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();

                command.CommandText = "INSERT INTO myTable (item_name) VALUES ("+item_name+")";

            }catch(Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(item_name)");
                return false;
            }

            return true;

        }

        static Boolean Add(string item_name, string item_quantity)
        {
            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();

                command.CommandText = "INSERT INTO myTable (item_name, item_quantity) VALUES ("+item_name+","+item_quantity+")";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(item_name, item_quantity)");
                return false;
            }

            return true;

        }

        static Boolean Add(string item_name, string item_quantity, string item_category)
        {
            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();

                command.CommandText = "INSERT INTO myTable (item_name, item_quantity, item_category) VALUES ("+item_name+","+item_quantity+","+item_category+")";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(item_name, item_quantity, item_category)");
                return false;
            }

            return true;

        }

        static Boolean Add(string item_name, string item_quantity, string item_category, DateOnly added)
        {
           
            // YYYY/MM/DD
            SqlDateTime SqlifiedDate = new SqlDateTime(added.Year, added.Month, added.Day);     

            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();

                command.CommandText = "INSERT INTO myTable (item_name, item_quantity, item_category, added) VALUES ("+item_name+","+item_quantity+","+item_category+","+SqlifiedDate+")";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(item_name, item_quantity, item_category, added)");
                return false;
            }

            return true;

        }

        static Boolean Add(string item_name, string item_quantity, string item_category, DateOnly added, DateOnly expires)
        {

            // YYYY/MM/DD
            SqlDateTime SqlifiedAddedDate = new SqlDateTime(added.Year, added.Month, added.Day);
            SqlDateTime SqlifiedExpiresDate = new SqlDateTime(expires.Year, expires.Month, expires.Day);

            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();

                command.CommandText = "INSERT INTO myTable (item_name, item_quantity, item_category, added, expires) VALUES (" + item_name + "," + item_quantity + "," + item_category + "," + SqlifiedAddedDate + "," + SqlifiedExpiresDate + ")";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(item_name, item_quantity, item_category, added, expires)");
                return false;
            }

            return true;

        }

        static Boolean Add(string item_name, string item_quantity, string item_category, DateOnly added, DateOnly expires, int limit)
        {

            // YYYY/MM/DD
            SqlDateTime SqlifiedAddedDate = new SqlDateTime(added.Year, added.Month, added.Day);
            SqlDateTime SqlifiedExpiresDate = new SqlDateTime(expires.Year, expires.Month, expires.Day);

            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();

                command.CommandText = "INSERT INTO myTable (item_name, item_quantity, item_category, added, expires, limit) VALUES (" + item_name + "," + item_quantity + "," + item_category + "," + SqlifiedAddedDate + "," + SqlifiedExpiresDate + "," + limit + ")";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(item_name, item_quantity, item_category, added, expires, limit)");
                return false;
            }

            return true;

        }

        //ALT >> added is optional, but not null.  If not included, the db will auto gen this field...
        //Side note: this is a really dumb way to do this >.> | all the other fields are optional, so this chaining is compounding...what is name isn't included, but everything else is?
        //Why am I so autsy?  I really must do K/V pairs to ease this b/s  Let's forget I did this ;-)

        static Boolean Add(string item_name, string item_quantity, string item_category, DateOnly expires, int limit)
        {

            // YYYY/MM/DD    
            SqlDateTime SqlifiedExpiresDate = new SqlDateTime(expires.Year, expires.Month, expires.Day);

            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();

                command.CommandText = "INSERT INTO myTable (item_name, item_quantity, item_category, expires, limit) VALUES (" + item_name + "," + item_quantity + "," + item_category + "," + SqlifiedExpiresDate + "," + limit + ")";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(item_name, item_quantity, item_category, expires, limit)");
                return false;
            }

            return true;

        }

        static Boolean Add(string item_name, string item_quantity, string item_category, int limit)
        {

            // YYYY/MM/DD    
            

            try
            {
                SQLiteCommand command;
                command = SqlDb.connection.CreateCommand();

                command.CommandText = "INSERT INTO myTable (item_name, item_quantity, item_category, limit) VALUES (" + item_name + "," + item_quantity + "," + item_category + "," + limit + ")";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " @SqlDB.Add(item_name, item_quantity, item_category, limit)");
                return false;
            }

            return true;

        }
        */

        //==============================//
         
       
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

        public void Build(String[] pairs)
        {
            //Was right to check this...length starts count at 1 in sharp
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

            for(int k = 0, v = k+1; v <= pairs.Length-1; k += 2)
            {
                Add(pairs[k], pairs[v]);
            }

            Console.WriteLine("SqlPacket Build Successful! =)");

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
      
        public IList<String> GetValues()
        {
            IList<String> ValueList = new List<String>();

            foreach (KVP<String, String> pair in Mappings)
            {
                ValueList.Add(pair.GetValue());
            }

            return ValueList;
        }

        public String GetKeyCSVString()
        {
            StringBuilder sb = new StringBuilder();
            int counter = 0;

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
            int counter = 0;

            foreach (KVP<String, String> pair in Mappings)
            {
                sb.Append(pair.GetValue());
                if (counter < Mappings.Count()) sb.Append(',');
                counter++;
            }

            return sb.ToString();

        }

        //Duplikey Method? :: For sh_ts and giggles?
        //(Immediate implication: 1 key many vals, secondary...would serve as an inline multidimentional arraylistmap for building many sqlpackets on interation)
        //Would be good to have an abstraction above this...call it a handler or something, but regardless...it could hold an array of sqlpackets, duplikey or otherwise
        //it'd also get the responsibility of scheduling/(multithreading) the writes
        //[could possibly require null keys after the first to signify 1 key many vals]

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


