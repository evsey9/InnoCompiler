namespace QUT.Gppg
{
    
}

namespace LexicalAnalyzer
{
    public partial class Parser : ShiftReduceParser<AstNode, LexLocation>
    {
        public Parser(AbstractScanner<AstNode, LexLocation> scanner) : base(scanner)
        {
            
        }
    }
}