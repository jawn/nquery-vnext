﻿using System;
using System.Collections.Generic;
using System.Linq;

using NQuery.Syntax;

namespace NQuery.Authoring.SymbolSearch
{
    public static class SymbolSearcher
    {
        public static SymbolSpan? FindSymbol(this SemanticModel semanticModel, int position)
        {
            if (semanticModel == null)
                throw new ArgumentNullException("semanticModel");

            var syntaxTree = semanticModel.Compilation.SyntaxTree;
            return (from t in syntaxTree.Root.FindStartTokens(position)
                    from n in t.Parent.AncestorsAndSelf()
                    from s in GetSymbolSpans(semanticModel, n)
                    where s.Span.ContainsOrTouches(position)
                    select s).Cast<SymbolSpan?>().FirstOrDefault();
        }

        public static IEnumerable<SymbolSpan> FindUsages(this SemanticModel semanticModel, Symbol symbol)
        {
            if (semanticModel == null)
                throw new ArgumentNullException("semanticModel");

            if (symbol == null)
                throw new ArgumentNullException("symbol");

            var syntaxTree = semanticModel.Compilation.SyntaxTree;

            return from n in syntaxTree.Root.DescendantNodes()
                   from s in GetSymbolSpans(semanticModel, n)
                   where s.Symbol == symbol
                   select s;
        }

        private static IEnumerable<SymbolSpan> GetSymbolSpans(SemanticModel semanticModel, SyntaxNode node)
        {
            switch (node.Kind)
            {
                case SyntaxKind.NameExpression:
                {
                    var expression = (NameExpressionSyntax) node;
                    var symbol = semanticModel.GetSymbol(expression);
                    if (symbol != null)
                        yield return SymbolSpan.CreateReference(symbol, expression.Name.Span);
                    break;
                }
                case SyntaxKind.PropertyAccessExpression:
                {
                    var expression = (PropertyAccessExpressionSyntax) node;
                    var symbol = semanticModel.GetSymbol(expression);
                    if (symbol != null)
                        yield return SymbolSpan.CreateReference(symbol, expression.Name.Span);
                    break;
                }
                case SyntaxKind.MethodInvocationExpression:
                {
                    var expression = (MethodInvocationExpressionSyntax)node;
                    var symbol = semanticModel.GetSymbol(expression);
                    if (symbol != null)
                        yield return SymbolSpan.CreateReference(symbol, expression.Name.Span);
                    break;
                }
                case SyntaxKind.FunctionInvocationExpression:
                {
                    var expression = (FunctionInvocationExpressionSyntax)node;
                    var symbol = semanticModel.GetSymbol(expression);
                    if (symbol != null)
                        yield return SymbolSpan.CreateReference(symbol, expression.Name.Span);
                    break;
                }
                case SyntaxKind.CountAllExpression:
                {
                    var countAllExpression = (CountAllExpressionSyntax)node;
                    var symbol = semanticModel.GetSymbol(countAllExpression);
                    yield return SymbolSpan.CreateReference(symbol, countAllExpression.Name.Span);
                    break;
                }
                case SyntaxKind.ExpressionSelectColumn:
                {
                    var selectColumn = (ExpressionSelectColumnSyntax)node;
                    if (selectColumn.Alias != null)
                    {
                        var symbol = semanticModel.GetDeclaredSymbol(selectColumn);
                        yield return SymbolSpan.CreateDefinition(symbol, selectColumn.Alias.Identifier.Span);
                    }
                    break;
                }
                case SyntaxKind.CommonTableExpression:
                {
                    // TODO: We should also support CTE column lists.
                    var commonTableExpression = (CommonTableExpressionSyntax)node;
                    var symbol = semanticModel.GetDeclaredSymbol(commonTableExpression);
                    yield return SymbolSpan.CreateDefinition(symbol, commonTableExpression.Name.Span);
                    break;
                }
                case SyntaxKind.DerivedTableReference:
                {
                    var derivedTable = (DerivedTableReferenceSyntax)node;
                    var symbol = semanticModel.GetDeclaredSymbol(derivedTable);
                    yield return SymbolSpan.CreateDefinition(symbol, derivedTable.Name.Span);
                    break;
                }
                case SyntaxKind.NamedTableReference:
                {
                    var namedTable = (NamedTableReferenceSyntax)node;
                    var tableInstanceSymbol = semanticModel.GetDeclaredSymbol(namedTable);
                    yield return SymbolSpan.CreateReference(tableInstanceSymbol.Table, namedTable.TableName.Span);
                    if (namedTable.Alias != null)
                        yield return SymbolSpan.CreateDefinition(tableInstanceSymbol, namedTable.Alias.Identifier.Span);
                    break;
                }
            }
        }
    }
}