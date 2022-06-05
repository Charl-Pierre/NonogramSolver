using System;
using System.Drawing;

namespace Nonogram_Solver
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello world");
            Board b = new Board("2,1:1,3:1,2:3:4:1", "1:5:2:5:2,1:2");
            //Board b = new Board("2, 1", "2:2:2:2");
            Solver s = new Solver(b);

            s.drawPos = new Point(50, 4);
            s.Solve(true);

            Console.ReadLine();
        }
    }
}