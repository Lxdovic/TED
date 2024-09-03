using System.Text.RegularExpressions;
using Highlight.Engines;
using Highlight.Patterns;

namespace TED.Core.Syntax;

public class CustomEngine : Engine {
    protected override string PreHighlight(Definition definition, string input) {
        if (definition == null) throw new ArgumentNullException("definition");

        return input;
    }

    protected override string PostHighlight(Definition definition, string input) {
        if (definition == null) throw new ArgumentNullException("definition");

        return input;
    }

    protected override string ProcessBlockPatternMatch(Definition definition, BlockPattern pattern, Match match) {
        var color = pattern.Style.Colors.ForeColor;

        return "\x1b[38;2;" + color.R + ";" + color.G + ";" + color.B + "m" + match.Value + "\x1b[0m";
    }

    protected override string ProcessMarkupPatternMatch(Definition definition, MarkupPattern pattern, Match match) {
        if (definition == null) throw new ArgumentNullException("definition");
        if (pattern == null) throw new ArgumentNullException("pattern");
        if (match == null) throw new ArgumentNullException("match");

        var color = pattern.Style.Colors.ForeColor;

        return "\x1b[38;2;" + color.R + ";" + color.G + ";" + color.B + "m" + match.Value + "\x1b[0m";
    }

    protected override string ProcessWordPatternMatch(Definition definition, WordPattern pattern, Match match) {
        var color = pattern.Style.Colors.ForeColor;

        return "\x1b[38;2;" + color.R + ";" + color.G + ";" + color.B + "m" + match.Value + "\x1b[0m";
    }
}