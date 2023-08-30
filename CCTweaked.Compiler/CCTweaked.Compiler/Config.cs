namespace CCTweaked.Compiler
{
    internal sealed class Config
    {
        public SystemPath? EntryFilePath { get; set; }
        public List<SystemPath> FilePaths { get; private set; } = new();
        public List<SystemPath> LibraryPaths { get; private set; } = new();
        public List<SystemPath> MetaPaths { get; private set; } = new();
    }
}
