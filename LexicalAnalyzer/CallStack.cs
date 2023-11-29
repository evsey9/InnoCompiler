using System;
using System.Collections.Generic;

namespace LexicalAnalyzer
{
    public class CallStack
    {
        public List<Dictionary<string, ExpressionNode>> Stack;

        public CallStack()
        {
            Stack = new List<Dictionary<string, ExpressionNode>> { new Dictionary<string, ExpressionNode>() };
        }

        public void AddNewFrame()
        {
            Stack.Add(new Dictionary<string, ExpressionNode>());
        }

        public void PopFrame()
        {
            Stack.RemoveAt(Stack.Count - 1);
        }

        public int FindKeyLocationInStack(string key)
        {
            int searchLocationStart = Stack.Count - 1;
            for (int i = searchLocationStart; i >= 0; i--)
            {
                if (Stack[i].ContainsKey(key))
                {
                    return i;
                }
            }
            //throw new SystemException("Key not in stack!");
            return -1;
        }

        public ExpressionNode GetVariable(string variableName)
        {
            var location = FindKeyLocationInStack(variableName);
            return location == -1 ? null : Stack[location][variableName];
        }
        
        public bool DeclareVariable(string variableName, ExpressionNode value = null)
        {
            if (Stack[Stack.Count - 1].ContainsKey(variableName))
            {
                //throw new SystemException("Variable already declared!");
                return false;
            }
            else
            {
                Stack[Stack.Count - 1][variableName] = value;
                return true;
            }
        }
        
        public bool SetVariable(string variableName, ExpressionNode value)
        {
            var location = FindKeyLocationInStack(variableName);
            if (location == -1)
            {
                return false;
            }
            Stack[location][variableName] = value;
            return true;
        }
    }
}