using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LifeDB.Resources.Code
{
    public class CounterRotary<KType,VType>
    {

        private KType[] keys   = new KType[25]; //NOT NULL - UNIQUE 
        private VType[]? values = new VType[25]; //NULLABLE - NOT UNIQUE

        private int indexer = 0;

        public Boolean Add(KVP<KType, VType?> kvp)
        {

            //ADD SIZE CHECK!

            if (!KeyExists(kvp.GetKey())) //if key doesn't exist
            {
                keys[GetAndInc()] = kvp.GetKey();
                values[GetAndInc()] = kvp.GetValue();
                return true;
            }
            else
            {
                return false;
            }

        }

        public Boolean Add(KType k, VType? v)
        {

            //ADD SIZE CHECK!

            if (!KeyExists(k)) //if key doesn't exist
            {
                keys[GetAndInc()] = k;
                values[GetAndInc()] = v;
                return true;
            }
            else
            {
                return false;
            }

        }

        public void RemoveByIndex(int index)
        {

        }

        public void RemoveByKey(KType key)
        {

            KType[] KClone = new KType[Length()];
            VType[] VClone = new VType[Length()];

            Boolean tripped = false;

            for(int i = 0; i < Length(); i++)
            {

                if (keys[i].Equals(key)) tripped = true;

                if (tripped == false)
                {

                    KClone[i] = keys[i];
                    VClone[i] = values[i];
                    continue;

                }

                if (i++ < Length()-1)
                {
                    KClone[i] = keys[i++];
                    VClone[i] = values[i++];
                }

            }

            keys = KClone;
            values = VClone;

        }


        //Internal Use//

        private int Length()
        {
            return keys.Length;
        }

        private Boolean KeyExists(KType key)
        {

            if (keys.Contains(key)) 
                return true;
            else 
                return false;
        
        }

        private int GetAndInc()
        {

            int i = indexer;
            indexer++;

            return i;

        }

        

    }



}


