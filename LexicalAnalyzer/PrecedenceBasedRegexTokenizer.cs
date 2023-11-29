using System;
using System.Collections.Generic;
using System.Linq;
using LexicalAnalyzer;

namespace LexicalAnalyzer
{
    internal class PrecedenceBasedRegexTokenizer : AbstractScanner<AstNode, LexLocation>, ITokenizer
    {
        private List<TokenDefinition> _tokenDefinitions;

        private List<Token> _tokenSequence = new List<Token>();
        
        private int _tokenIndex = 0;
        
        public PrecedenceBasedRegexTokenizer()
        {
            _tokenDefinitions = new List<TokenDefinition>();

            // Brackets
             _tokenDefinitions.Add(new TokenDefinition(Tokens.OpenCurlBr, "\\{", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.CloseCurlBr, "\\}", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.OpenRoundBr, "\\(", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.CloseRoundBr, "\\)", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.OpenSquareBr, "\\[", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.CloseSquareBr, "\\]", 2));

            // Values
             _tokenDefinitions.Add(new TokenDefinition(Tokens.StringVar, "'.*'|\".*\"", 1));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.IntVar, "[0-9]+", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.RealVar, "[0-9]+\\.[0-9]+", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.TrueKey, "true", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.FalseKey, "false", 3));

            // Expression
             _tokenDefinitions.Add(new TokenDefinition(Tokens.AndOp, "and\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.OrOp, "or\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.XorOp, "xor\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.Not, "not\\s", 3));

            //Relation
             _tokenDefinitions.Add(new TokenDefinition(Tokens.LessRel, "<", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.MoreRel, ">", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.LessEqRel, "<=", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.MoreEqRel, ">=", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.EqualRel, "=", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.NotEqRel, "/=", 2));

            //Factor
             _tokenDefinitions.Add(new TokenDefinition(Tokens.Plus, "\\+", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.Minus, "\\-", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.Multiplication, "\\*", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.Division, "\\/", 3));

             // Key words
             _tokenDefinitions.Add(new TokenDefinition(Tokens.PrintKey, "print\\s|print\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.ReturnKey, "return\\s|return\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.IfKey, "if\\s|if\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.ThenKey, "then\\s|then\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.ElseKey, "else\\s|else\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.EndKey, "end\\s|end\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.WhileKey, "while\\s|while\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.ForKey, "for\\s|for\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.InKey, "in\\s|in\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.LoopKey, "loop\\s|loop\\n", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.FuncKey, "func\\s|func\\n", 3));

            // Type
             _tokenDefinitions.Add(new TokenDefinition(Tokens.IntKey, "int\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.RealKey, "real\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.BoolKey, "bool\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.StringKey, "string\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.EmptyKey, "empty\\s", 3));

            // FunBody
             _tokenDefinitions.Add(new TokenDefinition(Tokens.IsKey, "is\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.ArrowKey, "=>", 2));
             
             
             // Declaration
             _tokenDefinitions.Add(new TokenDefinition(Tokens.VarKey, "var\\s", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.AssignOp, ":=", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.VarName, "[A-Za-z_]+[0-9A-Za-z_]*", 3));

            //Other
             _tokenDefinitions.Add(new TokenDefinition(Tokens.CommaSym, ",", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.SemicolonSym, ";", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.ColonSym, ":", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.OneLineComment, "//", 1));
             
             _tokenDefinitions.Add(new TokenDefinition(Tokens.UndefinedSymbol, ".", 5));
             
             // Spaces
             _tokenDefinitions.Add(new TokenDefinition(Tokens.SpaceSym, "\\s", 4));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.NewLineSym, "\\n", 2));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.TabSym, "\\t", 3));
             
             _tokenDefinitions.Add(new TokenDefinition(Tokens.Dot, "\\.", 3));
             _tokenDefinitions.Add(new TokenDefinition(Tokens.DotDot, "\\.\\.", 2));

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
                //    case Tokens.SpaceSym:
                //        yield return new Token(bestMatch.TokenType, "\\s");
                //        break;
                //    case Tokens.TabSym:
                //        yield return new Token(bestMatch.TokenType, "\\t");
                //        break;
                //    case Tokens.NewLineSym:
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
                LexLocation tokenLocation =
                    new LexLocation(tokenLine, tokenColumn, tokenLine, tokenColumn + bestMatch.Length);

                // Yield the token
                yield return new Token(bestMatch.TokenType, bestMatch.Value, bestMatch.StartLine, bestMatch.StartColumn, tokenLocation);

                // Update line and column for the next token
                line = tokenLine;
                column = tokenColumn;

                lastMatch = bestMatch;
            }

            yield return new Token(Tokens.SequenceTerminator);
        }

