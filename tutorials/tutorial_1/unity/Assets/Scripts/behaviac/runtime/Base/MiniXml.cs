/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.IO;
using System.Security;

namespace MiniXml
{
    internal class MiniXmlParser
    {
        public interface IXmlHandler
        {
            void OnStartParsing(MiniXmlParser parser);

            void OnEndParsing(MiniXmlParser parser);

            void OnStart(string szName, IAttributeList attrsList);

            void OnEnd(string szName);

            void OnProcess(string szName, string content);

            void OnChars(string content);

            void OnIgnorableSpaces(string content);
        }

        public interface IAttributeList
        {
            int AttrLength
            {
                get;
            }
            bool IsEmpty
            {
                get;
            }

            string GetAttrName(int i);

            string GetAttrValue(int i);

            string GetAttrValue(string name);

            string[] Names
            {
                get;
            }
            string[] Values
            {
                get;
            }
        }

        private class AttributeListImpl : IAttributeList
        {
            public int AttrLength
            {
                get
                {
                    return attrNames.Count;
                }
            }

            public bool IsEmpty
            {
                get
                {
                    return attrNames.Count == 0;
                }
            }

            public string GetAttrName(int i)
            {
                return (string)attrNames[i];
            }

            public string GetAttrValue(int i)
            {
                return (string)attrValues[i];
            }

            public string GetAttrValue(string name)
            {
                for (int i = 0; i < attrNames.Count; i++)
                    if ((string)attrNames[i] == name)
                    {
                        return (string)attrValues[i];
                    }

                return null;
            }

            public string[] Names
            {
                get
                {
                    return (string[])attrNames.ToArray(typeof(string));
                }
            }

            public string[] Values
            {
                get
                {
                    return (string[])attrValues.ToArray(typeof(string));
                }
            }

            private ArrayList attrNames = new ArrayList();
            private ArrayList attrValues = new ArrayList();

            internal void Clear()
            {
                attrNames.Clear();
                attrValues.Clear();
            }

            internal void Add(string name, string value)
            {
                attrNames.Add(name);
                attrValues.Add(value);
            }
        }

        private IXmlHandler xmlHandler_;
        private TextReader textReader_;
        private Stack elementNames_ = new Stack();
        private Stack xmlSpaces_ = new Stack();
        private string xmlBlank_;
        private StringBuilder stringBuffer_ = new StringBuilder(200);
        private char[] nameBufferArray_ = new char[30];
        private bool isBlank_;

        private AttributeListImpl _attributes = new AttributeListImpl();
        private int line = 1, column;
        private bool resetCol_;

        public MiniXmlParser()
        {
        }

        private static bool IsUnicodeSep(char c, bool start)
        {

            switch (Char.GetUnicodeCategory(c))
            {
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.LetterNumber:
                    return true;

                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.EnclosingMark:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    return !start;

                default:
                    return false;
            }
        }

        private Exception ErrorMsg_(string msg)
        {
            return new MiniXmlParserException(msg, line, column);
        }

        private Exception UnexpectedEndException_()
        {
            string[] arr = new string[elementNames_.Count];
            (elementNames_ as ICollection).CopyTo(arr, 0);
            return ErrorMsg_(String.Format(
                                 "Unexpected end of stream. Element stack content is {0}", String.Join(",", arr)));
        }

        private bool IsNameChararcter_(char c, bool start)
        {
            switch (c)
            {
                case ':':
                case '_':
                    return true;

                case '-':
                case '.':
                    return !start;
            }

            if (c > 0x100)
            {
                switch (c)
                {
                    case '\u0559':
                    case '\u06E5':
                    case '\u06E6':
                        return true;
                }

                if ('\u02BB' <= c && c <= '\u02C1')
                {
                    return true;
                }
            }

            return IsUnicodeSep(c, start);
        }

        private bool IsBlank_(int c)
        {
            switch (c)
            {
                case ' ':
                case '\r':
                case '\t':
                case '\n':
                    return true;

                default:
                    return false;
            }
        }

        public void SkipBlank()
        {
            SkipBlanks_(false);
        }

