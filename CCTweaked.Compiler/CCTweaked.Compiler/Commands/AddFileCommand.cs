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
            var path = new SystemPath(_argumentsContoller.Get("path"));

            path = CreateLuaFileIfNotExists(path);

            if (_configController.Config.FilePaths.All(x => x != path))
                _configController.Config.FilePaths.Add(path);

            _configController.RestoreConfig();
            _configController.UpdateConfig();
        }

        private static SystemPath CreateLuaFileIfNotExists(SystemPath path)
        {
            if (File.Exists(path))
                return path;

            if (string.IsNullOrEmpty(path.GetExtension()))
                path += ".lua";

            if (File.Exists(path))
                return path;

            File.WriteAllText(path, "");

            return path;
        }
    }
}
