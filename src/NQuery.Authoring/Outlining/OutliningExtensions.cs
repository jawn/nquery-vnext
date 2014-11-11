using System;
using System.Collections.Generic;

using NQuery.Text;

namespace NQuery.Authoring.Outlining
{
    public static class OutliningExtensions
    {
        public static IReadOnlyList<OutliningRegionSpan> FindRegions(this SyntaxNode root)
        {
            return root.FindRegions(root.FullSpan);
        }

        public static IReadOnlyList<OutliningRegionSpan> FindRegions(this SyntaxNode root, TextSpan span)
        {
            var result = new List<OutliningRegionSpan>();
            var worker = new OutliningWorker(root.SyntaxTree.Text, result, span);
            worker.Visit(root);
            return result;
        }
    }
}