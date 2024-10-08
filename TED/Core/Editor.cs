using System.Collections.ObjectModel;
using TED.Core.Syntax;
using TED.Ui;

namespace TED.Core;

internal static class Editor {
    private static string? _filePath;
    private static ObservableCollection<string>? _document;
    private static readonly Stack<string[]> _forwardDocumentHistory = new();
    private static readonly Stack<string[]> _backwardDocumentHistory = new();
    public static string CurrentLanguage = "Plain Text";
    public static BottomBar BottomBar = new();

    private static readonly List<char> StopChars = new() {
        '.', '!', '?', '<', '>', '(', ')', '[', ']', '{', '}', ';', ':', ','
    };

    public static void Run(string?[] args) {
        Console.Clear();
        Console.CancelKeyPress += (_, _) => Console.Clear();
        Console.TreatControlCAsInput = true;

        if (args.Length == 0) {
            Console.WriteLine(@"Usage: ted <filename>");
            return;
        }

        _filePath = args[0];
        _document = new ObservableCollection<string>(File.ReadAllLines(_filePath!));
        CurrentLanguage = Language.GetLanguageFromFileExtension(_filePath!);

        var view = new View(_document);

        while (true) {
            var input = Console.ReadKey(true);

            HandleKeys(input, _document, view);
            
            BottomBar.Columns.Push(new Column {
                Text = $" Line: {view.CurrentLine} Col: {view.CurrentCharacter}",
                Width = 16
            });
            
            BottomBar.Columns.Push(new Column {
                Text = $" Language: {CurrentLanguage} |",
                Width = 16
            });
            
            BottomBar.Columns.Push(new Column {
                Text = $" {DateTime.Now} |",
                Width = 40
            });
            
            BottomBar.Render();
        }
    }

    private static void ForwardDocumentHistory() {
        if (_forwardDocumentHistory.Count == 0) return;

        _backwardDocumentHistory.Push(_document!.ToArray());
        _document = new ObservableCollection<string>(_forwardDocumentHistory.Pop());
    }

    private static void BackwardDocumentHistory() {
        if (_backwardDocumentHistory.Count == 0) return;

        _forwardDocumentHistory.Push(_document!.ToArray());
        _document = new ObservableCollection<string>(_backwardDocumentHistory.Pop());
    }

    private static void HandleTyping(ObservableCollection<string> document, View view, char character) {
        var text = character.ToString();
        var lineIndex = view.CurrentLine;
        var start = view.CurrentCharacter;

        ForwardDocumentHistory();

        document[lineIndex] = document[lineIndex].Insert(start, text);
        view.CurrentCharacter += text.Length;
    }

    private static bool HandleBackspace(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        var line = document[view.CurrentLine];
        var start = view.CurrentCharacter;

        ForwardDocumentHistory();

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

        ForwardDocumentHistory();

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
        ForwardDocumentHistory();

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

            while (start > 0 && !char.IsWhiteSpace(line[start - 1])) {
                start--;

                if (StopChars.Contains(line[start - 1])) break;
            }

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

            while (start < line.Length && !char.IsWhiteSpace(line[start])) {
                start++;

                if (start >= line.Length || StopChars.Contains(line[start])) break;
            }

            view.CurrentCharacter = start;

            return true;
        }

        if (view.CurrentCharacter <= line.Length - 1)
            view.CurrentCharacter++;

        return true;
    }

    private static bool HandleUpArrow(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        if (inputModifiers.HasFlag(ConsoleModifiers.Control)) {
            view.ViewTop = Math.Max(0, view.ViewTop - 1);

            return true;
        }

        if (view.CurrentLine > 0) view.CurrentLine--;

        return true;
    }

    private static bool HandleDownArrow(ObservableCollection<string> document, View view,
        ConsoleModifiers inputModifiers) {
        if (inputModifiers.HasFlag(ConsoleModifiers.Control)) {
            view.ViewBottom = Math.Min(document.Count, view.ViewBottom + 1);

            return true;
        }

        if (view.CurrentLine < document.Count - 1) view.CurrentLine++;

        return true;
    }

    private static bool HandleEnd(ObservableCollection<string> document, View view, ConsoleModifiers inputModifiers) {
        if (inputModifiers.HasFlag(ConsoleModifiers.Control)) {
            view.CurrentLine = document.Count - 1;

            return true;
        }

        view.CurrentCharacter = document[view.CurrentLine].Length;

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
            ConsoleKey.End => HandleEnd(document, view, input.Modifiers),
            _ => false
        };

        if (input.Key != ConsoleKey.Backspace && input.KeyChar >= ' ') HandleTyping(document, view, input.KeyChar);
        else HandleControls(document, view, input);
    }

    private static void HandleControls(ObservableCollection<string> document, View view, ConsoleKeyInfo input) {
        if (input.Modifiers.HasFlag(ConsoleModifiers.Control))
            _ = input.Key switch {
                ConsoleKey.S => HandleSave(document),
                ConsoleKey.Z => HandleUndo(input),
                _ => false
            };
    }

    private static bool HandleUndo(ConsoleKeyInfo input) {
        if (input.Modifiers.HasFlag(ConsoleModifiers.Shift)) {
            BackwardDocumentHistory();
            return true;
        }

        ForwardDocumentHistory();
        return true;
    }

    private static bool HandleSave(ObservableCollection<string> document) {
        File.WriteAllLines(_filePath!, document);

        return true;
    }
}