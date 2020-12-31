﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using JsonLD.GenericJson;

namespace JsonLD
{
    //namespace System { public class SerializableAttribute: Attribute { } }

    internal static class JavaCompat
    {
        public static string Substring(string str, int from, int to)
        {
            return str.Substring(from, to);
        }
        public static string Substring(string str, int from)
        {
            return str.Substring(from);
        }
        public static string[] Split(this string str, string delims)
        {
            return str.Split(delims.ToCharArray());
        }
        public static string ToHexString(this int i)
        {
            return Convert.ToString(i, 16);
        }
        public static void AppendCodePoint(this StringBuilder b, int cp)
        {
            b.Append((char)cp);
        }
        public static IEnumerable<KeyValuePair<TKey,TValue>> GetEnumerableSelf<TKey,TValue>(this IDictionary<TKey,TValue> dict)
        {
            return dict;
        }
        public static byte[] GetBytesForString(string str, string encoding)
        {
            switch (encoding) 
            {
                case "UTF-8": return UTF8Encoding.UTF8.GetBytes(str);
                default: throw new InvalidOperationException();
            }
        }
        public static bool ContainsKey(this GenericJsonObject obj, string key)
        {
            if (key == null)
            {
                return false;
            }
            return ((IDictionary<string, GenericJsonToken>)obj).ContainsKey(key);
        }

        public static bool IsNull(this GenericJsonToken token)
        {
            return token == null || token.Type == GenericJsonTokenType.Null;
        }

        public static bool SafeCompare<T>(this GenericJsonToken token, T val)
        {
            try
            {
                return token == null ? val == null : token.Value<T>().Equals(val);
            }
            catch
            {
                return false;
            }
        }
    }

    internal class DecimalFormat
    {
        string fmt;
        public DecimalFormat(string fmt)
        {
            this.fmt = "{0:" + fmt + "}";
        }

        public string Format(object obj)
        {
            return string.Format(fmt, obj);
        }
    }

    internal static class Collections
    {
        public static void AddAll<T>(ICollection<T> dest, ICollection<T> source)
        {
            foreach (var val in source)
            {
                dest.Add(val);
            }
        }

        //public static void AddAll<T>(ICollection<T> dest, ICollection source)
        //{
        //    foreach (var val in source)
        //    {
        //        dest.Add(source);
        //    }
        //}

        public static void AddAllObj(IList dest, ICollection source)
        {
            foreach (var val in source)
            {
                dest.Add(val);
            }
        }

        public static TValue Remove<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key)
        {
            var val = dict[key];
            dict.Remove(key);
            return val;
        }
        public static void Remove<T>(IList<T> list, T key)
        {
            list.Remove(key);
        }
        public static GenericJsonToken Remove(GenericJsonObject dict, string key)
        {
            var val = dict[key];
            dict.Remove(key);
            return val;
        }

        public static IEnumerable<string> GetKeys(this GenericJsonToken obj)
        {
            return ((GenericJsonObject)obj).Select(x => ((GenericJsonProperty)x).Name);
        }

        public static bool IsEmpty<T>(this ICollection<T> col)
        {
            return col.Count == 0;
        }

        public static bool IsEmpty(this GenericJsonObject obj)
        {
            return obj.IsEmpty();
        }

        public static void Reverse<T>(this IList<T> list)
        {
            if (list is List<T>)
            {
                ((List<T>)list).Reverse();
            }
            else if (list is GenericJsonArray)
            {
                // TODO(sblom): This is really awful; figure out how to really sort a GenericJsonArray in place.
                GenericJsonArray arr = (GenericJsonArray)list;
                // .Select(x => x) is a workaround for .NET 3.5's List constructor's failure to
                // disbelieve Newtonsoft.Json when IJCollection.Count returns 0.
                List<GenericJsonToken> tmp = arr.Select(x => x).ToList();
                tmp.Reverse();
                arr.Clear();
                foreach (var t in tmp)
                {
                    arr.Add(t);
                }
            }
            else
            {
                throw new InvalidOperationException("Attempted to .Reverse() an unsupported type.");
            }
        }

        class GenericJsonTokenStringCompare : Comparer<GenericJsonToken>
        {
            public override int Compare(GenericJsonToken x, GenericJsonToken y)
            {
                return string.Compare((string)x, (string)y);
            }
        }

