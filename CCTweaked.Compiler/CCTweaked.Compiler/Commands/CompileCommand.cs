using CCTweaked.Compiler.Controllers;

namespace CCTweaked.Compiler.Commands
{
    [Command("compile")]
    internal sealed class CompileCommand : ICommand
    {
        private const string _outputFilePath = "bundle.lua";
        private readonly ConfigController _configController;

        public CompileCommand(
            ConfigController configController
        )
        {
            _configController = configController;
        }

        public void Execute()
        {
            _configController.RestoreConfig();

            if (_configController.Config.EntryFilePath == null)
                throw new Exception("Entry file not set");

            File.Delete(_outputFilePath);

            using var streamWriter = new StreamWriter(_outputFilePath);

            StreamReader streamReader;
            CodeTrimmer codeTrimmer;

            foreach (var filePath in _configController.Config.FilePaths)
                WriteModule(streamWriter, filePath);

            foreach (var libraryPath in _configController.Config.LibraryPaths)
                foreach (var filePath in DirectoryUtils.DeepEnumerateFiles(libraryPath))
                    WriteModule(streamWriter, filePath);

            streamReader = new StreamReader(_configController.Config.EntryFilePath);

            codeTrimmer = new CodeTrimmer(streamWriter, streamReader);
            codeTrimmer.Trim();

            streamReader.Dispose();
        }

        private static void WriteModule(StreamWriter writer, SystemPath filePath)
        {
            var reader = new StreamReader(filePath);

            writer.Write($"{filePath.GetModuleName()} = (function () ");

            var codeTrimmer = new CodeTrimmer(writer, reader);
            codeTrimmer.Trim();

            writer.Write(" end)() ");

            reader.Dispose();
        }
    }
}
