using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace RedScarf.EasyCsvEditor
{
    public class ClassText
    {
        const char TAB_FORWARD = '}';
        const char TAB_BACK = '{';
        const string TAB = "\t";
        const string LINE_BREAK = "\r\n";
        const string USING_NAMESPACE_HEAD = "using System;" + LINE_BREAK;
        static HashSet<char> lineBreakSymbol = new HashSet<char>()
        {
            TAB_BACK,
            TAB_FORWARD,
            ';'
        };

        bool isClass;
        string className;
        string namespaceName;
        List<FieldNode> fieldNodeList;

        public ClassText(string namespaceName, string className,bool isClass)
        {
            this.isClass = isClass;
            this.namespaceName = namespaceName;
            this.className = className;
            fieldNodeList = new List<FieldNode>();
        }

        public void Add(FieldNode node)
        {
            fieldNodeList.Add(node);
        }

        public void Remove(FieldNode node)
        {
            fieldNodeList.Remove(node);
        }

        public string GetClassText()
        {
            var str = USING_NAMESPACE_HEAD;
            if (!string.IsNullOrEmpty(namespaceName))
            {
                str += "namespace " + namespaceName
                    + "{"
                    + "{class}"
                    + "}";
            }
            else
            {
                str += "{class}";
            }

            var classStr = "public "+(isClass?"class ":"struct ") + className
                        + "{"
                        + "{field}"
                        + "}";

            var fieldStr = "";
            foreach (var field in fieldNodeList)
            {
                fieldStr += field.ToString();
            }

            classStr = classStr.Replace("{field}", fieldStr);
            str = str.Replace("{class}", classStr);

            var strBuilder = new StringBuilder();
            var s = "";
            var c = ' ';
            var t = "";
            for (var i=0;i<str.Length;i++)
            {
                c = str[i];
                s = c.ToString();
                if (c==TAB_FORWARD)
                {
                    t=t.Remove(t.Length - TAB.Length, TAB.Length);
                    strBuilder.Replace(TAB,"",strBuilder.Length- TAB.Length, TAB.Length);
                }
                if (c==TAB_BACK)
                {
                    t += TAB;
                }
                if (lineBreakSymbol.Contains(c))
                {
                    s = c + LINE_BREAK+t;
                }
                strBuilder.Append(s);
            }

            return strBuilder.ToString();
        }
    }

    public class FieldNode
    {
        public string name;
        public string type;

        public FieldNode(string name,string type)
        {
            this.name = name;
            this.type = type;
        }

        public override string ToString()
        {
            return "public " + type + " " + name+";";
        }
    }
}