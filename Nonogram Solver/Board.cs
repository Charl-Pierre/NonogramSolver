using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace Nonogram_Solver
{
    public enum Cell
    {
        Empty, 
        Full, 
        Cleared, 
        Marked
    }
    public class Board
    {

        private Cell[,] grid;
        private int[][] rowsDesc;
        private int[][] columnsDesc;
        
        public Cell[,] Grid
        {
            get => grid;
            set => grid = value;
        }

        /// <summary>
        /// An array of arrays containing all the integers describing the rows of the grid
        /// </summary>
        public int[][] RowsDescriptor
        {
            get => rowsDesc;
            set => rowsDesc = value;
        }
        
        /// <summary>
        /// An array of arrays containing all the integers describing the columns of the grid
        /// </summary>
        public int[][] ColumnsDescriptor
        {
            get => columnsDesc;
            set => columnsDesc = value;
        }
        
        public Board(string horizontalDescription, string verticalDescription)
        {
            rowsDesc = GetDescriptor(horizontalDescription);
            columnsDesc = GetDescriptor(verticalDescription);
            Grid = new Cell[columnsDesc.Length, rowsDesc.Length];
        }

        public int Width => Grid.GetLength(0);
        public int Height => Grid.GetLength(1);

        
        public void Draw(Point point, bool showDescriptors = true)
        {
            Util.Rectangle(point, new Point(Width*2+2, Height+2));
            if (showDescriptors)
            {
                for (int i = 0; i < rowsDesc.Length; i++)
                {
                    for (int j = 0; j < rowsDesc[i].Length; j++)
                    {
                        int value = rowsDesc[i][rowsDesc[i].Length - j - 1];
                        ConsoleColor bgColor = ((i+j) % 2 == 0) ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                        Util.WriteAt(value.ToString().PadRight(2), point.Add(-2*(j+1), i+1), ConsoleColor.Black, bgColor);
                    }
                }
                for (int i = 0; i < columnsDesc.Length; i++)
                {
                    for (int j = 0; j < columnsDesc[i].Length; j++)
                    {
                        int value = columnsDesc[i][columnsDesc[i].Length - j - 1];
                        ConsoleColor bgColor = ((i+j) % 2 == 0) ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                        Util.WriteAt(value.ToString().PadRight(2), point.Add(i*2+1, -1*(j+1)), ConsoleColor.Black, bgColor);
                    }
                }
            }

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Cell value = grid[i, j];
                    ConsoleColor c = ConsoleColor.Black;
                    switch (value)
                    {
                        case Cell.Full:
                            c = ConsoleColor.Green;
                            break;
                        case Cell.Cleared:
                            c = ConsoleColor.Gray;
                            break;
                        case Cell.Marked:
                            c = ConsoleColor.Yellow;
                            break;
                    }
                    Util.WriteAt(value != Cell.Empty? "  ": "", point.Add(1+i*2, 1+j), ConsoleColor.Black, c);
                }
            }
        }

        /// <summary>
        /// Takes a string-based description of either the rows or columns of a grid and converts it to an int[][] array.
        /// </summary>
        /// <param name="description">A string numbers delimited with commas (,) and colons (:) containing a horizontal/vertical description of a grid. Ex. "1,2,3:4,5,6".</param>
        /// <returns>An int[][] array with the outer dimension holding each row/column and the inner dimension holding a list of integers describing said row/column.</returns>
        private int[][] GetDescriptor(string description)
        {
            //Split the input into rows of strings
            string[] rows = description.Split(':');
            
            //Create the descriptor array.
            int[][] descriptor = new int[rows.Length][];
            
            //Fill each row of the array.
            for (int i = 0; i < rows.Length; i++)
            {
                descriptor[i] = rows[i].Split(',').Select(int.Parse).ToArray();
            }

            return descriptor;
        }

        public Cell[] GetRow(int rowIndex)
        {
            Cell[] row = new Cell[Width];
            for (int i = 0; i < Width; i++)
                row[i] = Grid[i, rowIndex];
            
            return row;
        }
        
        public Cell[] GetColumn(int columnIndex)
        {
            Cell[] column = new Cell[Height];
            for (int i = 0; i < Height; i++)
                column[i] = Grid[columnIndex, i];
            
            return column;
        }
        
    }
}