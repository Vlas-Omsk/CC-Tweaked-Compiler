using CCTweaked.Compiler.ConfigLoaders;
using CCTweaked.Compiler.Controllers;

namespace CCTweaked.Compiler.Commands
{
    [Command("add", "config", "type:*")]
    internal sealed class AddConfigCommand : ICommand
    {
        private readonly ConfigController _configController;
        private readonly ArgumentsContoller _argumentsContoller;

        public AddConfigCommand(
            ConfigController configController,
            ArgumentsContoller argumentsContoller
        )
        {
            _configController = configController;
            _argumentsContoller = argumentsContoller;
        }

        public void Execute()
        {
            switch (_argumentsContoller.Get("type"))
            {
                case "luarc":
                    if (LuarcConfigLoader.Exists())
                        break;

                    _configController.AddConfigLoader(
                        LuarcConfigLoader.Create(_configController.Config)
                    );
                    break;
                default:
                    throw new Exception();
            }
        }
    }
}
