namespace CCTweaked.Compiler
{
    internal static class SystemPathExtensions
    {
        public static string GetModuleName(this SystemPath self)
        {
            var moduleName = self.GetFileNameWithoutExtension();

            return char.ToUpper(moduleName[0]) + moduleName.Substring(1);
        }
    }
}
