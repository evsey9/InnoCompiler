namespace LexicalAnalyzer
{
    public class Token
    {
        public Token(TokenType tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        public Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public Token(TokenType tokenType, string value, int line, int column)
        {
            TokenType = tokenType;
            Value = value;
            Line = line;
            Column = column;
        }

        public TokenType TokenType { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Token Clone()
        {
            return new Token(TokenType, Value);
        }
    }
}