
using System.Collections.Generic;
using System.IO;

namespace Tenmove.Runtime
{
    public interface ITMXmlValue
    {
        string String
        {
            get;
        }

        bool Bool
        {
            get;
        }

        int Int32
        {
            get;
        }

        long Int64
        {
            get;
        }

        float Float32
        {
            get;
        }

        double Float64
        {
            get;
        }
    }

    public interface ITMXmlAttribute
    {
        string Name
        {
            get;
        }

        ITMXmlValue Value
        {
            get;
        }
    }

    public interface ITMXmlAttributeAccessor
    {
        int Count
        {
            get;
        }

        ITMXmlAttribute this[string attriName]
        {
            get;
        }

        ITMXmlAttribute this[int attriIndex]
        {
            get;
        }
    }

    public interface ITMXmlElement
    {
        string Name
        {
            get;
        }

        string Value
        {
            get;
        }

        ITMXmlElement[] this[string name]
        {
            get;
        }

        ITMXmlAttributeAccessor Attribute
        {
            get;
        }
    }

    public class XmlParser
    {
        private class XmlBase
        {
            protected readonly string m_Name;
            protected readonly string m_Value;

            public XmlBase(string name, string value)
            {
                Debugger.Assert(null != name, "Name can not be null!");
                Debugger.Assert(null != value, "Value can not be null!");

                m_Name = name;
                m_Value = value;
            }

            public string Name
            {
                get { return m_Name; }
            }

            public string Value
            {
                get { return m_Value; }
            }
        }

        public struct XmlValue : ITMXmlValue
        {
            private readonly string m_Value;
            public XmlValue(string value)
            {
                Debugger.Assert(null != value, "Value can not be null!");
                m_Value = value;
            }

            public string String
            {
                get { return m_Value; }
            }

            public bool Bool
            {
                get
                {
                    bool res;
                    if (bool.TryParse(m_Value, out res))
                        return res;
                    else
                        Debugger.LogWarning("Parse bool has failed![Content:'{0}']", m_Value);

                    return default(bool);
                }
            }

            public float Float32
            {
                get
                {
                    float res;
                    if (float.TryParse(m_Value, out res))
                        return res;
                    else
                        Debugger.LogWarning("Parse float has failed![Content:'{0}']", m_Value);

                    return default(float);
                }
            }

            public double Float64
            {
                get
                {
                    double res;
                    if (double.TryParse(m_Value, out res))
                        return res;
                    else
                        Debugger.LogWarning("Parse double has failed![Content:'{0}']", m_Value);

                    return default(double);
                }
            }

            public int Int32
            {
                get
                {
                    int res;
                    if (int.TryParse(m_Value, out res))
                        return res;
                    else
                        Debugger.LogWarning("Parse int has failed![Content:'{0}']", m_Value);

                    return default(int);
                }
            }

            public long Int64
            {
                get
                {
                    long res;
                    if (long.TryParse(m_Value, out res))
                        return res;
                    else
                        Debugger.LogWarning("Parse long has failed![Content:'{0}']", m_Value);

                    return default(long);
                }
            }
        }

        private class XmlNode : XmlBase
        {
            private readonly XmlNode m_Parent;
            private readonly LinkedList<XmlNode> m_Children;
            private readonly LinkedListNode<XmlNode> m_ThisNode;

            public XmlNode(string name, string value,XmlNode parent)
                :base(name,value)
            {
                m_Parent = parent;
                m_Children = new LinkedList<XmlNode>();

                if (null != m_Parent)
                {
                    m_Parent.m_Children.AddLast(this);
                    m_ThisNode = m_Parent.m_Children.Last;
                }
                else
                    m_ThisNode = null;
            }

            public XmlNode GetChild(int i)
            {
                int cnt = i;
                LinkedListNode<XmlNode> cur = m_Children.First;
                while (null != cur)
                {
                    if (0 == cnt--)
                        return cur.Value;
                    cur = cur.Next;
                }

                return null;
            }

