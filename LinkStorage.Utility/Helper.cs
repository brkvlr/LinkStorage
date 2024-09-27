using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Utility
{
    public static class Helper
    {
        public static string RandomPassword()
        {
            string randomString = "";
            char[] upperCase = "QWERTYUIOPASDFGHJKLZXCVBNM".ToCharArray();
            char[] lowerCase = "qwertyuiopasdfghjklzxcvbnm".ToCharArray();
            char[] number = "1234567890".ToCharArray();
            Random rnd = new Random();
            for (int i = 0; i < 7; i++)
            {

                int rndChoice = rnd.Next(1, 4);

                if (rndChoice == 1)
                {
                    int upperCaseRnd = rnd.Next(0, upperCase.Length);
                    randomString += upperCase[upperCaseRnd];
                }
                else if (rndChoice == 2)
                {
                    int lowerCaseRnd = rnd.Next(0, lowerCase.Length);
                    randomString += lowerCase[lowerCaseRnd];
                }
                else
                {
                    int numberRnd = rnd.Next(0, number.Length);
                    randomString += number[numberRnd];

                }



            }
            return randomString;
        }
        public static string RandomStringGenerator(int length)
        {
            string randomString = "";
            char[] upperCase = "QWERTYUIOPASDFGHJKLZXCVBNM".ToCharArray();
            char[] lowerCase = "qwertyuiopasdfghjklzxcvbnm".ToCharArray();
            char[] number = "1234567890".ToCharArray();
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {

                int rndChoice = rnd.Next(1, 4);

                if (rndChoice == 1)
                {
                    int upperCaseRnd = rnd.Next(0, upperCase.Length);
                    randomString += upperCase[upperCaseRnd];
                }
                else if (rndChoice == 2)
                {
                    int lowerCaseRnd = rnd.Next(0, lowerCase.Length);
                    randomString += lowerCase[lowerCaseRnd];
                }
                else
                {
                    int numberRnd = rnd.Next(0, number.Length);
                    randomString += number[numberRnd];

                }



            }
            return randomString;
        }
    }
}