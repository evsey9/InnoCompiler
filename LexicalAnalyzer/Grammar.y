%namespace LexicalAnalyzer
%output=RealTreeParser.cs 
//%partial 
//%sharetokens

//%visibility internal

%YYSTYPE AstNode

%token <Token> OpenCurlBr CloseCurlBr OpenRoundBr CloseRoundBr OpenSquareBr CloseSquareBr
%token <Token> VarKey AssignOp TrueKey FalseKey AndOp OrOp XorOp
%token <Token> LessRel MoreRel LessEqRel MoreEqRel EqualRel NotEqRel Plus Minus Multiplication Division
%token <Token> PrintKey ReturnKey IfKey ThenKey ElseKey EndKey WhileKey ForKey InKey LoopKey FuncKey
%token <Token> IntKey RealKey BoolKey StringKey EmptyKey IsKey ArrowKey CommaSym SemicolonSym
%token <Token> ColonSym SequenceTerminator OneLineComment SpaceSym NewLineSym TabSym UndefinedSymbol
%token <Token> ReadInt ReadReal ReadString Dot DotDot Not

%token  VarName IntVar RealVar StringVar

%start Program

%%

Program: Declaration { $$ = new ProgramNode((DeclarationNode)$1); } /* Добавить приведения Типов */
       | Program Declaration { $$ = ((ProgramNode)$1).AddDeclaration((DeclarationNode)$2); }
       | /* empty */ { $$ = new ProgramNode(); }
       ;

Declaration: VarKey VarName { $$ = new DeclarationNode((string)$2, null); }
          | VarKey VarName AssignOp Expression SemicolonSym { $$ = new DeclarationNode((string)$2, (ExpressionNode)$4); }
          ;

Expression: Relation OrOp Relation { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.Or, (ExpressionNode)$3); }
          | Relation AndOp Relation { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.And, (ExpressionNode)$3); }
          | Relation XorOp Relation { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.Xor, (ExpressionNode)$3); }
          |  Relation { $$ = $1; }
          ;

Relation: Factor { $$ = $1; }
        | Factor LessRel Factor { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.Less, (ExpressionNode)$3); }
        | Factor MoreRel Factor { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.More, (ExpressionNode)$3); }
        | Factor LessEqRel Factor { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.LessOrEqual, (ExpressionNode)$3); }
        | Factor MoreEqRel Factor { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.MoreOrEqual, (ExpressionNode)$3); }
        | Factor EqualRel Factor { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.Equal, (ExpressionNode)$3); }
        | Factor NotEqRel Factor { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.NotEqual, (ExpressionNode)$3); }
        ;

Factor: Term { $$ = $1; }
      | Term Plus Term { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.Add, (ExpressionNode)$3); }
      | Term Minus Term { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.Subtract, (ExpressionNode)$3); }
      ;

Term: Unary { $$ = $1; }
    | Unary Multiplication Unary { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.Multiply, (ExpressionNode)$3); }
    | Unary Division Unary { $$ = new BinaryExpressionNode((ExpressionNode)$1, BinaryOperator.Divide, (ExpressionNode)$3); }
    ;

Unary: Primary { $$ = $1; }
     | Plus Primary { $$ = new UnaryExpressionNode(UnaryOperator.Plus, (ExpressionNode)$2); }
     | Minus Primary { $$ = new UnaryExpressionNode(UnaryOperator.Minus, (ExpressionNode)$2); }
     | Not Primary { $$ = new UnaryExpressionNode(UnaryOperator.Not, (ExpressionNode)$2); }
     | Primary IsKey TypeIndicator { $$ = new TypeConversionNode($1, $3); }
     | Literal { $$ = $1; }
     ;

Primary: VarName { $$ = new VariableNode($1); }
       | VarName Tail { $$ = new AccessNode((ExpressionNode)$1, (AccessTailNode)$2); }
       | ReadInt { $$ = new ReadIntNode(); }
       | ReadReal { $$ = new ReadRealNode(); }
       | ReadString { $$ = new ReadStringNode(); }
       | OpenRoundBr Expression CloseRoundBr { $$ = $2; }
       ;

Tail: Dot IntVar { $$ = new AccessNode($2); }
    | Dot VarName { $$ = new AccessNode($2); }
    | OpenSquareBr Expression CloseSquareBr { $$ = new AccessNode($2); }
    | OpenRoundBr ExpressionList CloseRoundBr { $$ = new FunctionCallNode($2); }
    ;

