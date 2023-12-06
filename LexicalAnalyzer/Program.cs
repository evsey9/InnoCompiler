using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using LexicalAnalyzer;

namespace LexicalAnalyzer
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            
            ITokenizer Tokenizer = new PrecedenceBasedRegexTokenizer();
            string filename = "input.txt";
            if (args.Length > 0)
            {
                filename = args[0];
            }
            string query = File.ReadAllText(filename);
            RunAndPrint(Tokenizer, "Run tokenizer", query);
            
        }
        
        public void RunAndPrint(ITokenizer tokenizer, string startMessage, string query)
        {
            Console.WriteLine(startMessage);
            Console.WriteLine("");
            PrecedenceBasedRegexTokenizer newTokenizer = new PrecedenceBasedRegexTokenizer();
            var tokenSequence = tokenizer.Tokenize(query).ToList();
            foreach (var token in tokenSequence)
            {
	            //Condition in order to hide Space, Tab and New line symbols
	            if (token.TokenType == Tokens.SpaceSym || token.TokenType == Tokens.TabSym ||
	                token.TokenType == Tokens.NewLineSym)
                    continue;
	            Console.WriteLine(string.Format("TokenType: {0}, Value: {1}, Line: {2}, Column: {3}", token.TokenType, token.Value, token.Line, token.Column));
            }
            newTokenizer.PrepareTokens(query);
            var parser = new Parser(newTokenizer);
            bool parseResult = parser.Parse();
            Console.WriteLine("");
            Console.WriteLine($"Process complete. Parse successful: {parseResult}");
            Console.WriteLine($"Starting interpretation.");
            
            var firstNode = parser.CurrentSemanticValue;
            var callStack = new CallStack();
            var interpretResult = firstNode.InterpretNode(ref callStack);
            Console.WriteLine();
        }
    }
}
