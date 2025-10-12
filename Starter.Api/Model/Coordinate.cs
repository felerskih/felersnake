namespace Starter.Api;

/// <summary>
/// Coordinate on the 2D grid game board.
/// Coordinates begin at zero.
/// </summary>
public class Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Coordinate other = (Coordinate)obj;
        return this.X == other.X && this.Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}