namespace CCTweaked.Compiler.ConfigLoaders
{
    internal sealed class GlobalConfigLoader : IConfigLoader
    {
        public static readonly SystemPath FilePath = new SystemPath("global.lua");

        public void Update(Config config)
        {
            File.Delete(FilePath);

            using var streamWriter = new StreamWriter(FilePath);

            foreach (var filePath in config.FilePaths)
                WriteModel(streamWriter, filePath);

            foreach (var libraryPath in config.LibraryPaths)
                foreach (var filePath in DirectoryUtils.DeepEnumerateFiles(libraryPath))
                    WriteModel(streamWriter, filePath);
        }

        private static void WriteModel(StreamWriter writer, SystemPath filePath)
        {
            var moduleName = filePath.GetModuleName();
            var modulePath = Path.ChangeExtension(
                filePath.GetRelativePath()
                    .Path
                    .Replace(Path.DirectorySeparatorChar, '.'),
                null
            );

            writer.WriteLine($"{moduleName} = require('{modulePath}')");
        }
    }
}
