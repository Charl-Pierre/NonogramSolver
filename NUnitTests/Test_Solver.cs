using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nonogram_Solver;
using NUnit.Framework;

namespace NUnitTests
{
    [TestFixture]
    public class Test_Solver
    {
        private int[] desc;
        [Test]
        public void Test_Formulas()
        {
            
            //Test minimum length formula
            desc = new int[] { 3,1,2};
            int descriptorIndex = 1;
            int minimumLength = desc.Skip(descriptorIndex).Sum() + desc.Length-1-descriptorIndex;
            Assert.That(minimumLength == 4);
            
            //Test Cloning of sequence
            Cell[] seq = new Cell[] { Cell.Full, Cell.Full, Cell.Empty };
            Cell[] seq2 = seq.Clone() as Cell[];
            seq2[0] = Cell.Empty;
            Assert.That(seq[0] == Cell.Full);
            
            //Verify that sequence is a reference and not value type.
            Cell[] seq3 = seq;
            seq3[0] = Cell.Marked;
            Assert.That(seq[0] == Cell.Marked);
        }

        [Test]
        public void Test_ApplyBlock()
        {
            Cell[] startSeq = new Cell[] { Cell.Empty, Cell.Empty, Cell.Empty };
            int[] desc = new int[] { 2 };
            var res = Solver.ApplyBlock(startSeq, 1, 2);
            Assert.That($"{string.Join("", res.Select(s => { return s == Cell.Full ? "X" : "-";}))}" == "-XX");
        }

        [Test]
        public void Test_Combinations()
        {
            Cell[] p = new Cell[] { Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty,Cell.Empty,Cell.Empty,Cell.Empty,Cell.Empty,Cell.Empty };
            int[] desc = new int[] { 2,2,1 };
            List<Cell[]> combis = Solver.GenerateCombinations(p, desc, 0, 0);

            string str = "";
            
            foreach (Cell[] combi in combis)
            {
                str += $"Seq {string.Join("", combi.Select(s => { return s == Cell.Full ? "X" : "-";}))} ,";
            }
            
            Assert.Fail(str);
        }

        [Test]
        public void Test_SolveSequence()
        {
            Cell[] p = new Cell[] { Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty  };
            int[] desc = new int[] { 2, 1 };

            Cell[] seq = Solver.SolveSequence(p, desc);
            Assert.Fail(Solver.SequenceToString(seq));
        }
        
    }
}