        public static void SortInPlace<T>(this IList<T> list)
        {
            if (list is List<T>)
            {
                ((List<T>)list).Sort();
            }
            else if (list is GenericJsonArray)
            {
                // TODO(sblom): This is really awful; figure out how to really sort a GenericJsonArray in place.
                GenericJsonArray arr = (GenericJsonArray)list;
                // .Select(x => x) is a workaround for .NET 3.5's List constructor's failure to
                // disbelieve Newtonsoft.Json when IJCollection.Count returns 0.
                List<GenericJsonToken> tmp = arr.Select(x => x).ToList();
                tmp.Sort(new GenericJsonTokenStringCompare());
                arr.Clear();
                foreach (var t in tmp)
                {
                    arr.Add(t);
                }
            }
            else
            {
                throw new InvalidOperationException("Attempted to .Sort() an unsupported type.");
            }
        }

        public static void SortInPlace<T>(this IList<T> list, IComparer<T> cmp)
        {
            if (list is List<T>)
            {
                ((List<T>)list).Sort(cmp);
            }
            else if (list is GenericJsonArray)
            {
                // TODO(sblom): This is really awful; figure out how to really sort a GenericJsonArray in place.
                GenericJsonArray arr = (GenericJsonArray)list;
                IComparer<GenericJsonToken> comparer = (IComparer<GenericJsonToken>)cmp;
                // .Select(x => x) is a workaround for .NET 3.5's List constructor's failure to
                // disbelieve Newtonsoft.Json when IJCollection.Count returns 0.
                var tmp = arr.Select(x => x).ToList();
                tmp.Sort(comparer);
                tmp.Select((t, i) => arr[i] = tmp[i]);
            }
            else
            {
                throw new InvalidOperationException("Attempted to .Sort() an unsupported type.");
            }
        }

        //public static bool ContainsKey(this IDictionary dict, object key)
        //{
        //    return dict.Contains(key);
        //}
        public static int LastIndexOf<T>(this IList<T> list, T val) {
            if (list is List<T>)
            {
                return ((List<T>)list).LastIndexOf(val);
            }
            else
            {
                throw new InvalidOperationException("Attempted to .Sort() an unsupported type.");
            }
        }

        public static void PutAll(this IDictionary<string,GenericJsonToken> dest, IDictionary<string,string> src)
        {
            foreach (var entry in src)
            {
                dest.Add(entry.Key, entry.Value);
            }
        }
    }

    internal class Pattern: Regex
    {
        private string _rx;

        public Pattern(string rx): base(rx)
        {
            _rx = rx;
        }

        static public Pattern Compile(string rx)
        {
            return new Pattern(rx);
        }

        static public string Quote(string str)
        {
            return Regex.Escape(str);
        }

        public Matcher Matcher(string str)
        {
            return new Matcher(this, str);
        }

        public string GetPattern()
        {
            return _rx;
        }

        new public static bool Matches(string val, string rx)
        {
            return Regex.IsMatch(val, rx);
        }
        new public bool Matches(string val)
        {
            return IsMatch(val);
        }

        public MatchCollection GetMatches(string val)
        {
            return base.Matches(val);
        }
    }

    internal class Matcher
    {
        Pattern _pattern;
        string _str;

        MatchCollection matches;
        IEnumerator matchesEnumerator;

        public Matcher(Pattern pattern, string str)
        {
            _pattern = pattern;
            _str = str;
        }

        public string ReplaceAll(string rep)
        {
            return _pattern.Replace(_str, rep);
        }

        public bool Matches()
        {
            return _pattern.IsMatch(_str);
        }

        public string Group(int i)
        {
            if (matchesEnumerator == null)
            {
                this.Find();
            }
            var match = matchesEnumerator.Current as Match;
            return match.Groups[i].Success ? match.Groups[i].Value : null;
        }

        public int End()
        {
            var match = matchesEnumerator.Current as Match;
            return match.Index + match.Length;
        }

        public bool Find()
        {
            if (matches == null){
                matches = _pattern.GetMatches(_str);
                matchesEnumerator = matches.GetEnumerator();
            }

            return matchesEnumerator.MoveNext();            
        }
    }


    internal class MessageDigest : IDisposable
    {
        SHA1 md;
        Stream stream;

        public static MessageDigest GetInstance(string algorithm)
        {
            if (algorithm != "SHA-1") throw new ArgumentException();
            return new MessageDigest();
        }

        public MessageDigest()
        {
            md = SHA1.Create();
            stream = new MemoryStream();
        }

        public void Update(byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        public byte[] Digest()
        {
            stream.Seek(0, SeekOrigin.Begin);
            return md.ComputeHash(stream);
        }

        public void Dispose()
        {
            stream.Dispose();
            md.Dispose();
        }
    }
}
