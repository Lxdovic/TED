using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Highlight;
using TED.Core.Syntax;

namespace TED.Core;

internal sealed class View {
    private readonly ObservableCollection<string>? _document;
    private readonly int _maxLineIndexDigits;
    private int _currentCharacter;
    private int _currentLine;

    private int _targetCurrentCharacter;
    private int _viewBottom = Console.WindowHeight - 1;
    private int _viewLeft;
    private int _viewRight = Console.WindowWidth - 1;
    private int _viewTop;

    public View(ObservableCollection<string>? document) {
        _document = document;
        _maxLineIndexDigits = (int)Math.Floor(Math.Log10(document!.Count) + 1);
        _document!.CollectionChanged += DocumentChanged;
        Render();
    }

    public int ViewTop {
        get => _viewTop;
        set {
            if (_viewTop != value) {
                _viewTop = value;
                _viewBottom = _viewTop + Console.WindowHeight - 1;
                Render();
            }
        }
    }

    public int ViewBottom {
        get => _viewBottom;
        set {
            if (_viewBottom != value) {
                _viewBottom = value;
                _viewTop = _viewBottom - Console.WindowHeight + 1;
                Render();
            }
        }
    }

    public int CurrentLine {
        get => _currentLine;
        set {
            if (_currentLine != value) {
                _currentLine = value;
                _currentCharacter = Math.Min(_document![_currentLine].Length, _targetCurrentCharacter);

                if (_currentLine < ViewTop) ViewTop = _currentLine;
                else if (_currentLine >= ViewBottom) ViewBottom = _currentLine + 1;

                UpdateCursorPosition();
            }
        }
    }

    public int CurrentCharacter {
        get => _currentCharacter;
        set {
            if (_currentCharacter != value) {
                _currentCharacter = value;
                _targetCurrentCharacter = value;

                if (_currentCharacter < ViewLeft) ViewLeft = _currentCharacter;
                else if (_currentCharacter > ViewRight) ViewRight = _currentCharacter;

                UpdateCursorPosition();
            }
        }
    }

    public int ViewLeft {
        get => _viewLeft;
        set {
            if (_viewLeft != value && value >= 0) {
                _viewLeft = value;
                _viewRight = _viewLeft + Console.WindowWidth - 1;
                Render();
            }
        }
    }

    public int ViewRight {
        get => _viewRight;
        set {
            if (_viewRight != value) {
                _viewRight = value;
                _viewLeft = _viewRight - Console.WindowWidth + 1;
                Render();
            }
        }
    }

    private void DocumentChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        Render();
    }

    private int GetVisibleLength(string input) {
        var ansiEscapeCodePattern = @"\x1b\[[0-9;]*m";
        var plainText = Regex.Replace(input, ansiEscapeCodePattern, string.Empty);
        return plainText.Length;
    }

    private string GetVisibleSubstring(string input, int startIndex, int length) {
        var ansiEscapeCodePattern = @"\x1b\[[0-9;]*m";
        var matches = Regex.Matches(input, ansiEscapeCodePattern);
        var visibleLength = 0;
        var visibleStartIndex = 0;
        var visibleEndIndex = input.Length;
        var activeAnsiCodes = new List<string>();

        for (var i = 0; i < input.Length; i++) {
            if (matches.Any(m => m.Index == i)) {
                var match = matches.First(m => m.Index == i);
                activeAnsiCodes.Add(match.Value);
                i += match.Length - 1;
                continue;
            }

            if (visibleLength == startIndex) {
                visibleStartIndex = i;
                break;
            }

            visibleLength++;
        }

        visibleLength = 0;
        for (var i = visibleStartIndex; i < input.Length; i++) {
            if (matches.Any(m => m.Index == i)) {
                var match = matches.First(m => m.Index == i);
                i += match.Length - 1;
                continue;
            }

            if (visibleLength == length) {
                visibleEndIndex = i;
                break;
            }

            visibleLength++;
        }

        var visibleSubstring = input.Substring(visibleStartIndex, visibleEndIndex - visibleStartIndex);
        var resetCode = "\x1b[0m";
        return string.Join(string.Empty, activeAnsiCodes) + visibleSubstring + resetCode;
    }

    private void Render() {
        var documentInView = string.Join(Environment.NewLine, _document!.Skip(ViewTop).Take(ViewBottom - ViewTop));
        var highlighter = new Highlighter(new CustomEngine());
        var highlightedCode = highlighter.Highlight(Editor.CurrentLanguage, documentInView).Split(Environment.NewLine);

        Console.CursorVisible = false;

        Console.Clear();

        for (var index = 0; index < highlightedCode.Length; index++) {
            var line = highlightedCode[index];
            var startIndex = Math.Min(GetVisibleLength(line), ViewLeft);
            var length = Math.Max(0,
                Math.Min(GetVisibleLength(line) - ViewLeft, Console.WindowWidth - _maxLineIndexDigits - 1));

            var displayLine = GetVisibleSubstring(line, startIndex, length);
            var lineNumber = string.Format($"{{0,{_maxLineIndexDigits}}}", ViewTop + index + 1);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(lineNumber);
            Console.ResetColor();
            Console.WriteLine($@" {displayLine}");
        }

        UpdateCursorPosition();

        Console.CursorVisible = true;
    }

    private void UpdateCursorPosition() {
        var cursorTop = Math.Max(CurrentLine - ViewTop, 0);
        var cursorLeft = Math.Max(CurrentCharacter - ViewLeft + _maxLineIndexDigits + 1, 0); // Adjust for line numbers

        cursorTop = Math.Min(cursorTop, Console.BufferHeight - 1);
        cursorLeft = Math.Min(cursorLeft, Console.BufferWidth - 1);

        Console.SetCursorPosition(cursorLeft, cursorTop);
    }
}