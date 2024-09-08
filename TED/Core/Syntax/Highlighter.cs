using System.Text.RegularExpressions;

namespace TED.Core.Syntax;

public struct Token {
    public string Kind { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    
    public Token(string kind, int start, int length) {
        Kind = kind;
        Start = start;
        Length = length;
    }
}

public class Highlighter {
    private readonly Dictionary<string, Regex> Patterns = new() {
        { "string", new Regex(@"([""'])(?:(?=(\\?))\2.)*?\1") },
        { "comment", new Regex(@"\/\/.*|\/\*.+\*\/") },
        
    };

    // public void Test() {
    //     var stringsRegex = Patterns["string"];
    //     var commentsRegex = Patterns["comment"];
    //     var input = "// var test = \"Hello, World!\";\n" +
    //                 "var test2 = @\"Hello, World!\";\n" +
    //                 "var test3 = $\"Hello, World!\";\n" +
    //                 "var test4 = $\"Hello, {World}!\";\n" +
    //                 "var test5 = /* Hello, World! */;\n" +
    //                 "var test6 = 'w';\n";
    //     
    //     var strings = /* awdawdawd */ stringsRegex.Replace(input, match => $"\x1b[38;2;220;169;60m{match.Value}\x1b[0m");
    //     var comments = commentsRegex.Replace(input, match => $"\x1b[38;2;20;169;30m{match.Value}\x1b[0m");
    //     
    //     Console.WriteLine(strings);
    //     Console.WriteLine();
    //     Console.WriteLine(comments);
    // }
    //
    public void Test() {
        var stringsRegex = Patterns["string"];
        var commentsRegex = Patterns["comment"];
        var input = "// var test = \"Hello, World!\";\n" +
                    "var test2 = @\"Hello, World!\";\n" +
                    "var test3 = $\"Hello, World!\";\n" +
                    "var test4 = $\"Hello, {World}!\";\n" +
                    "var test5 = /* Hello, World! */;\n" +
                    "var test6 = 'w';\n";
        
        var strings = /* awda "awdawd"wdawd */ stringsRegex.Replace(input,/* awda "awdawd"wdawd */ match => $"\x1b[38;2;220;169;60m{match.Value}\x1b[0m");
        var comments = commentsRegex.Replace(input, match => $"\x1b[38;2;20;169;30m{match.Value}\x1b[0m");
        
        Console.WriteLine(strings);
        Console.WriteLine();
        Console.WriteLine(comments);
    }

    public string Highlight(string line) {
        var commentMatches = Patterns["comment"].Matches(line);
        var nonCommentParts = new List<(int start, int length)>();

        int lastIndex = 0;
        foreach (Match commentMatch in commentMatches) {
            if (commentMatch.Index > lastIndex) {
                nonCommentParts.Add((lastIndex, commentMatch.Index - lastIndex));
            }
            lastIndex = commentMatch.Index + commentMatch.Length;
        }
        if (lastIndex < line.Length) {
            nonCommentParts.Add((lastIndex, line.Length - lastIndex));
        }

        foreach (var (start, length) in nonCommentParts) {
            var part = line.Substring(start, length);
            foreach (var (kind, pattern) in Patterns) {
                if (kind == "comment") continue;
                var matches = pattern.Matches(part);
                foreach (Match match in matches) {
                    var color = kind switch {
                        "string" => "220;169;60",
                        _ => "255;255;255"
                    };
                    part = part.Replace(match.Value, $"\x1b[38;2;{color}m{match.Value}\x1b[0m");
                }
            }
            line = line.Remove(start, length).Insert(start, part);
        }

        foreach (Match commentMatch in commentMatches) {
            line = line.Replace(commentMatch.Value, $"\x1b[38;2;20;169;30m{commentMatch.Value}\x1b[0m");
        }

        return line;
    }
}