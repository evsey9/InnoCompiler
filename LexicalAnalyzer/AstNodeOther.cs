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
        
        public ListNode()
        {
            NodeList = new List<T>();
        }
        
        public ListNode(T node)
        {
            NodeList.Add(node);
        }
        
        public ListNode(List<T> nodeList)
        {
            NodeList = nodeList;
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
    };

    public class StringNodeListNode : ListNode<StringNode>
    {
    };

    public class TupleElementNodeListNode : ListNode<TupleElementNode>
    {
    };
    
    public class DeclarationNodeListNode : ListNode<DeclarationNode>
    {
    };
    
    public class StatementNodeListNode : ListNode<StatementNode>
    {
    };

}