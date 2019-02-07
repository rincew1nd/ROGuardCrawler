using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ROGuardCrawler.Utils
{
    public static class Helpers
    {
        public static T SafeParse<T>(this string value)
        {
            try
            {
                if (typeof(T) == typeof(int))
                    value = value.FullTrim(",", ".", " ");
                else if (typeof(T) == typeof(float))
                    value = value.Replace(".", ",");

                var converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromString(value);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not parse {value} to {typeof(T)}");
            }
            return default(T);
        }

        public static HtmlNode GetNodeSafe(this HtmlNode node, string xPath)
        {
            try
            {
                return node.SelectSingleNode(xPath);
            }
            catch
            {
                Console.WriteLine($"Could not find node {xPath} in {node.XPath}");
            }
            return null;
        }

        public static string GetInnerTextSafe(this HtmlNode node, string xPath)
        {
            try
            {
                return node.SelectSingleNode(xPath).InnerText;
            }
            catch
            {
                Console.WriteLine($"Could not find node {xPath} in {node.XPath}");
            }
            return null;
        }

        public static string FullTrim(this string value, params string[] chars)
        {
            return chars.Aggregate(value, (current, c) => current.Replace(c, string.Empty));
        }
    }
}
