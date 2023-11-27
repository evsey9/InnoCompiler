using System.Collections.Generic;

namespace LexicalAnalyzer
{
    public interface ITokenizer
    {
        IEnumerable<Token> Tokenize(string queryDsl);
    }
}