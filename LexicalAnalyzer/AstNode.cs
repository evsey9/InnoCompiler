using LexicalAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
//just some comment so github can work with it
namespace LexicalAnalyzer
{
    public abstract class AstNode
    {
        public abstract AstNode InterpretNode(ref CallStack stack);
    }

    public class ErrorNode : AstNode
    {
        public string ErrorString;

        public ErrorNode(string errorString)
        {
            ErrorString = errorString;
        }

        public ErrorNode()
        {
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            return this;
        }
    }

    // Program node representing the entire program
    public class ProgramNode : AstNode
    {
        public List<StatementNode> Statements { get; } = new List<StatementNode>();

        public ProgramNode()
        {
        }

        public ProgramNode AddStatement(StatementNode statement)
        {
            Statements.Add(statement);
            return this;
        }
        public ProgramNode(StatementNode statement)
        {
            AddStatement(statement);
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            for (int i = 0; i < Statements.Count; i++)
            {
                AstNode resultNode = Statements[i].InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }
                Statements[i] = (StatementNode)resultNode;
            }

            return this;
        }
    }

    // Statement node representing a statement in the program
    public abstract class StatementNode : AstNode { }

    // Declaration node representing variable declarations
    public class DeclarationNode : StatementNode
    {
        public string VariableName { get; }

        public ExpressionNode AssignedExpression;

        public DeclarationNode(StringNode variableName, ExpressionNode assignedExpression)
        {
            VariableName = variableName.GetString();
            AssignedExpression = assignedExpression;
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            if (AssignedExpression != null)
            {
                var resultNode = AssignedExpression.InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }

                AssignedExpression = (ExpressionNode)resultNode;
            }

            if (stack.DeclareVariable(VariableName, AssignedExpression))
            {
                return this;
            }
            else
            {
                return new ErrorNode("Variable already declared!");
            }
        }
    }
    
    public class ArrayAssignmentNode : StatementNode
    {
        public AccessNode VariableReference;

        public ExpressionNode AssignedExpression;

        public ArrayAssignmentNode(AccessNode variableReference, ExpressionNode assignedExpression)
        {
            VariableReference = variableReference;
            AssignedExpression = assignedExpression;
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            if (VariableReference != null)
            {
                var resultNode = VariableReference.InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }

                VariableReference = (AccessNode)resultNode;
            }

            if (AssignedExpression != null)
            {
                var resultNode = AssignedExpression.InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }

                AssignedExpression = (ExpressionNode)resultNode;
            }
            
            var varName = ((VariableNode)VariableReference.Target).VariableName;
            if (stack.GetVariable(varName) is ArrayLiteralNode targetArray)
            {
                if (VariableReference.Tail is BracketAccessNode bracketAccessNode)
                {
                    targetArray.Set(((IntegerLiteralNode)bracketAccessNode.Index).Value, AssignedExpression);
                }
                else
                {
                    return new ErrorNode("Tail assignment of non-array access!");
                }
            }
            else
            {
                return new ErrorNode("Variable with square brackets access is not an array!");
            }

            return this;
        }
    }

    // Expression node representing expressions
    public abstract class ExpressionNode : AstNode { }

    // Binary expression node representing binary operations
    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Left;
        public BinaryOperator Operator { get; }
        public ExpressionNode Right;

        public BinaryExpressionNode(ExpressionNode left, BinaryOperator op, ExpressionNode right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            var leftResultNode = Left.InterpretNode(ref stack);
            if (leftResultNode is ErrorNode leftNode)
            {
                return leftNode;
            }

            Left = (ExpressionNode)leftResultNode;
            
            var rightResultNode = Right.InterpretNode(ref stack);
            if (rightResultNode is ErrorNode rightNode)
            {
                return rightNode;
            }

            Right = (ExpressionNode)rightResultNode;

            bool leftIsBool = false;
            bool leftIsInt = false;
            bool leftIsReal = false;
            bool leftIsString = false;
            bool leftIsArray = false;
            bool leftIsTuple = false;

            bool rightIsBool = false;
            bool rightIsInt = false;
            bool rightIsReal = false;
            bool rightIsString = false;
            bool rightIsArray = false;
            bool rightIsTuple = false;
            
            bool leftBooleanValue = false;
            bool rightBooleanValue = false;

            int leftIntValue = 0;
            int rightIntValue = 0;
            
            double leftRealValue = 0;
            double rightRealValue = 0;

            string leftStringValue = "";
            string rightStringValue = "";

            ArrayLiteralNode leftArrayValue = new ArrayLiteralNode(new ExpressionNodeListNode());
            ArrayLiteralNode rightArrayValue = new ArrayLiteralNode(new ExpressionNodeListNode());
            
            TupleLiteralNode leftTupleValue = new TupleLiteralNode(new TupleElementNodeListNode());
            TupleLiteralNode rightTupleValue = new TupleLiteralNode(new TupleElementNodeListNode());
            
            
            
            switch (Left)
            {
                case BooleanLiteralNode booleanLeft:
                    leftBooleanValue = booleanLeft.Value;
                    leftIsBool = true;
                    break;
                case IntegerLiteralNode integerLeft:
                    leftIntValue = integerLeft.Value;
                    leftIsInt = true;
                    break;
                case RealLiteralNode realLeft:
                    leftRealValue = realLeft.Value;
                    leftIsReal = true;
                    break;
                case StringLiteralNode stringLeft:
                    leftStringValue = stringLeft.Value;
                    leftIsString = true;
                    break;
                case ArrayLiteralNode arrayLeft:
                    leftArrayValue = arrayLeft;
                    leftIsArray = true;
                    break;
                case TupleLiteralNode tupleLeft:
                    leftTupleValue = tupleLeft;
                    leftIsTuple = true;
                    break;
                default:
                    return new ErrorNode("Wrong literal type for binary expression!");
            }

            switch (Right)
            {
                case BooleanLiteralNode booleanRight:
                    rightBooleanValue = booleanRight.Value;
                    rightIsBool = true;
                    break;
                case IntegerLiteralNode integerRight:
                    rightIntValue = integerRight.Value;
                    rightIsInt = true;
                    break;
                case RealLiteralNode realRight:
                    rightRealValue = realRight.Value;
                    rightIsReal = true;
                    break;
                case StringLiteralNode stringRight:
                    rightStringValue = stringRight.Value;
                    rightIsString = true;
                    break;
                case ArrayLiteralNode arrayRight:
                    rightArrayValue = arrayRight;
                    rightIsArray = true;
                    break;
                case TupleLiteralNode tupleRight:
                    rightTupleValue = tupleRight;
                    rightIsTuple = true;
                    break;
                default:
                    return new ErrorNode("Wrong literal type for binary expression!");
            }

            switch (Operator)
            {
                case BinaryOperator.Add:
                    if (leftIsInt)
                    {
                        if (rightIsInt)
                        {
                            return new IntegerLiteralNode(leftIntValue + rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new RealLiteralNode(leftIntValue + rightRealValue);
                        }
                    }
                    if (leftIsReal)
                    {
                        if (rightIsInt)
                        {
                            return new RealLiteralNode(leftRealValue + rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new RealLiteralNode(leftRealValue + rightRealValue);
                        }
                    }

                    if (leftIsString && rightIsString)
                    {
                        return new StringLiteralNode(leftStringValue + rightStringValue);
                    }
                    
                    if (leftIsTuple && rightIsTuple)
                    {
                        return leftTupleValue + rightTupleValue;
                    }
                    
                    if (leftIsArray && rightIsArray)
                    {
                        return leftArrayValue + rightArrayValue;
                    }
                    break;
                case BinaryOperator.Subtract:
                    if (leftIsInt)
                    {
                        if (rightIsInt)
                        {
                            return new IntegerLiteralNode(leftIntValue - rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new RealLiteralNode(leftIntValue - rightRealValue);
                        }
                    }
                    if (leftIsReal)
                    {
                        if (rightIsInt)
                        {
                            return new RealLiteralNode(leftRealValue - rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new RealLiteralNode(leftRealValue - rightRealValue);
                        }
                    }
                    break;
                case BinaryOperator.Multiply:
                    if (leftIsInt)
                    {
                        if (rightIsInt)
                        {
                            return new IntegerLiteralNode(leftIntValue * rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new RealLiteralNode(leftIntValue * rightRealValue);
                        }
                    }
                    if (leftIsReal)
                    {
                        if (rightIsInt)
                        {
                            return new RealLiteralNode(leftRealValue * rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new RealLiteralNode(leftRealValue * rightRealValue);
                        }
                    }
                    break;
                case BinaryOperator.Divide:
                    if (leftIsInt)
                    {
                        if (rightIsInt)
                        {
                            return new IntegerLiteralNode(leftIntValue / rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new RealLiteralNode(leftIntValue / rightRealValue);
                        }
                    }
                    if (leftIsReal)
                    {
                        if (rightIsInt)
                        {
                            return new RealLiteralNode(leftRealValue / rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new RealLiteralNode(leftRealValue / rightRealValue);
                        }
                    }
                    break;
                case BinaryOperator.And:
                    if (leftIsBool && rightIsBool)
                    {
                        return new BooleanLiteralNode(leftBooleanValue && rightBooleanValue);
                    }
                    break;
                case BinaryOperator.Or:
                    if (leftIsBool && rightIsBool)
                    {
                        return new BooleanLiteralNode(leftBooleanValue || rightBooleanValue);
                    }
                    break;
                case BinaryOperator.Xor:
                    if (leftIsBool && rightIsBool)
                    {
                        return new BooleanLiteralNode(leftBooleanValue ^ rightBooleanValue);
                    }
                    break;
                case BinaryOperator.Less:
                    if (leftIsInt)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftIntValue < rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftIntValue < rightRealValue);
                        }
                    }
                    if (leftIsReal)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftRealValue < rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftRealValue < rightRealValue);
                        }
                    }
                    break;
                case BinaryOperator.More:
                    if (leftIsInt)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftIntValue > rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftIntValue > rightRealValue);
                        }
                    }
                    if (leftIsReal)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftRealValue > rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftRealValue > rightRealValue);
                        }
                    }
                    break;
                case BinaryOperator.LessOrEqual:
                    if (leftIsInt)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftIntValue <= rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftIntValue <= rightRealValue);
                        }
                    }
                    if (leftIsReal)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftRealValue <= rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftRealValue <= rightRealValue);
                        }
                    }
                    break;
                case BinaryOperator.MoreOrEqual:
                    if (leftIsInt)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftIntValue >= rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftIntValue >= rightRealValue);
                        }
                    }
                    if (leftIsReal)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftRealValue >= rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftRealValue >= rightRealValue);
                        }
                    }
                    break;
                case BinaryOperator.Equal:
                    if (leftIsInt)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftIntValue == rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftIntValue == rightRealValue);
                        }
                    }
                    if (leftIsReal)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftRealValue == rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftRealValue == rightRealValue);
                        }
                    }
                    break;
                case BinaryOperator.NotEqual:
                    if (leftIsInt)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftIntValue != rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftIntValue != rightRealValue);
                        }
                    }
                    if (leftIsReal)
                    {
                        if (rightIsInt)
                        {
                            return new BooleanLiteralNode(leftRealValue != rightIntValue);
                        }

                        if (rightIsReal)
                        {
                            return new BooleanLiteralNode(leftRealValue != rightRealValue);
                        }
                    }
                    break;
            }
            return new ErrorNode("Invalid types for binary operation!");
        }
    }

    // Unary expression node representing unary operations
    public class UnaryExpressionNode : ExpressionNode
    {
        public UnaryOperator Operator { get; }
        public ExpressionNode Operand;

        public UnaryExpressionNode(UnaryOperator op, ExpressionNode operand)
        {
            Operator = op;
            Operand = operand;
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            var resultNode = Operand.InterpretNode(ref stack);
            if (resultNode is ErrorNode node)
            {
                return node;
            }

            Operand = (ExpressionNode)resultNode;

            bool IsBool = false;
            bool IsInt = false;
            bool IsReal = false;

            bool BooleanValue = false;
            int IntValue = 0;
            double RealValue = 0;

            switch (Operand)
            {
                case BooleanLiteralNode booleanLeft:
                    BooleanValue = booleanLeft.Value;
                    IsBool = true;
                    break;
                case IntegerLiteralNode integerLeft:
                    IntValue = integerLeft.Value;
                    IsInt = true;
                    break;
                case RealLiteralNode realLeft:
                    RealValue = realLeft.Value;
                    IsReal = true;
                    break;
                default:
                    return new ErrorNode("Wrong literal type for unary expression!");
            }

            switch (Operator)
            {
                case UnaryOperator.Plus:
                    if (IsInt || IsReal)
                        return Operand;
                    break;
                case UnaryOperator.Minus:
                    if (IsInt)
                        return new IntegerLiteralNode(-IntValue);
                    if (IsReal)
                        return new RealLiteralNode(-RealValue);
                    break;
                case UnaryOperator.Not:
                    if (IsBool)
                        return new BooleanLiteralNode(!BooleanValue);
                    break;
            }
            return new ErrorNode("Wrong literal type for unary expression!");
        }
    }

    // Literal nodes representing literals (int, real, boolean, string, etc.)
    public abstract class LiteralNode : ExpressionNode
    {
    };

    public class IntegerLiteralNode : LiteralNode
    {
        public int Value { get; }

        public IntegerLiteralNode(int value)
        {
            Value = value;
        }
        
        public override string ToString()
        {
            return Value.ToString();
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            return this;
        }
    }
    
    public class RealLiteralNode : LiteralNode
    {
        public double Value { get; }

        public RealLiteralNode(double value)
        {
            Value = value;
        }
        
        public override string ToString()
        {
            return Value.ToString();
        }
        
        public override AstNode InterpretNode(ref CallStack stack)
        {
            return this;
        }
    }
    
    public class BooleanLiteralNode : LiteralNode
    {
        public bool Value { get; }

        public BooleanLiteralNode(bool value)
        {
            Value = value;
        }
        
        public override string ToString()
        {
            return Value.ToString();
        }
        
        public override AstNode InterpretNode(ref CallStack stack)
        {
            return this;
        }
    }
    
    public class StringLiteralNode : LiteralNode
    {
        public string Value { get; }

        public StringLiteralNode(string value)
        {
            Value = value;
        }
        
        public override string ToString()
        {
            return Value;
        }

        public StringLiteralNode(StringNode value)
        {
            Value = value.GetString();
        }
        
        public override AstNode InterpretNode(ref CallStack stack)
        {
            return this;
        }
    }

    // Define enumeration for binary operators
    public enum BinaryOperator
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        And,
        Or,
        Xor,
        Less,
        More, // TODO: change to greater
        LessOrEqual,
        MoreOrEqual, // change to greater
        Equal,
        NotEqual
    }

    // Define enumeration for unary operators
    public enum UnaryOperator
    {
        Plus,
        Minus,
        Not,
        TypeConversion
    }

    // Assignment statement node
    public class AssignmentNode : StatementNode
    {
        public string VariableName { get; }
        public ExpressionNode AssignedExpression;

        public AssignmentNode(StringNode variableName, ExpressionNode assignedExpression)
        {
            VariableName = variableName.GetString();
            AssignedExpression = assignedExpression;
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            if (AssignedExpression != null)
            {
                var resultNode = AssignedExpression.InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }

                AssignedExpression = (ExpressionNode)resultNode;
            }

            if (stack.SetVariable(VariableName, AssignedExpression))
            {
                return this;
            }
            return new ErrorNode("Variable not declared!");
        }
    }

    // Print statement node
    public class PrintNode : StatementNode
    {
        public List<ExpressionNode> Expressions { get; }

        public PrintNode(ExpressionNodeListNode expressions)
        {
            Expressions = expressions.GetList();
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            for (int i = 0; i < Expressions.Count; i++)
            {
                AstNode resultNode = Expressions[i].InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }
                Expressions[i] = (ExpressionNode)resultNode;
                Console.Out.Write($"{Expressions[i]} ");
            }

            Console.Out.WriteLine();
            return this;
        }
    }

    // Return statement node
    public class ReturnNode : StatementNode
    {
        private ExpressionNode returnValue;

        public ExpressionNode GetReturnValue()
        {
            return returnValue;
        }

        public ReturnNode(ExpressionNode returnValue) => this.returnValue = returnValue;
        public override AstNode InterpretNode(ref CallStack stack)
        {
            if (returnValue != null)
            {
                var resultNode = returnValue.InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }

                returnValue = (ExpressionNode)resultNode;
            }

            return this;
        }
    }

    // If statement node
    public class IfNode : StatementNode
    {
        public ExpressionNode Condition;
        public List<StatementNode> TrueBranch;

        private List<StatementNode> FalseBranch;

        public List<StatementNode> GetFalseBranch()
        {
            return FalseBranch;
        }

        public IfNode(ExpressionNode condition, StatementNodeListNode trueBranch, StatementNodeListNode falseBranch)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            TrueBranch = trueBranch.GetList() ?? throw new ArgumentNullException(nameof(trueBranch));
            this.FalseBranch = falseBranch.GetList();
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            var resultNode = Condition.InterpretNode(ref stack);
            if (resultNode is ErrorNode node)
            {
                return node;
            }

            Condition = (ExpressionNode)resultNode;

            if (Condition is BooleanLiteralNode booleanResultNode)
            {
                if (booleanResultNode.Value)
                {
                    for (int i = 0; i < TrueBranch.Count; i++)
                    {
                        AstNode resultNode2 = TrueBranch[i].InterpretNode(ref stack);
                        if (resultNode2 is ErrorNode node2)
                        {
                            return node2;
                        }

                        TrueBranch[i] = (StatementNode)resultNode2;
                    }
                }
                else
                {
                    for (int i = 0; i < FalseBranch.Count; i++)
                    {
                        AstNode resultNode2 = FalseBranch[i].InterpretNode(ref stack);
                        if (resultNode2 is ErrorNode node2)
                        {
                            return node2;
                        }

                        FalseBranch[i] = (StatementNode)resultNode2;
                    }
                }
            }
            else
            {
                return new ErrorNode("Condition is not a boolean!");
            }

            return this;
        }
    }

    // Loop statement node
    public class LoopNode : StatementNode
    {
        public ExpressionNode Condition { get; }
        public List<StatementNode> LoopBody { get; }

        public LoopNode(ExpressionNode condition, StatementNodeListNode loopBody)
        {
            Condition = condition;
            LoopBody = loopBody.GetList();
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            return this;  //TODO: MAKE THIS WORK
        }
    }

    // Access node representing variable or function access
    public class AccessNode : ExpressionNode
    {
        public ExpressionNode Target;
        public AccessTailNode Tail;

        public AccessNode(ExpressionNode target, AccessTailNode tail)
        {
            Target = target;
            Tail = tail;
        }

        public AccessNode(ExpressionNode target)
        {
            Target = target;
            Tail = null; // or set a default AccessTailNode as needed
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            if (Target != null)
            {
                var resultNode = Target.InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }

                Target = (ExpressionNode)resultNode;
            }
            
            if (Tail != null)
            {
                var resultNode = Tail.InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }
                Tail = (AccessTailNode)resultNode;
                var varName = ((VariableNode)Target).VariableName;
                switch (Tail)
                {
                    case BracketAccessNode bracketAccessNode:
                        if (stack.GetVariable(varName) is ArrayLiteralNode targetArray)
                        {
                            return targetArray.Get(((IntegerLiteralNode)bracketAccessNode.Index).Value);
                        }
                        return new ErrorNode("Variable with square brackets access is not an array!");
                        break;
                    case DotAccessNode dotAccessNode:
                        if (stack.GetVariable(varName) is TupleLiteralNode targetTuple)
                        {
                            if (dotAccessNode.UseMemberName) 
                            {
                                return targetTuple.Get(dotAccessNode.MemberName);
                            }
                            else
                            {
                                return targetTuple.Get(dotAccessNode.Index);
                            }
                        }
                        return new ErrorNode("Variable with dot access is not a tuple!");
                        break;
                    case FunctionCallAccessNode functionCallAccessNode:
                        return this;
                        break;
                }
            }
            return this;
        }
    }

    // AccessTail node representing various access methods (e.g., dot, square brackets, function call)
    public abstract class AccessTailNode : AstNode { }

    // Dot access (e.g., obj.field)
    public class DotAccessNode : AccessTailNode
    {
        public string MemberName { get; }
        public int Index;
        public bool UseMemberName;

        public DotAccessNode(VariableNode memberNameNode)
        {
            MemberName = memberNameNode.VariableName;
            UseMemberName = true;
        }

        public DotAccessNode(IntegerLiteralNode indexNode)
        {
            Index = indexNode.Value;
            UseMemberName = false;
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            return this;
        }
    }

    // Square bracket access (e.g., array[index])
    public class BracketAccessNode : AccessTailNode
    {
        public ExpressionNode Index;

        public BracketAccessNode(ExpressionNode index)
        {
            Index = index;
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            var resultNode = Index.InterpretNode(ref stack);
            if (resultNode is ErrorNode node)
            {
                return node;
            }

            Index = (ExpressionNode)resultNode;
            if (Index is IntegerLiteralNode)
            {
                return this;
            }

            return new ErrorNode("Expression for square brackets access does not evaluate to integer!");
        }
    }

    // Function call access (e.g., func(args))
    public class FunctionCallAccessNode : AccessTailNode
    {
        public List<ExpressionNode> Arguments { get; }

        public FunctionCallAccessNode(ExpressionNodeListNode arguments)
        {
            Arguments = arguments.GetList();
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            throw new NotImplementedException();  // TODO: Fix
        }
    }
}

