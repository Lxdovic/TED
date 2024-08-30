namespace TED.TreeSitter;

public class Point : IComparable<Point> {
    public Point(int row, int column) {
        Row = row;
        Column = column;
    }

    public int Row { get; }
    public int Column { get; }

    public int CompareTo(Point other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var rowComparison = Row.CompareTo(other.Row);
        if (rowComparison != 0) return rowComparison;
        return Column.CompareTo(other.Column);
    }

    public override string ToString() {
        return $"({Row}, {Column})";
    }

    #region Equality Members

    protected bool Equals(Point other) {
        return Row == other.Row && Column == other.Column;
    }

    public override bool Equals(object obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Point)obj);
    }

    public override int GetHashCode() {
        unchecked {
            return (Row * 397) ^ Column;
        }
    }

    #endregion
}