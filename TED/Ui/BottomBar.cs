
namespace TED.Ui;

public struct Column {
    public string Text { get; set; }
    public int Width { get; set; }
}

public class BottomBar {
    public Stack<Column> Columns { get; set; } = new();
    
    public void Render() {
        var cursorPosition = Console.GetCursorPosition();

        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        // Console.BackgroundColor = ConsoleColor.DarkGray;
        // Console.ForegroundColor = ConsoleColor.White;
        
        var cumulativeWidth = 0;
        
        while (Columns.Count > 0) {
            var column = Columns.Pop();
            cumulativeWidth += column.Width;
            
            if (cumulativeWidth > Console.WindowWidth) {
                break;
            }
            
            // Console.Write(column.Text.PadRight(column.Width));
        }
        
        // Console.Write("".PadRight(Console.WindowWidth - cumulativeWidth));
        // Console.ResetColor();
        Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top);
    }
}