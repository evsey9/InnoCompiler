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
    };
    
    public class VariableDefinitionNodeListNode : ListNode<VariableDefinitionNode>
    {
        public VariableDefinitionNodeListNode(List<VariableDefinitionNode> nodeList)
        {
            NodeList = nodeList;
        }
        
        public VariableDefinitionNodeListNode(VariableDefinitionNode node)
        {
            NodeList.Add(node);
        }
        
        public VariableDefinitionNodeListNode()
        {
            NodeList = new List<VariableDefinitionNode>();
        }
    };

}