// Type conversion node
public class TypeConversionNode : UnaryExpressionNode
{
    public TypeIndicator TargetType { get; }

    public TypeConversionNode(ExpressionNode operand, TypeIndicator targetType)
        : base(UnaryOperator.TypeConversion, operand)
    {
        TargetType = targetType;
    }
}

// Variable node
public class VariableNode : ExpressionNode
{
    public string VariableName { get; }

    public VariableNode(StringNode variableName)
    {
        VariableName = variableName.GetString();
    }

    public override AstNode InterpretNode(ref CallStack stack)
    {
        var gottenVariable = stack.GetVariable(VariableName);
        if (gottenVariable != null)
        {
            return gottenVariable;
        }

        return new ErrorNode("Variable not declared!");
    }
}

// ReadInt node
public class ReadIntNode : ExpressionNode
{
    // You can add additional properties or methods if needed
    public override AstNode InterpretNode(ref CallStack stack)
    {
        return new IntegerLiteralNode(Convert.ToInt32(Console.ReadLine()));
    }
}

// ReadReal node
public class ReadRealNode : ExpressionNode
{
    // You can add additional properties or methods if needed
    public override AstNode InterpretNode(ref CallStack stack)
    {
        return new RealLiteralNode(Convert.ToDouble(Console.ReadLine()));
    }
}

