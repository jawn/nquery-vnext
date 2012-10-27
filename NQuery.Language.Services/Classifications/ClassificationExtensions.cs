using System;
using System.Collections.Generic;

namespace NQuery.Language.Services.Classifications
{
    public static class ClassificationExtensions
    {
        public static IReadOnlyList<SyntaxClassificationSpan> ClassifySyntax(this SyntaxNode root)
        {
            return root.ClassifySyntax(root.FullSpan);
        }

        public static IReadOnlyList<SyntaxClassificationSpan> ClassifySyntax(this SyntaxNode root, TextSpan span)
        {
            var result = new List<SyntaxClassificationSpan>();
            var worker = new SyntaxClassificationWorker(result, span);
            worker.ClassifyNode(root);
            return result;
        }

        public static IReadOnlyList<SemanticClassificationSpan> ClassifySemantic(this SyntaxNode root, SemanticModel semanticModel)
        {
            return root.ClassifySemantic(semanticModel, root.FullSpan);
        }

        public static IReadOnlyList<SemanticClassificationSpan> ClassifySemantic(this SyntaxNode root, SemanticModel semanticModel, TextSpan span)
        {
            var result = new List<SemanticClassificationSpan>();
            var worker = new SemanticClassificationWorker(result, semanticModel, span);
            worker.ClassifyNode(root);
            return result;
        }

    }
}