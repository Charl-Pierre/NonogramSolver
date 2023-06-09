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
            int minimumLength = desc.Length-1-descriptorIndex;
            for (int i = descriptorIndex; i < desc.Length; i++)
                minimumLength += desc[i];
            
            Assert.That(minimumLength == 4);
            
            //Test old minimum length formula
            int altMinimumLength = desc.Skip(descriptorIndex).Sum() + desc.Length-1-descriptorIndex;
            Assert.That(altMinimumLength == 4);
            
            //Test Cloning of sequence
            Cell[] seq = { Cell.Full, Cell.Full, Cell.Empty };
            Cell[] seq2 = seq.Clone() as Cell[];
            seq2[0] = Cell.Empty;
            Assert.That(seq[0] == Cell.Full);
            
            //Verify that sequence is a reference and not value type.
            Cell[] seq3 = seq;
            seq3[0] = Cell.Cleared;
            Assert.That(seq[0] == Cell.Cleared);
        }

        [Test]
        public void Test_SequenceToString()
        {
            //TODO
        }

        [Test]
        public void Test_ApplyBlock()
        {
            Cell[] p = new Cell[] { Cell.Empty, Cell.Cleared, Cell.Cleared, Cell.Empty, Cell.Full, Cell.Empty  };
            int[] desc = new int[] { 2 };
            var res = Solver.ApplyBlock(p, 1, desc[0]);
            Assert.Fail(Solver.SequenceToString(res));
        }

        [Test]
        public void Test_Combinations()
        {
            Cell[] p = new Cell[] { Cell.Empty, Cell.Full, Cell.Full, Cell.Full, Cell.Cleared, Cell.Empty  };
            int[] desc = new int[] { 3 };
            List<Cell[]> combis = Solver.GenerateCombinations(p, desc, 0, 0);

            string str = "";

            for (int i = 0; i < combis.Count; i++)
            {
                str += $"Sequence {i}: {Solver.SequenceToString(combis[i])}";
            }
            
            Assert.Fail(str);
        }

        [Test]
        public void Test_SolveSequence()
        {
            Cell[] p = new Cell[] { Cell.Empty, Cell.Full, Cell.Empty, Cell.Full, Cell.Empty, Cell.Empty  };
            int[] desc = new int[] { 3 };

            Cell[] seq = Solver.SolveSequence(p, desc);
            Assert.Fail(Solver.SequenceToString(seq));
        }
        
    }
}