// ReadString node
public class ReadStringNode : ExpressionNode
{
    public override AstNode InterpretNode(ref CallStack stack)
    {
        return new StringLiteralNode(Console.ReadLine());
    }
}

// Function call node
public class FunctionCallNode : ExpressionNode
{
    public ExpressionNode Function { get; }
    public List<ExpressionNode> Arguments { get; }

    public FunctionCallNode(ExpressionNode function, ExpressionNodeListNode arguments)
    {
        Function = function;
        Arguments = arguments.GetList();
    }

    public FunctionCallNode(ExpressionNode function)
        : this(function, new ExpressionNodeListNode())
    {
        // idk just set it empty
    }

    public override AstNode InterpretNode(ref CallStack stack)
    {
        throw new NotImplementedException(); //TODO : fIX THIS
    }
}

// For loop node
public class ForLoopNode : StatementNode
{
    public string VariableName { get; }
    public TypeIndicator VariableType { get; }
    public List<StatementNode> LoopBody { get; }

    public ForLoopNode(StringNode variableName, TypeIndicator variableType, StatementNodeListNode loopBody)
    {
        VariableName = variableName.GetString();
        VariableType = variableType;
        LoopBody = loopBody.GetList();
    }

    public override AstNode InterpretNode(ref CallStack stack)
    {
        throw new NotImplementedException();  // TODO: FIX
    }
}

