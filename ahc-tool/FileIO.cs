namespace AhcTool;

static class FileIO {
    public static void DeleteChildren(DirectoryInfo dir) {
        foreach (var f in dir.EnumerateFiles()) {
            f.Delete();
        }

        foreach (var d in dir.EnumerateDirectories()) {
            DeleteChildren(d);
            d.Delete();
        }
    }
}
