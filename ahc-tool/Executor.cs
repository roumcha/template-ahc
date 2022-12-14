namespace AhcTool;

using System.Diagnostics;
using System.Text.RegularExpressions;

partial class Executor {
    readonly Settings _settings;
    readonly string _testCaseName;
    readonly FileInfo _inputFile, _outFile, _errFile, _evalFile;

    public Executor(FileInfo inputFile, Settings settings) {
        _inputFile = inputFile;
        _settings = settings;
        _testCaseName = Path.GetFileNameWithoutExtension(_inputFile.Name);

        _outFile = new(
            Path.Combine(_settings.TempOutDir.FullName, $"{_testCaseName}_out.txt")
        );

        _errFile = new(
            Path.Combine(_settings.TempOutDir.FullName, $"{_testCaseName}_err.txt")
        );

        _evalFile = new(
            Path.Combine(_settings.TempOutDir.FullName, $"{_testCaseName}_eval.txt")
        );
    }

    public ExecutionResult Run()
    => _settings.IsInteractiveProblem ? this.RunInteractive() : this.RunOnce();

    ExecutionResult RunOnce() {
        var (status1, time) = this.ExecuteSubmission();
        this.ExecuteEvaluationTool();
        var (status2, score) = this.ParseEvaluationFile();

        return new(
            TestCaseName: _testCaseName,
            Status: (Status)Math.Min((int)status1, (int)status2),
            Score: score,
            Time: time,
            InputFile: _inputFile,
            OutputFile: _outFile,
            EvaluationFile: _evalFile,
            Msg: ""
        );
    }

    ExecutionResult RunInteractive() {
        var (status1, time) = this.ExecuteTestTool();
        var (status2, score) = this.ParseEvaluationFile();

        return new(
            TestCaseName: _testCaseName,
            Status: (Status)Math.Min((int)status1, (int)status2),
            Score: score,
            Time: time,
            InputFile: _inputFile,
            OutputFile: _outFile,
            EvaluationFile: _evalFile,
            Msg: ""
        );
    }

    (Status, int Time) ExecuteSubmission() {
        var inputText = File.ReadAllText(_inputFile.FullName);
        using var outStream = new StreamWriter(_outFile.FullName);
        using var errStream = new StreamWriter(_errFile.FullName);
        var stopwatch = new Stopwatch();

        var pInfo = new ProcessStartInfo() {
            FileName =
                _settings.LanguageMode == LanguageMode.Python
                ? "py"
                : $"\"{_settings.SubmissionBinOrScript.FullName}\"",
            Arguments =
                _settings.LanguageMode == LanguageMode.Python
                ? $"\"{_settings.SubmissionBinOrScript.FullName}\""
                : "",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = new Process() { StartInfo = pInfo };
        process.Exited += (s, e) => stopwatch.Stop();
        stopwatch.Start();
        process.Start();

        var inTask = process.StandardInput.WriteLineAsync(inputText);
        var outTask = Task.Run(() => ForwardText(process.StandardOutput, outStream));
        var errTask = Task.Run(() => ForwardText(process.StandardError, errStream));

        process.WaitForExit();
        inTask.Wait(); outTask.Wait(); errTask.Wait();

        var time = stopwatch.ElapsedMilliseconds;
        var status =
            process.ExitCode != 0 ? Status.RE
            : time > _settings.TimeLimit ? Status.TLE
            : Status.AC;
        return (status, (int)time);
    }

    void ExecuteEvaluationTool() {
        using var evalStream = new StreamWriter(_evalFile.FullName);

        var pInfo = new ProcessStartInfo() {
            FileName = $"\"{_settings.EvaluationTool.FullName}\"",
            Arguments = this.ReplaceArgVariables(_settings.EvaluationToolArgs),
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = new Process() { StartInfo = pInfo };
        process.Start();
        var fwTask = Task.Run(() => ForwardText(process.StandardOutput, evalStream));
        process.WaitForExit(); fwTask.Wait();
    }

    (Status, int Time) ExecuteTestTool() {
        var inputText = File.ReadAllText(_inputFile.FullName);
        using var outStream = new StreamWriter(_outFile.FullName);
        using var evalStream = new StreamWriter(_evalFile.FullName);
        var stopwatch = new Stopwatch();

        var pInfo = new ProcessStartInfo() {
            FileName = $"\"{_settings.TestTool.FullName}\"",
            Arguments = this.ReplaceArgVariables(_settings.TestToolArgs),
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        var process = new Process() { StartInfo = pInfo };
        process.Exited += (s, e) => stopwatch.Stop();
        stopwatch.Start();
        process.Start();

        var inTask = Task.Run(() => {
            process.StandardInput.WriteLine(inputText);
            process.StandardInput.Close();
        });
        var outTask = Task.Run(() => ForwardText(process.StandardOutput, outStream));
        var evalTask = Task.Run(() => ForwardText(process.StandardError, evalStream));

        inTask.Wait();
        outTask.Wait();
        evalTask.Wait();

        var time = stopwatch.ElapsedMilliseconds;
        var status =
            process.ExitCode != 0 ? Status.RE
            : time > _settings.TimeLimit ? Status.TLE
            : Status.AC;
        return (status, (int)time);
    }

    [GeneratedRegex(
        @"(illegal|invalid|too|exception|error|unexpected|out ?of|terminated)",
        RegexOptions.IgnoreCase
    )]
    private static partial Regex WaRegex();

    [GeneratedRegex(
        @"(?<=score[ =:]+)[\d]+(\.[\d]+)?(?=[ \n\r$])",
        RegexOptions.IgnoreCase
    )]
    private static partial Regex ScoreRegex();

    (Status, double Score) ParseEvaluationFile() {
        var text = File.ReadAllText(_evalFile.FullName);

        if (WaRegex().IsMatch(text)) {
            return (Status.WA, 0);
        }

        var scoreMatches = ScoreRegex().Matches(text);
        if (scoreMatches.Count == 0) {
            throw new Exception("?????????????????????????????????????????????");
        }

        if (!double.TryParse(scoreMatches[^1].Value, out var score)) {
            throw new Exception("?????????????????????????????????????????????");
        }

        return (Status.AC, score);
    }

    string ReplaceArgVariables(string argStr) {
        var submissionCmd = _settings.LanguageMode == LanguageMode.Python
            ? $"py.exe \"{_settings.SubmissionBinOrScript.FullName}\""
            : $"\"{_settings.SubmissionBinOrScript.FullName}\"";

        return argStr
            .Replace("$in", $"\"{_inputFile.FullName}\"")
            .Replace("$out", $"\"{_outFile.FullName}\"")
            .Replace("$cmd", submissionCmd);
    }

    static void ForwardText(StreamReader source, StreamWriter target) {
        while (!source.EndOfStream) {
            if (source.ReadLine() is string s) {
                target.WriteLine(s);
            }
        }
    }
}
