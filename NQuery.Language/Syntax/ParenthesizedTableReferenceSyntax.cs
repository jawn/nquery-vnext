using System;
using System.Collections.Generic;

namespace NQuery.Language
{
    public sealed class ParenthesizedTableReferenceSyntax : TableReferenceSyntax
    {
        private readonly SyntaxToken _leftParenthesis;
        private readonly TableReferenceSyntax _tableReference;
        private readonly SyntaxToken _rightParenthesis;

        public ParenthesizedTableReferenceSyntax(SyntaxTree syntaxTree, SyntaxToken leftParenthesis, TableReferenceSyntax tableReference, SyntaxToken rightParenthesis, SyntaxToken? commaToken)
            : base(syntaxTree, commaToken)
        {
            _leftParenthesis = leftParenthesis.WithParent(this);
            _tableReference = tableReference;
            _rightParenthesis = rightParenthesis.WithParent(this);
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.ParenthesizedTableReference; }
        }

        public override IEnumerable<SyntaxNodeOrToken> ChildNodesAndTokens()
        {
            yield return _leftParenthesis;
            yield return _tableReference;
            yield return _rightParenthesis;
            if (CommaToken != null)
                yield return CommaToken.Value;
        }

        public SyntaxToken LeftParenthesis
        {
            get { return _leftParenthesis; }
        }

        public TableReferenceSyntax TableReference
        {
            get { return _tableReference; }
        }

        public SyntaxToken RightParenthesis
        {
            get { return _rightParenthesis; }
        }
    }
}