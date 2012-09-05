using System;
using System.Collections.Generic;

namespace NQuery.Language
{
    public sealed class GroupByColumnSyntax : SyntaxNode
    {
        private readonly ExpressionSyntax _expression;
        private readonly SyntaxToken? _comma;

        public GroupByColumnSyntax(SyntaxTree syntaxTree, ExpressionSyntax expression, SyntaxToken? comma)
            : base(syntaxTree)
        {
            _expression = expression;
            _comma = comma.WithParent(this);
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.GroupByColumn; }
        }

        public override IEnumerable<SyntaxNodeOrToken> ChildNodesAndTokens()
        {
            yield return _expression;
            if (_comma != null)
                yield return _comma.Value;
        }

        public ExpressionSyntax Expression
        {
            get { return _expression; }
        }

        public SyntaxToken? Comma
        {
            get { return _comma; }
        }
    }
}