// Type indicator enumeration
public enum TypeIndicatorEnum
{
    Int,
    Real,
    Bool,
    String,
    Empty,
    Vector,
    Tuple,
    Function,
    Range
}

// Array literal node
public class ArrayLiteralNode : ExpressionNode
{
    public Dictionary<int, ExpressionNode> Elements;

    public ArrayLiteralNode(ExpressionNodeListNode elements)
    {
        List<ExpressionNode> elementsList = elements.GetList();
        Elements = new Dictionary<int, ExpressionNode>();
        for (int i = 0; i < elementsList.Count; i++)
        {
            Elements[i + 1] = elementsList[i];
        }
    }
    
    public ArrayLiteralNode(Dictionary<int, ExpressionNode> elements)
    {
        Elements = elements;
    }

    public ExpressionNode Get(int index)
    {
        return Elements[index];
    }

    public void Set(int index, ExpressionNode value)
    {
        Elements[index] = value;
    }
    
    public static ArrayLiteralNode operator +(ArrayLiteralNode a, ArrayLiteralNode b)
    {
        Dictionary<int, ExpressionNode> newElements = new Dictionary<int, ExpressionNode>();
        foreach (var key in a.Elements.Keys)
        {
            newElements[key] = a.Elements[key];
        }
        foreach (var key in b.Elements.Keys)
        {
            newElements[key] = b.Elements[key];
        }
        return new ArrayLiteralNode(newElements);
    }

