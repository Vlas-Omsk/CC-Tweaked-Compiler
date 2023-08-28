using CCTweaked.Compiler.ConfigLoaders;
using CCTweaked.Compiler.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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