using CCTweaked.Compiler.Controllers;

namespace CCTweaked.Compiler.Commands
{
    [Command("restore")]
    internal sealed class RestoreCommand : ICommand
    {
        private readonly ConfigController _configController;

        public RestoreCommand(
            ConfigController configController
        )
        {
            _configController = configController;
        }

        public void Execute()
        {
            _configController.RestoreConfig();
            _configController.UpdateConfig();
        }
    }
}