    public override AstNode InterpretNode(ref CallStack stack)
    {
        var newElements = new Dictionary<int, ExpressionNode>();
        foreach (var key in Elements.Keys)
        {
            AstNode resultNode = Elements[key].InterpretNode(ref stack);
            if (resultNode is ErrorNode node)
            {
                return node;
            }
            newElements[key] = (ExpressionNode)resultNode;
        }
        Elements = newElements;
        return this;
    }
}

// Tuple literal node
public class TupleLiteralNode : ExpressionNode
{
    public List<TupleElementNode> Elements { get; }

    public TupleLiteralNode(TupleElementNodeListNode elements)
    {
        Elements = elements.GetList();
    }
    
    public TupleLiteralNode(List<TupleElementNode> elements)
    {
        Elements = elements;
    }
    
    public ExpressionNode Get(int index)
    {
        return Elements[index - 1].Value;
    }
        
    public ExpressionNode Get(string key)
    {
        return Elements.Find(elem => elem.VariableName == key).Value;
    }

    public void Set(int index, ExpressionNode value)
    {
        Elements[index - 1] = new TupleElementNode(new StringNode(Elements[index - 1].VariableName), value);
    }
        
    public void Set(string key, ExpressionNode value)
    {
        int index = Elements.FindIndex(elem => elem.VariableName == key);
        if (index > -1)
        {
            Elements[index] = new TupleElementNode(new StringNode(key), value);
        }
    }

