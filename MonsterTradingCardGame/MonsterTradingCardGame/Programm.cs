using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG
{
    class Programm
    {
        static void Main(string[] parameter)
        {
            Console.Write("abc");
            Random Dice = new Random();
            Console.WriteLine(Dice.Next(0, 0));
            Console.ReadLine();
            return;
        }
    }
}
