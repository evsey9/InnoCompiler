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

Program: Statement SemicolonSym { $$ = new ProgramNode((StatementNode)$1); }
       | Program Statement SemicolonSym { $$ = ((ProgramNode)$1).AddStatement((StatementNode)$2); }
       | /* empty */ { $$ = new ProgramNode(); }
       ;

Statement: Declaration { $$ = $1; } 
       |Assignment { $$ = $1; }
       | Print { $$ = $1; }
       | Return { $$ = $1; }
       | If { $$ = $1; }
       | Loop { $$ = $1; }
       ;

Declaration: VarKey VarName { $$ = new DeclarationNode((StringNode)$2, null); }
          | VarKey VarName AssignOp Expression SemicolonSym { $$ = new DeclarationNode((StringNode)$2, (ExpressionNode)$4); }
          ;

VariableDefinitionList: VariableDefinition { $$ = new List<VariableDefinitionNode> { $1 }; }
                    | VariableDefinitionList CommaSym VariableDefinition { $1.Add($3); $$ = $1; }
                    ;

VariableDefinition: VarName { $$ = new VariableDefinitionNode($1, null); }
                | VarName AssignOp Expression { $$ = new VariableDefinitionNode($1, $3); }
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
     | Primary IsKey TypeIndicator { $$ = new TypeConversionNode((ExpressionNode)$1, (TypeIndicator)$3); }
     | Literal { $$ = $1; }
     ;

Primary: VarName { $$ = new VariableNode((StringNode)$1); }
       | VarName Tail { $$ = new AccessNode((ExpressionNode)$1, (AccessTailNode)$2); }
       | ReadInt { $$ = new ReadIntNode(); }
       | ReadReal { $$ = new ReadRealNode(); }
       | ReadString { $$ = new ReadStringNode(); }
       | OpenRoundBr Expression CloseRoundBr { $$ = $2; }
       ;

Tail: Dot IntVar { $$ = new AccessNode((ExpressionNode)$2); }
    | Dot VarName { $$ = new AccessNode((ExpressionNode)$2); }
    | OpenSquareBr Expression CloseSquareBr { $$ = new AccessNode((ExpressionNode)$2); }
    | OpenRoundBr ExpressionList CloseRoundBr { $$ = new FunctionCallNode((ExpressionNode)$2); }
    ;

ExpressionList: Expression { $$ = new ExpressionNodeListNode((ExpressionNode)$1); } /*Поменять в соответствии с созданием листа в C#*/
             | ExpressionList CommaSym Expression { ((ExpressionNodeListNode)$1).Add((ExpressionNode)$3); $$ = $1; }
             ;

Assignment: VarName AssignOp Expression SemicolonSym { $$ = new AssignmentNode((StringNode)$1, (ExpressionNode)$3); }
          ;

Print: PrintKey ExpressionList { $$ = new PrintNode((ExpressionNodeListNode)$2); }
     ;

Return: ReturnKey Expression SemicolonSym { $$ = new ReturnNode((ExpressionNode)$2); }
      | ReturnKey SemicolonSym { $$ = new ReturnNode(null); }
      ;

If: IfKey Expression ThenKey Body EndKey { $$ = new IfNode((ExpressionNode)$2, (StatementNodeListNode)$4, null); }
   | IfKey Expression ThenKey Body ElseKey Body EndKey { $$ = new IfNode((ExpressionNode)$2, (StatementNodeListNode)$4, $6); }
   ;

Loop: WhileKey Expression LoopBody { $$ = new LoopNode((ExpressionNode)$2, (StatementNodeListNode)$3); }
    | ForKey VarName InKey TypeIndicator LoopBody { $$ = new ForLoopNode((TypeIndicator)$2, (ExpressionNode)$4, (StatementNodeListNode)$5); }
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

Literal: IntVar { $$ = new LiteralNode(int.Parse(((StringNode)$1).GetString())); }
        | RealVar { $$ = new LiteralNode(double.Parse(((StringNode)$1).GetString())); }
        | TrueKey { $$ = new LiteralNode(true); }
        | FalseKey { $$ = new LiteralNode(false); }
        | StringVar { $$ = new LiteralNode(((StringNode)$1).GetString()); }
        | ArrayLiteral { $$ = (ArrayLiteralNode)$1; }
        | TupleLiteral { $$ = (TupleLiteralNode)$1; }
        | FunctionLiteral { $$ = (FunctionLiteralNode)$1; }
        ;

ArrayLiteral: OpenSquareBr ExpressionList CloseSquareBr { $$ = new ArrayLiteralNode((ExpressionNodeListNode)$2); }
            ;

TupleLiteral: OpenCurlBr TupleContent CloseCurlBr { $$ = new TupleLiteralNode((TupleElementNodeListNode)$2); }
            ;

TupleContent: OpenSquareBr TupleElementList CloseSquareBr { $$ = $2; }
            | /* empty */ { $$ = new TupleElementNodeListNode(); }
            ;

TupleElementList: TupleElement { $$ = new TupleElementNodeListNode((TupleElementNode)$1); }
               | TupleElementList CommaSym TupleElement { ((TupleElementNodeListNode)$1).Add((TupleElementNode)$3); $$ = $1; }
               ;

TupleElement: OpenSquareBr VarName AssignOp Expression CloseSquareBr { $$ = new TupleElementNode((StringNode)$2, (ExpressionNode)$4); }
            ;

FunctionLiteral: FuncKey Parameters FunBody { $$ = new FunctionLiteralNode((StringNodeListNode)$2, (StatementNodeListNode)$3); }
              ;

Parameters: OpenRoundBr VarNameList CloseRoundBr { $$ = $2; }
          | /* empty */ { $$ = new StringNodeListNode(); }
          ;

VarNameList: VarName { $$ = new StringNodeListNode((StringNode)$1); }
          | VarNameList CommaSym VarName { ((StringNodeListNode)$1).Add((StringNode)$3); $$ = $1; }
          ;

FunBody: IsKey Body EndKey { $$ = $2; }
       | ArrowKey Expression { $$ = new FunctionBodyNode((ExpressionNode)$2); }
       ;

Body: OpenCurlBr DeclarationList CloseCurlBr { $$ = $2; }
    | OpenCurlBr StatementList CloseCurlBr { $$ = $2; }
    | OpenCurlBr ExpressionList CloseCurlBr { $$ = $2; }
    ;

DeclarationList: Declaration { $$ = new DeclarationNodeListNode((DeclarationNode)$1); }
              | DeclarationList Declaration { ((DeclarationNodeListNode)$1).Add((DeclarationNode)$2); $$ = $1; }
              ;

StatementList: Statement { $$ = new StatementNodeListNode((StatementNode)$1); }
            | StatementList Statement { ((StatementNodeListNode)$1).Add((StatementNode)$2); $$ = $1; }
            ;


%%