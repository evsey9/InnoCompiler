using System.Collections.Generic;
using System.Text.RegularExpressions;
using LexicalAnalyzer;

namespace LexicalAnalyzer
{
    public class TokenDefinition
    {
        private Regex _regex;
        private readonly Tokens _returnsToken;
        private readonly int _precedence;

        public TokenDefinition(Tokens returnsToken, string regexPattern, int precedence)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase|RegexOptions.Compiled);
            _returnsToken = returnsToken;
            _precedence = precedence;
        }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            var matches = _regex.Matches(inputString);
            for(int i = 0; i < matches.Count; i++)
            {
                yield return new TokenMatch()
                {
                    StartIndex = matches[i].Index,
                    EndIndex = matches[i].Index + matches[i].Length,
                    TokenType = _returnsToken,
                    Value = matches[i].Value,
                    Precedence = _precedence,
                    Length = matches[i].Length
                };
            }
        }
    }
    
    public class TokenMatch
    {
        public Tokens TokenType { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int Precedence { get; set; }
        public int StartLine { get; set; } 
        public int StartColumn { get; set; }
        
        public int Length { get; set; }
    }
}





