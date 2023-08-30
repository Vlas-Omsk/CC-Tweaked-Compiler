using PinkJson2;
using PinkJson2.Formatters;

namespace CCTweaked.Compiler.ConfigLoaders
{
    internal sealed class LuarcConfigLoader : IConfigLoader
    {
        private static readonly SystemPath _path = new SystemPath(".luarc.json");
        private const string _workspaceLibraryKey = "workspace.library";
        private readonly IJson _json;

        public LuarcConfigLoader(IJson json)
        {
            _json = json;
        }

        public void Update(Config config)
        {
            var workspaceLibraryJson = _json[_workspaceLibraryKey].AsArray();

            workspaceLibraryJson.Clear();

            foreach (var metaPath in config.MetaPaths)
                workspaceLibraryJson.AddValueLast(metaPath);

            workspaceLibraryJson.AddValueLast(GlobalConfigLoader.FilePath);

            Write();
        }

        private void Write()
        {
            File.WriteAllText(_path, _json.ToString(new PrettyFormatter()));
        }

        public static LuarcConfigLoader Create(Config config)
        {
            var json = new Dictionary<string, object>()
            {
                { "runtime.version", "Lua 5.2" },
                { _workspaceLibraryKey, config.LibraryPaths.Concat(config.FilePaths).Concat(config.MetaPaths) }
            }.Serialize().ToJson();

            var configLoader = new LuarcConfigLoader(json);

            configLoader.Write();

            return configLoader;
        }

        public static LuarcConfigLoader Load()
        {
            var json = Json.Parse(File.ReadAllText(_path)).ToJson();

            return new LuarcConfigLoader(json);
        }

        public static bool Exists()
        {
            return File.Exists(_path);
        }
    }
}
