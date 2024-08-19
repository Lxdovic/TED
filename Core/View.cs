using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace TED.Core;

internal sealed class View {
    private readonly ObservableCollection<string> _document;
    private readonly Action<string> _lineRenderer;
    private int _currentCharacter;
    private int _currentLine;
    private int _targetCurrentCharacter;
    private int _viewBottom = Console.WindowHeight - 1;
    private int _viewTop;

    public View(Action<string> lineRenderer, ObservableCollection<string> document) {
        _lineRenderer = lineRenderer;
        _document = document;
        _document.CollectionChanged += DocumentChanged;
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
                _currentCharacter = Math.Min(_document[_currentLine].Length, _targetCurrentCharacter);

                if (_currentLine < ViewTop)
                    ViewTop = _currentLine;
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

                UpdateCursorPosition();
            }
        }
    }

    private void DocumentChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        Render();
    }

    private void Render() {
        Console.CursorVisible = false;

        Console.Clear();

        for (var i = ViewTop; i < ViewBottom; i++) {
            if (i >= _document.Count) break;

            _lineRenderer(_document[i]);
        }

        Console.CursorVisible = true;
        UpdateCursorPosition();
    }

    private void UpdateCursorPosition() {
        Console.CursorTop = Math.Max(CurrentLine - ViewTop, 0);
        Console.CursorLeft = CurrentCharacter;
    }
}