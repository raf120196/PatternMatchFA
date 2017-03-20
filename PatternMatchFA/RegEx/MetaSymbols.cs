namespace PatternMatchFA
{
    internal class MetaSymbols
    {
        public const char CONCANATE = '.';
        public const char ALTERNATE = '|';
        public const char ZERO_OR_MORE = '*';
        public const char ONE_OR_MORE = '+';
        public const char ZERO_OR_ONE = '?';
        public const char OPEN_PREN = '(';
        public const char CLOSE_PREN = ')';
        public const char COMPLEMENT = '^';
        public const char ANY_ONE_CHAR = '_';
        public const string ANY_ONE_CHAR_TRANS = "~";
        public const char ESCAPE = '\\';
        public const string EPSILON = "ε";
        public const char CHARSET_START = '[';
        public const char CHARSET_END = ']';
        public const char RANGE = '-';
        public const string DUMMY = "'";
        public const char MATCH_START = '^';
        public const char MATCH_END = '$';
        public const char NEW_LINE = 'n';
        public const char TAB = 't';
    }
}