        public void SkipBlanks_(bool expected)
        {
            while (true)
            {
                switch (PeekNext_())
                {
                    case ' ':
                    case '\r':
                    case '\t':
                    case '\n':
                        ReadNext_();

                        if (expected)
                        {
                            expected = false;
                        }

                        continue;
                }

                if (expected)
                {
                    throw ErrorMsg_("Whitespace is expected.");
                }

                return;
            }
        }

        private void HandleBlank_()
        {
            while (IsBlank_(PeekNext_()))
            {
                stringBuffer_.Append((char)ReadNext_());
            }

            if (PeekNext_() != '<' && PeekNext_() >= 0)
            {
                isBlank_ = false;
            }
        }


        private int ReadNext_()
        {
            int i = textReader_.Read();

            if (i == '\n')
            {
                resetCol_ = true;
            }

            if (resetCol_)
            {
                line++;
                resetCol_ = false;
                column = 1;
            }
            else
            {
                column++;
            }

            return i;
        }

        private int PeekNext_()
        {
            return textReader_.Peek();
        }

        public void Expect(int c)
        {
            int p = ReadNext_();

            if (p < 0)
            {
                throw UnexpectedEndException_();
            }

            else if (p != c)
            {
                throw ErrorMsg_(String.Format("Expected '{0}' but got {1}", (char)c, (char)p));
            }
        }

        public string ReadName()
        {
            int idx = 0;

            if (PeekNext_() < 0 || !IsNameChararcter_((char)PeekNext_(), true))
            {
                throw ErrorMsg_("XML name start character is expected.");
            }

            for (int i = PeekNext_(); i >= 0; i = PeekNext_())
            {
                char c = (char)i;

                if (!IsNameChararcter_(c, false))
                {
                    break;
                }

                if (idx == nameBufferArray_.Length)
                {
                    char[] tmp = new char[idx * 2];
                    Array.Copy(nameBufferArray_, 0, tmp, 0, idx);
                    nameBufferArray_ = tmp;
                }

                nameBufferArray_[idx++] = c;
                ReadNext_();
            }

            if (idx == 0)
            {
                throw ErrorMsg_("Valid XML name is expected.");
            }

            return new string(nameBufferArray_, 0, idx);
        }

        public void Parse(TextReader input, IXmlHandler handler)
        {
            this.textReader_ = input;
            this.xmlHandler_ = handler;

            handler.OnStartParsing(this);

            while (PeekNext_() >= 0)
            {
                ReadAll();
            }

            HandleBufferContent_();

            if (elementNames_.Count > 0)
            {
                throw ErrorMsg_(String.Format("Insufficient close tag: {0}", elementNames_.Peek()));
            }

            handler.OnEndParsing(this);

            Cleanup();
        }

        private string ReadUntil_(char until, bool handleReferences)
        {
            while (true)
            {
                if (PeekNext_() < 0)
                {
                    throw UnexpectedEndException_();
                }

                char c = (char)ReadNext_();

                if (c == until)
                {
                    break;
                }

                else if (handleReferences && c == '&')
                {
                    ReadRef_();
                }

                else
                {
                    stringBuffer_.Append(c);
                }
            }

            string ret = stringBuffer_.ToString();
            stringBuffer_.Length = 0;
            return ret;
        }

        private void Cleanup()
        {
            line = 1;
            column = 0;
            xmlHandler_ = null;
            textReader_ = null;

            elementNames_.Clear();
            xmlSpaces_.Clear();

            _attributes.Clear();
            stringBuffer_.Length = 0;
            xmlBlank_ = null;
            isBlank_ = false;
        }

