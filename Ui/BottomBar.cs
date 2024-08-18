namespace TED.Ui;

public static class BottomBar {
    public static void Render(string text) {
        var cursorPosition = Console.GetCursorPosition();

        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.Write(text);
        Console.Write(new string(' ', Console.WindowWidth - text.Length));

        Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top);
    }
}