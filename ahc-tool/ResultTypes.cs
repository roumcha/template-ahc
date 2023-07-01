using System.Text;
using Kokuban;

namespace AhcTool;

enum Status { AC, TLE, RE, WA, IE }

record ExecutionResult(
    string TestCaseName,
    Status Status,
    int Time,
    string Msg
);

record EvaluationResult(
    string TestCaseName,
    Status Status,
    double Score,
    int Time,
    string Msg
) {
    public string Summarize() {
        var sb = new StringBuilder();
        sb.Append($"[{Status}]\t");
        sb.Append($"{TestCaseName}\t");
        sb.Append($"{Score:0.0}pt\t");
        sb.Append($"{Time,4}ms");

        return Status != Status.AC ? sb.ToString() : Chalk.Red + sb.ToString();
    }

    public static string SummarizeAll(EvaluationResult[] results) {
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

        var scores = results.Select(x => x.Score).OrderBy(x => x).ToArray();
        var sum = scores.Sum();
        var avg = scores.Average();
        var med = scores.Length % 2 == 1
            ? scores[scores.Length / 2]
            : (scores[scores.Length / 2 - 1] + scores[scores.Length / 2]) / 2;
        var min = results.Min(x => x.Score);
        var max = results.Max(x => x.Score);

        var sb = new StringBuilder();
        sb.AppendLine($"Res:\t{status}");
        sb.AppendLine($"Sum:\t{sum:0.0}");
        sb.AppendLine($"Avg:\t{avg:0.0}");
        sb.AppendLine($"Med:\t{med:0.0}");
        sb.AppendLine($"Min:\t{min:0.0}");
        sb.Append($"Max:\t{max:0.0}");
        return sb.ToString();
    }
}
