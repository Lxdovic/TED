using System;
using System.Drawing;
using System.Text.RegularExpressions;
using Highlight.Engines;
using Highlight.Patterns;

namespace TED.Syntax;

public class CustomEngine : Engine {
    protected override string PreHighlight(Definition definition, string input) {
        if (definition == null) throw new ArgumentNullException("definition");

        return input;
    }

    protected override string PostHighlight(Definition definition, string input) {
        if (definition == null) throw new ArgumentNullException("definition");

        return input;
    }

    private static ConsoleColor ClosestConsoleColor(byte r, byte g, byte b) {
        ConsoleColor ret = 0;
        double rr = r, gg = g, bb = b, delta = double.MaxValue;

        foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor))) {
            var n = Enum.GetName(typeof(ConsoleColor), cc);
            var c = Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
            var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
            if (t == 0.0)
                return cc;
            if (t < delta) {
                delta = t;
                ret = cc;
            }
        }

        return ret;
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

    private string ProcessMarkupPatternAttributeMatches(Definition definition, MarkupPattern pattern, Match match) {
        return "";
    }
}