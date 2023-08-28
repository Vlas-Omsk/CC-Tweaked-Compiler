namespace CCTweaked.Compiler.Controllers
{
    internal sealed class ArgumentsContoller
    {
        private readonly Dictionary<string, string[]> _manyPairs = new();
        private readonly Dictionary<string, string> _pairs = new();
        private string[] _args;

        public void SetArgs(string[] args)
        {
            _args = args;
        }

        public string Get(int i)
        {
            return _args[i];
        }

        public void Set(string name, string value)
        {
            _pairs[name] = value;
        }

        public string Get(string name)
        {
            return _pairs[name];
        }

        public void SetMany(string name, string[] value)
        {
            _manyPairs[name] = value;
        }

        public string[] GetMany(string name)
        {
            return _manyPairs[name];
        }
    }
}
