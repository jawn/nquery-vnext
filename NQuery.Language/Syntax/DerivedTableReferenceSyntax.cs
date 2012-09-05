using System;
using System.Collections.Generic;

namespace NQuery.Language
{
    public sealed class DerivedTableReferenceSyntax : TableReferenceSyntax
    {
        private readonly SyntaxToken _leftParenthesis;
        private readonly QuerySyntax _query;
        private readonly SyntaxToken _rightParenthesis;
        private readonly SyntaxToken? _asKeyword;
        private readonly SyntaxToken _name;

        public DerivedTableReferenceSyntax(SyntaxTree syntaxTree, SyntaxToken leftParenthesis, QuerySyntax query, SyntaxToken rightParenthesis, SyntaxToken? asKeyword, SyntaxToken name, SyntaxToken? commaToken)
            : base(syntaxTree, commaToken)
        {
            _leftParenthesis = leftParenthesis.WithParent(this);
            _query = query;
            _rightParenthesis = rightParenthesis.WithParent(this);
            _asKeyword = asKeyword.WithParent(this);
            _name = name.WithParent(this);
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.DerivedTableReference; }
        }

        public override IEnumerable<SyntaxNodeOrToken> ChildNodesAndTokens()
        {
            yield return _leftParenthesis;
            yield return _query;
            yield return _rightParenthesis;
            if (_asKeyword != null)
                yield return _asKeyword.Value;
            yield return _name;
            if (CommaToken != null)
                yield return CommaToken.Value;
        }

        public SyntaxToken LeftParenthesis
        {
            get { return _leftParenthesis; }
        }

        public QuerySyntax Query
        {
            get { return _query; }
        }

        public SyntaxToken RightParenthesis
        {
            get { return _rightParenthesis; }
        }

        public SyntaxToken? AsKeyword
        {
            get { return _asKeyword; }
        }

        public SyntaxToken Name
        {
            get { return _name; }
        }
    }
}