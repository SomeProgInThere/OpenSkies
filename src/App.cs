
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenSkies;

public static class App {
    private static void Main() {
        var windowSettings = new NativeWindowSettings() {
            ClientSize = new Vector2i(800, 600),
            Title = "OpenSkies",
            Flags = ContextFlags.ForwardCompatible,
        };
    
        using var window = new Window(GameWindowSettings.Default, windowSettings);
        window.Run();
    }
}