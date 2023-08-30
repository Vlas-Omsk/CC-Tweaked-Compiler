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
            var path = new SystemPath(_argumentsContoller.Get("path"));

            if (!Directory.Exists(path))
                throw new Exception("Directory not exists");

            if (_configController.Config.MetaPaths.All(x => x != path))
                _configController.Config.MetaPaths.Add(path);

            _configController.RestoreConfig();
            _configController.UpdateConfig();
        }
    }
}
