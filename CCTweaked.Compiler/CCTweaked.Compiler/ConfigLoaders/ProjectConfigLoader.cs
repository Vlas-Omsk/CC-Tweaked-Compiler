using PinkJson2;
using PinkJson2.Formatters;
using PinkJson2.Serializers;

namespace CCTweaked.Compiler.ConfigLoaders
{
    internal sealed class ProjectConfigLoader : IConfigLoader
    {
        private static readonly SystemPath _path = new SystemPath(".luaproj.json");

        public void Update(Config config)
        {
            using var streamWriter = new StreamWriter(_path);

            new PrettyFormatter().Format(config.Serialize(), streamWriter);
        }

        public static ProjectConfigLoader Create(Config config)
        {
            var projectConfigLoader = new ProjectConfigLoader();

            projectConfigLoader.Update(config);

            return projectConfigLoader;
        }

        public static ProjectConfigLoader Load(Config config)
        {
            var json = Json.Parse(File.ReadAllText(_path)).ToJson();

            new ObjectDeserializer().Deserialize(json, config);

            return new ProjectConfigLoader();
        }

        public static bool Exists()
        {
            return File.Exists(_path);
        }
    }
}
