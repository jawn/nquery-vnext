using System;

namespace NQuery.Language
{
    public enum SyntaxKind
    {
        // Tokens

        EndOfFileToken,
        BadToken,

        IdentifierToken,
        NumericLiteralToken,
        StringLiteralToken,
        DateLiteralToken,
        AtToken,

        BitwiseNotToken,
        BitwiseAndToken,
        BitwiseOrToken,
        BitwiseXorToken,
        LeftParenthesisToken,
        RightParenthesisToken,
        PlusToken,
        MinusToken,
        MultiplyToken,
        DivideToken,
        ModulusToken,
        PowerToken,
        CommaToken,
        DotToken,
        EqualsToken,
        UnequalsToken,
        LessToken,
        LessOrEqualToken,
        GreaterToken,
        GreaterOrEqualToken,
        NotLessToken,
        NotGreaterToken,
        RightShiftToken,
        LeftShiftToken,

        // Keywords
        
        AndKeyword,
        OrKeyword,
        IsKeyword,
        NullKeyword,
        NotKeyword,
        LikeKeyword,
        SoundslikeKeyword,
        SimilarKeyword,
        BetweenKeyword,
        InKeyword,
        CastKeyword,
        AsKeyword,
        CoalesceKeyword,
        NullIfKeyword,
        CaseKeyword,
        WhenKeyword,
        ThenKeyword,
        ElseKeyword,
        EndKeyword,
        TrueKeyword,
        FalseKeyword,
        ToKeyword,

        // Contextual keywords

        SelectKeyword,
        TopKeyword,
        DistinctKeyword,
        FromKeyword,
        WhereKeyword,
        GroupKeyword,
        ByKeyword,
        HavingKeyword,
        OrderKeyword,
        AscKeyword,
        DescKeyword,
        UnionKeyword,
        AllKeyword,
        IntersectKeyword,
        ExceptKeyword,
        ExistsKeyword,
        AnyKeyword,
        SomeKeyword,
        JoinKeyword,
        InnerKeyword,
        CrossKeyword,
        LeftKeyword,
        RightKeyword,
        OuterKeyword,
        FullKeyword,
        OnKeyword,
        WithKeyword,
        TiesKeyword,

        // Trivia

        WhitespaceTrivia,
        EndOfLineTrivia,
        MultiLineCommentTrivia,
        SingleLineCommentTrivia,
        SkippedTokensTrivia,

        // UnaryExpressions

        ComplementExpression,
        IdentityExpression,
        NegationExpression,
        LogicalNotExpression,

        // Nodes

        CompilationUnit,

        // Binary expressions

        BitAndExpression,
        BitOrExpression,
        BitXorExpression,
        AddExpression,
        SubExpression,
        MultiplyExpression,
        DivideExpression,
        ModulusExpression,
        PowerExpression,
        EqualExpression,
        NotEqualExpression,
        LessExpression,
        LessOrEqualExpression,
        GreaterExpression,
        GreaterOrEqualExpression,
        NotLessExpression,
        NotGreaterExpression,
        LeftShiftExpression,
        RightShiftExpression,
        LogicalAndExpression,
        LogicalOrExpression,
        LikeExpression,
        SoundslikeExpression,
        SimilarToExpression,

        // Expressions

        ParenthesizedExpression,
        BetweenExpression,
        IsNullExpression,
        CastExpression,
        CaseExpression,
        CaseLabel,
        CoalesceExpression,
        NullIfExpression,
        InExpression,
        LiteralExpression,
        VariableExpression,
        NameExpression,
        PropertyAccessExpression,
        CountAllExpression,
        FunctionInvocationExpression,
        MethodInvocationExpression,
        ArgumentList,

        SingleRowSubselect,
        ExistsSubselect,
        AllAnySubselect,

        // Queries

        ParenthesizedTableReference,
        NamedTableReference,
        CrossJoinedTableReference,
        InnerJoinedTableReference,
        OuterJoinedTableReference,
        DerivedTableReference,

        ExceptQuery,
        UnionQuery,
        IntersectQuery,
        OrderedQuery,
        OrderByColumn,
        ParenthesizedQuery,
        CommonTableExpressionQuery,
        CommonTableExpression,
        CommonTableExpressionColumnName,
        CommonTableExpressionColumnNameList,
        SelectQuery,
        TopClause,
        WildcardSelectColumn,
        ExpressionSelectColumn,
        SelectClause,
        FromClause,
        WhereClause,
        GroupByClause,
        GroupByColumn,
        HavingClause,
        Alias,
    }
}