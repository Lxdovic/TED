using System.Collections.ObjectModel;

namespace TED;

internal static class Program {
    private static int _currentLine;

    private static void Main(string[] args) {
        Console.Clear();

        var document = new ObservableCollection<string> { "" };

        while (true) {
            var input = Console.ReadKey();

            HandleKeys(input, document);
        }
    }

    private static bool HandleEnter(ObservableCollection<string> document) {
        document[_currentLine] += "\n";
        document.Add("");

        _currentLine++;

        return true;
    }

    private static bool HandleBackspace(ObservableCollection<string> document) {
        if (document[_currentLine].Length == 0) {
            if (_currentLine == 0) return false;

            document.RemoveAt(_currentLine);
            _currentLine--;
            document[_currentLine] = document[_currentLine][..^1];

            return true;
        }

        document[_currentLine] = document[_currentLine][..^1];

        return true;
    }

    private static bool HandleCharacter(char character, ObservableCollection<string> document) {
        if (character < ' ') return false;
        
        document[_currentLine] += character;

        return true;
    }

    private static void HandleKeys(ConsoleKeyInfo input, ObservableCollection<string> document) {
        var operationSuccess = input.Key switch {
            ConsoleKey.Enter => HandleEnter(document),
            ConsoleKey.Backspace => HandleBackspace(document),
            _ => HandleCharacter(input.KeyChar, document)
        };
        
        if (operationSuccess) RenderDocument(document);
    }

    private static void RenderDocument(ObservableCollection<string> document) {
        Console.Clear();

        foreach (var line in document) RenderLine(line);

        Console.CursorTop = _currentLine;
    }

    public static void RenderLine(string line) {
        Console.Write(line);
    }
}