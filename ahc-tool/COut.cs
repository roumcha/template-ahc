namespace AhcTool;

static class COut {
    static readonly object s_lock = new();

    public static void Default(object src) {
        lock (s_lock) {
            Console.ResetColor();
            Console.Write(src);
        }
    }

    public static void Color(object src, ConsoleColor color) {
        lock (s_lock) {
            Console.ForegroundColor = color;
            Console.Write(src);
            Console.ResetColor();
        }
    }

    public static void Defaultn(object src) {
        lock (s_lock) {
            Console.ResetColor();
            Console.WriteLine(src);
        }
    }

    public static void Colorn(object src, ConsoleColor color) {
        lock (s_lock) {
            Console.ForegroundColor = color;
            Console.WriteLine(src);
        }
    }

    public static void ClearLine() {
        lock (s_lock) {
            var top = Console.CursorTop;
            Console.SetCursorPosition(0, top);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, top);
            Console.ResetColor();
        }
    }

    public static void BackToPreviousLine() {
        lock (s_lock) {
            var top = Console.CursorTop;
            Console.SetCursorPosition(0, top);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, top - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, top - 1);
            Console.ResetColor();
        }
    }

    public static void PrintSummary(ExecutionResult result, string otherInfo) {
        lock (s_lock) {
            if (result.Status == Status.AC) {
                Console.ResetColor();
            } else {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.Write($"[{result.Status}]\t");
            Console.Write($"{result.TestCaseName}\t");
            Console.Write($"{result.Score:0.0}pt\t");
            Console.WriteLine($"{result.Time:D4}ms");

            Console.ResetColor();
            Console.WriteLine(otherInfo);
        }
    }

    public static void PrintSummaryAll(ExecutionResult[] results) {
        var sum = results.Sum(x => x.Score);
        var average = results.Average(x => x.Score);

        Status status = Status.AC;

        if (results.Any(x => x.Status == Status.IE)) {
            status = Status.IE;
        } else if (results.Any(x => x.Status == Status.RE)) {
            status = Status.RE;
        } else if (results.Any(x => x.Status == Status.WA)) {
            status = Status.WA;
        } else if (results.Any(x => x.Status == Status.TLE)) {
            status = Status.TLE;
        }

        lock (s_lock) {
            Console.ResetColor();
            Console.WriteLine($"結果: {status}");
            Console.WriteLine($"合計: {sum:0.0}");
            Console.WriteLine($"平均: {average:0.0}");
        }
    }

    internal class Err {
        public static void Default(object src) {
            lock (s_lock) {
                Console.ResetColor();
                Console.Error.WriteLine(src);
            }
        }

        public static void Color(object src, ConsoleColor color) {
            lock (s_lock) {
                Console.ForegroundColor = color;
                Console.Error.WriteLine(src);
                Console.ResetColor();
            }
        }

        public static void Defaultn(object src) {
            lock (s_lock) {
                Console.ResetColor();
                Console.Error.Write(src);
            }
        }

        public static void Colorn(object src, ConsoleColor color) {
            lock (s_lock) {
                Console.ForegroundColor = color;
                Console.Error.Write(src);
                Console.ResetColor();
            }
        }
    }
}
