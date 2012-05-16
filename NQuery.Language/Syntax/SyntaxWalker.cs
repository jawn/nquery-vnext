using System;

namespace NQuery.Language
{
    public abstract class SyntaxWalker : SyntaxVisitor
    {
        public override void DefaultVisit(SyntaxNode node)
        {
            foreach (var syntaxNodeOrToken in node.GetChildren())
            {
                if (syntaxNodeOrToken.IsToken)
                    VisitToken(syntaxNodeOrToken.AsToken());
                else
                    Dispatch(node);
            }
        }

        public virtual void VisitToken(SyntaxToken token)
        {
            VisitLeadingTrivia(token);
            VisitTrailingTrivia(token);
        }

        public virtual void VisitLeadingTrivia(SyntaxToken token)
        {
            foreach (var syntaxTrivia in token.LeadingTrivia)
                VisitTrivia(syntaxTrivia);
        }

        public virtual void VisitTrailingTrivia(SyntaxToken token)
        {
            foreach (var syntaxTrivia in token.TrailingTrivia)
                VisitTrivia(syntaxTrivia);
        }

        public virtual void VisitTrivia(SyntaxTrivia trivia)
        {
        }
    }
}