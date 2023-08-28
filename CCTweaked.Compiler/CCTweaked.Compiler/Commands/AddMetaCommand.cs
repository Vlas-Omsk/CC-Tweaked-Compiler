using CCTweaked.Compiler.Controllers;

namespace CCTweaked.Compiler.Commands
{
    [Command("add", "meta", "path:*")]
    internal sealed class AddMetaCommand : ICommand
    {
        private readonly ConfigController _configController;
        private readonly ArgumentsContoller _argumentsContoller;

        public AddMetaCommand(
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

            if (!Directory.Exists(path))
                throw new Exception("Directory not exists");

            var fullPath = Path.GetFullPath(path);

            if (_configController.Config.MetaPaths.All(x => Path.GetFullPath(x) != fullPath))
                _configController.Config.MetaPaths.Add(Path.GetRelativePath(Directory.GetCurrentDirectory(), path));

            _configController.RestoreConfig();
            _configController.UpdateConfig();
        }
    }
}
