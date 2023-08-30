namespace CCTweaked.Compiler
{
    internal enum SystemPathType
    {
        Absolute,
        Relative,
    }

    internal readonly struct SystemPath
    {
        public SystemPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            Path = path;
            Type = System.IO.Path.IsPathRooted(Path) ?
                SystemPathType.Absolute :
                SystemPathType.Relative;
        }

        public SystemPath(params string[] paths) :
            this(System.IO.Path.Combine(paths))
        {
        }

        public string Path { get; }
        public SystemPathType Type { get; }

        public SystemPath GetAbsolutePath()
        {
            return GetAbsolutePathString();
        }

        private string GetAbsolutePathString()
        {
            return Type switch
            {
                SystemPathType.Absolute => Path,
                SystemPathType.Relative => System.IO.Path.GetFullPath(Path),
                _ => throw new InvalidOperationException(),
            };
        }

        public SystemPath GetRelativePath()
        {
            return GetRelativePath(Directory.GetCurrentDirectory());
        }

        public SystemPath GetRelativePath(string relativeTo)
        {
            return new SystemPath(
                System.IO.Path.GetRelativePath(relativeTo, GetAbsolutePathString())
            );
        }

        public string GetFileNameWithoutExtension()
        {
            return System.IO.Path.GetFileNameWithoutExtension(Path);
        }

        public string GetExtension()
        {
            return System.IO.Path.GetExtension(Path);
        }

        public override int GetHashCode()
        {
            return GetAbsolutePathString().GetHashCode();
        }

        public bool Equals(string value)
        {
            return GetAbsolutePathString() == System.IO.Path.GetFullPath(value);
        }

        public bool Equals(SystemPath value)
        {
            return Equals(value.Path);
        }

        public override bool Equals(object obj)
        {
            if (obj is string @string)
                return Equals(@string);
            if (obj is SystemPath @path)
                return Equals(@path);

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return Path;
        }

        public static SystemPath operator +(SystemPath left, string right)
        {
            return new SystemPath(left.GetAbsolutePathString(), right);
        }

        public static bool operator ==(SystemPath left, SystemPath right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SystemPath left, SystemPath right)
        {
            return !(left == right);
        }

        public static implicit operator string(SystemPath path)
        {
            return path.Path;
        }

        public static implicit operator SystemPath(string path)
        {
            return new SystemPath(path);
        }
    }
}
