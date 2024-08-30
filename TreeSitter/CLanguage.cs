using System.Runtime.InteropServices;

namespace TED.TreeSitter;

public class CLanguage {
    private const string DllName = "resources/tree-sitter-c.so";

    [DllImport(DllName)]
    private static extern IntPtr tree_sitter_c();

    public static Language Create() {
        return new Language(tree_sitter_c());
    }
}