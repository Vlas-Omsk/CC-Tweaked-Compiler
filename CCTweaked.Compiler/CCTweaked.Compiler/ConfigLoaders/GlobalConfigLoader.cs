using System.IO;

namespace CCTweaked.Compiler.ConfigLoaders
{
    internal sealed class GlobalConfigLoader : IConfigLoader
    {
        public const string FilePath = "global.lua";

        public void Update(Config config)
        {
            File.Delete(FilePath);

            using var streamWriter = new StreamWriter(FilePath);

            foreach (var filePath in config.FilePaths)
                WriteModele(streamWriter, filePath);

            foreach (var libraryPath in config.LibraryPaths)
                foreach (var filePath in DirectoryUtils.DeepEnumerateFiles(libraryPath))
                    WriteModele(streamWriter, filePath);
        }

        private static void WriteModele(StreamWriter writer, string filePath)
        {
            var moduleName = ModuleUtils.GetModuleNameFromFilePath(filePath);
            var modulePath = Path.ChangeExtension(filePath.Replace(Path.DirectorySeparatorChar, '.'), null);

            writer.WriteLine($"{moduleName} = require('{modulePath}')");
        }
    }
}
