using Cysharp.Diagnostics;

namespace AhcTool;

class Evaluator {
    readonly string _caseName;
    readonly ExecutionResult _executionResult;
    readonly FileInfo _inFile, _outFile, _errFile, _evalFile;
    readonly Settings _settings;

    public Evaluator(
        string caseName, ExecutionResult executionResult,
        FileInfo inFile, FileInfo outFile, FileInfo errFile, FileInfo evalFile,
        Settings settings
    ) {
        _caseName = caseName;
        _executionResult = executionResult;
        _inFile = inFile; _outFile = outFile; _errFile = errFile; _evalFile = evalFile;
        _settings = settings;
    }

    public EvaluationResult Run() {
        if (_executionResult.Status != Status.AC) {
            return new(
                _caseName, _executionResult.Status, 0, _executionResult.Time, ""
            );
        }

        if (!_settings.IsInteractiveProblem) {
            this.RunEvaluationTool().Wait();
        }

        return this.BuildResult();
    }

    async Task RunEvaluationTool() {
        var cmd = this.ReplaceCommandVars(_settings.EvaluationCmd);
        var (process, stdOut, stdErr) = ProcessX.GetDualAsyncEnumerable(cmd);
        var outText = string.Join(Environment.NewLine, await stdOut.ToTask());
        var errText = string.Join(Environment.NewLine, await stdErr.ToTask());
        process.WaitForExit();

        using var evalStream = new StreamWriter(_evalFile.FullName);
        evalStream.WriteLine("----- stdout -----");
        evalStream.WriteLine(outText);
        evalStream.WriteLine("----- stderr -----");
        evalStream.WriteLine(errText);
    }

    string ReplaceCommandVars(string cmd) {
        return cmd
            .Replace("$in", _inFile.FullName)
            .Replace("$out", _outFile.FullName);
    }

    EvaluationResult BuildResult() {
        var text = File.ReadAllText(_evalFile.FullName);

        if (_settings.WaRegex.IsMatch(text)) {
            return new(_caseName, Status.WA, 0, _executionResult.Time, "");
        }

        if (_settings.ReRegex.IsMatch(text)) {
            return new(_caseName, Status.RE, 0, _executionResult.Time, "");
        }

        var scoreMatches = _settings.ScoreRegex.Matches(text);
        if (scoreMatches.Count == 0) {
            throw new Exception("スコアが読み取れませんでした。");
        }

        if (!double.TryParse(scoreMatches[^1].Value, out var score)) {
            throw new Exception("スコアが読み取れませんでした。");
        }

        return new(_caseName, Status.AC, score, _executionResult.Time, "");
    }

}
