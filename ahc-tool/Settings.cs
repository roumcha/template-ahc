namespace AhcTool;

using Microsoft.Extensions.Configuration;

enum LanguageMode { Bin, Python }

record Settings(
    bool IsInteractiveProblem,
    LanguageMode LanguageMode,
    FileInfo SubmissionBinOrScript,
    FileInfo TestTool,
    string TestToolArgs,
    FileInfo EvaluationTool,
    string EvaluationToolArgs,
    int ParallelDegree,
    int TimeLimit,
    DirectoryInfo TempDir,
    DirectoryInfo TempInDir,
    DirectoryInfo TempOutDir
) {
    class Loaded {
        public bool IsInteractiveProblem { get; set; }
        public string SubmissionBinOrScript { get; set; } = "";
        public string TestTool { get; set; } = "";
        public string TestToolArgs { get; set; } = "";
        public string EvaluationTool { get; set; } = "";
        public string EvaluationToolArgs { get; set; } = "";
        public int ParallelDegree { get; set; }
        public int TimeLimitInMilliSeconds { get; set; }
    }

    public static Settings Init() {
        var timeStr = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        var loaded =
            new ConfigurationBuilder()
            .AddJsonFile(
                Path.Combine(Directory.GetCurrentDirectory(), "settings.jsonc")
            )
            .Build()
            .Get<Loaded>();

        if (loaded is null) {
            throw new FileLoadException("./settings.json を読み込めません。");
        }

        var submission = new FileInfo(loaded.SubmissionBinOrScript!);
        if (!submission.Exists) {
            throw new FileNotFoundException("提出プログラムが見つかりません。");
        }

        var test = new FileInfo(loaded.TestTool!);
        if (loaded.IsInteractiveProblem && !test.Exists) {
            throw new FileNotFoundException("テストプログラムが見つかりません。");
        }

        var evaluation = new FileInfo(loaded.EvaluationTool!);
        if (!evaluation.Exists) {
            throw new FileNotFoundException("採点プログラムが見つかりません。");
        }

        return new(
            IsInteractiveProblem: loaded.IsInteractiveProblem,
            LanguageMode:
                loaded.SubmissionBinOrScript!.EndsWith("py")
                ? LanguageMode.Python
                : LanguageMode.Bin,
            SubmissionBinOrScript: submission,
            TestTool: test,
            TestToolArgs: loaded.TestToolArgs!,
            EvaluationTool: evaluation,
            EvaluationToolArgs: loaded.EvaluationToolArgs!,
            ParallelDegree: loaded.ParallelDegree,
            TimeLimit: loaded.TimeLimitInMilliSeconds,
            TempDir: new("./ahc-tool-temp"),
            TempInDir: new("./ahc-tool-temp/in"),
            TempOutDir: new($"./ahc-tool-temp/out_{timeStr}")
        );
    }
}
