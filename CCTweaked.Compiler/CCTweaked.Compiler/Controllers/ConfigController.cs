using CCTweaked.Compiler.ConfigLoaders;

namespace CCTweaked.Compiler.Controllers
{
    internal sealed class ConfigController
    {
        private readonly List<IConfigLoader> _configLoaders = new();

        public Config Config { get; } = new();

        public void AddConfigLoader(IConfigLoader configLoader)
        {
            _configLoaders.Add(configLoader);
        }

        public void UpdateConfig()
        {
            foreach (var configLoader in _configLoaders)
                configLoader.Update(Config);
        }

        public void RestoreConfig()
        {
            foreach (var filePath in Config.FilePaths.ToArray())
                if (!File.Exists(filePath))
                    Config.FilePaths.Remove(filePath);

            foreach (var libraryPath in Config.LibraryPaths.ToArray())
                if (!Directory.Exists(libraryPath))
                    Config.LibraryPaths.Remove(libraryPath);

            foreach (var metaPath in Config.MetaPaths.ToArray())
                if (!Directory.Exists(metaPath))
                    Config.MetaPaths.Remove(metaPath);

            if (Config.EntryFilePath != null && !File.Exists(Config.EntryFilePath))
                Config.EntryFilePath = null;

            if (Config.EntryFilePath.HasValue)
                Config.FilePaths.Remove(Config.EntryFilePath.Value);

        }

        public static ConfigController Create()
        {
            var controller = new ConfigController();

            if (ProjectConfigLoader.Exists())
                controller._configLoaders.Add(ProjectConfigLoader.Load(controller.Config));
            else
                controller._configLoaders.Add(ProjectConfigLoader.Create(controller.Config));

            if (LuarcConfigLoader.Exists())
                controller._configLoaders.Add(LuarcConfigLoader.Load());

            return controller;
        }
    }
}
