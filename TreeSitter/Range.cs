namespace TED.TreeSitter;

public class Range {
    public Range(int startByte, int endByte, Point startPoint, Point endPoint) {
        StartByte = startByte;
        EndByte = endByte;
        StartPoint = startPoint;
        EndPoint = endPoint;
    }

    public int StartByte { get; }
    public int EndByte { get; }
    public Point StartPoint { get; }
    public Point EndPoint { get; }
}