using Kokuban;
using Cysharp.Diagnostics;

namespace AhcTool;

class Program {
    static readonly Version s_version = new(6, 0, 0);
    readonly Settings s_settings;
    readonly DirectoryInfo s_logDir;

    static async Task Main(string[] args) {
        try {
            if (args is []) {
                Console.WriteLine(Chalk.Cyan["-------- 準備 --------"]);
                var app = new Program();
                await app.Compile();
                app.ExecuteAll();
            } else if (args[0] == "-m") {
                Console.WriteLine(Chalk.Cyan["-------- 準備 --------"]);
                var app = new Program();
                await app.Compile();
                app.ExecuteManually();
            } else if (args[0].Contains("help") || args[0] == "-h") {
                PrintHelp();
            } else if (args[0].Contains("reset")) {
                string? yn = null;
                while (yn is null || yn.ToLower() != "y") {
                    Console.Write("設定ファイルの変更を元に戻してもよろしいですか。(y/n) >");
                    yn = Console.ReadLine();
                    if (yn is "n") return;
                }

                Settings.Reset();
            } else {
                Console.WriteLine(Chalk.Red + "対応していない引数が与えられました。");
                PrintHelp();
            }
        } catch (Exception e) {
            Console.WriteLine(Chalk.Red + e.Message);
            Console.ResetColor();
        }
    }

    public Program() {
        Console.WriteLine("- 設定の読み込み");
        s_settings = Settings.Load();

        Console.WriteLine("- フォルダ作成");
        var now = DateTime.Now;
        for (int i = 0; ; i++) {
            var dir = new DirectoryInfo($"local-test-files/{now:MMdd}_{i:00}");
            if (dir.Exists) continue;

            s_logDir = dir;
            s_logDir.Create();
            break;
        }
    }

    public async Task Compile() {
        if (s_settings.CompilationCmd.Trim() == "") return;

        Console.WriteLine("- コンパイル");

        using var cts = new CancellationTokenSource(
            TimeSpan.FromMilliseconds(s_settings.CompilationTimeLimit)
        );

        await foreach (var item in ProcessX.StartAsync(s_settings.CompilationCmd).WithCancellation(cts.Token)) {
            Console.WriteLine(item);
        }

        if (cts.IsCancellationRequested) {
            throw new TimeoutException("コンパイル時間が上限を超えました。");
        }

        if (!s_settings.CompilationSuccessFile.Exists) {
            throw new FileNotFoundException("コンパイルが正常に完了しませんでした。");
        }
    }

    public void ExecuteAll() {
        Console.WriteLine(Chalk.Cyan["-------- 実行・採点 --------"]);

        double avg = 0;
        int cnt = 0;
        object lockObj = new();

        // 並列実行
        var results =
            s_settings.InFilesDir.EnumerateFiles()
            .Where(x => !s_settings.InFilesExcluded.Contains(x.Name))
            .AsParallel()
            .WithDegreeOfParallelism(s_settings.ParallelDegree)
            .Select(inFile => {
                var caseName = Path.GetFileNameWithoutExtension(inFile.Name);
                var outFile = new FileInfo(Path.Combine(s_logDir.FullName, $"{caseName}_out.txt"));
                var errFile = new FileInfo(Path.Combine(s_logDir.FullName, $"{caseName}_err.txt"));
                var evalFile = new FileInfo(Path.Combine(s_logDir.FullName, $"{caseName}_eval.txt"));

                // 実行
                var executor = new Executor(
                    caseName,
                    inFile, outFile, errFile, evalFile,
                    s_settings
                );

                var executionResult = executor.Run();

                // 評価
                var evaluator = new Evaluator(
                    caseName, executionResult,
                    inFile, outFile, errFile, evalFile,
                    s_settings
                );

                var evaluationResult = evaluator.Run();

                // 結果を更新・表示
                var caseSummary = evaluationResult.Summarize();
                lock (lockObj) {
                    avg = (avg * cnt + evaluationResult.Score) / (cnt + 1);
                    cnt++;

                    ConsoleOp.ClearLine();
                    Console.WriteLine(caseSummary);
                    Console.Write($"Avg: {avg}");
                }

                return evaluationResult;
            })
            .ToArray();

        Console.WriteLine(Chalk.Cyan["-------- 結果の要約 --------"]);
        Console.WriteLine(EvaluationResult.SummarizeAll(results));
    }

    public void ExecuteManually() {
        throw new NotImplementedException();
    }

    static void PrintHelp() {
        Console.WriteLine(
            $"""

            local-test-tool v{s_version}
            ヒューリスティックスコンテストのテストケースをローカルで実行します。
            コンパイルや実行のコマンドは、 ./settings.jsonc に記述してください。

            オプション: 
                (none)      設定に従い、並列テストを実行します。
                -m          手動での入出力により、テストを実行します。
                help / -h   ヘルプを表示します。
                reset       ./settings.jsonc を初期化します。
            """
        );
    }
}
