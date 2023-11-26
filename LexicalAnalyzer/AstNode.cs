﻿using System;
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

        public DeclarationNode(string variableName, ExpressionNode assignedExpression)
        {
            VariableName = variableName;
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

    // You can define other AST node classes based on your language's grammar

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
        // Add other operators as needed
    }

    // Define enumeration for unary operators
    public enum UnaryOperator
    {
        Plus,
        Minus,
        Not,
    }

    // Statement node representing a statement in the program
    public abstract class StatementNode : AstNode { }

    // Assignment statement node
    public class AssignmentNode : StatementNode
    {
        public string VariableName { get; }
        public ExpressionNode AssignedExpression { get; }

        public AssignmentNode(string variableName, ExpressionNode assignedExpression)
        {
            VariableName = variableName;
            AssignedExpression = assignedExpression;
        }
    }

    // Print statement node
    public class PrintNode : StatementNode
    {
        public List<ExpressionNode> Expressions { get; }

        public PrintNode(List<ExpressionNode> expressions)
        {
            Expressions = expressions;
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

        public IfNode(ExpressionNode condition, List<StatementNode> trueBranch, List<StatementNode> falseBranch)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            TrueBranch = trueBranch ?? throw new ArgumentNullException(nameof(trueBranch));
            this.falseBranch = falseBranch;
        }
    }

    // Loop statement node
    public class LoopNode : StatementNode
    {
        public ExpressionNode Condition { get; }
        public List<StatementNode> LoopBody { get; }

        public LoopNode(ExpressionNode condition, List<StatementNode> loopBody)
        {
            Condition = condition;
            LoopBody = loopBody;
        }
    }

    // Access node representing variable or function access
    public class AccessNode : ExpressionNode
    {
        public ExpressionNode Target { get; }
        public AccessTailNode Tail { get; }

        public AccessNode(ExpressionNode target, AccessTailNode tail)
        {
            Target = target;
            Tail = tail;
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

        public FunctionCallAccessNode(List<ExpressionNode> arguments)
        {
            Arguments = arguments;
        }
    }
}