        public void ReadAll()
        {
            string name;

            if (IsBlank_(PeekNext_()))
            {
                if (stringBuffer_.Length == 0)
                {
                    isBlank_ = true;
                }

                HandleBlank_();
            }

            if (PeekNext_() == '<')
            {
                ReadNext_();

                switch (PeekNext_())
                {
                    case '!':
                        ReadNext_();

                        if (PeekNext_() == '[')
                        {
                            ReadNext_();

                            if (ReadName() != "CDATA")
                            {
                                throw ErrorMsg_("Invalid declaration markup");
                            }

                            Expect('[');
                            ReadCDATA_();
                            return;
                        }
                        else if (PeekNext_() == '-')
                        {
                            ReadComment_();
                            return;
                        }
                        else if (ReadName() != "DOCTYPE")
                        {
                            throw ErrorMsg_("Invalid declaration markup.");
                        }

                        else
                        {
                            throw ErrorMsg_("This parser does not support document type.");
                        }

                    case '?':
                        HandleBufferContent_();
                        ReadNext_();
                        name = ReadName();
                        SkipBlank();
                        string text = String.Empty;

                        if (PeekNext_() != '?')
                        {
                            while (true)
                            {
                                text += ReadUntil_('?', false);

                                if (PeekNext_() == '>')
                                {
                                    break;
                                }

                                text += "?";
                            }
                        }

                        xmlHandler_.OnProcess(
                            name, text);
                        Expect('>');
                        return;

                    case '/':
                        HandleBufferContent_();

                        if (elementNames_.Count == 0)
                        {
                            throw UnexpectedEndException_();
                        }

                        ReadNext_();
                        name = ReadName();
                        SkipBlank();
                        string expected = (string)elementNames_.Pop();
                        xmlSpaces_.Pop();

                        if (xmlSpaces_.Count > 0)
                        {
                            xmlBlank_ = (string)xmlSpaces_.Peek();
                        }

                        else
                        {
                            xmlBlank_ = null;
                        }

                        if (name != expected)
                        {
                            throw ErrorMsg_(String.Format("End tag mismatch: expected {0} but found {1}", expected, name));
                        }

                        xmlHandler_.OnEnd(name);
                        Expect('>');
                        return;

                    default:
                        HandleBufferContent_();
                        name = ReadName();

                        while (PeekNext_() != '>' && PeekNext_() != '/')
                        {
                            ReadAttr_(_attributes);
                        }

                        xmlHandler_.OnStart(name, _attributes);
                        _attributes.Clear();
                        SkipBlank();

                        if (PeekNext_() == '/')
                        {
                            ReadNext_();
                            xmlHandler_.OnEnd(name);
                        }
                        else
                        {
                            elementNames_.Push(name);
                            xmlSpaces_.Push(xmlBlank_);
                        }

                        Expect('>');
                        return;
                }
            }
            else
            {
                ReadChars_();
            }
        }

        private void ReadChars_()
        {
            isBlank_ = false;

            while (true)
            {
                int i = PeekNext_();

                switch (i)
                {
                    case -1:
                        return;

                    case '<':
                        return;

                    case '&':
                        ReadNext_();
                        ReadRef_();
                        continue;

                    default:
                        stringBuffer_.Append((char)ReadNext_());
                        continue;
                }
            }
        }

        private void ReadRef_()
        {
            if (PeekNext_() == '#')
            {
                ReadNext_();
                ReadCharRef_();
            }
            else
            {
                string name = ReadName();
                Expect(';');

                switch (name)
                {
                    case "amp":
                        stringBuffer_.Append('&');
                        break;

                    case "quot":
                        stringBuffer_.Append('"');
                        break;

                    case "apos":
                        stringBuffer_.Append('\'');
                        break;

                    case "lt":
                        stringBuffer_.Append('<');
                        break;

                    case "gt":
                        stringBuffer_.Append('>');
                        break;

                    default:
                        throw ErrorMsg_("General non-predefined entity reference is not supported in this parser.");
                }
            }
        }

        private void HandleBufferContent_()
        {
            if (stringBuffer_.Length == 0)
            {
                return;
            }

            if (isBlank_)
            {
                xmlHandler_.OnIgnorableSpaces(stringBuffer_.ToString());
            }

            else
            {
                xmlHandler_.OnChars(stringBuffer_.ToString());
            }

            stringBuffer_.Length = 0;
            isBlank_ = false;
        }


        private int ReadCharRef_()
        {
            int n = 0;

            if (PeekNext_() == 'x')
            {
                ReadNext_();

                for (int i = PeekNext_(); i >= 0; i = PeekNext_())
                {
                    if ('0' <= i && i <= '9')
                    {
                        n = n << 4 + i - '0';
                    }

                    else if ('A' <= i && i <= 'F')
                    {
                        n = n << 4 + i - 'A' + 10;
                    }

                    else if ('a' <= i && i <= 'f')
                    {
                        n = n << 4 + i - 'a' + 10;
                    }

                    else
                    {
                        break;
                    }

                    ReadNext_();
                }
            }
            else
            {
                for (int i = PeekNext_(); i >= 0; i = PeekNext_())
                {
                    if ('0' <= i && i <= '9')
                    {
                        n = n << 4 + i - '0';
                    }

                    else
                    {
                        break;
                    }

                    ReadNext_();
                }
            }

            return n;
        }