        public void PrepareTokens(string query)
        {
            var tokenSequence = Tokenize(query).ToList();
            foreach (var token in tokenSequence)
            {
                //Condition in order to hide Space, Tab and New line symbols
                if (token.TokenType == Tokens.SpaceSym || token.TokenType == Tokens.TabSym ||
                    token.TokenType == Tokens.NewLineSym)
                    continue;
                _tokenSequence.Add(token);
            }
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
            
            Token curToken = _tokenSequence[_tokenIndex];
            // CODE HERE
            switch (curToken.TokenType)
            {
                /*case Tokens.error:
                    break;
                case Tokens.EOF:
                    break;
                case Tokens.OpenCurlBr:
                    break;
                case Tokens.CloseCurlBr:
                    break;
                case Tokens.OpenRoundBr:
                    break;
                case Tokens.CloseRoundBr:
                    break;
                case Tokens.OpenSquareBr:
                    break;
                case Tokens.CloseSquareBr:
                    break;
                case Tokens.VarKey:
                    break;
                case Tokens.AssignOp:
                    break;
                case Tokens.TrueKey:
                    break;
                case Tokens.FalseKey:
                    break;
                case Tokens.AndOp:
                    break;
                case Tokens.OrOp:
                    break;
                case Tokens.XorOp:
                    break;
                case Tokens.LessRel:
                    break;
                case Tokens.MoreRel:
                    break;
                case Tokens.LessEqRel:
                    break;
                case Tokens.MoreEqRel:
                    break;
                case Tokens.EqualRel:
                    break;
                case Tokens.NotEqRel:
                    break;
                case Tokens.Plus:
                    break;
                case Tokens.Minus:
                    break;
                case Tokens.Multiplication:
                    break;
                case Tokens.Division:
                    break;
                case Tokens.PrintKey:
                    break;
                case Tokens.ReturnKey:
                    break;
                case Tokens.IfKey:
                    break;
                case Tokens.ThenKey:
                    break;
                case Tokens.ElseKey:
                    break;
                case Tokens.EndKey:
                    break;
                case Tokens.WhileKey:
                    break;
                case Tokens.ForKey:
                    break;
                case Tokens.InKey:
                    break;
                case Tokens.LoopKey:
                    break;
                case Tokens.FuncKey:
                    break;
                case Tokens.IntKey:
                    break;
                case Tokens.RealKey:
                    break;
                case Tokens.BoolKey:
                    break;
                case Tokens.StringKey:
                    break;
                case Tokens.EmptyKey:
                    break;
                case Tokens.IsKey:
                    break;
                case Tokens.ArrowKey:
                    break;
                case Tokens.CommaSym:
                    break;
                case Tokens.SemicolonSym:
                    break;
                case Tokens.ColonSym:
                    break;*/
                case Tokens.SequenceTerminator:
                    return (int)Tokens.EOF;
                    break;
                /*case Tokens.OneLineComment:
                    break;
                case Tokens.SpaceSym:
                    break;
                case Tokens.NewLineSym:
                    break;
                case Tokens.TabSym:
                    break;
                case Tokens.UndefinedSymbol:
                    break;
                case Tokens.ReadInt:
                    break;
                case Tokens.ReadReal:
                    break;
                case Tokens.ReadString:
                    break;
                case Tokens.Dot:
                    break;
                case Tokens.DotDot:
                    break;
                case Tokens.Not:
                    break;*/
                case Tokens.VarName:
                    yylval = new StringNode(curToken.Value);
                    break;
                case Tokens.IntVar:
                    yylval = new StringNode(curToken.Value);
                    break;
                case Tokens.RealVar:
                    yylval = new StringNode(curToken.Value);
                    break;
                case Tokens.StringVar:
                    yylval = new StringNode(curToken.Value.Substring(1, curToken.Value.Length - 2));
                    break;
            }
            // CODE END
            _tokenIndex += 1;
            return (int)curToken.TokenType;
        }
        
        public override LexLocation yylloc {
            get
            {
                return _tokenSequence[_tokenIndex].Location;
            }       
            set { /* skip */ }                   // yylloc to be ignored entirely.
        }
        
         
    }
}