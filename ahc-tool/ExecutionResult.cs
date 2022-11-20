namespace AhcTool;

enum Status { IE, TLE, RE, WA, AC }

record ExecutionResult(
    string TestCaseName,
    Status Status,
    double Score,
    int Time,
    FileInfo InputFile,
    FileInfo OutputFile,
    FileInfo EvaluationFile,
    string Msg
);