        private void ReadAttr_(AttributeListImpl a)
        {
            SkipBlanks_(true);

            if (PeekNext_() == '/' || PeekNext_() == '>')
            {
                return;
            }

            string name = ReadName();
            string value;
            SkipBlank();
            Expect('=');
            SkipBlank();

            switch (ReadNext_())
            {
                case '\'':
                    value = ReadUntil_('\'', true);
                    break;

                case '"':
                    value = ReadUntil_('"', true);
                    break;

                default:
                    throw ErrorMsg_("Invalid attribute value markup.");
            }

            if (name == "xml:space")
            {
                xmlBlank_ = value;
            }

            a.Add(name, value);
        }

        private void ReadComment_()
        {
            Expect('-');
            Expect('-');

            while (true)
            {
                if (ReadNext_() != '-')
                {
                    continue;
                }

                if (ReadNext_() != '-')
                {
                    continue;
                }

                if (ReadNext_() != '>')
                {
                    throw ErrorMsg_("'--' is not allowed inside comment markup.");
                }

                break;
            }
        }


        private void ReadCDATA_()
        {
            int nBracket = 0;

            while (true)
            {
                if (PeekNext_() < 0)
                {
                    throw UnexpectedEndException_();
                }

                char c = (char)ReadNext_();

                if (c == ']')
                {
                    nBracket++;
                }

                else if (c == '>' && nBracket > 1)
                {
                    for (int i = nBracket; i > 2; i--)
                    {
                        stringBuffer_.Append(']');
                    }

                    break;
                }
                else
                {
                    for (int i = 0; i < nBracket; i++)
                    {
                        stringBuffer_.Append(']');
                    }

                    nBracket = 0;
                    stringBuffer_.Append(c);
                }
            }
        }

    }

    internal class SecurityParser : MiniXmlParser, MiniXmlParser.IXmlHandler
    {
        private SecurityElement root;

        public SecurityParser()
        : base()
        {
            stack = new Stack();
        }

        public void LoadXml(string xml)
        {
            root = null;
            stack.Clear();

            Parse(new StringReader(xml), this);
        }

        public SecurityElement ToXml()
        {
            return root;
        }

        private SecurityElement current;
        private Stack stack;

        public void OnStartParsing(MiniXmlParser parser)
        {
        }

        public void OnProcess(string name, string text)
        {
        }

        public void OnIgnorableSpaces(string s)
        {
        }

        public void OnStart(string name, MiniXmlParser.IAttributeList attrs)
        {
            SecurityElement newel = new SecurityElement(name);

            if (root == null)
            {
                root = newel;
                current = newel;
            }
            else
            {
                SecurityElement parent = (SecurityElement)stack.Peek();
                parent.AddChild(newel);
            }

            stack.Push(newel);
            current = newel;
            int n = attrs.AttrLength;

            for (int i = 0; i < n; i++)
            {
                string attrName = SecurityElement.Escape(attrs.GetAttrName(i));
                string attrValue = SecurityElement.Escape(attrs.GetAttrValue(i));
                current.AddAttribute(attrName, attrValue);
            }
        }

        public void OnEnd(string name)
        {
            current = (SecurityElement)stack.Pop();
        }

        public void OnChars(string ch)
        {
            current.Text = ch;
        }

        public void OnEndParsing(MiniXmlParser parser)
        {
        }
    }

    internal class MiniXmlParserException : SystemException
    {
        private int line;
        private int column;

        public MiniXmlParserException(string msg, int line, int column)
        : base(String.Format("{0}. At ({1},{2})", msg, line, column))
        {
            this.line = line;
            this.column = column;
        }

        public int Line
        {
            get
            {
                return line;
            }
        }

        public int Column
        {
            get
            {
                return column;
            }
        }
    }

}

