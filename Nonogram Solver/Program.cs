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
            Solver s = new Solver(b);
            b.Draw(new Point(25,8));
            s.Solve();
            b.Draw(new Point(25,8));
            Console.ReadLine();
        }
    }
}