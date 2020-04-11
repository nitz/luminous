namespace System
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Luminous;

    /// <summary>Extension methods for the String class.</summary>
    public static class StringExtensions
    {
        public static string Coalesce(this string @this, string other)
        {
            return !string.IsNullOrEmpty(@this) ? @this : other;
        }
        public static string Coalesce(this string @this, params string[] values)
        {
            if (!string.IsNullOrEmpty(@this) || values == null || values.Length == 0) return @this;
            for (int i = 0; i < values.Length; i++)
            {
                string value = values[i];
                if (!string.IsNullOrEmpty(value)) return value;
            }
            return null;
        }

        public static string Coalesce(this string @this, IEnumerable<string> values)
        {
            if (!string.IsNullOrEmpty(@this) || values == null) return @this;
            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value)) return value;
            }
            return null;
        }
        public static string CoalesceWithEmpty(this string @this, params string[] values)
        {

            return @this.Coalesce(values) ?? string.Empty;
        }

        public static string CoalesceWithEmpty(this string @this, IEnumerable<string> values)
        {

            return @this.Coalesce(values) ?? string.Empty;
        }
        public static string ToStringOrNull<T>(this T @this)
            where T : class
        {
            if (@this == null) return null;
            return @this.ToString();
        }
        public static string ToStringOrNull<T>(this T @this, string format, IFormatProvider formatProvider = null)
            where T : class, IFormattable
        {
            if (@this == null) return null;
            return @this.ToString(format, formatProvider);
        }
        public static string ToStringOrNull<T>(this T? @this)
            where T : struct
        {
            if (!@this.HasValue) return null;
            return @this.Value.ToString();
        }
        public static string ToStringOrNull<T>(this T? @this, string format, IFormatProvider formatProvider = null)
            where T : struct, IFormattable
        {
            if (!@this.HasValue) return null;
            return @this.Value.ToString(format, formatProvider);
        }
        public static string SubstringTo(this string @this, int startIndex, int endIndex)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }
            if (!(startIndex >= 0))
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Contract assertion not met: startIndex >= 0");
            }
            if (!(startIndex <= endIndex || startIndex == endIndex + 1))
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Contract assertion not met: startIndex <= endIndex || startIndex == endIndex + 1");
            }
            if (!(endIndex < @this.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex), "Contract assertion not met: endIndex < @this.Length");
            }

            return @this.Substring(startIndex, endIndex - startIndex + 1);
        }
        public static string Clip(this string @this, int startClipLength, int endClipLength)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }
            if (!(startClipLength >= 0))
            {
                throw new ArgumentOutOfRangeException(nameof(startClipLength), "Contract assertion not met: startClipLength >= 0");
            }
            if (!(endClipLength >= 0))
            {
                throw new ArgumentOutOfRangeException(nameof(endClipLength), "Contract assertion not met: endClipLength >= 0");
            }
            if (!(@this.Length - startClipLength - endClipLength >= 0))
            {
                throw new ArgumentOutOfRangeException(nameof(@this), "Contract assertion not met: @this.Length - startClipLength - endClipLength >= 0");
            }

            return @this.Substring(startClipLength, @this.Length - startClipLength - endClipLength);
        }
        public static string ToXmlEncodedString(this string @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }

            var sb = new StringBuilder();
            using (var xw = XmlWriter.Create(sb, new XmlWriterSettings
            {
                CheckCharacters = false,
                OmitXmlDeclaration = true,
            }))
            {
                new XElement("_", @this).Save(xw);
            }
            return sb.ToString().Clip(3, 4);
        }
        public static string ToXmlDecodedString(this string @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }

            using (var xw = XmlReader.Create(new StringReader(string.Format("<_>{0}</_>", @this)), new XmlReaderSettings
            {
                CheckCharacters = false,
            }))
            {
                XDocument xd = XDocument.Load(xw);
                XElement el = null;
                if (xd == null || (el = xd.Element("_")) == null)
                    throw new ArgumentException("The specified string is not a valid XML encoded string.");
                return el.Value;
            }
        }

        public static string Indent(this string @this, int width = 4, char indentChar = ' ')
        {
            if (!(width >= 0))
            {
                throw new ArgumentException("Contract assertion not met: width >= 0", nameof(width));
            }

            return string.Join(
                Environment.NewLine,
                (from line in @this.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                 let l = string.IsNullOrEmpty(line)
                         ? string.Empty
                         : new string(indentChar, width) + line
                 select l
                ).ToArray()
            );
        }

        public static Either<string, FormatToken>[] FormatTokenSplit(this string @this)
        {
            if (@this == null) return null;

            var tokens = new List<Either<string, FormatToken>>();
            if (@this.Length == 0) return tokens.ToArray();

            var stack = new Stack<char>(@this.ToCharArray().Reverse());
            StringBuilder sb = new StringBuilder();
            bool isToken = false;

            while (stack.Count > 0)
            {
                if (stack.Peek() == '{')
                {
                    if (isToken)
                    {
                        throw new FormatException("Token inside other token is illegal.");
                    }
                    stack.Pop();
                    // double ‘{’?
                    if (stack.Count > 0 && stack.Peek() == '{')
                    {
                        //  ‘{’ char encoded
                        stack.Pop();
                        sb.Append('{');
                        continue;
                    }
                    // next char is not ‘{’—next token started?
                    if (stack.Count == 0)
                    {
                        throw new FormatException("Closing parenthesis is missing.");
                    }
                    if (sb.Length > 0)
                    {
                        tokens.Add(sb.ToString());
                        sb.Length = 0;
                    }
                    isToken = true;
                    continue;
                }
                else if (stack.Peek() == '}')
                {
                    stack.Pop();
                    if (isToken)
                    {
                        tokens.Add(new FormatToken(sb.ToString()));
                        sb.Length = 0;
                        isToken = false;
                        continue;
                    }
                    // double ‘}’?
                    if (stack.Count > 0 && stack.Peek() == '}')
                    {
                        //  ‘}’ char encoded
                        stack.Pop();
                        sb.Append('}');
                        continue;
                    }
                    throw new FormatException("Opening parenthesis is missing.");
                }
                else sb.Append(stack.Pop());
            }
            if (isToken)
            {
                throw new FormatException("Closing parenthesis is missing.");
            }
            if (sb.Length > 0)
            {
                tokens.Add(sb.ToString());
            }
            return tokens.ToArray();
        }

        public static string Format2(this string @this, params object[] args)
        {
            if (@this == null) throw new NullReferenceException();
            if (args == null) throw new ArgumentNullException("args");

            if (args.Length == 0) return @this;

            var parts = FormatTokenSplit(@this);
            for (int index = 0; index < parts.Length; index++)
            {
                var part = parts[index];
                if (part.IsSecond)
                {
                    var formatToken = part.Second.Value;
                    int i = formatToken.IndexOfAny(",:".ToCharArray());
                    if (i == -1)
                    {
                        i = int.Parse(formatToken, NumberFormatInfo.InvariantInfo);
                        if (i < args.Length)
                        {
                            parts[index] = args[i].ToStringOrNull();
                        }
                        else
                        {
                            parts[index] = new FormatToken((i - args.Length).ToString());
                        }
                    }
                    else
                    {
                        int j = int.Parse(formatToken.Substring(0, i), NumberFormatInfo.InvariantInfo);
                        if (j < args.Length)
                        {
                            parts[index] = string.Format("{0" + formatToken.Substring(i) + "}", args[j]);
                        }
                        else
                        {
                            parts[index] = new FormatToken((j - args.Length) + formatToken.Substring(i));
                        }
                    }
                }
            }
            bool containsTokens = parts.Any(e => e.IsSecond);
            var sb = new StringBuilder(@this.Length);
            for (int index = 0; index < parts.Length; index++)
            {
                var part = parts[index];
                if (part.IsFirst)
                {
                    if (containsTokens)
                    {
                        sb.Append(part.First.Replace("{", "{{").Replace("}", "}}"));
                    }
                    else
                    {
                        sb.Append(part.First);
                    }
                }
                else
                {
                    var formatToken = part.Second.Value;
                    sb.Append('{')
                      .Append(formatToken)
                      .Append('}');
                }
            }
            return sb.ToString();
        }

        public static SecureString ToSecureString(this string @this)
        {
            return ToSecureString(@this, true);
        }

        public static SecureString ToSecureString(this string @this, bool makeReadOnly)
        {
            var ss = new SecureString();
            @this.ToCharArray().ToList().ForEach(c => ss.AppendChar(c));
            if (makeReadOnly)
            {
                ss.MakeReadOnly();
            }
            return ss;
        }

        private static byte[] salt = new byte[] { 0xfe, 0x71, 0x35, 0x20, 0x11, 0x12, 0x05, 0x09, 0x13, 0x7c, 0x3a, 0x52, 0xac };
        public static string ToHash(SecureString @this)
        {
            byte[] ssBytes;
            IntPtr bstr = IntPtr.Zero;
            try
            {
                ssBytes = new byte[@this.Length * 2];
                Marshal.Copy((bstr = Marshal.SecureStringToBSTR(@this)),
                ssBytes, 0, ssBytes.Length);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstr);
            }
            Rfc2898DeriveBytes k = new Rfc2898DeriveBytes(ssBytes, salt, 1024);
            using (SHA512Managed sha = new SHA512Managed())
            {
                byte[] hashed = sha.ComputeHash(k.GetBytes(256));
                sha.Clear();
                return Convert.ToBase64String(hashed);
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("{{{Value}}}")]
    public struct FormatToken
    {
        public readonly string Value;
        public FormatToken(string value)
        {
            Value = value;
        }
    }
}

