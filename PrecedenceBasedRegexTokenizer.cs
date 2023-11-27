using System.Collections.Generic;
using System.Linq;
using LexicalAnalyzer;

namespace LexicalAnalyzer
{
    internal class PrecedenceBasedRegexTokenizer : AbstractScanner<Token, LexLocation>, ITokenizer
    {
        private List<TokenDefinition> _tokenDefinitions;

        public PrecedenceBasedRegexTokenizer()
        {
            _tokenDefinitions = new List<TokenDefinition>();

            // Brackets
             _tokenDefinitions.Add(new TokenDefinition(TokenType.OpenCurlBr, "\\{", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.CloseCurlBr, "\\}", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.OpenRoundBr, "\\(", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.CloseRoundBr, "\\)", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.OpenSquareBr, "\\[", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.CloseSquareBr, "\\]", 2));

            // Values
             _tokenDefinitions.Add(new TokenDefinition(TokenType.StringVar, "'.*'|\".*\"", 1));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.IntVar, "[0-9]+", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.RealVar, "[0-9]+\\.[0-9]+", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.TrueKey, "true", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.FalseKey, "false", 3));

            // Expression
             _tokenDefinitions.Add(new TokenDefinition(TokenType.AndOp, "and\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.OrOp, "or\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.XorOp, "xor\\s", 3));

            //Relation
             _tokenDefinitions.Add(new TokenDefinition(TokenType.LessRel, "<", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.MoreRel, ">", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.LessEqRel, "<=", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.MoreEqRel, ">=", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.EqualRel, "=", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.NotEqRel, "/=", 2));

            //Factor
             _tokenDefinitions.Add(new TokenDefinition(TokenType.Plus, "\\+", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.Minus, "\\-", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.Multiplication, "\\*", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.Division, "\\/", 3));

             // Key words
             _tokenDefinitions.Add(new TokenDefinition(TokenType.PrintKey, "print\\s|print\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.ReturnKey, "return\\s|return\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.IfKey, "if\\s|if\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.ThenKey, "then\\s|then\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.ElseKey, "else\\s|else\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.EndKey, "end\\s|end\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.WhileKey, "while\\s|while\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.ForKey, "for\\s|for\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.InKey, "in\\s|in\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.LoopKey, "loop\\s|loop\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.FuncKey, "func\\s|func\\n", 3));

            // Type
             _tokenDefinitions.Add(new TokenDefinition(TokenType.IntKey, "int\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.RealKey, "real\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.BoolKey, "bool\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.StringKey, "string\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.EmptyKey, "empty\\s", 3));

            // FunBody
             _tokenDefinitions.Add(new TokenDefinition(TokenType.IsKey, "is\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.ArrowKey, "=>", 3));
             
             
             // Declaration
             _tokenDefinitions.Add(new TokenDefinition(TokenType.VarKey, "var\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.AssignOp, ":=", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.VarName, "[A-Za-z_]+[0-9A-Za-z_]*", 3));

            //Other
             _tokenDefinitions.Add(new TokenDefinition(TokenType.CommaSym, ",", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.SemicolonSym, ";", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.ColonSym, ":", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.OneLineComment, "//", 1));
             
             _tokenDefinitions.Add(new TokenDefinition(TokenType.UndefinedSymbol, ".", 5));
             
             // Spaces
             _tokenDefinitions.Add(new TokenDefinition(TokenType.SpaceSym, "\\s", 4));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.NewLineSym, "\\n", 2));
             _tokenDefinitions.Add(new TokenDefinition(TokenType.TabSym, "\\t", 3));

        }

        public IEnumerable<Token> Tokenize(string lqlText)
        {
            int line = 1;  // Initialize line number
            int column = 1;  // Initialize column number

            var tokenMatches = FindTokenMatches(lqlText);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            for (int i = 0; i < groupedByIndex.Count; i++)
            {
                var bestMatch = groupedByIndex[i].OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;

                //Condition for correct value saving 
                //switch (bestMatch.TokenType)
                //{
                //    case TokenType.SpaceSym:
                //        yield return new Token(bestMatch.TokenType, "\\s");
                //        break;
                //    case TokenType.TabSym:
                //        yield return new Token(bestMatch.TokenType, "\\t");
                //        break;
                //    case TokenType.NewLineSym:
                //        yield return new Token(bestMatch.TokenType, "\\n");
                //        break;
                //    default:
                //        yield return new Token(bestMatch.TokenType, bestMatch.Value);
                //        break;
                //}

                // Track line and column numbers
                int tokenLine = line;
                int tokenColumn = column;

                foreach (char c in bestMatch.Value)
                {
                    if (c == '\n')
                    {
                        tokenLine++;
                        tokenColumn = 1;
                    }
                    else
                    {
                        tokenColumn++;
                    }
                }

                // Update line and column information for the token
                bestMatch.StartLine = tokenLine;
                bestMatch.StartColumn = tokenColumn;

                // Yield the token
                yield return new Token(bestMatch.TokenType, bestMatch.Value, bestMatch.StartLine, bestMatch.StartColumn);

                // Update line and column for the next token
                line = tokenLine;
                column = tokenColumn;

                lastMatch = bestMatch;
            }

            yield return new Token(TokenType.SequenceTerminator);
        }

        private List<TokenMatch> FindTokenMatches(string lqlText)
        {
            var tokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in _tokenDefinitions)
                tokenMatches.AddRange(tokenDefinition.FindMatches(lqlText).ToList());

            return tokenMatches;
        }

        public override int yylex()
        {
            throw new System.NotImplementedException();
        }
    }
}