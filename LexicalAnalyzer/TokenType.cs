namespace LexicalAnalyzer
{
    public enum TokenType
    {
        // Brackets
        OpenCurlBr,
        CloseCurlBr,
        OpenRoundBr,
        CloseRoundBr,
        OpenSquareBr,
        CloseSquareBr,
        
        // Declaration
        VarKey,
        AssignOp,
        VarName,
        
        // Values
        StringVar,
        IntVar,
        RealVar,
        TrueKey,
        FalseKey,
        
        // Expression
        AndOp,
        OrOp,
        XorOp,
        
        //Relation
        LessRel,
        MoreRel,
        LessEqRel,
        MoreEqRel,
        EqualRel,
        NotEqRel,
        
        //Factor
        Plus,
        Minus,
        Multiplication,
        Division,
        
        // Key words
        PrintKey,
        ReturnKey,
        IfKey,
        ThenKey,
        ElseKey,
        EndKey,
        WhileKey,
        ForKey,
        InKey,
        LoopKey,
        FuncKey,
        
        // Type
        IntKey,
        RealKey,
        BoolKey,
        StringKey,
        EmptyKey,
        
        // FunBody
        IsKey,
        ArrowKey,
        
        //Other
        CommaSym,
        SemicolonSym,
        ColonSym,
        SequenceTerminator,
        OneLineComment,
        
        // Spaces
        SpaceSym,
        NewLineSym,
        TabSym,
        
        UndefinedSymbol
    } 
}