    public static TupleLiteralNode operator +(TupleLiteralNode a, TupleLiteralNode b)
    {
        List<TupleElementNode> combinedLists = b.Elements;
        combinedLists.InsertRange(0, a.Elements);
        return new TupleLiteralNode(combinedLists);
    }

    public override AstNode InterpretNode(ref CallStack stack)
    {
        for (int i = 0; i < Elements.Count; i++)
        {
            AstNode resultNode = Elements[i].InterpretNode(ref stack);
            if (resultNode is ErrorNode node)
            {
                return node;
            }
            Elements[i] = (TupleElementNode)resultNode;
        }

        return this;
    }
}

// Tuple element node
public class TupleElementNode : AstNode
{
    public string VariableName { get; }
    public ExpressionNode Value;

    public TupleElementNode(StringNode variableName, ExpressionNode value)
    {
        VariableName = variableName.GetString();
        Value = value;
    }
    
    public TupleElementNode(ExpressionNode value)
    {
        VariableName = "";
        Value = value;
    }

    public override AstNode InterpretNode(ref CallStack stack)
    {
        var resultNode = Value.InterpretNode(ref stack);
        if (resultNode is ErrorNode node)
        {
            return node;
        }
        Value = (ExpressionNode)resultNode;
        return this;
    }
}

