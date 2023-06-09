using System;
using System.Collections.Generic;
using System.Drawing;

namespace Nonogram_Solver
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, (string, string)> Puzzles = new Dictionary<string, (string, string)>();
            Puzzles.Add("Dolphin", (
                "8:7,3:16:11,4:13,2:14,2:18:8,4:6,4:5,5:4,2,2:4,3,1:3,2,1:3,2:3,2:2,1,4:2,1,4:2,1,4:3,1,4:5,4:11:10:5:5:6",
                "1:2:2:4:11:13:15:9,3:8,3:7,5,2:7,3,4:6,2,3:8,3,2:13,2:10,2:5,5,3:4,1,4,6:1,2,2,8:3,1,10:3,1,4,4:2,1,2,3:2,1,2:2,1:2,1:1"));
            Puzzles.Add("Cupcake", (
                "5:1,3:2,3:5,2:2,2,2:7,4:2,2,2:7,4:2,2,2:7,6:2,3,3:2,5:8,1,1:1,1,1,1,1:1,1,1,1,1:1,1,1,1,1:3,1,1,1,7:2,1,1,3:3,1,2:8",
                "3,1:7,1:4,1,7:3,1,1,1,2:2,1,1,1,1,2:1,2,1,1,1,8:2,1,1,1,1,1,1:1,1,1,1,1,1,1:2,1,1,1,1,8:3,1,1,1,1,1,1:3,1,1,1,1,1:4,1,1,9:4,1,1,2:13:4,1:2,1:1:1:1:1"));
            Puzzles.Add("Weird man",(
                "4,3:6,1,2:4,1,3,1:2,1,2,3:6,2,3:2,3,4:2,8,2:2,2,2,4,2:1,2,2,5:3,2,4,2:2,2,5:2,2:2,4:4,1:2,9:1,2:7,3,5:4,2,1,1:8,1,1:2,1,1",
                "3:2,1:2,2,2:2,2,4:1,2,4:1,1,4,3:1,1,6,1,1:8,1,1,1:8,5,1:3,2,3,1:3,1,4,2:2,1,2,9:2,2,1,1,3:3,1,2,1,4:2,1,2,1,1:1,2,1,1,1,4:3,1,1,1,1,1:1,2,5,1,1:2,1,4,1,1:3,1,1"));

            string input = "";

            while (true)
            {
                (string, string) puzzle;
                if (Puzzles.TryGetValue(input, out puzzle))
                {
                    Board b = new Board(puzzle);
                    Solver s = new Solver(b);


                    s.Solve(false);
                    Console.ReadLine();
                }
                else
                {
                    Console.Clear();
                    Util.WriteAt("Select a nonogram to solve.", new Point(0, 0));
                    var enumerator = Puzzles.GetEnumerator();
                    for (int i = 0; i < Puzzles.Count; i++)
                    {
                        enumerator.MoveNext();
                        string Name = enumerator.Current.Key;
                        Util.WriteAt($"{i + 1}. {Name}", new Point(0, i + 1));
                    }

                    Util.WriteAt("Puzzle name:", new Point(0, Puzzles.Count + 2));
                    Console.SetCursorPosition(13, Puzzles.Count + 2);

                    input = Console.ReadLine();
                }
            } 
        }
    }
}