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
	            if (token.TokenType == TokenType.SpaceSym || token.TokenType == TokenType.TabSym ||
	                token.TokenType == TokenType.NewLineSym)
		            continue;
	            Console.WriteLine(string.Format("TokenType: {0}, Value: {1}, Line: {2}, Column: {3}", token.TokenType, token.Value, token.Line, token.Column));
            }
                

            Console.WriteLine("");
            Console.WriteLine("Process complete");

            var parser = new ShiftReduceParser(AbstractScanner<>);

            Console.ReadLine();
        }
    }
}