using CCTweaked.Compiler.ConfigLoaders;
using CCTweaked.Compiler.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PinkJson2;
using PinkJson2.Formatters;

namespace CCTweaked.Compiler
{
    internal sealed class Program
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Program> _logger;

        public Program(
            IServiceProvider serviceProvider,
            ILogger<Program> logger
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Start(string[] args)
        {
            using var serviceScope = _serviceProvider.CreateScope();

            var commandsController = serviceScope.ServiceProvider.GetRequiredService<CommandsController>();

            if (commandsController.TryExecuteCommand(args))
                return;

            _logger.LogInformation("Command not found");
        }

        private static void Main(string[] args)
        {
            TypeConverter.Default.AddPrimitiveType(typeof(SystemPath));
            TypeConverter.Default.Register(new TypeConversion(
                typeof(SystemPath),
                TypeConversionDirection.ToType,
                (object obj, Type targetType, ref bool handled) =>
                {
                    if (obj is string @string)
                    {
                        handled = true;
                        return new SystemPath(@string);
                    }

                    handled = false;
                    return null;
                }
            ));
            TypeConverter.Default.Register(new TypeConversion(
                typeof(SystemPath),
                TypeConversionDirection.FromType,
                (object obj, Type targetType, ref bool handled) =>
                {
                    if (targetType == typeof(FormattedValue))
                    {
                        handled = true;
                        return new FormattedValue(
                            ((SystemPath)obj).GetRelativePath()
                        );
                    }

                    handled = false;
                    return null;
                }
            ));

            var serviceCollection = new ServiceCollection();

            var configController = new ConfigController();

            configController.AddConfigLoader(new GlobalConfigLoader());

            if (ProjectConfigLoader.Exists())
                configController.AddConfigLoader(ProjectConfigLoader.Load(configController.Config));
            else
                configController.AddConfigLoader(ProjectConfigLoader.Create(configController.Config));

            if (LuarcConfigLoader.Exists())
                configController.AddConfigLoader(LuarcConfigLoader.Load());

            serviceCollection.AddSingleton(configController);
            serviceCollection.AddSingleton<CommandsController>();
            serviceCollection.AddScoped<ArgumentsContoller>();

            CommandsController.AddCommands(serviceCollection);

            serviceCollection.AddLogging(x => x.AddConsole());

            serviceCollection.AddSingleton<Program>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var program = serviceProvider.GetRequiredService<Program>();

            program.Start(args);
        }
    }
}