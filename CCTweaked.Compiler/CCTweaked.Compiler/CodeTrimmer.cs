namespace CCTweaked.Compiler
{
    internal sealed class CodeTrimmer
    {
        private readonly static char[] _commentEnd = new char[] { '-', '-', ']' };
        private readonly StreamWriter _writer;
        private readonly StreamReader _reader;
        private char _current;
        private char? _peek;

        public CodeTrimmer(StreamWriter writer, StreamReader reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public void Trim()
        {
            var prevIsWhiteSpace = false;
            var prevIsMinus = false;

            while (TryReadNext())
            {
                if (char.IsWhiteSpace(_current))
                {
                    if (!prevIsWhiteSpace)
                    {
                        _writer.Write(" ");
                        prevIsWhiteSpace = true;
                    }
                    continue;
                }

                prevIsWhiteSpace = false;

                if (_current == '-')
                {
                    if (prevIsMinus)
                    {
                        TrimComment();
                        prevIsMinus = false;
                        continue;
                    }

                    prevIsMinus = true;
                    continue;
                }

                if (prevIsMinus)
                {
                    _writer.Write("-");
                    prevIsMinus = false;
                }

                _writer.Write(_current);
            }
        }

        private void TrimComment()
        {
            if (_peek == '\n')
                return;
            if (!TryReadNext())
                throw new Exception();

            var firstChar = _current;

            if (_peek == '\n')
                return;
            if (!TryReadNext())
                throw new Exception();

            var secondChar = _current;

            if (firstChar == '[' && secondChar == '[')
            {
                var prev = new Queue<char>(4);

                while (TryReadNext())
                {
                    if (prev.Count == 3 &&
                        _current == ']' &&
                        prev.SequenceEqual(_commentEnd))
                        return;

                    prev.Enqueue(_current);

                    if (prev.Count == 4)
                        prev.Dequeue();
                }

                throw new Exception();
            }
            else
            {
                while (TryReadNext())
                {
                    if (_current == '\n')
                        return;
                }

                throw new Exception();
            }
        }

        private bool TryReadNext()
        {
            var current = _reader.Read();

            if (_peek.HasValue)
            {
                _current = _peek.Value;

                _peek = current != -1 ? (char)current : null;
            }
            else
            {
                if (current == -1)
                    return false;

                _current = (char)current;

                current = _reader.Read();

                if (current == -1)
                    return false;

                _peek = (char)current;
            }

            return true;
        }
    }
}
