﻿using LexicalAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalyzer
{
    public abstract class AstNode { }

    // Program node representing the entire program
    public class ProgramNode : AstNode
    {
        public List<DeclarationNode> Declarations { get; } = new List<DeclarationNode>();

        public ProgramNode()
        {
        }
        
        public ProgramNode AddDeclaration(DeclarationNode declaration)
        {
            Declarations.Add(declaration);
            return this;
        }
        public ProgramNode(DeclarationNode declaration)
        {
            
            AddDeclaration(declaration);
        }
    }

    // Declaration node representing variable declarations
    public class DeclarationNode : AstNode
    {
        public string VariableName { get; }
        public ExpressionNode AssignedExpression { get; }

        public DeclarationNode(StringNode variableName, ExpressionNode assignedExpression)
        {
            VariableName = variableName.GetString();
            AssignedExpression = assignedExpression;
        }
    }

    // Expression node representing expressions
    public abstract class ExpressionNode : AstNode { }

    // Binary expression node representing binary operations
    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; }
        public BinaryOperator Operator { get; }
        public ExpressionNode Right { get; }

        public BinaryExpressionNode(ExpressionNode left, BinaryOperator op, ExpressionNode right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }
    }

    // Unary expression node representing unary operations
    public class UnaryExpressionNode : ExpressionNode
    {
        public UnaryOperator Operator { get; }
        public ExpressionNode Operand { get; }

        public UnaryExpressionNode(UnaryOperator op, ExpressionNode operand)
        {
            Operator = op;
            Operand = operand;
        }
    }

    // Literal nodes representing literals (int, real, boolean, string, etc.)
    public class LiteralNode : ExpressionNode
    {
        public object Value { get; }

        public LiteralNode(object value)
        {
            Value = value;
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

    // Statement node representing a statement in the program
    public abstract class StatementNode : AstNode { }

    // Assignment statement node
    public class AssignmentNode : StatementNode
    {
        public string VariableName { get; }
        public ExpressionNode AssignedExpression { get; }

        public AssignmentNode(StringNode variableName, ExpressionNode assignedExpression)
        {
            VariableName = variableName.GetString();
            AssignedExpression = assignedExpression;
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
    }

    // Return statement node
    public class ReturnNode : StatementNode
    {
        private readonly ExpressionNode returnValue;

        public ExpressionNode GetReturnValue()
        {
            return returnValue;
        }

        public ReturnNode(ExpressionNode returnValue) => this.returnValue = returnValue;
    }

    // If statement node
    public class IfNode : StatementNode
    {
        public ExpressionNode Condition { get; }
        public List<StatementNode> TrueBranch { get; }

        private readonly List<StatementNode> falseBranch;

        public List<StatementNode> GetFalseBranch()
        {
            return falseBranch;
        }

        public IfNode(ExpressionNode condition, StatementNodeListNode trueBranch, StatementNodeListNode falseBranch)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            TrueBranch = trueBranch.GetList() ?? throw new ArgumentNullException(nameof(trueBranch));
            this.falseBranch = falseBranch.GetList();
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
    }

    // Access node representing variable or function access
    public class AccessNode : ExpressionNode
    {
        public ExpressionNode Target { get; }
        //public AccessTailNode Tail { get; }

        public AccessNode(ExpressionNode target)
        {
            Target = target;
            //Tail = tail;
        }
    }

    // AccessTail node representing various access methods (e.g., dot, square brackets, function call)
    public abstract class AccessTailNode : AstNode { }

    // Dot access (e.g., obj.field)
    public class DotAccessNode : AccessTailNode
    {
        public string MemberName { get; }

        public DotAccessNode(string memberName)
        {
            MemberName = memberName;
        }
    }

    // Square bracket access (e.g., array[index])
    public class BracketAccessNode : AccessTailNode
    {
        public ExpressionNode Index { get; }

        public BracketAccessNode(ExpressionNode index)
        {
            Index = index;
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
}

// ReadInt node
public class ReadIntNode : ExpressionNode
{
    // You can add additional properties or methods if needed
}

// ReadReal node
public class ReadRealNode : ExpressionNode
{
    // You can add additional properties or methods if needed
}

// ReadString node
public class ReadStringNode : ExpressionNode
{
    // You can add additional properties or methods if needed
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
}

// For loop node
public class ForLoopNode : StatementNode
{
    public string VariableName { get; }
    public TypeIndicator VariableType { get; }
    public ExpressionNode Collection { get; }
    public List<StatementNode> LoopBody { get; }

    public ForLoopNode(string variableName, TypeIndicator variableType, ExpressionNode collection, StatementNodeListNode loopBody)
    {
        VariableName = variableName;
        VariableType = variableType;
        Collection = collection;
        LoopBody = loopBody.GetList();
    }
}

// Type indicator enumeration
public enum TypeIndicator
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
    public List<ExpressionNode> Elements { get; }

    public ArrayLiteralNode(ExpressionNodeListNode elements)
    {
        Elements = elements.GetList();
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
}

// Tuple element node
public class TupleElementNode : AstNode
{
    public string VariableName { get; }
    public ExpressionNode Value { get; }

    public TupleElementNode(StringNode variableName, ExpressionNode value)
    {
        VariableName = variableName.GetString();
        Value = value;
    }
}

// Function literal node
public class FunctionLiteralNode : ExpressionNode
{
    public List<StringNode> Parameters { get; }
    public List<StatementNode> Body { get; }

    public FunctionLiteralNode(StringNodeListNode parameters, StatementNodeListNode body)
    {
        Parameters = parameters.GetList();
        Body = body.GetList();
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
}


