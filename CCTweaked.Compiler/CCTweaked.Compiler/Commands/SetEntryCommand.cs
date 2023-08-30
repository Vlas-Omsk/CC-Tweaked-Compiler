using CCTweaked.Compiler.Controllers;

namespace CCTweaked.Compiler.Commands
{
    [Command("set", "entry", "path:*")]
    internal sealed class SetEntryCommand : ICommand
    {
        private readonly ConfigController _configController;
        private readonly ArgumentsContoller _argumentsContoller;

        public SetEntryCommand(
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

            if (!TryFindExistingLuaFile(path, out path))
                throw new Exception("File not exists");

            _configController.Config.EntryFilePath = path;

            _configController.RestoreConfig();
            _configController.UpdateConfig();
        }

        private static bool TryFindExistingLuaFile(SystemPath path, out SystemPath newPath)
        {
            if (File.Exists(path))
            {
                newPath = path;
                return true;
            }

            if (string.IsNullOrEmpty(path.GetExtension()))
                path += ".lua";

            if (File.Exists(path))
            {
                newPath = path;
                return true;
            }

            newPath = null;
            return false;
        }
    }
}
