namespace CCTweaked.Compiler
{
    internal sealed class Config
    {
        public string EntryFilePath { get; set; }
        public List<string> FilePaths { get; private set; } = new();
        public List<string> LibraryPaths { get; private set; } = new();
        public List<string> MetaPaths { get; private set; } = new();
    }
}
