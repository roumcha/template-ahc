namespace AhcTool;

class Program {
    static void Main(string[] argv) {
        try {
            new Program(argv).Run();
        } finally {
            COut.Default("");
        }
    }

    readonly Settings _settings;

    enum LangMode { Bin, Python }

    public Program(string[] argv) {
        COut.Defaultn("");
        COut.Colorn("-------- 初期化 --------", ConsoleColor.Cyan);

        // 引数の処理（今のところ不使用）
        for (int i = 0; i < argv.Length; ++i) {
            argv[i] = argv[i].Trim().Trim('"').TrimEnd(new[] { '/', '\\' });
        }

        foreach (var a in argv) {
            COut.Colorn("- " + a.ToString(), ConsoleColor.Gray);
        }

        // 設定ファイルの読み込み
        COut.Colorn("- setings.jsonc の読み込み", ConsoleColor.Gray);
        _settings = Settings.Init();
    }

    public void Run() {
        COut.Colorn("-------- 前処理 --------", ConsoleColor.Cyan);

        COut.Colorn("- フォルダ作成", ConsoleColor.Gray);
        _settings.TempDir.Create();
        _settings.TempOutDir.Create();

        COut.Colorn("-------- 実行・採点 --------", ConsoleColor.Cyan);

        var avg = 0.0;
        var cnt = 0;
        object lockObj = new();

        // 並列実行
        var results =
            _settings.TempInDir.EnumerateFiles()
            .Where(x => x.Name != ".keep")
            .AsParallel()
            .WithDegreeOfParallelism(_settings.ParallelDegree)
            .Select(inputFile => {
                // 実行
                var res = new Executor(inputFile, _settings).Run();

                // 結果を表示
                lock (lockObj) {
                    COut.BackToPreviousLine();
                    avg = (avg * cnt + res.Score) / (cnt + 1);
                    cnt++;
                    COut.PrintSummary(res, $"平均: {avg:0.0}");
                }

                return res;
            })
            .ToArray();

        COut.ClearLine();
        COut.Defaultn("");

        COut.Colorn("-------- 要約表示 --------", ConsoleColor.Cyan);
        COut.PrintSummaryAll(results);
    }
}
