namespace CCTweaked.Compiler
{
    internal enum SyntaxPartType
    {
        Static,
        Any,
        Many
    }

    internal sealed class SyntaxPart
    {
        public SyntaxPart(SyntaxPartType type, string name, string value)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        public SyntaxPart(SyntaxPartType type, string name)
        {
            Type = type;
            Name = name;
        }

        public SyntaxPartType Type { get; }
        public string Name { get; }
        public string Value { get; }

        public static SyntaxPart Parse(string value)
        {
            var split = value.Split(':');

            string argName, argValue;

            if (split.Length == 1)
            {
                argName = null;
                argValue = split[0];
            }    
            else if (split.Length == 2)
            {
                argName = split[0];
                argValue = split[1];
            }
            else
            {
                throw new Exception("Invalid syntax");
            }

            return new SyntaxPart(
                argValue switch
                {
                    "*" => SyntaxPartType.Any,
                    "+" => SyntaxPartType.Many,
                    _ => SyntaxPartType.Static,
                },
                 argName,
                 argValue
            );
        }
    }
}
