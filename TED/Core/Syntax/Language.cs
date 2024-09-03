namespace TED.Core.Syntax;

public static class Language {
    private static readonly Dictionary<string, string> ExtensionsToLanguages = new() {
        { ".aspx", "ASPX" },
        { ".c", "C" },
        { ".cpp", "C++" },
        { ".cs", "C#" },
        { ".cob", "COBOL" },
        { ".e", "Eiffel" },
        { ".f", "Fortran" },
        { ".hs", "Haskell" },
        { ".html", "HTML" },
        { ".java", "Java" },
        { ".js", "JavaScript" },
        { ".m", "Mercury" },
        { ".il", "MSIL" },
        { ".pas", "Pascal" },
        { ".pl", "Perl" },
        { ".php", "PHP" },
        { ".py", "Python" },
        { ".rb", "Ruby" },
        { ".sql", "SQL" },
        { ".vb", "Visual Basic" },
        { ".vbs", "VBScript" },
        { ".vbnet", "VB.NET" },
        { ".xml", "XML" }
    };

    public static string GetLanguageFromFileExtension(string fileName) {
        ExtensionsToLanguages.TryGetValue(Path.GetExtension(fileName), out var language);

        return language ?? "Plain Text";
    }
}