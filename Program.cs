using System.Collections.ObjectModel;
using TED.Core;
using TED.Ui;

namespace TED;

internal static class Program {
    private static string? _filePath;
    private static ObservableCollection<string>? _document;

    private static void Main(string?[] args) {
        Console.Clear();
        Console.CancelKeyPress += (_, _) => Console.Clear();

        if (args.Length == 0) {
            Console.WriteLine("Usage: ted <filename>");
            return;
        }

        _filePath = args[0];
        _document = new ObservableCollection<string>(File.ReadAllLines(_filePath!));
        var view = new View(_document);

        while (true) {
            var input = Console.ReadKey(true);

            HandleKeys(input, _document, view);

            BottomBar.Render(
                $"Line: {view.CurrentLine} Col: {view.CurrentCharacter} input: {input.Key} {input.Modifiers}");
        }
    }

    private static bool HandleTyping(ObservableCollection<string> document, View view, char character) {
        if (character < ' ') return false;

        var text = character.ToString();
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

        if (inputModifiers.HasFlag(ConsoleModifiers.Control) && start > 0) {
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

        if (inputModifiers.HasFlag(ConsoleModifiers.Control) && start < line.Length) {
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
            document[view.CurrentLine] = line + nextLine;
            document.RemoveAt(view.CurrentLine + 1);
            return true;
        }

        var before = line.Substring(0, start);
        var after = line.Substring(start + 1);
        document[lineIndex] = before + after;

        return true;
    }

    private static bool HandleEnter(ObservableCollection<string> document, View view, ConsoleModifiers inputModifiers) {
        if (inputModifiers.HasFlag(ConsoleModifiers.Control)) {
            document.Insert(view.CurrentLine + 1, "");
            view.CurrentLine++;

            return true;
        }

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
        if (view.CurrentCharacter == 0) {
            if (view.CurrentLine == 0)
                return false;

            view.CurrentLine--;
            view.CurrentCharacter = document[view.CurrentLine].Length;

            return true;
        }

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

        if (view.CurrentCharacter > 0) {
            view.CurrentCharacter--;

            return true;
        }

        return false;
    }

    private static bool HandleRightArrow(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        var line = document[view.CurrentLine];

        if (view.CurrentCharacter == line.Length) {
            if (view.CurrentLine == document.Count - 1)
                return false;

            view.CurrentLine++;
            view.CurrentCharacter = 0;

            return true;
        }

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
        // if (inputModifiers.HasFlag(ConsoleModifiers.Control)) {
        //     view.ViewTop = Math.Max(0, view.ViewTop - 1);
        //
        //     return true;
        // }

        if (view.CurrentLine > 0) view.CurrentLine--;

        return true;
    }

    private static bool HandleDownArrow(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        // if (inputModifiers.HasFlag(ConsoleModifiers.Control)) {
        //     view.ViewBottom = Math.Min(document.Count, view.ViewBottom + 1);
        //
        //     return true;
        // }

        if (view.CurrentLine < document.Count - 1) view.CurrentLine++;

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
            ConsoleKey.None => false,
            _ => HandleInput(document, view, input)
        };
    }

    private static bool HandleInput(ObservableCollection<string> document, View view, ConsoleKeyInfo input) {
        if (input.Modifiers.HasFlag(ConsoleModifiers.Control)) {
            _ = input.Key switch {
                ConsoleKey.S => HandleSave(document),
                _ => false
            };

            return true;
        }

        return HandleTyping(document, view, input.KeyChar);
    }

    private static bool HandleSave(ObservableCollection<string> document) {
        File.WriteAllLines(_filePath!, document);

        return true;
    }
}