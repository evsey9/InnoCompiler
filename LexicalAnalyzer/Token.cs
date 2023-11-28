namespace LexicalAnalyzer
{
    public class Token
    {
        public Token(Tokens tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        public Token(Tokens tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public Token(Tokens tokenType, string value, int line, int column, LexLocation location)
        {
            TokenType = tokenType;
            Value = value;
            Line = line;
            Column = column;
            Location = location;
        }

        public Tokens TokenType { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        
        public LexLocation Location { get; set; }

        public Token Clone()
        {
            return new Token(TokenType, Value);
        }
    }
}