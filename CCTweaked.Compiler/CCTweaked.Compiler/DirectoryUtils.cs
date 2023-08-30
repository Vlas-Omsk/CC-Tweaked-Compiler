namespace CCTweaked.Compiler
{
    internal static class DirectoryUtils
    {
        public static IEnumerable<SystemPath> DeepEnumerateFiles(SystemPath directoryPath)
        {
            foreach (var file in Directory.EnumerateFiles(directoryPath))
                yield return new SystemPath(file);

            foreach (var directory in Directory.EnumerateDirectories(directoryPath))
            {
                foreach (var entry in DeepEnumerateFiles(new SystemPath(directory)))
                    yield return entry;
            }
        }
    }
}
