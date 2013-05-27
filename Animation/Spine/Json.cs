#region File Description
//-----------------------------------------------------------------------------
// Json
//
// Copyright (c) 2012 Calvin Rien
//
// Based on the JSON parser by Patrick van Bergen
// http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
//
// Simplified it so that it doesn't throw exceptions
// and can be used in Unity iPhone with maximum code stripping.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
#endregion

namespace WaveEngine.Components.Animation.Spine
{
    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    /// JSON uses Arrays and objects. These correspond here to the datatypes IList and IDictionary.
    /// All numbers are parsed to floats.
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a float, an integer,a string, null, true, or false</returns>
        public static object Deserialize(TextReader json)
        {
            if (json == null)
            {
                return null;
            }

            return Parser.Parse(json);
        }

        /// <summary>
        /// Parser class.
        /// </summary>
        private sealed class Parser : IDisposable
        {
            /// <summary>
            /// The WHITE_SPACE
            /// </summary>
            private static string whiteSpace = " \t\n\r";

            /// <summary>
            /// The WORD_BREAK
            /// </summary>
            private static string wordBreak = " \t\n\r{}[],:\"";

            /// <summary>
            /// Token enum.
            /// </summary>
            public enum TOKEN
            {
                /// <summary>
                /// The NONE
                /// </summary>
                NONE,

                /// <summary>
                /// The CURL y_ OPEN
                /// </summary>
                CURLY_OPEN,

                /// <summary>
                /// The CURL y_ CLOSE
                /// </summary>
                CURLY_CLOSE,

                /// <summary>
                /// The SQUARE d_ OPEN
                /// </summary>
                SQUARED_OPEN,

                /// <summary>
                /// The SQUARE d_ CLOSE
                /// </summary>
                SQUARED_CLOSE,

                /// <summary>
                /// The COLON
                /// </summary>
                COLON,

                /// <summary>
                /// The COMMA
                /// </summary>
                COMMA,

                /// <summary>
                /// The string
                /// </summary>
                String,

                /// <summary>
                /// The NUMBER
                /// </summary>
                NUMBER,

                /// <summary>
                /// The TRUE
                /// </summary>
                TRUE,

                /// <summary>
                /// The FALSE
                /// </summary>
                FALSE,

                /// <summary>
                /// The NULL
                /// </summary>
                NULL
            }

            /// <summary>
            /// The json
            /// </summary>
            private TextReader json;

            /// <summary>
            /// Initializes a new instance of the <see cref="Parser" /> class.
            /// </summary>
            /// <param name="reader">The reader.</param>
            public Parser(TextReader reader)
            {
                this.json = reader;
            }

            /// <summary>
            /// Parses the specified reader.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <returns>return parse value.</returns>
            public static object Parse(TextReader reader)
            {
                using (var instance = new Parser(reader))
                {
                    return instance.ParseValue();
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.json.Dispose();
                this.json = null;
            }

            /// <summary>
            /// Parses the object.
            /// </summary>
            /// <returns>Return string, string dictionary.</returns>
            private Dictionary<string, object> Parseobject()
            {
                Dictionary<string, object> table = new Dictionary<string, object>();

                // ditch opening brace
                this.json.Read();

                // {
                while (true)
                {
                    switch (this.NextToken)
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.CURLY_CLOSE:
                            return table;
                        default:
                            // name
                            string name = this.Parsestring();
                            if (name == null)
                            {
                                return null;
                            }

                            // :
                            if (this.NextToken != TOKEN.COLON)
                            {
                                return null;
                            }

                            // ditch the colon
                            this.json.Read();

                            // value
                            table[name] = this.ParseValue();
                            break;
                    }
                }
            }

            /// <summary>
            /// Parses the array.
            /// </summary>
            /// <returns>Return list.</returns>
            private List<object> ParseArray()
            {
                List<object> array = new List<object>();

                // ditch opening bracket
                this.json.Read();

                // [
                var parsing = true;
                while (parsing)
                {
                    TOKEN nextToken = this.NextToken;

                    switch (nextToken)
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.SQUARED_CLOSE:
                            parsing = false;
                            break;
                        default:
                            object value = this.ParseByToken(nextToken);

                            array.Add(value);
                            break;
                    }
                }

                return array;
            }

            /// <summary>
            /// Parses the value.
            /// </summary>
            /// <returns>Return value as object.</returns>
            private object ParseValue()
            {
                TOKEN nextToken = this.NextToken;
                return this.ParseByToken(nextToken);
            }

