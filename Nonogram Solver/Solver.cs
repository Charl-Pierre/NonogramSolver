using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;

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
            
            //Calculate the total number of blocks according to the row descriptors
            for (int i = 0; i < board.RowsDescriptor.Length; i++)
            {
                for (int j = 0; j < board.RowsDescriptor[i].Length; j++)
                {
                    totalHorBlocks += board.RowsDescriptor[i][j];
                }
            }
            //Calculate the total number of blocks according to the column descriptors
            for (int i = 0; i < board.ColumnsDescriptor.Length; i++)
            {
                for (int j = 0; j < board.ColumnsDescriptor[i].Length; j++)
                {
                    totalVerBlocks += board.ColumnsDescriptor[i][j];
                }
            }
            
            if (totalHorBlocks != totalVerBlocks)
                Util.WriteAt($"WARNING: Block total mismatch. {totalHorBlocks} blocks found in rowDescriptors, {totalVerBlocks} blocks found in verticalDescriptors.", new Point(0,0), ConsoleColor.White, ConsoleColor.DarkRed);
            
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
            
            if (!bestConfidenceIsVertical)
                Board.SetRow(sequence, bestConfidenceIndex);
            else
                Board.SetColumn(sequence, bestConfidenceIndex);
            
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

        /// <summary>
        /// Solve a given sequence as best as possible with the given information.
        /// </summary>
        /// <param name="sequence">The sequence to be solved.</param>
        /// <param name="desc">The descriptor list of the specified sequence.</param>
        /// <returns>The specified sequence with all solvable cells filled in.</returns>
        public static Cell[] SolveSequence(Cell[] sequence, int[] desc, bool printDebug = false)
        {
            List<Cell[]> possibleCombinations = GenerateCombinations(sequence, desc, 0, 0);

            if (possibleCombinations.Count < 1)
                return null;

            Cell[] result = new Cell[sequence.Length];
            
            //Loop through each cell in the sequence
            for (int i = 0; i < sequence.Length; i++)
                result[i] = CheckConsensus(possibleCombinations, i);
            
            return result;
        }

        /// <summary>
        /// Checks whether a cell can be determined by checking whether it's always Full or always Cleared.
        /// </summary>
        /// <param name="possibleCombinations">A list of all permutations of a sequence.</param>
        /// <param name="index">The index to check consensus of.</param>
        /// <returns>Returns the most accurate estimation of a cell based on the observed permutations.</returns>
        private static Cell CheckConsensus(List<Cell[]> possibleCombinations, int index)
        {
            //Determine whether cell can be determined by checking if it's always Full or always Cleared/Empty.
            Cell c = possibleCombinations[0][index];
            for (int j = 1; j < possibleCombinations.Count; j++)
            {
                if (possibleCombinations[j][index] != c)
                    return Cell.Empty;
            }

            return c == Cell.Full ? Cell.Full : Cell.Cleared;
        }

        /// <summary>
        /// Recursively generate all permutations of a given sequence based on its existing values and its descriptor.
        /// </summary>
        /// <param name="sequence">The sequence to be permuted.</param>
        /// <param name="desc">The descriptor list of the specified sequence.</param>
        /// <param name="descriptorIndex">The current index within the descriptor.</param>
        /// <param name="startIndex">The index within the sequence from which to start permutation generation.</param>
        /// <returns>A list of all possible sequences with the given information.</returns>
        public static List<Cell[]> GenerateCombinations(Cell[] sequence, int[] desc, int descriptorIndex, int startIndex)
        {
            List<Cell[]> partialCombinations = new List<Cell[]>();
            
            //The minimum remaining length from the current descriptorIndex. (Ex. 3,1,2 gives index(0) = 8, index(1) = 4 and index(2) = 2.
            int minimumLength = desc.Skip(descriptorIndex).Sum() + desc.Length-1-descriptorIndex;
            
            //Loop through all possible starting combinations for the current descriptor
            for (int i = 0; startIndex + i + minimumLength - 1 < sequence.Length; i++)
            {
                
                //Attempt to place block into input sequence
                Cell[] newSequence = ApplyBlock(sequence, startIndex+i, desc[descriptorIndex]);
                if (newSequence == null) continue;

                //Check if this is the last descriptor in the list
                if (descriptorIndex < desc.Length - 1)
                    //Generate all combinations with this sequence and add them to the list
                    partialCombinations.AddRange(GenerateCombinations(newSequence, desc, descriptorIndex + 1,
                        startIndex + desc[descriptorIndex] + i + 1));
                else
                    //Add the sequence to the possible combinations.
                    partialCombinations.Add(newSequence);
            }

            return partialCombinations;
        }

        public static Cell[] ApplyBlock(Cell[] sequence, int startIndex, int blockLength)
        {
            Cell[] result = (sequence.Clone() as Cell[]);
            
            for (int i = 0; i < blockLength; i++)
            {
                //Catch out of bounds error
                if (i + startIndex >= result.Length)
                    return null;
                
                if (result[i] == Cell.Cleared)
                    return null;
                result[startIndex + i] = Cell.Full;
                
            }

            //Check that if the next cell is within bounds, that that cell is not full.
            if (startIndex + blockLength + 1 < result.Length)
                if (result[startIndex + blockLength + 1] == Cell.Full)
                    return null;

            return result;
        }

        /// <summary>
        /// Converts a sequence of cells into a string with + representing Full, - represents Cleared and . represents Empty.
        /// </summary>
        /// <param name="seq">The sequence to be converted.</param>
        /// <returns>A human-readable string of the specified sequence.</returns>
        public static string SequenceToString(Cell[] seq)
        {
            string str = "";
            for (int i = 0; i < seq.Length; i++)
            {
                switch (seq[i])
                {
                    case Cell.Empty:
                        str += ".";
                        break;
                    case Cell.Full:
                        str += "+";
                        break;
                    case Cell.Cleared:
                        str += "-";
                        break;
                }
            }

            return str;
        }
    }
}