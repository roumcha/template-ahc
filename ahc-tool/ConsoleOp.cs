namespace AhcTool;

static class ConsoleOp {
    public static void ClearLine() {
        var top = Console.CursorTop;
        Console.SetCursorPosition(0, top);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, top);
        Console.ResetColor();
    }

    public static void GoToPreviousLine() {
        var top = Console.CursorTop;
        Console.SetCursorPosition(0, top - 1);
        Console.ResetColor();
    }
}
