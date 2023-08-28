namespace CCTweaked.Compiler
{
    internal static class DirectoryUtils
    {
        public static IEnumerable<string> DeepEnumerateFiles(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path))
                yield return file;

            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                foreach (var entry in DeepEnumerateFiles(directory))
                    yield return entry;
            }
        }
    }
}
