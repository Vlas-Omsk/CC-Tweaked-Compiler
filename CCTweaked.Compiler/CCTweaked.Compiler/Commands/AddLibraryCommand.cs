using CCTweaked.Compiler.Controllers;

namespace CCTweaked.Compiler.Commands
{
    [Command("add", "library", "path:*")]
    internal sealed class AddLibraryCommand : ICommand
    {
        private readonly ConfigController _configController;
        private readonly ArgumentsContoller _argumentsContoller;

        public AddLibraryCommand(
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

            if (!Directory.Exists(path))
                throw new Exception("Directory not exists");

            if (_configController.Config.LibraryPaths.All(x => x != path))
                _configController.Config.LibraryPaths.Add(path);

            _configController.RestoreConfig();
            _configController.UpdateConfig();
        }
    }
}
