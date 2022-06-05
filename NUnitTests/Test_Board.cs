using System;
using System.Diagnostics;
using Nonogram_Solver;
using NUnit.Framework;
using System.Linq;
using System.Linq.Expressions;

namespace NUnitTests
{
    [TestFixture]
    public class Test_Board
    {
        [Test]
        public void Test_BoardConstructor()
        {
            string horizontalDesc = "1,2:3,4:5,6";
            string verticalDesc = "1,2:3,4:5,6:7,8";
            
            Board b = new Board(horizontalDesc, verticalDesc);
            
            Assert.IsNotNull(b.Grid);
            Assert.IsNotNull(b.RowDescriptors);
            Assert.IsNotNull(b.ColumnDescriptors);
            
            Assert.That(b.Width == verticalDesc.Count(c => c == ':')+1, $"Width mismatch, expected 3, was {b.Width}");
            Assert.That(b.Height == horizontalDesc.Count(c => c == ':')+1, $"Height mismatch, expected 4, was {b.Height}");
        }

        [Test]
        public void Test_GetRowAndColumn()
        {
            string horizontalDesc = "1:1:1";
            string verticalDesc = "1:1:1";
            
            Board b = new Board(horizontalDesc, verticalDesc);

            b.Grid[1, 1] = Cell.Full;
            b.Grid[2, 1] = Cell.Cleared;
            b.Grid[1, 0] = Cell.Cleared;
            
            Assert.IsNotNull(b.Grid);
            
            Cell[] row = b.GetRow(1);
            Assert.That(row[0] == Cell.Empty);
            Assert.That(row[1] == Cell.Full);
            Assert.That(row[2] == Cell.Cleared);
            
            Cell[] column = b.GetColumn(1);
            Assert.That(column[0] == Cell.Cleared);
            Assert.That(column[1] == Cell.Full);
            Assert.That(column[2] == Cell.Empty);
        }

        [Test]
        public void Test_SetRowAndColumn()
        {
            string horizontalDesc = "1:1:1";
            string verticalDesc = "1:1:1";
            
            Board b = new Board(horizontalDesc, verticalDesc);
            
            Assert.That(b.GetRow(1).Count(c => c == Cell.Empty) == 3);

            Cell[] seq = new Cell[] { Cell.Full, Cell.Full, Cell.Cleared };
            
            Assert.That(b.SetColumn(seq, 1) == 3);

            int affectedColumns = b.SetRow(seq, 1);
            Assert.That(affectedColumns == 2, $"SetRow failure. Affected cells: {affectedColumns}");

            Assert.That(seq.SequenceEqual(b.GetRow(1)));
            Assert.That(seq.SequenceEqual(b.GetColumn(1)));
            
            seq = new Cell[] { Cell.Cleared, Cell.Empty, Cell.Empty};
            Assert.That(b.SetColumn(seq, 1) == 1);
            Assert.That(b.SetRow(seq, 1) == 1);
        }

        [Test]
        public void Test_GetDescriptor()
        {
            throw new NotImplementedException();
        }
        
    }
}