ExpressionList: Expression { $$ = new List<ExpressionNode> { $1 }; } /*Поменять в соответствии с созданием листа в C#*/
             | ExpressionList CommaSym Expression { $1.Add($3); $$ = $1; }
             ;

Statement: Assignment { $$ = $1; }
         | Print { $$ = $1; }
         | Return { $$ = $1; }
         | If { $$ = $1; }
         | Loop { $$ = $1; }
         ;

Assignment: VarName AssignOp Expression SemicolonSym { $$ = new AssignmentNode($1, $3); }
          ;

Print: PrintKey ExpressionList { $$ = new PrintNode($2); }
     ;

Return: ReturnKey Expression SemicolonSym { $$ = new ReturnNode($2); }
      | ReturnKey SemicolonSym { $$ = new ReturnNode(null); }
      ;

If: IfKey Expression ThenKey Body EndKey { $$ = new IfNode($2, $4, null); }
   | IfKey Expression ThenKey Body ElseKey Body EndKey { $$ = new IfNode($2, $4, $6); }
   ;

Loop: WhileKey Expression LoopBody { $$ = new LoopNode($2, $3); }
    | ForKey VarName InKey TypeIndicator LoopBody { $$ = new ForLoopNode($2, $4, $5); }
    ;

LoopBody: LoopKey Body EndKey { $$ = $2; }
        ;

TypeIndicator: IntKey { $$ = TypeIndicator.Int; }
             | RealKey { $$ = TypeIndicator.Real; }
             | BoolKey { $$ = TypeIndicator.Bool; }
             | StringKey { $$ = TypeIndicator.String; }
             | EmptyKey { $$ = TypeIndicator.Empty; }
             | OpenSquareBr CloseSquareBr { $$ = TypeIndicator.Vector; }
             | OpenCurlBr CloseCurlBr { $$ = TypeIndicator.Tuple; }
             | FuncKey { $$ = TypeIndicator.Function; }
             | Expression DotDot Expression { $$ = TypeIndicator.Range($1, $3); }
             ;

Literal: IntVar { $$ = new LiteralNode(int.Parse($1)); }
        | RealVar { $$ = new LiteralNode(double.Parse($1)); }
        | TrueKey { $$ = new LiteralNode(true); }
        | FalseKey { $$ = new LiteralNode(false); }
        | StringVar { $$ = new LiteralNode($1); }
        | ArrayLiteral { $$ = $1; }
        | TupleLiteral { $$ = $1; }
        | FunctionLiteral { $$ = $1; }
        ;

ArrayLiteral: OpenSquareBr ExpressionList CloseSquareBr { $$ = new ArrayLiteralNode($2); }
            ;

TupleLiteral: OpenCurlBr TupleContent CloseCurlBr { $$ = new TupleLiteralNode($2); }
            ;

TupleContent: OpenSquareBr TupleElementList CloseSquareBr { $$ = $2; }
            | /* empty */ { $$ = new List<TupleElementNode>(); }
            ;

TupleElementList: TupleElement { $$ = new List<TupleElementNode> { $1 }; }
               | TupleElementList CommaSym TupleElement { $1.Add($3); $$ = $1; }
               ;

TupleElement: OpenSquareBr VarName AssignOp Expression CloseSquareBr { $$ = new TupleElementNode($2, $4); }
            ;

FunctionLiteral: FuncKey Parameters FunBody { $$ = new FunctionLiteralNode($2, $3); }
              ;

Parameters: OpenRoundBr VarNameList CloseRoundBr { $$ = $2; }
          | /* empty */ { $$ = new List<string>(); }
          ;

VarNameList: VarName { $$ = new List<string> { $1 }; }
          | VarNameList CommaSym VarName { $1.Add($3); $$ = $1; }
          ;

FunBody: IsKey Body EndKey { $$ = $2; }
       | ArrowKey Expression { $$ = new FunctionBodyNode($2); }
       ;

Body: OpenCurlBr DeclarationList CloseCurlBr { $$ = $2; }
    | OpenCurlBr StatementList CloseCurlBr { $$ = $2; }
    | OpenCurlBr ExpressionList CloseCurlBr { $$ = $2; }
    ;

DeclarationList: Declaration { $$ = new List<DeclarationNode> { $1 }; }
              | DeclarationList Declaration { $1.Add($2); $$ = $1; }
              ;

StatementList: Statement { $$ = new List<StatementNode> { $1 }; }
            | StatementList Statement { $1.Add($2); $$ = $1; }
            ;


%%