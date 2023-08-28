using CCTweaked.Compiler.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Reflection;

namespace CCTweaked.Compiler.Controllers
{
    internal sealed class CommandsController
    {
        private static readonly ImmutableDictionary<CommandAttribute, Type> _commands;
        private readonly IServiceProvider _serviceProvider;
        private readonly ArgumentsContoller _argumentsContoller;
        private readonly ILogger<CommandsController> _logger;

        static CommandsController()
        {
            _commands = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Select(x => (Type: x, Attribute: x.GetCustomAttribute<CommandAttribute>()))
                .Where(x => x.Attribute != null)
                .ToImmutableDictionary(x => x.Attribute, x => x.Type);
        }

        public CommandsController(
            IServiceProvider serviceProvider,
            ArgumentsContoller argumentsContoller,
            ILogger<CommandsController> logger
        )
        {
            _serviceProvider = serviceProvider;
            _argumentsContoller = argumentsContoller;
            _logger = logger;
        }

        public bool TryExecuteCommand(string[] args)
        {
            foreach (var commandPair in _commands)
            {
                if (!IsCommandEquals(commandPair.Key, args))
                    continue;

                var command = (ICommand)_serviceProvider.GetRequiredService(commandPair.Value);
                var commandName = command.GetType().Name;

                _logger.LogInformation("Executing command {commandName}", commandName);

                SetArgs(commandPair.Key, args);

                command.Execute();

                _logger.LogInformation("Executed command {commandName}", commandName);

                return true;
            }

            return false;
        }

        private void SetArgs(CommandAttribute commandAttribute, string[] args)
        {
            _argumentsContoller.SetArgs(args);

            for (var i = 0; i < commandAttribute.Syntax.Length; i++)
            {
                var syntaxPart = commandAttribute.Syntax[i];
                var arg = args[i];

                if (syntaxPart.Name != null)
                {
                    if (syntaxPart.Type is SyntaxPartType.Static or SyntaxPartType.Any)
                        _argumentsContoller.Set(syntaxPart.Name, arg);
                    else if (syntaxPart.Type is SyntaxPartType.Many)
                        _argumentsContoller.SetMany(syntaxPart.Name, args[i..]);
                    else
                        throw new Exception();
                }
            }
        }

        private static bool IsCommandEquals(CommandAttribute commandAttribute, string[] args)
        {
            for (var i = 0; i < commandAttribute.Syntax.Length; i++)
            {
                var syntaxPart = commandAttribute.Syntax[i];

                if (i > args.Length - 1)
                    return false;

                var arg = args[i];

                switch (syntaxPart.Type)
                {
                    case SyntaxPartType.Any:
                        continue;
                    case SyntaxPartType.Static:
                        if (syntaxPart.Value != arg)
                            return false;
                        continue;
                    case SyntaxPartType.Many:
                        return true;
                    default:
                        throw new Exception();
                }
            }

            if (commandAttribute.Syntax.Length < args.Length)
                return false;

            return true;
        }

        public static void AddCommands(IServiceCollection serviceCollection)
        {
            foreach (var command in _commands)
                serviceCollection.AddTransient(command.Value);
        }
    }
}
