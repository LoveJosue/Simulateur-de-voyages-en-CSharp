using System;

namespace Tp1;

public class Point
{
    public int X 
    {
        set;
        get;
    }
    public int Y
    {
        set;
        get;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}