// Function literal node
public class FunctionLiteralNode : ExpressionNode
{
    public List<StringNode> Parameters { get; }
    public FunctionLiteralBodyNode Body { get; }

    public FunctionLiteralNode(StringNodeListNode parameters, FunctionLiteralBodyNode body)
    {
        Parameters = parameters.GetList();
        Body = body;
    }

    public override AstNode InterpretNode(ref CallStack stack)
    {
        throw new NotImplementedException();  // TODO : FIX THIS
    }
}

// Function body node
public class FunctionBodyNode : AstNode
{
    public ExpressionNode Expression { get; }

    public FunctionBodyNode(ExpressionNode expression)
    {
        Expression = expression;
    }

    public override AstNode InterpretNode(ref CallStack stack)
    {
        throw new NotImplementedException();  // TODO: FIX THIS
    }
}

public class TypeIndicator : AstNode
{
    // костыльно, но что поделать
    private TypeIndicatorEnum Value { get; }
    private AstNode StartValue { get; }
    private AstNode EndValue { get; }

    private TypeIndicator(TypeIndicatorEnum value)
    {
        Value = value;
    }

    private TypeIndicator(TypeIndicatorEnum value, AstNode startValue, AstNode endValue)
    {
        Value = value;
        StartValue = startValue;
        EndValue = endValue;
    }

    public static TypeIndicator Int => new TypeIndicator(TypeIndicatorEnum.Int);
    public static TypeIndicator Real => new TypeIndicator(TypeIndicatorEnum.Real);
    public static TypeIndicator Bool => new TypeIndicator(TypeIndicatorEnum.Bool);
    public static TypeIndicator String => new TypeIndicator(TypeIndicatorEnum.String);
    public static TypeIndicator Empty => new TypeIndicator(TypeIndicatorEnum.Empty);
    public static TypeIndicator Vector => new TypeIndicator(TypeIndicatorEnum.Vector);
    public static TypeIndicator Tuple => new TypeIndicator(TypeIndicatorEnum.Tuple);
    public static TypeIndicator Function => new TypeIndicator(TypeIndicatorEnum.Function);

    public static TypeIndicator Range(AstNode startValue, AstNode endValue)
    {
        return new TypeIndicator(TypeIndicatorEnum.Range, startValue, endValue);
    }

    public override AstNode InterpretNode(ref CallStack stack)
    {
        return this; // TODO : FIX THIS
    }
}
