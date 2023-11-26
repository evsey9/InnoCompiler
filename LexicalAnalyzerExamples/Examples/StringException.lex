/*
   This file exercises various internal exceptions in gplex.
 */

%namespace LexScanner
%option noparser nofiles

evilstring blah$((((((
string "abc\07xxx"
impossible foo/bar$
badstring ^blah$
%%
x{badstring}  |
{badstring}x  |
{impossible}  |
{string}      Console.WriteLine("keyword " + yytext);

%%

    public static void Main(string[] argp) { 
        Scanner scnr = new Scanner();
        for (int i = 0; i < argp.Length; i++) {
            Console.WriteLine("Scanning \"" + argp[i] + "\"");
            scnr.SetSource(argp[i], 0);
            scnr.yylex();
        }
    }

