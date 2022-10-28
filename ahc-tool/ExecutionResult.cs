namespace AhcTool;

enum Status { IE, RE, WA, TLE, AC }

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