            protected XmlNode _NextSibling
            {
                get { return null == m_ThisNode ? null : (null == m_ThisNode.Next ? null : m_ThisNode.Next.Value); }
            }

            protected XmlNode _PrevSibling
            {
                get { return null == m_ThisNode ? null : (null == m_ThisNode.Previous ? null : m_ThisNode.Previous.Value); }
            }

            protected XmlNode _FindFirstChild(string nodeName)
            {
                LinkedListNode<XmlNode> cur = m_Children.First;
                while (null != cur)
                {
                    if(cur.Value.Name.Equals(nodeName))
                        return cur.Value;

                    cur = cur.Next;
                }

                return null;
            }

            protected bool _FindAllChildren(string name,List<XmlNode> children)
            {
                children.Clear();
                LinkedListNode<XmlNode> cur = m_Children.First;
                while (null != cur)
                {
                    if (cur.Value.Name.Equals(name))
                        children.Add(cur.Value);

                    cur = cur.Next;
                }

                return children.Count > 0;
            }

            protected XmlNode _FindInOffspring(string nodeName,XmlNode node)
            {
                if (node.Name.Equals(nodeName))
                    return node;

                LinkedListNode<XmlNode> cur = node.m_Children.First;
                while(null != cur)
                {
                    XmlNode child = _FindInOffspring(nodeName, cur.Value);
                    if (null != child)
                        return child;

                    cur = cur.Next;
                }
                 
                return null;
            }

            protected void _ClearChildren()
            {
                m_Children.Clear();
            }
        }

        private class XmlAttributeAccessor : ITMXmlAttributeAccessor
        {
            private readonly List<XmlAttribute> m_Attributes;

            public XmlAttributeAccessor(List<XmlAttribute> attributes)
            {
                Debugger.Assert(null != attributes, "Attribute list can not be null!");
                m_Attributes = attributes;
            }

            public ITMXmlAttribute this[string attriName]
            {
                get { return _GetAttributeByName(attriName); }
            }

            public ITMXmlAttribute this[int attriIndex]
            {
                get { return _GetAttributeByIndex(attriIndex); }
            }

            public int Count
            {
                get { return m_Attributes.Count; }
            }

            public void Clear()
            {
                m_Attributes.Clear();
            }

            private ITMXmlAttribute _GetAttributeByIndex(int i)
            {
                if (0 <= i && i < m_Attributes.Count)
                    return m_Attributes[i];

                return NullAttribute;
            }

            private ITMXmlAttribute _GetAttributeByName(string name)
            {
                for (int i = 0, icnt = m_Attributes.Count; i < icnt; ++i)
                {
                    if (m_Attributes[i].Name == name)
                        return m_Attributes[i];
                }

                return NullAttribute;
            }
        }

        private class XmlElement : XmlNode,ITMXmlElement
        {
            private readonly XmlAttributeAccessor m_AttributeAccessor;

            public XmlElement(string name, string value,XmlNode parent, List<XmlAttribute> attributes)
                :base(name,value,parent)
            {
                m_AttributeAccessor = new XmlAttributeAccessor(attributes);
            }

            /// public ITMXmlElement this[string childName]
            /// {
            ///     get
            ///     {
            ///         ITMXmlElement res = _FindInOffspring(childName, this) as XmlElement;
            ///         if (null != res)
            ///             return res;
            ///         else
            ///             return NullElement;
            ///     }
            /// }
            /// 

            public ITMXmlElement[] this[string childName]
            {
                get
                {
                    ITMXmlElement[] res;
                    List<XmlNode> children = FrameStackList<XmlNode>.Acquire();
                    if (_FindAllChildren(childName, children))
                    {
                        List<ITMXmlElement> element = FrameStackList<ITMXmlElement>.Acquire();
                        for (int i = 0, icnt = children.Count; i < icnt; ++i)
                            element.Add(children[i] as XmlElement);
                        res = element.ToArray();
                        FrameStackList<ITMXmlElement>.Recycle(element);
                    }
                    else
                        res = new ITMXmlElement[] { NullElement };
                    FrameStackList<XmlNode>.Recycle(children);

                    return res;
                }
            }