            /// <summary>
            /// Parses the by token.
            /// </summary>
            /// <param name="token">The token.</param>
            /// <returns>Return value as object.</returns>
            private object ParseByToken(TOKEN token)
            {
                switch (token)
                {
                    case TOKEN.String:
                        return this.Parsestring();
                    case TOKEN.NUMBER:
                        return this.ParseNumber();
                    case TOKEN.CURLY_OPEN:
                        return this.Parseobject();
                    case TOKEN.SQUARED_OPEN:
                        return this.ParseArray();
                    case TOKEN.TRUE:
                        return true;
                    case TOKEN.FALSE:
                        return false;
                    case TOKEN.NULL:
                        return null;
                    default:
                        return null;
                }
            }

            /// <summary>
            /// Parses the string.
            /// </summary>
            /// <returns>Return string.</returns>
            private string Parsestring()
            {
                StringBuilder s = new StringBuilder();
                char c;

                // ditch opening quote
                this.json.Read();

                bool parsing = true;
                while (parsing)
                {
                    if (this.json.Peek() == -1)
                    {
                        parsing = false;
                        break;
                    }

                    c = this.NextChar;
                    switch (c)
                    {
                        case '"':
                            parsing = false;
                            break;
                        case '\\':
                            if (this.json.Peek() == -1)
                            {
                                parsing = false;
                                break;
                            }

                            c = this.NextChar;
                            switch (c)
                            {
                                case '"':
                                case '\\':
                                case '/':
                                    s.Append(c);
                                    break;
                                case 'b':
                                    s.Append('\b');
                                    break;
                                case 'f':
                                    s.Append('\f');
                                    break;
                                case 'n':
                                    s.Append('\n');
                                    break;
                                case 'r':
                                    s.Append('\r');
                                    break;
                                case 't':
                                    s.Append('\t');
                                    break;
                                case 'u':
                                    var hex = new StringBuilder();

                                    for (int i = 0; i < 4; i++)
                                    {
                                        hex.Append(this.NextChar);
                                    }

                                    s.Append((char)Convert.ToInt32(hex.ToString(), 16));
                                    break;
                            }

                            break;
                        default:
                            s.Append(c);
                            break;
                    }
                }

                return s.ToString();
            }

