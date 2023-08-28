namespace CCTweaked.Compiler
{
    internal static class ModuleUtils
    {
        public static string GetModuleNameFromFilePath(string filePath)
        {
            var moduleName = Path.GetFileNameWithoutExtension(filePath);

            return char.ToUpper(moduleName[0]) + moduleName.Substring(1);
        }
    }
}
