using System.Runtime.InteropServices;
using TreeSitter;

namespace TED.TreeSitter;

public class CLanguage {
    private const string DllName = "resources/libtree-sitter.so";

    [DllImport(DllName)]
    private static extern IntPtr tree_sitter_c();

    public static Language Create() {
        return new Language(tree_sitter_c());
    }
}