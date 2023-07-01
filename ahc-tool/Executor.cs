using System.Diagnostics;
using Cysharp.Diagnostics;

namespace AhcTool;

class Executor {
    readonly string _caseName;
    readonly FileInfo _inFile, _outFile, _errFile, _evalFile;
    readonly Settings _settings;

    public Executor(
        string caseName,
        FileInfo inFile, FileInfo outFile, FileInfo errFile, FileInfo evalFile,
        Settings settings
    ) {
        _caseName = caseName;
        _inFile = inFile; _outFile = outFile; _errFile = errFile; _evalFile = evalFile;
        _settings = settings;
    }

    public ExecutionResult Run() {
        var inText = File.ReadAllText(_inFile.FullName);
        using var outStream = new StreamWriter(_outFile.FullName);
        using var errStream = new StreamWriter(
            _settings.IsInteractiveProblem ? _evalFile.FullName : _errFile.FullName
        );

        var cmd = _settings.IsInteractiveProblem
            ? _settings.ExecutionCmd
            : this.ReplaceArgVariables(_settings.EvaluationCmd);

        var stopwatch = new Stopwatch();
        var (process, stdOut, stdErr) = ProcessX.GetDualAsyncEnumerable(cmd);
        stopwatch.Start();
        process.Exited += (s, e) => stopwatch.Stop();

        var inTask = Task.Run(() => {
            process.StandardInput.WriteLine(inText);
            process.StandardInput.Close();
        });
        var outTask = Task.Run(async () => {
            await foreach (var item in stdOut) outStream.WriteLine(item);
        });
        var errTask = Task.Run(async () => {
            await foreach (var item in stdErr) errStream.WriteLine(item);
        });

        if (!process.WaitForExit((int)(_settings.ProblemTimeLimit * 1.1))) {
            process.Kill();
        }

        var re = CheckStdIO(inTask, outTask, errTask);

        var time = Math.Max(
            (int)stopwatch.ElapsedMilliseconds,
            process.TotalProcessorTime.Milliseconds
        );

        var status =
            time > _settings.ProblemTimeLimit
                ? Status.TLE
                : re ? Status.RE : Status.AC;

        process.Dispose();

        return new(
            _caseName,
            status,
            time,
            ""
        );
    }

    string ReplaceArgVariables(string argStr) {
        return argStr
            .Replace("$in", $"\"{_inFile.FullName}\"")
            .Replace("$out", $"\"{_outFile.FullName}\"")
            .Replace("$cmd", _settings.ExecutionCmd);
    }

    static bool CheckStdIO(Task inTask, Task outTask, Task errTask) {
        var res = true;

        try {
            inTask.Wait();
        } catch (Exception) {
            res = false;
        }

        try {
            outTask.Wait();
        } catch (Exception) {
            res = false;
        }

        try {
            errTask.Wait();
        } catch (Exception) {
            res = false;
        }

        return res;
    }
}