            public ITMXmlElement NextSibling
            {
                get
                {
                    ITMXmlElement element = _NextSibling as XmlElement;
                    return null == element ? NullElement : element;
                }
            }

            public ITMXmlElement PrevSibling
            {
                get
                {
                    ITMXmlElement element = _PrevSibling as XmlElement;
                    return null == element ? NullElement : element;
                }
            }

            public ITMXmlAttributeAccessor Attribute
            {
                get { return m_AttributeAccessor; }
            }

            public void Clear()
            {
                m_AttributeAccessor.Clear();
                _ClearChildren();
            }
        }

        private class XmlAttribute : XmlBase, ITMXmlAttribute
        {
            private readonly XmlElement m_OwnerElement;

            public XmlAttribute(string name, string value,XmlElement owner)
                : base(name,value)
            {
                Debugger.Assert(null != owner, "Owner element can not null!");

                m_OwnerElement = owner;
            }

            ITMXmlValue ITMXmlAttribute.Value
            {
                get { return new XmlValue(m_Value); }
            }
        }

        public static readonly ITMXmlElement NullElement = new XmlElement("null", "", null, new List<XmlAttribute>());
        public static readonly ITMXmlAttribute NullAttribute = new XmlAttribute("null", "", NullElement as XmlElement);

        private readonly XmlElement m_Root;

        public XmlParser()
        {
            m_Root = new XmlElement("parser_root", "root", null, new List<XmlAttribute>());
        }

        public bool Parse(Stream stream)
        {
            if (null != stream)
            {
                m_Root.Clear();
                Stack<XmlElement> elementStack = new Stack<XmlElement>();
                elementStack.Push(m_Root);
                using (System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stream))
                {
                    if (null != xmlReader)
                    {
                        /// xmlReader.MoveToContent();
                        try
                        {
                            while (xmlReader.Read())
                            {
                                switch (xmlReader.NodeType)
                                {
                                    case System.Xml.XmlNodeType.Element:
                                        {/// <Element>
                                            string name = xmlReader.Name;

                                            List<KeyValuePair<string, string>> attributeList = FrameStackList<KeyValuePair<string, string>>.Acquire();
                                            for (int attInd = 0; attInd < xmlReader.AttributeCount; attInd++)
                                            {
                                                xmlReader.MoveToAttribute(attInd);
                                                string key = xmlReader.Name;
                                                xmlReader.ReadAttributeValue();
                                                attributeList.Add(new KeyValuePair<string, string>(key, xmlReader.Value));
                                            }

                                            xmlReader.Read();
                                            string value = string.Empty;
                                            if (System.Xml.XmlNodeType.Text == xmlReader.NodeType)
                                                value = xmlReader.Value;

                                            List<XmlAttribute> attributes = new List<XmlAttribute>();
                                            XmlElement element = new XmlElement(name, value, elementStack.Peek(), attributes);
                                            for (int attInd = 0; attInd < attributeList.Count; attInd++)
                                            {
                                                KeyValuePair<string, string> pair = attributeList[attInd];
                                                attributes.Add(new XmlAttribute(pair.Key, pair.Value, element));
                                            }
                                            FrameStackList<KeyValuePair<string, string>>.Recycle(attributeList);

                                            elementStack.Push(element);
                                        }
                                        break;
                                    case System.Xml.XmlNodeType.EndElement:
                                        {/// </Element>
                                            elementStack.Pop();
                                        }
                                        break;
                                }
                            }

                            return true;
                        }
                        catch (System.Exception e)
                        {
                            Debugger.LogException("Parse xml exception:{0}",e);
                            return false;
                        }
                    }
                    else
                        Debugger.LogWarning("Read xml document has failed!");
                }
            }

            return false;
        }

        public ITMXmlElement[] this[string name]
        {
            get { return m_Root[name]; }
        }       
    }
}