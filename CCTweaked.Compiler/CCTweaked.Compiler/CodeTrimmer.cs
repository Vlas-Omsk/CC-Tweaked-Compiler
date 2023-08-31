namespace CCTweaked.Compiler
{
    internal sealed class CodeTrimmer
    {
        private readonly static char[] _commentEnd = new char[] { '-', '-', ']' };
        private readonly StreamWriter _writer;
        private readonly StreamReader _reader;
        private char _current;
        private char? _peek;
        private bool _prevIsMinus = false;
        private bool _isInString = false;
        private char _stringBeginChar = '\0';
        private bool _prevIsWhiteSpace = false;

        public CodeTrimmer(StreamWriter writer, StreamReader reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public void Trim()
        {
            while (TryReadNext())
            {
                if (HandleComments())
                    continue;

                if (HandleStrings())
                {
                    _prevIsWhiteSpace = false;
                    continue;
                }

                if (HandleWhiteSpaces())
                    continue;

                _prevIsWhiteSpace = false;

                _writer.Write(_current);
            }
        }

        private bool HandleComments()
        {
            if (_current == '-')
            {
                if (_prevIsMinus)
                {
                    TrimComment();
                    _prevIsMinus = false;
                    return true;
                }

                _prevIsMinus = true;
                return true;
            }

            if (_prevIsMinus)
            {
                _writer.Write("-");
                _prevIsMinus = false;
            }

            return false;
        }

        private bool HandleStrings()
        {
            if (!_isInString && 
                _current is '\'' or '"')
            {
                if (_peek == _current)
                {
                    _writer.Write(_current);
                    TryReadNext();
                    _writer.Write(_current);
                    return true;
                }

                _isInString = true;
                _stringBeginChar = _current;

                _writer.Write(_current);
                return true;
            }

            if (_isInString)
            {
                if (_peek == _stringBeginChar &&
                    _current != '\\')
                {
                    _writer.Write(_current);
                    TryReadNext();
                    _writer.Write(_stringBeginChar);
                    _isInString = false;
                    return true;
                }

                _writer.Write(_current);
                return true;
            }

            return false;
        }

        private bool HandleWhiteSpaces()
        {
            if (char.IsWhiteSpace(_current))
            {
                if (!_prevIsWhiteSpace)
                {
                    _writer.Write(" ");
                    _prevIsWhiteSpace = true;
                }
                return true;
            }

            return false;
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
