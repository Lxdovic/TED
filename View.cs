using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace TED;

internal sealed class View {
    private readonly int _cursorTop;
    private readonly ObservableCollection<string> _document;
    private readonly Action<string> _lineRenderer;
    private int _currentCharacter;
    private int _currentLine;
    private int _renderedLineCount;

    public View(Action<string> lineRenderer, ObservableCollection<string> document) {
        _lineRenderer = lineRenderer;
        _document = document;
        _document.CollectionChanged += DocumentChanged;
        _cursorTop = Console.CursorTop;
        Render();
    }

    public int CurrentLine {
        get => _currentLine;
        set {
            if (_currentLine != value) {
                _currentLine = value;
                _currentCharacter = Math.Min(_document[_currentLine].Length, _currentCharacter);

                UpdateCursorPosition();
            }
        }
    }

    public int CurrentCharacter {
        get => _currentCharacter;
        set {
            if (_currentCharacter != value) {
                _currentCharacter = value;
                UpdateCursorPosition();
            }
        }
    }

    private void DocumentChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        Render();
    }

    private void Render() {
        Console.CursorVisible = false;

        var lineCount = 0;

        foreach (var line in _document) {
            Console.SetCursorPosition(0, _cursorTop + lineCount);

            _lineRenderer(line);

            Console.WriteLine(new string(' ', Console.WindowWidth - line.Length));

            lineCount++;
        }

        var numberOfBlankLines = _renderedLineCount - lineCount;
        if (numberOfBlankLines > 0) {
            var blankLine = new string(' ', Console.WindowWidth);
            for (var i = 0; i < numberOfBlankLines; i++) {
                Console.SetCursorPosition(0, _cursorTop + lineCount + i);
                Console.WriteLine(blankLine);
            }
        }

        _renderedLineCount = lineCount;

        Console.CursorVisible = true;
        UpdateCursorPosition();
    }

    private void UpdateCursorPosition() {
        Console.CursorTop = _cursorTop + _currentLine;
        Console.CursorLeft = _currentCharacter;
    }
}