using System.Collections.Generic;

namespace NQuery.Language
{
    public sealed class CommonTableExpressionSyntax : SyntaxNode
    {
        private readonly SyntaxToken _identifer;
        private readonly CommonTableExpressionColumnNameListSyntax _columnNameList;
        private readonly SyntaxToken _asKeyword;
        private readonly SyntaxToken _leftParenthesis;
        private readonly QuerySyntax _query;
        private readonly SyntaxToken _rightParenthesis;

        public CommonTableExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken identifer, CommonTableExpressionColumnNameListSyntax columnNameList, SyntaxToken asKeyword, SyntaxToken leftParenthesis, QuerySyntax query, SyntaxToken rightParenthesis)
            : base(syntaxTree)
        {
            _identifer = identifer.WithParent(this);
            _columnNameList = columnNameList;
            _asKeyword = asKeyword.WithParent(this);
            _leftParenthesis = leftParenthesis.WithParent(this);
            _query = query;
            _rightParenthesis = rightParenthesis.WithParent(this);
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.CommonTableExpression; }
        }

        public override IEnumerable<SyntaxNodeOrToken> ChildNodesAndTokens()
        {
            yield return _identifer;
            if (_columnNameList != null)
                yield return _columnNameList;
            yield return _asKeyword;
            yield return _leftParenthesis;
            yield return _query;
            yield return _rightParenthesis;
        }

        public SyntaxToken Identifer
        {
            get { return _identifer; }
        }

        public CommonTableExpressionColumnNameListSyntax ColumnNameList
        {
            get { return _columnNameList; }
        }

        public SyntaxToken AsKeyword
        {
            get { return _asKeyword; }
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
    }
}