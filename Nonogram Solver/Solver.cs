using System;
using System.Drawing;
using System.Linq;

namespace Nonogram_Solver
{
    public class Solver
    {
        private Board board;
        private int totalBlocks;

        public Board Board
        {
            get => board;
            set => board = value;
        }
        public Solver(Board board)
        {
            this.board = board;
            int totalHorBlocks = 0;
            int totalVerBlocks = 0;
            for (int i = 0; i < board.RowsDescriptor.Length; i++)
            {
                for (int j = 0; j < board.RowsDescriptor[i].Length; j++)
                {
                    totalHorBlocks += board.RowsDescriptor[i][j];
                }
            }
            for (int i = 0; i < board.ColumnsDescriptor.Length; i++)
            {
                for (int j = 0; j < board.ColumnsDescriptor[i].Length; j++)
                {
                    totalVerBlocks += board.ColumnsDescriptor[i][j];
                }
            }
            
            totalBlocks = totalHorBlocks;
        }

        public void Solve()
        {
            //Loop through all the row descriptors and calculate the corresponding confidence
            int[] rowConfidence = new int[Board.RowsDescriptor.Length];
            for (int i = 0; i < Board.RowsDescriptor.Length; i++)
                rowConfidence[i] = GetConfidence(Board.GetRow(i), Board.RowsDescriptor[i]);

            //Loop through all the column descriptors and calculate the corresponding confidence
            int[] columnConfidence = new int[Board.ColumnsDescriptor.Length];
            for (int i = 0; i < Board.ColumnsDescriptor.Length; i++)
                columnConfidence[i] = GetConfidence( Board.GetColumn(i), Board.ColumnsDescriptor[i]);

            //Find the best row/column
            int bestConfidenceIndex = 0;
            bool bestConfidenceIsVertical = false;
            if (rowConfidence.Max() >= columnConfidence.Max())
            {
                for (int i = 0; i < rowConfidence.Length; i++)
                    if (rowConfidence[i] > rowConfidence[bestConfidenceIndex]) bestConfidenceIndex = i;
            }
            else
            {
                bestConfidenceIsVertical = true;
                for (int i = 0; i < columnConfidence.Length; i++)
                    if (columnConfidence[i] > columnConfidence[bestConfidenceIndex]) bestConfidenceIndex = i;
            }

            //Get the sequence of the best row/column
            Cell[] sequence = bestConfidenceIsVertical
                ? Board.GetColumn(bestConfidenceIndex)
                : Board.GetRow(bestConfidenceIndex);

            //Get the segment descriptor of the best row/column
            int[] desc = bestConfidenceIsVertical
                ? Board.ColumnsDescriptor[bestConfidenceIndex]
                : Board.RowsDescriptor[bestConfidenceIndex];

            //Solve the sequence
            sequence = SolveSequence(sequence, desc);
            
            //TODO: Apply the solved sequence to the grid.
        }
        
        /// <summary>
        /// Calculates the confidence of a sequence, given its descriptor.
        /// </summary>
        /// <param name="sequence">The sequence to be evaluated</param>
        /// <param name="desc">The descriptor to be used for evaluation</param>
        /// <returns>An integer between 0 and sequence length, signifying the number of cells that can be confidently determined.</returns>
        public int GetConfidence(Cell[] sequence, int[] desc, bool printDebug = false)
        {
            //TODO: add support for (semi)solved sequences
            int uncertainty = sequence.Length - (desc.Sum() + desc.Length - 1);
            int confidence = 0;
            foreach (int num in desc)
                confidence += Math.Max(num - uncertainty, 0);
            
            confidence += Math.Max((desc.Length - 1) * (1 - uncertainty), 0);
            if (printDebug)
                Console.WriteLine($"Seq {string.Join("", sequence.Select(s => { return s == Cell.Full ? "X" : "-";}))} " +
                                  $"with desc {string.Join(",", desc)} " +
                                  $"has conf of {confidence}");
            return confidence;
        }

        public Cell[] SolveSequence(Cell[] sequence, int[] desc, bool printDebug = false)
        {
            throw new NotImplementedException();
        }
    }
}