            /// <summary>
            /// Parses the number.
            /// </summary>
            /// <returns>Return number as object.</returns>
            private object ParseNumber()
            {
                string number = this.NextWord;
                float parsedFloat;
                float.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedFloat);
                return parsedFloat;
            }

            /// <summary>
            /// Eats the whitespace.
            /// </summary>
            private void EatWhitespace()
            {
                while (whiteSpace.IndexOf(this.PeekChar) != -1)
                {
                    this.json.Read();

                    if (this.json.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            /// <summary>
            /// Gets the peek char.
            /// </summary>
            /// <value>
            /// The peek char.
            /// </value>
            private char PeekChar
            {
                get
                {
                    return Convert.ToChar(this.json.Peek());
                }
            }

            /// <summary>
            /// Gets the next char.
            /// </summary>
            /// <value>
            /// The next char.
            /// </value>
            private char NextChar
            {
                get
                {
                    return Convert.ToChar(this.json.Read());
                }
            }

            /// <summary>
            /// Gets the next word.
            /// </summary>
            /// <value>
            /// The next word.
            /// </value>
            private string NextWord
            {
                get
                {
                    StringBuilder word = new StringBuilder();

                    while (wordBreak.IndexOf(this.PeekChar) == -1)
                    {
                        word.Append(this.NextChar);

                        if (this.json.Peek() == -1)
                        {
                            break;
                        }
                    }

                    return word.ToString();
                }
            }

            /// <summary>
            /// Gets the next token.
            /// </summary>
            /// <value>
            /// The next token.
            /// </value>
            private TOKEN NextToken
            {
                get
                {
                    this.EatWhitespace();

                    if (this.json.Peek() == -1)
                    {
                        return TOKEN.NONE;
                    }

                    char c = this.PeekChar;
                    switch (c)
                    {
                        case '{':
                            return TOKEN.CURLY_OPEN;
                        case '}':
                            this.json.Read();
                            return TOKEN.CURLY_CLOSE;
                        case '[':
                            return TOKEN.SQUARED_OPEN;
                        case ']':
                            this.json.Read();
                            return TOKEN.SQUARED_CLOSE;
                        case ',':
                            this.json.Read();
                            return TOKEN.COMMA;
                        case '"':
                            return TOKEN.String;
                        case ':':
                            return TOKEN.COLON;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '-':
                            return TOKEN.NUMBER;
                    }

                    string word = this.NextWord;

                    switch (word)
                    {
                        case "false":
                            return TOKEN.FALSE;
                        case "true":
                            return TOKEN.TRUE;
                        case "null":
                            return TOKEN.NULL;
                    }

                    return TOKEN.NONE;
                }
            }
        }

        /// <summary>
        /// Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A JSON encoded string, or null if object 'json' is not serializable
        /// </returns>
        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        /// <summary>
        /// Serializer class
        /// </summary>
        private sealed class Serializer
        {
            /// <summary>
            /// The builder
            /// </summary>
            private StringBuilder builder;

            /// <summary>
            /// Initializes a new instance of the <see cref="Serializer" /> class.
            /// </summary>
            public Serializer()
            {
                this.builder = new StringBuilder();
            }

            /// <summary>
            /// Serializes the specified obj.
            /// </summary>
            /// <param name="obj">The obj.</param>
            /// <returns>Return string.</returns>
            public static string Serialize(object obj)
            {
                var instance = new Serializer();

                instance.SerializeValue(obj);

                return instance.builder.ToString();
            }

            /// <summary>
            /// Serializes the value.
            /// </summary>
            /// <param name="value">The value.</param>
            private void SerializeValue(object value)
            {
                IList asList;
                IDictionary asDict;
                string asStr;

                if (value == null)
                {
                    this.builder.Append("null");
                }
                else if ((asStr = value as string) != null)
                {
                    this.Serializestring(asStr);
                }
                else if (value is bool)
                {
                    this.builder.Append(value.ToString().ToLower());
                }
                else if ((asList = value as IList) != null)
                {
                    this.SerializeArray(asList);
                }
                else if ((asDict = value as IDictionary) != null)
                {
                    this.Serializeobject(asDict);
                }
                else if (value is char)
                {
                    this.Serializestring(value.ToString());
                }
                else
                {
                    this.SerializeOther(value);
                }
            }

            /// <summary>
            /// Serializes the object.
            /// </summary>
            /// <param name="obj">The obj.</param>
            private void Serializeobject(IDictionary obj)
            {
                bool first = true;

                this.builder.Append('{');

                foreach (object e in obj.Keys)
                {
                    if (!first)
                    {
                        this.builder.Append(',');
                    }

                    this.Serializestring(e.ToString());
                    this.builder.Append(':');

                    this.SerializeValue(obj[e]);

                    first = false;
                }

                this.builder.Append('}');
            }

            /// <summary>
            /// Serializes the array.
            /// </summary>
            /// <param name="anArray">An array.</param>
            private void SerializeArray(IList anArray)
            {
                this.builder.Append('[');

                bool first = true;

                foreach (object obj in anArray)
                {
                    if (!first)
                    {
                        this.builder.Append(',');
                    }

                    this.SerializeValue(obj);

                    first = false;
                }

                this.builder.Append(']');
            }

            /// <summary>
            /// Serializes the string.
            /// </summary>
            /// <param name="str">The STR.</param>
            private void Serializestring(string str)
            {
                this.builder.Append('\"');

                char[] charArray = str.ToCharArray();
                foreach (var c in charArray)
                {
                    switch (c)
                    {
                        case '"':
                            this.builder.Append("\\\"");
                            break;
                        case '\\':
                            this.builder.Append("\\\\");
                            break;
                        case '\b':
                            this.builder.Append("\\b");
                            break;
                        case '\f':
                            this.builder.Append("\\f");
                            break;
                        case '\n':
                            this.builder.Append("\\n");
                            break;
                        case '\r':
                            this.builder.Append("\\r");
                            break;
                        case '\t':
                            this.builder.Append("\\t");
                            break;
                        default:
                            int codepoint = Convert.ToInt32(c);
                            if ((codepoint >= 32) && (codepoint <= 126))
                            {
                                this.builder.Append(c);
                            }
                            else
                            {
                                builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                            }

                            break;
                    }
                }

                this.builder.Append('\"');
            }

            /// <summary>
            /// Serializes the other.
            /// </summary>
            /// <param name="value">The value.</param>
            private void SerializeOther(object value)
            {
                if (value is float
                    || value is int
                    || value is uint
                    || value is long
                          || value is float
                    || value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is ulong
                    || value is decimal)
                {
                    this.builder.Append(value.ToString());
                }
                else
                {
                    this.Serializestring(value.ToString());
                }
            }
        }
    }
}