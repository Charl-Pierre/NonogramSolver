using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

static class Util
{
    //TODO: Write documentation for the whole class.
    public static void WriteAt(char letter, Point point, ConsoleColor color = ConsoleColor.White, ConsoleColor bgColor = ConsoleColor.Black)
    {
        Util.WriteAt(letter.ToString(), point, color, bgColor);
    }

    public static void WriteAt(string text, Point point, ConsoleColor color = ConsoleColor.White, ConsoleColor bgColor = ConsoleColor.Black)
    {
        ConsoleColor prevColor = Console.ForegroundColor;
        Console.ForegroundColor = color;

        ConsoleColor prevbgColor = Console.BackgroundColor;
        Console.BackgroundColor = bgColor;

        Point prevPoint = new Point(Console.CursorLeft, Console.CursorTop);
        Console.SetCursorPosition(point.X, point.Y);
        Console.Write(text);
        Console.SetCursorPosition(prevPoint.X, prevPoint.Y);

        Console.ForegroundColor = prevColor;
        Console.BackgroundColor = prevbgColor;
    }

    public static void ClearArea(Point position, int length)
    {
        WriteAt("".PadLeft(length, ' '), position);
    }

    public static void WriteCentered(string text, Point Point)
    {
        WriteAt(text, new Point(Point.X - (text.Length / 2), Point.Y));
    }

    public static void Line(Point point, int length, bool orientation, ConsoleColor color = ConsoleColor.White, ConsoleColor bgColor = ConsoleColor.Black)
    {
        for (int i = 0; i < length; i++)
        {
            Point drawPos = new Point(point.X + (orientation ? i : 0), point.Y + (orientation ? 0 : i));
            Util.WriteAt(orientation ? '-' : '|', drawPos, color, bgColor);
        }
    }

    public static void Rectangle(Point point, Point size, ConsoleColor color = ConsoleColor.White, ConsoleColor bgColor = ConsoleColor.Black)
    {

        Util.WriteAt('+', point, color, bgColor);
        Util.WriteAt('+', new Point(point.X + size.X - 1, point.Y), color, bgColor);
        Util.WriteAt('+', new Point(point.X, point.Y + size.Y - 1), color, bgColor);
        Util.WriteAt('+', new Point(point.X + size.X - 1, point.Y + size.Y - 1), color, bgColor);
        
        Line(new Point(point.X + 1, point.Y), size.X - 2, true, color, bgColor);
        Line(new Point(point.X, point.Y + 1), size.Y - 2, false, color, bgColor);

        Line(new Point(point.X + 1, point.Y + size.Y - 1), size.X - 2, true, color, bgColor);
        Line(new Point(point.X + size.X - 1, point.Y + 1), size.Y - 2, false, color, bgColor);
    }

    /*
     *  Function to randomly shuffle a list
     */
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int swap = new Random().Next(list.Count);
            (list[i], list[swap]) = (list[swap], list[i]);
        }
    }
}

/// <summary>
/// Extension of the Point class that simplifies addition and subtraction of Points
/// </summary>
static class PointExtension
{
    /// <summary>
    /// Adds two points and returns the resulting point
    /// </summary>
    /// <param name="point">Point to be added with.</param>
    public static Point Add(this Point p, Point point)
    {
        return p + (Size)point;
        
    }
    
    /// <summary>
    /// Adds a point with an x and y value and returns the resulting point.
    /// </summary>
    public static Point Add(this Point p, int x, int y)
    {
        return p + new Size(x, y);
        
    }

    /// <summary>
    /// Subtracts two points and returns the resulting point
    /// </summary>
    /// <param name="point">Point to be subtracted with.</param>
    public static Point Sub(this Point p, Point point)
    {
        return p - (Size)point;
    }
    
    /// <summary>
    /// Subtracts an x and y from a point and returns the resulting point.
    /// </summary>
    public static Point Sub(this Point p, int x, int y)
    {
        return p - new Size(x, y);
    }
}
