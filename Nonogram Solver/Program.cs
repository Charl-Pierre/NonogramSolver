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
            Board b = new Board(
                "8:7,3:16:11,4:13,2:14,2:18:8,4:6,4:5,5:4,2,2:4,3,1:3,2,1:3,2:3,2:2,1,4:2,1,4:2,1,4:3,1,4:5,4:11:10:5:5:6", 
                "1:2:2:4:11:13:15:9,3:8,3:7,5,2:7,3,4:6,2,3:8,3,2:13,2:10,2:5,5,3:4,1,4,6:1,2,2,8:3,1,10:3,1,4,4:2,1,2,3:2,1,2:2,1:2,1:1");
            Solver s = new Solver(b);

            //s.drawPos = new Point(15, 6);
            s.Solve(true);
            Console.ReadLine();
        }
    }
}