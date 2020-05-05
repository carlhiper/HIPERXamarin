using System;
using System.Collections.Generic;

namespace HIPER.Helpers
{
    public class RandomPhrases
    {
 

        public static string RandomCheerPhrase(string name)
        {
            List<string> cheerPhrases = new List<string>()
            {
                " Congratulate " + name + ".",
                " Give " + name + " a high five.",
                " Show your colleague appreciation.",
                " Post " + name + " and congratulate!",
                " " + name + " has been producing quite a bit lately.",
                " Show " + name + " you also can finish goals.",
                " Get on it and deliver."
            };


            Random rand = new Random();
            int value = rand.Next(0, cheerPhrases.Count - 1);

            return cheerPhrases[value];
        }



    }
}
