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
        public int[][] RowDescriptors
        {
            get => rowsDesc;
            set => rowsDesc = value;
        }
        
        /// <summary>
        /// An array of arrays containing all the integers describing the columns of the grid
        /// </summary>
        public int[][] ColumnDescriptors
        {
            get => columnsDesc;
            set => columnsDesc = value;
        }
        
        /// <summary>
        /// Create an unsolved Nonogram.
        /// Both Descriptions take a string of similar format, namely
        /// a string numbers delimited with commas (,) and colons (:)
        /// containing a horizontal/vertical description of a grid. Ex. "1,2,3:4,5,6".
        /// </summary>
        /// <param name="horizontalDescription">A string containing a list of integers referring to the descriptors seen on the left of a Nonogram puzzle.</param>
        /// <param name="verticalDescription">A string containing a list of integers referring to the descriptors seen at the top of a Nonogram puzzle.</param>
        public Board(string horizontalDescription, string verticalDescription)
        {
            rowsDesc = GetDescriptor(horizontalDescription);
            columnsDesc = GetDescriptor(verticalDescription);
            Grid = new Cell[columnsDesc.Length, rowsDesc.Length];
        }
        
        /// <summary>
        /// Create an unsolved Nonogram.
        /// Takes a tuple of 2 descriptor strings referring to the rows and columns respectively.
        /// </summary>
        /// <param name="descriptors">A tuple of 2 strings, both containing a list of integers referring to the descriptors seen on the left/top of a Nonogram puzzle respectively.</param>
        public Board((string, string) descriptors)
        {
            rowsDesc = GetDescriptor(descriptors.Item1);
            columnsDesc = GetDescriptor(descriptors.Item2);
            Grid = new Cell[columnsDesc.Length, rowsDesc.Length];
        }

        public int Width => Grid.GetLength(0);
        public int Height => Grid.GetLength(1);
        
        /// <summary>
        /// Draws the board at a specified position.
        /// </summary>
        /// <param name="point">The position that the board should be drawn at. Refers to the top-left corner of the board (excluding descriptors).</param>
        /// <param name="showDescriptors">Whether the descriptors should be draw on the left/top of the board.</param>
        public void Draw(Point point, bool showDescriptors = true)
        {
            //Draw a rectangle around the board.
            Util.Rectangle(point, new Point(Width*2+2, Height+2));
            
            //Draw the row/column descriptors if enabled.
            if (showDescriptors)
            {
                //Loop through all the row descriptors.
                for (int i = 0; i < rowsDesc.Length; i++)
                {
                    //For each descriptor, draw its values backwards, with the last value being draw closest to the board.
                    for (int j = 0; j < rowsDesc[i].Length; j++)
                    {
                        //Get value from descriptor
                        int value = rowsDesc[i][rowsDesc[i].Length - j - 1];
                        
                        //Determine background color to create a checkerboard pattern.
                        ConsoleColor bgColor = ((i+j) % 2 == 0) ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                        
                        //Draw the value in a 2x1 space (Due to the 1:2 ratio of console text). This leaves space for a possible 2nd digit.
                        Util.WriteAt(value.ToString().PadRight(2), point.Add(-2*(j+1), i+1), ConsoleColor.Black, bgColor);
                    }
                }
                
                //Loop through all the column descriptors.
                for (int i = 0; i < columnsDesc.Length; i++)
                {
                    //For each descriptor, draw its values backwards, with the last value being draw closest to the board.
                    for (int j = 0; j < columnsDesc[i].Length; j++)
                    {
                        //Get value from descriptor
                        int value = columnsDesc[i][columnsDesc[i].Length - j - 1];
                        
                        //Determine background color to create a checkerboard pattern.
                        ConsoleColor bgColor = ((i+j) % 2 == 0) ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                        
                        //Draw the value in a 2x1 space (Due to the 1:2 ratio of console text). This leaves space for a possible 2nd digit.
                        Util.WriteAt(value.ToString().PadRight(2), point.Add(i*2+1, -1*(j+1)), ConsoleColor.Black, bgColor);
                    }
                }
            }

            //Loop through all the cells within the board.
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    //Get the value from a cell.
                    Cell value = grid[i, j];
                    
                    //Determine the color of the cell based on its contents.
                    ConsoleColor c = ConsoleColor.Black;
                    switch (value)
                    {
                        case Cell.Full:
                            c = ConsoleColor.White;
                            break;
                        case Cell.Cleared:
                            c = ConsoleColor.Gray;
                            break;
                    }
                    
                    //Draw the cell in the console.
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

        /// <summary>
        /// Deep-copy a specific row as a 1-dimensional sequence of cells
        /// </summary>
        /// <param name="rowIndex">The row to be copied</param>
        /// <returns>An array of cells in the specified row</returns>
        public Cell[] GetRow(int rowIndex)
        {
            Cell[] row = new Cell[Width];
            for (int i = 0; i < Width; i++)
                row[i] = Grid[i, rowIndex];
            
            return row;
        }
        
        /// <summary>
        /// Deep-copy a specific column as a 1-dimensional sequence of cells
        /// </summary>
        /// <param name="columnIndex">The column to be copied</param>
        /// <returns>An array of cells in the specified column</returns>
        public Cell[] GetColumn(int columnIndex)
        {
            Cell[] column = new Cell[Height];
            for (int i = 0; i < Height; i++)
                column[i] = Grid[columnIndex, i];
            
            return column;
        }

        /// <summary>
        /// Overwrite a given sequence of cells onto a row of the grid.
        /// </summary>
        /// <param name="sequence">The sequence of cells to be applied.</param>
        /// <param name="rowIndex">The row which to overwrite</param>
        /// /// <returns>The number of cells changed.</returns>
        public int SetRow(Cell[] sequence, int rowIndex)
        {
            int cellsChanged = 0;
            for (int i = 0; i < Width; i++)
            {
                if (sequence[i] != Cell.Empty && Grid[i, rowIndex] != sequence[i])
                {
                    Grid[i, rowIndex] = sequence[i];
                    cellsChanged++;
                }
            }
            return cellsChanged;
        }
        
        /// <summary>
        /// Overwrite a given sequence of cells onto a column of the grid.
        /// </summary>
        /// <param name="sequence">The sequence of cells to be applied.</param>
        /// <param name="columnIndex">The column which to overwrite</param>
        /// <returns>The number of cells changed.</returns>
        public int SetColumn(Cell[] sequence, int columnIndex)
        {
            int cellsChanged = 0;
            for (int i = 0; i < Height; i++)
            {
                if (sequence[i] != Cell.Empty && Grid[columnIndex, i] != sequence[i])
                {
                    Grid[columnIndex, i] = sequence[i];
                    cellsChanged++;
                }
            }
            return cellsChanged;
        }
    }
}