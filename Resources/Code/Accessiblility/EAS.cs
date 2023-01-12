using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeDB.Resources.Code.Accessiblility
{
    //Exception Audio System
    public static class EAS
    {

        private static Boolean ALLOW_EAS;

        public static void accept(Exception e)
        {

            //EDIT TO BE A BROAD AS POSSIBLE
            //CONSOLE.BEEP() X NUM OF TIMES BASED ON EXCEPTION PASSED
            //IDK WHY, BUT I LOVE DA BEEPS! \O.O/
            // ^_~ we like the stock


        }

        public static void EASON()
        {
            ALLOW_EAS = true;
        }

        public static void EASOFF()
        {
            ALLOW_EAS = false;
        }

    }
}
