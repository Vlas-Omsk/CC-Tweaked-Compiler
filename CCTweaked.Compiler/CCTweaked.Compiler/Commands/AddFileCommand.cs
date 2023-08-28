using CCTweaked.Compiler.Controllers;

namespace CCTweaked.Compiler.Commands
{
    [Command("add", "file", "path:*")]
    internal sealed class AddFileCommand : ICommand
    {
        private readonly ConfigController _configController;
        private readonly ArgumentsContoller _argumentsContoller;

        public AddFileCommand(
            ConfigController configController,
            ArgumentsContoller argumentsContoller
        )
        {
            _configController = configController;
            _argumentsContoller = argumentsContoller;
        }

        public void Execute()
        {
            var path = _argumentsContoller.Get("path");

            path = CreateLuaFileIfNotExists(path);

            var fullPath = Path.GetFullPath(path);

            if (_configController.Config.FilePaths.All(x => Path.GetFullPath(x) != fullPath))
                _configController.Config.FilePaths.Add(Path.GetRelativePath(Directory.GetCurrentDirectory(), path));

            _configController.RestoreConfig();
            _configController.UpdateConfig();
        }

        private string CreateLuaFileIfNotExists(string path)
        {
            if (File.Exists(path))
                return path;

            if (string.IsNullOrEmpty(Path.GetExtension(path)))
                path += ".lua";

            if (File.Exists(path))
                return path;

            File.WriteAllText(path, "");

            return path;
        }
    }
}
