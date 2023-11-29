using System.Collections.Generic;

namespace LexicalAnalyzer
{
    public class StringNode : AstNode
    {
        public string StringValue;

        public StringNode(string stringValue)
        {
            StringValue = stringValue;
        }

        public string GetString()
        {
            return StringValue;
        }

        public void SetString(string newString)
        {
            StringValue = newString;
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            return this;
        }
    }

    public class ListNode<T> : AstNode
    {
        public List<T> NodeList = new List<T>();
        
        public ListNode(List<T> nodeList)
        {
            NodeList = nodeList;
        }
        
        public ListNode(T node)
        {
            NodeList.Add(node);
        }
        
        public ListNode()
        {
            NodeList = new List<T>();
        }

        public List<T> GetList()
        {
            return NodeList;
        }

        public void Add(T node)
        {
            NodeList.Add(node);
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            return this;
        }
    }

    public class ExpressionNodeListNode : ListNode<ExpressionNode>
    {
        public ExpressionNodeListNode(List<ExpressionNode> nodeList)
        {
            NodeList = nodeList;
        }
        
        public ExpressionNodeListNode(ExpressionNode node)
        {
            NodeList.Add(node);
        }
        
        public ExpressionNodeListNode()
        {
            NodeList = new List<ExpressionNode>();
        }
        
        public override AstNode InterpretNode(ref CallStack stack)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                AstNode resultNode = NodeList[i].InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }
                NodeList[i] = (ExpressionNode)resultNode;
            }

            return this;
        }
    };

    public class StringNodeListNode : ListNode<StringNode>
    {
        public StringNodeListNode(List<StringNode> nodeList)
        {
            NodeList = nodeList;
        }
        
        public StringNodeListNode(StringNode node)
        {
            NodeList.Add(node);
        }
        
        public StringNodeListNode()
        {
            NodeList = new List<StringNode>();
        }
        
        public override AstNode InterpretNode(ref CallStack stack)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                AstNode resultNode = NodeList[i].InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }
                NodeList[i] = (StringNode)resultNode;
            }

            return this;
        }
    };

    public class TupleElementNodeListNode : ListNode<TupleElementNode>
    {
        public TupleElementNodeListNode(List<TupleElementNode> nodeList)
        {
            NodeList = nodeList;
        }
        
        public TupleElementNodeListNode(TupleElementNode node)
        {
            NodeList.Add(node);
        }
        
        public TupleElementNodeListNode()
        {
            NodeList = new List<TupleElementNode>();
        }
        
        public override AstNode InterpretNode(ref CallStack stack)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                AstNode resultNode = NodeList[i].InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }
                NodeList[i] = (TupleElementNode)resultNode;
            }

            return this;
        }
    };
    
    public class DeclarationNodeListNode : ListNode<DeclarationNode>
    {
        public DeclarationNodeListNode(List<DeclarationNode> nodeList)
        {
            NodeList = nodeList;
        }
        
        public DeclarationNodeListNode(DeclarationNode node)
        {
            NodeList.Add(node);
        }
        
        public DeclarationNodeListNode()
        {
            NodeList = new List<DeclarationNode>();
        }
        
        public override AstNode InterpretNode(ref CallStack stack)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                AstNode resultNode = NodeList[i].InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }
                NodeList[i] = (DeclarationNode)resultNode;
            }

            return this;
        }
    };
    
    public class StatementNodeListNode : ListNode<StatementNode>
    {
        public StatementNodeListNode(List<StatementNode> nodeList)
        {
            NodeList = nodeList;
        }
        
        public StatementNodeListNode(StatementNode node)
        {
            NodeList.Add(node);
        }
        
        public StatementNodeListNode()
        {
            NodeList = new List<StatementNode>();
        }
        
        public override AstNode InterpretNode(ref CallStack stack)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                AstNode resultNode = NodeList[i].InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }
                NodeList[i] = (StatementNode)resultNode;
            }

            return this;
        }
    };

    public class FunctionLiteralBodyNode : AstNode
    {
        public StatementNodeListNode StatementBody;
        public ExpressionNode ExpressionBody;
        public bool UsesStatementBody;

        public FunctionLiteralBodyNode(StatementNodeListNode statementBody)
        {
            StatementBody = statementBody;
            UsesStatementBody = true;
        }

        public FunctionLiteralBodyNode(ExpressionNode expressionBody)
        {
            ExpressionBody = expressionBody;
            UsesStatementBody = false;
        }

        public override AstNode InterpretNode(ref CallStack stack)
        {
            if (UsesStatementBody)
            {
                var resultNode = StatementBody.InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }
                StatementBody = (StatementNodeListNode)resultNode;
            }
            else
            {
                var resultNode = ExpressionBody.InterpretNode(ref stack);
                if (resultNode is ErrorNode node)
                {
                    return node;
                }
                ExpressionBody = (ExpressionNode)resultNode;
            }
            return this;
        }
    }

}