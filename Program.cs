using System.Collections.ObjectModel;

namespace TED;

internal static class Program {
    private static void Main(string[] args) {
        Console.Clear();

        var document = new ObservableCollection<string> { "" };
        var view = new View(RenderLine, document);

        while (true) {
            var input = Console.ReadKey(true);

            HandleKeys(input, document, view);
        }
    }

    private static bool HandleTyping(ObservableCollection<string> document, View view, string text) {
        var lineIndex = view.CurrentLine;
        var start = view.CurrentCharacter;
        document[lineIndex] = document[lineIndex].Insert(start, text);
        view.CurrentCharacter += text.Length;

        return true;
    }

    private static bool HandleBackspace(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        var line = document[view.CurrentLine];
        var start = view.CurrentCharacter;

        if (inputModifiers.HasFlag(ConsoleModifiers.Control)) {
            while (start > 0 && char.IsWhiteSpace(line[start - 1]))
                start--;

            while (start > 0 && !char.IsWhiteSpace(line[start - 1]))
                start--;

            var cBefore = line.Substring(0, start);
            var cAfter = line.Substring(view.CurrentCharacter);
            document[view.CurrentLine] = cBefore + cAfter;
            view.CurrentCharacter = start;

            return true;
        }

        if (start == 0) {
            if (view.CurrentLine == 0)
                return false;

            var currentLine = document[view.CurrentLine];
            var previousLine = document[view.CurrentLine - 1];
            document.RemoveAt(view.CurrentLine);
            view.CurrentLine--;
            document[view.CurrentLine] = previousLine + currentLine;
            view.CurrentCharacter = previousLine.Length;
            return true;
        }

        var before = line.Substring(0, start - 1);
        var after = line.Substring(start);
        document[view.CurrentLine] = before + after;
        view.CurrentCharacter--;

        return true;
    }

    private static bool HandleDelete(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        var line = document[view.CurrentLine];
        var start = view.CurrentCharacter;

        if (inputModifiers.HasFlag(ConsoleModifiers.Control)) {
            while (start < line.Length && char.IsWhiteSpace(line[start]))
                start++;

            while (start < line.Length && !char.IsWhiteSpace(line[start]))
                start++;

            var cBefore = line.Substring(0, view.CurrentCharacter);
            var cAfter = line.Substring(start);
            document[view.CurrentLine] = cBefore + cAfter;

            return true;
        }

        var lineIndex = view.CurrentLine;

        if (start >= line.Length) {
            if (view.CurrentLine == document.Count - 1)
                return false;

            var nextLine = document[view.CurrentLine + 1];
            document[view.CurrentLine] = nextLine;
            document.RemoveAt(view.CurrentLine + 1);
            return true;
        }

        var before = line.Substring(0, start);
        var after = line.Substring(start + 1);
        document[lineIndex] = before + after;

        return true;
    }

    private static bool HandleEnter(ObservableCollection<string> document, View view, ConsoleModifiers inputModifiers) {
        return InsertLine(document, view);
    }

    private static bool InsertLine(ObservableCollection<string> document, View view) {
        var remainder = document[view.CurrentLine].Substring(view.CurrentCharacter);
        document[view.CurrentLine] = document[view.CurrentLine].Substring(0, view.CurrentCharacter);

        var lineIndex = view.CurrentLine + 1;
        document.Insert(lineIndex, remainder);
        view.CurrentCharacter = 0;
        view.CurrentLine = lineIndex;

        return true;
    }

    private static bool HandleLeftArrow(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        if (inputModifiers.HasFlag(ConsoleModifiers.Control)) {
            var line = document[view.CurrentLine];
            var start = view.CurrentCharacter;
            while (start > 0 && char.IsWhiteSpace(line[start - 1]))
                start--;

            while (start > 0 && !char.IsWhiteSpace(line[start - 1]))
                start--;

            view.CurrentCharacter = start;

            return true;
        }

        if (view.CurrentCharacter > 0)
            view.CurrentCharacter--;

        return true;
    }

    private static bool HandleRightArrow(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        var line = document[view.CurrentLine];

        if (inputModifiers.HasFlag(ConsoleModifiers.Control)) {
            var start = view.CurrentCharacter;
            while (start < line.Length && char.IsWhiteSpace(line[start]))
                start++;

            while (start < line.Length && !char.IsWhiteSpace(line[start]))
                start++;

            view.CurrentCharacter = start;

            return true;
        }

        if (view.CurrentCharacter <= line.Length - 1)
            view.CurrentCharacter++;

        return true;
    }

    private static bool HandleUpArrow(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        if (view.CurrentLine > 0)
            view.CurrentLine--;

        return true;
    }

    private static bool HandleDownArrow(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        if (view.CurrentLine < document.Count - 1)
            view.CurrentLine++;

        return true;
    }

    private static void HandleKeys(ConsoleKeyInfo input, ObservableCollection<string> document, View view) {
        _ = input.Key switch {
            ConsoleKey.Backspace => HandleBackspace(document, view, input.Modifiers),
            ConsoleKey.Enter => HandleEnter(document, view, input.Modifiers),
            ConsoleKey.DownArrow => HandleDownArrow(document, view, input.Modifiers),
            ConsoleKey.UpArrow => HandleUpArrow(document, view, input.Modifiers),
            ConsoleKey.LeftArrow => HandleLeftArrow(document, view, input.Modifiers),
            ConsoleKey.RightArrow => HandleRightArrow(document, view, input.Modifiers),
            ConsoleKey.Delete => HandleDelete(document, view, input.Modifiers),
            _ => HandleTyping(document, view, input.KeyChar.ToString())
        };
    }

    private static void RenderLine(string line) {
        Console.Write(line);
    }
}