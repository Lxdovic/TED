using System;
using System.Globalization;

// Replacement for System.Drawing.Color which is not available on non-Windows platforms
namespace Highlight {
    public class Color {
        public static Color Empty = new Color("#FFFFFF", 255, 255, 255);

        public Color(string name, int r, int g, int b) {
            Name = name;
            R = r;
            G = g;
            B = b;
        }

        public string Name { get; set; } // Either Hex or Web Color Name
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public static Color FromName(string name) {
            // Todo: Add extended colors from https://en.wikipedia.org/wiki/Web_colors#HTML_color_names
            if (name.IndexOf("#") == -1)
                name = name switch {
                    "white" => "#FFFFFF",
                    "silver" => "#C0C0C0",
                    "gray" => "#808080",
                    "black" => "#000000",
                    "red" => "#FF0000",
                    "maroon" => "#800000",
                    "yellow" => "#FFFF00",
                    "olive" => "#808000",
                    "lime" => "#00FF00",
                    "green" => "#008000",
                    "aqua" => "#00FFFF",
                    "teal" => "#008080",
                    "blue" => "#0000FF",
                    "navy" => "#000080",
                    "fuchsia" => "#FF00FF",
                    "purple" => "#800080",
                    "transparent" => "#00000000",
                    "darkred" => "#8B0000",
                    "orange" => "#FFA500",
                    "darkblue" => "#00008B",
                    "lawngreen" => "#7CFC00",
                    "brown" => "#A52A2A",
                    _ => throw new Exception("Unknown color name: " + name)
                };

            var red = int.Parse(name.Substring(1, 2), NumberStyles.AllowHexSpecifier);
            var green = int.Parse(name.Substring(3, 2), NumberStyles.AllowHexSpecifier);
            var blue = int.Parse(name.Substring(5, 2), NumberStyles.AllowHexSpecifier);

            return new Color(name, red, green, blue);
        }
    }
}