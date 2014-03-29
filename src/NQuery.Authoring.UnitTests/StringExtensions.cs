﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

using NQuery.Text;

namespace NQuery.Authoring.UnitTests
{
    internal static class StringExtensions
    {
        public static string NormalizeCode(this string text)
        {
            return text.Unindent().Trim();
        }

        public static string Unindent(this string text)
        {
            var minIndent = Int32.MaxValue;

            using (var stringReader = new StringReader(text))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var indent = line.Length - line.TrimStart().Length;
                    minIndent = Math.Min(minIndent, indent);
                }
            }

            var sb = new StringBuilder();
            using (var stringReader = new StringReader(text))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    var unindentedLine = line.Length < minIndent
                        ? line
                        : line.Substring(minIndent);
                    sb.AppendLine(unindentedLine);
                }
            }

            return sb.ToString();
        }

        public static string Substring(this string text, TextSpan span)
        {
            return text.Substring(span.Start, span.Length);
        }

        public static string ParseSpans(this string text, out ImmutableArray<TextSpan> spans)
        {
            var resultSpans = new List<TextSpan>();
            var sb = new StringBuilder();
            var spanStartStack = new Stack<int>();
            foreach (var c in text)
            {
                switch (c)
                {
                    case '{':
                        spanStartStack.Push(sb.Length);
                        break;
                    case '}':
                        if (spanStartStack.Count == 0)
                            throw new FormatException("Missing open brace");

                        resultSpans.Add(TextSpan.FromBounds(spanStartStack.Pop(), sb.Length));
                        break;
                    case '|':
                        resultSpans.Add(new TextSpan(sb.Length, 0));
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            spans = resultSpans.OrderBy(s => s.Start).ThenBy(s => s.End).ToImmutableArray();
            return sb.ToString();
        }

        public static string ParseSinglePosition(this string text, out int position)
        {
            ImmutableArray<TextSpan> spans;
            var result = text.ParseSpans(out spans);

            if (spans.Length != 1 || spans[0].Length != 0)
                throw new ArgumentException("The position must be marked with a single pipe, such as 'SELECT e.Empl|oyeeId'");

            position = spans[0].Start;
            return result;
        }

        public static string ParseSingleSpan(this string text, out TextSpan span)
        {
            ImmutableArray<TextSpan> spans;
            var result = text.ParseSpans(out spans);

            if (spans.Length != 1)
                throw new ArgumentException("The span must be marked with braces, such as 'SELECT {e.EmployeeId}'");

            span = spans[0];
            return result;
        }
    }
}