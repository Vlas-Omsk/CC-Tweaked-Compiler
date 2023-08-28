namespace CCTweaked.Compiler
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class CommandAttribute : Attribute
    {
        public CommandAttribute(params string[] syntax)
        {
            Syntax = syntax.Select(SyntaxPart.Parse).ToArray();
        }

        public SyntaxPart[] Syntax { get; }
    }
}
