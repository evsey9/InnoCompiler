using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

            //            string query = @"
//      func main()
//            {
//                var str := 'Hello world';
//            real a := 9..3;
//            int may := 12 * 15 + a - 17;

//        // Array Declaration and Assignment
//        array:= [1, 8, 12.5, ''];

//            // If Statement
//            if (12 > 16) and(num <= a or hello) then
//        // While Loop
//        while (13 > 16) loop
//            num = num + 1;
//            end
//        end
//       }";

//            Console.WriteLine("");
//            Console.WriteLine("The query:");
//            Console.WriteLine(query);
//            Console.WriteLine("");
//            Console.WriteLine("Tokens generated:");

            //var tokenSequence = tokenizer.Tokenize(query).ToList();
          //  foreach (var token in tokenSequence)
          //  {
	         //   //Condition in order to hide Space, Tab and New line symbols
	         //   //if (token.TokenType == Tokens.SpaceSym || token.TokenType == Tokens.TabSym ||
	         //   //    token.TokenType == Tokens.NewLineSym)
		        ////    continue;
	         //   Console.WriteLine(string.Format("TokenType: {0}, Value: {1}, Line: {2}, Column: {3}", token.TokenType, token.Value, token.Line, token.Column));
          //  }
                

          //  Console.WriteLine("");
          //  Console.WriteLine("Process complete");
          //  Console.WriteLine("Starting to parse!");
            //ITokenizer Tokenizer = new PrecedenceBasedRegexTokenizer();
            
            
            
            string query = @"

";
            
            PrecedenceBasedRegexTokenizer newTokenizer = new PrecedenceBasedRegexTokenizer();
            var tokenSequence = tokenizer.Tokenize(query).ToList();
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

// Code snippets:

//var testStr := 'hello everynyan!';
//print testStr;
//var testIntOne := 1;
//var testIntTwo := 3;
//var result := testIntOne + testIntTwo;

//var myVar := 3.1; //������� �� ��������, ��� ������� �� ��������� � grammar.y � ������ System.FormatException: "Input string was not in a correct format."

//var t := [1]; //array
//var newArray := myArray + [4, 5, 6];
//t[0] := [2]; //�� ����� � ����� ������ ��� � ���� ��� ���� ��� ����� �� ����� � ��������� ������������ (��� �� ���� � tail, �� �� ����� ��� ������)

//t:= { [[a:= 1],[b:= 2]]}; //tuple
//var x := t.b; //�������� ����������� �� �������� ������ ��� � ���� ��� ����������� ���� ���-��.���-�� (��� �� ���� � tail, �� �� ����� ��� ������)

//if myVar > 2 then
//    { myVar:= 2}
//else
//{ myVar:= 4}
//end //�� ������ �� ��������, �� �������� ������� �� ����, ���� ������� ; ����� �����������, �� �� �������� ������ ���

//while m > 2 loop
//    {m := m - 1}
//end // �� �� ���� ��� � � ����

//for myvar in Int loop
//    { m:= m + 1}
//end // ��� ��� Real ������-�� ������ ���� TypeIndicator � ���������� ����� �� �� ������ ��� � � �������� ������� ������ �������� System.InvalidCastException: "Unable to cast object of type 'LexicalAnalyzer.StringNode' to type 'TypeIndicator'."

//func (x) is
//{ m:= m + 1}
//end // ���������� �������� ��������, �� �� �� ������������ ��� ��� ����� func � �������

//func (x) => x := x + 1; /�� �� ��� � ����

//����� ��������� ����� ��������� ����������� �� ��������