using System;
using System.Drawing;

namespace Nonogram_Solver
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello world");
            //Board b = new Board("2,1:1,3:1,2:3:4:1", "1:5:2:5:2,1:2");
            Board b = new Board("3,3,1:3,2,1:2,2,1", "2:2:2:2:2:2:2:2:3");
            b.Grid[1, 1] = Cell.Full;
            b.Draw(new Point(25,8));
            
            Solver s = new Solver(b);
            s.Solve();
            Console.ReadLine();
        }
    }
}