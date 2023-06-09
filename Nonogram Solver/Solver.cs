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
        
        public Point drawPos;


        public Board Board
        {
            get => board;
            set => board = value;
        }

        public Solver(Board board)
        {
            this.Board = board;
            
            //Calculate minimum draw position
            drawPos = new Point(
                Board.RowDescriptors.Select(desc => desc.Length).Max()*2+1,
                Board.ColumnDescriptors.Select(desc => desc.Length).Max());

            Console.SetWindowSize(drawPos.X + 2*board.Width + 10, drawPos.Y + board.Height + 5);

            int totalHorBlocks = 0;
            int totalVerBlocks = 0;
            
            //Calculate the total number of blocks according to the row descriptors
            for (int i = 0; i < board.RowDescriptors.Length; i++)
            {
                for (int j = 0; j < board.RowDescriptors[i].Length; j++)
                {
                    totalHorBlocks += board.RowDescriptors[i][j];
                }
            }
            //Calculate the total number of blocks according to the column descriptors
            for (int i = 0; i < board.ColumnDescriptors.Length; i++)
            {
                for (int j = 0; j < board.ColumnDescriptors[i].Length; j++)
                {
                    totalVerBlocks += board.ColumnDescriptors[i][j];
                }
            }
            
            if (totalHorBlocks != totalVerBlocks)
                Util.WriteAt($"WARNING: Block total mismatch. {totalHorBlocks} blocks found in rowDescriptors, {totalVerBlocks} blocks found in verticalDescriptors.", new Point(0,0), ConsoleColor.White, ConsoleColor.DarkRed);
            
            totalBlocks = totalHorBlocks;
        }
        
        //TODO: add debug messages.
        /// <summary>
        /// Solves the given puzzle and returns the number of rounds it took.
        /// Solution works by recursively generating all possible combinations for each row/column and removing any that contain discrepancies.
        /// Rounds becomes significantly faster over the course of the process as the amount of possible combinations decreases.
        /// </summary>
        /// <param name="printDebug">Whether to print various metadata during the process</param>
        /// <returns>The number of rounds used to solve the puzzle.</returns>
        public int Solve(bool printDebug = false)
        {
            Console.Clear();
            Board.Draw(drawPos);
            
            //Whether a certain row is up to date or not.
            bool[] checkedRows = new bool[Board.Height];
            bool[] checkedColumns = new bool[Board.Width];
            int outOfDateRows = Int32.MaxValue, outOfDateColumns = Int32.MaxValue;
            int round = 0;
            for (; outOfDateRows > 0 && outOfDateColumns > 0; round++)
            {
                //Update all rows
                for (int i = 0; i < Board.Height; i++)
                {
                    //Only update rows that are not up to date.
                    if (!checkedRows[i])
                    {
                        Cell[] row = Board.GetRow(i);
                        Cell[] backupRow = Board.GetRow(i);
                        int[] descriptor = Board.RowDescriptors[i];
                    
                        //Solve the sequence as best as possible.
                        Cell[] newRow = SolveSequence(row, descriptor);
                
                        //Check all affected columns
                        for (int j = 0; j < newRow.Length; j++)
                            //If a cell in the sequence has been changed, set its column to out of date.
                            if (newRow[j] != backupRow[j])
                                checkedColumns[j] = false;
                        
                        //Apply the changes to the board and mark the row as updated.
                        int affectedCells = Board.SetRow(newRow, i);
                        checkedRows[i] = true;
                        
                        //Redraw more often at start
                        if (outOfDateRows > Board.Height*0.75 && affectedCells != 0)
                            Board.Draw(drawPos);
                    }
                }
                
                Board.Draw(drawPos);
                outOfDateColumns = checkedColumns.Count(val => !val);
                
                if (printDebug) Console.Write($"Rnd {round+1}A: OoD columns: {outOfDateColumns}");

                //Update all columns.
                for (int i = 0; i < Board.Width; i++)
                {
                    //Only update columns that are not up to date.
                    if (!checkedColumns[i])
                    {
                        Cell[] column = Board.GetColumn(i);
                        Cell[] backupColumn = Board.GetColumn(i);
                        int[] descriptor = Board.ColumnDescriptors[i];
                    
                        //Solve the sequence as best as possible.
                        Cell[] newColumn = SolveSequence(column, descriptor);

                        //Check all affected rows
                        for (int j = 0; j < newColumn.Length; j++)
                            //If a cell in the sequence has been changed, set its row to out of date.
                            if (newColumn[j] != backupColumn[j])
                                checkedRows[j] = false;
                        
                        //Apply the changes to the board and mark the column as updated.
                        int affectedCells = Board.SetColumn(newColumn, i);
                        checkedColumns[i] = true;
                        
                        //Redraw more often at start
                        if (outOfDateColumns > Board.Height*0.75 && affectedCells != 0)
                            Board.Draw(drawPos);
                    }
                }
                
                Board.Draw(drawPos);
                outOfDateRows = checkedRows.Count(val => !val);
                
                if (printDebug) Console.Write($"Rnd {round+1}B: OoD rows: {outOfDateRows}");
            }
            
            Console.WriteLine($"Finished in {round} rounds.");
            return round;
        }
        
        /// <summary>
        /// [DEPRECATED]
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
        /// Determines whether a sequence is solvable.
        /// </summary>
        /// <param name="sequence">The sequence to be checked.</param>
        /// <param name="desc">The descriptor list of the specified sequence.</param>
        /// <returns>A boolean indicating whether a sequence could possibly be the correct solution.</returns>
        public static bool IsSolvable(Cell[] sequence, int[] desc, int startIndex)
        {
            List<Cell[]> possibleDescriptorSequences = GenerateCombinations(new Cell[sequence.Length], desc, 0, startIndex);

            for (int i = 0; i < possibleDescriptorSequences.Count; i++)
            {
                Cell[] testSequence = possibleDescriptorSequences[i];
                if (CompareSequences(sequence, testSequence))
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Helper function for IsSolvable. Used to compare each cell of 2 sequences.
        /// </summary>
        /// <param name="sequence">The sequence to check for validity.</param>
        /// <param name="testSequence">The sequence to be compared against.</param>
        /// <returns>A boolean indicating whether two sequences are similar enough to consider valid.</returns>
        public static bool CompareSequences(Cell[] sequence, Cell[] testSequence)
        {
            for (int i = 0; i < testSequence.Length; i++)
            {
                if (testSequence[i] == Cell.Full)
                {
                    if (sequence[i] == Cell.Cleared) return false;
                }
                else
                    if (sequence[i] == Cell.Full) return false;
            }
            return true;
        }

        /// <summary>
        /// Solve a given sequence as best as possible with the given information.
        /// </summary>
        /// <param name="sequence">The sequence to be solved.</param>
        /// <param name="desc">The descriptor list of the specified sequence.</param>
        /// <returns>The specified sequence with all solvable cells filled in.</returns>
        public static Cell[] SolveSequence(Cell[] sequence, int[] desc, bool printDebug = false)
        {
            //Perform preliminary checks.
            int firstNonClear = sequence.Length-1;
            int emptyCells = 0;

            //Find the first non-clear cell and count the number of empty cells
            for (int i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] != Cell.Cleared) firstNonClear = Math.Min(firstNonClear, i);
                if (sequence[i] == Cell.Empty) emptyCells++;
            }

            //If there are no empty cells, the sequence is solved.
            if (emptyCells == 0) return sequence;

            //Generate all possible combinations, using the first non-clear cell as starting index.
            List<Cell[]> possibleCombinations = GenerateCombinations(sequence, desc, 0, firstNonClear);

            //If there is more than one possibility, rule out discrepant ones
            if (possibleCombinations.Count > 1)
                //Loop through the list backwards
                for (int i = possibleCombinations.Count-1; i >= 0; i--)
                    //If a combination can't be solved anymore, remove it from the list of combinations.
                    if (!IsSolvable(possibleCombinations[i], desc, firstNonClear)) possibleCombinations.RemoveAt(i);

            //If the number of combinations is 0, then sequence can't be solved yet.
            if (possibleCombinations.Count < 1)
                return sequence;

            //Create the result sequence.
            Cell[] result = new Cell[sequence.Length];
            
            //Loop through each cell in the sequence
            for (int i = 0; i < sequence.Length; i++)
                //Determine what the cell should be based on the possible combinations.
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
            for (int i = 1; i < possibleCombinations.Count; i++)
            {
                if (!CellsAreSimilar(c, possibleCombinations[i][index]))
                    return Cell.Empty;
            }

            return c == Cell.Full ? Cell.Full : Cell.Cleared;
        }
        
        /// <summary>
        /// Determine whether two cells are similar
        /// Pairs considered similar are:
        /// 1. Full & Full
        /// 2. Empty & Clear
        /// 3. Clear & Clear
        /// 4. Empty & Empty
        /// </summary>
        /// <param name="c1">Cell 1 to be checked.</param>
        /// <param name="c2">Cell 1 to be checked.</param>
        /// <returns>Whether the two cells are considered similar.</returns>
        private static bool CellsAreSimilar(Cell c1, Cell c2)
        {
            if (c1 == Cell.Full || c2 == Cell.Full)
            {
                if (c1 == c2)
                    return true;
                return false;
            }
            return true;
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
            int minimumLength = desc.Length-1-descriptorIndex;
            for (int i = descriptorIndex; i < desc.Length; i++)
                minimumLength += desc[i];
            
            //Old formula, slower formula.
            //int minimumLength = desc.Skip(descriptorIndex).Sum() + desc.Length-1-descriptorIndex;

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

        /// <summary>
        /// Tries to place a block (a group of adjacent cells) within a given sequence.
        /// Method returns Null if for whatever reason, it fails.
        /// </summary>
        /// <param name="sequence">The sequence to be edited.</param>
        /// <param name="startIndex">The starting index of the block</param>
        /// <param name="blockLength">The length of the block</param>
        /// <returns>Returns the given sequence with a block placed at the specified position.</returns>
        public static Cell[] ApplyBlock(Cell[] sequence, int startIndex, int blockLength)
        {
            if (startIndex > 0 && sequence[startIndex - 1] == Cell.Full)
                return null;
            
            Cell[] result = (sequence.Clone() as Cell[]);
            
            for (int i = 0; i < blockLength; i++)
            {
                //Catch out of bounds error
                if (i + startIndex >= result.Length)
                    return null;
                
                if (result[startIndex + i] == Cell.Cleared)
                    return null;
                result[startIndex + i] = Cell.Full;
                
            }

            if (startIndex + blockLength < result.Length)
            {
                //Check that if the next cell is within bounds, that that cell is not full.
                if (result[startIndex + blockLength] == Cell.Full)
                    return null;

                //Set the cell behind the block to cleared.
                result[startIndex + blockLength] = Cell.Cleared;
            }
            
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