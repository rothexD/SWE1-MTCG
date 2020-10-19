using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG
{
    class Programm
    {
        static void Main(string[] parameter)
        {
            Console.WriteLine("abc");
            Random Dice = new Random(0);       
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(Dice.Next(0, 5));
            }          
            Console.ReadLine();




            return;
            
        }
    }
}
