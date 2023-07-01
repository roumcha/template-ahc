using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AhcTool;

record Settings(
    bool IsInteractiveProblem,
    int ProblemTimeLimit,
    int ProblemMemoryLimit,
    DirectoryInfo InFilesDir,
    string[] InFilesExcluded,
    string CompilationCmd,
    int CompilationTimeLimit,
    FileInfo CompilationSuccessFile,
    string ExecutionCmd,
    string EvaluationCmd,
    Regex ReRegex,
    Regex WaRegex,
    Regex ExecutionTimeRegex,
    Regex ScoreRegex,
    int ParallelDegree
) {
    public static Settings Load() {
        SettingsJson loaded;
        try {
            var jsonStr = File.ReadAllText("settings.jsonc");

            loaded =
                JsonSerializer.Deserialize(jsonStr, SourceGenerationContext.Default.SettingsJson)
                ?? throw new Exception();
        } catch {
            throw new FileLoadException("./settings.jsonc を読み込めませんでした。");
        }

        if (!Directory.Exists(loaded.InFilesDir)) {
            throw new DirectoryNotFoundException($"入力フォルダ {loaded.InFilesDir} が見つかりません。");
        }

        return new(
            IsInteractiveProblem: loaded.IsInteractiveProblem,
            ProblemTimeLimit: loaded.ProblemTimeLimit,
            ProblemMemoryLimit: loaded.ProblemMemoryLimit,
            InFilesDir: new(loaded.InFilesDir),
            InFilesExcluded: loaded.InFilesExcluded,
            CompilationCmd: loaded.CompilationCmd,
            CompilationTimeLimit: loaded.CompilationTimeLimit,
            CompilationSuccessFile: new(loaded.CompilationSuccessFile),
            ExecutionCmd: loaded.ExecutionCmd,
            EvaluationCmd: loaded.EvaluationCmd,
            ReRegex: new(loaded.ReRegex),
            WaRegex: new(loaded.WaRegex),
            ExecutionTimeRegex: new(loaded.ExecutionTimeRegex),
            ScoreRegex: new(loaded.ScoreRegex),
            ParallelDegree: loaded.ParallelDegree
        );
    }

    public static void Reset() {
        File.WriteAllText("settings.jsonc",
            """
            {
                // インタラクティブ問題か否か
                "IsInteractiveProblem": false,

                // 問題の実行時間制限（ミリ秒）
                "ProblemTimeLimit": 2000,

                // 問題のメモリ制限（MB）（未実装。無視されます）
                "ProblemMemoryLimit": 1024,

                // 入力ファイルのあるフォルダ
                "InFilesDir": "local-test-files/in",

                // 除外する入力ファイル名
                "InFilesExcluded": [".keep"],

                // コンパイルコマンド（無ければ空）
                "CompilationCmd": "dotnet publish -c Release -o publish --nologo",

                // コンパイル時間の上限（ミリ秒）
                "CompilationTimeLimit": 15000,

                // コンパイル成功を判定するファイル
                "CompilationSuccessFile": "publish/app.exe",

                // 実行コマンド
                "ExecutionCmd": "publish/app.exe",

                // 採点コマンド
                // 変数: $in（入力ファイル）, $out（出力ファイル）
                // 変数（インタラクティブ問題のみ）: $cmd（実行コマンド）
                "EvaluationCmd": "vis.exe \"$in\" \"$out\"",

                // 採点ツールの出力から RE を判定する正規表現（インタラクティブ問題のみ）
                "ReRegex": "runtime ?error",

                // 採点ツールの出力から WA を判定する正規表現
                "WaRegex": "(illegal|invalid|too|exception|error|unexpected|out ?of|terminated|unexpected)",

                // 採点ツールの出力から実行時間を取り出す正規表現（インタラクティブ問題のみ）
                "ExecutionTimeRegex": "(?<=time[ =:]+)[\\d]+(\\.[\\d]+)?(?=( ?ms| |\\n|\\r|$))",

                // 採点ツールの出力から得点を取り出す正規表現
                "ScoreRegex": "(?<=score ?[=:] ?)[\\d]+(\\.[\\d]+)?(?=(pt| |\\n|\\r|$))",

                // 同時実行数
                // ファイル入出力などのため、CPUコアを余らせた方がいいかも
                // 0 の場合、min(1, CPUスレッド数 / 2 - 1) を自動的に設定します
                "ParallelDegree": 0
            }

            """
        );
    }
}

record SettingsJson(
    bool IsInteractiveProblem,
    int ProblemTimeLimit,
    int ProblemMemoryLimit,
    string InFilesDir,
    string[] InFilesExcluded,
    string CompilationCmd,
    int CompilationTimeLimit,
    string CompilationSuccessFile,
    string ExecutionCmd,
    string EvaluationCmd,
    string ReRegex,
    string WaRegex,
    string ExecutionTimeRegex,
    string ScoreRegex,
    int ParallelDegree
);

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(SettingsJson))]
partial class SourceGenerationContext : JsonSerializerContext { }

