using System;
using System.Diagnostics;
using System.Linq;
using LexicalAnalyzer;

namespace LexicalAnalyzer
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            new Program().Run();
        }

        public void Run()
        {
            
            ITokenizer Tokenizer = new PrecedenceBasedRegexTokenizer();
            RunAndPrint(Tokenizer, "Run tokenizer");
            
        }
        
        public void RunAndPrint(ITokenizer tokenizer, string startMessage)
        {
            Console.WriteLine(startMessage);
            Console.WriteLine("");

            string query = @"
func main {
	var str := 'Hello world'
	real a := 9..3
	int may := 12 * 15 + a - 17;

	array := [1, 8, 12.5, '']

	if (12 > 16) and (num <= a or hello) 
	then 
	for i in array
	array[i] := i + 6
	end

	loop print str end

	is end => hi

| | ^ % .

	if true
	then false /=, >=, <=
and 10-6

}";

            Console.WriteLine("");
            Console.WriteLine("The query:");
            Console.WriteLine(query);
            Console.WriteLine("");
            Console.WriteLine("Tokens generated:");

            var tokenSequence = tokenizer.Tokenize(query).ToList();
            foreach (var token in tokenSequence)
            {
	            //Condition in order to hide Space, Tab and New line symbols
	            //if (token.TokenType == Tokens.SpaceSym || token.TokenType == Tokens.TabSym ||
	            //    token.TokenType == Tokens.NewLineSym)
		        //    continue;
	            Console.WriteLine(string.Format("TokenType: {0}, Value: {1}, Line: {2}, Column: {3}", token.TokenType, token.Value, token.Line, token.Column));
            }
                

            Console.WriteLine("");
            Console.WriteLine("Process complete");
            Console.WriteLine("Starting to parse!");
            //ITokenizer Tokenizer = new PrecedenceBasedRegexTokenizer();
            
            
            
            query = @"
var testStr = 'hello everynyan!';
var testIntOne = 1;
var testIntTwo = 3;
var result = testIntOne + testIntTwo;
";
            
            PrecedenceBasedRegexTokenizer newTokenizer = new PrecedenceBasedRegexTokenizer();
            tokenSequence = tokenizer.Tokenize(query).ToList();
            foreach (var token in tokenSequence)
            {
	            //Condition in order to hide Space, Tab and New line symbols
	            //if (token.TokenType == Tokens.SpaceSym || token.TokenType == Tokens.TabSym ||
	            //    token.TokenType == Tokens.NewLineSym)
	            //    continue;
	            Console.WriteLine(string.Format("TokenType: {0}, Value: {1}, Line: {2}, Column: {3}", token.TokenType, token.Value, token.Line, token.Column));
            }
            newTokenizer.PrepareTokens(query);
            var parser = new Parser(newTokenizer);
            bool parseResult = parser.Parse();

            Console.WriteLine("");
            Console.WriteLine($"Process complete. Parse successful: {parseResult}");

            //Console.ReadLine();
        }
    }
}