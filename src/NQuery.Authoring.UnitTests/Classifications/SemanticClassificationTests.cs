using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NQuery.Authoring.Classifications;
using NQuery.Symbols;

namespace NQuery.Authoring.UnitTests.Classifications
{
    [TestClass]
    public class SemanticClassificationTests
    {
        [TestMethod]
        public void SemanticClassification_Classifies()
        {
            var query = @"
                WITH Emps AS (
                    SELECT  e.*
                    FROM    Employees e
                )
                SELECT  COUNT(*),
                        SUM(d.EmployeeId)
                FROM    (
                            SELECT *,
                                   FirstName + ' ' + LastName AS FullName
                            FROM Emps
                        ) d
                WHERE   d.ReportsTo = @Manager
                AND     LEN(LastName) = LastName.Length
                AND     LastName.Substring(0, ReportsTo) = '2'
            ";

            var pieces = new[]
                         {
                             Tuple.Create("e", SemanticClassification.SchemaTable),
                             Tuple.Create("Employees", SemanticClassification.SchemaTable),
                             Tuple.Create("e", SemanticClassification.SchemaTable),
                             Tuple.Create("COUNT", SemanticClassification.Aggregate),
                             Tuple.Create("SUM", SemanticClassification.Aggregate),
                             Tuple.Create("d", SemanticClassification.DerivedTable),
                             Tuple.Create("EmployeeId", SemanticClassification.Column),
                             Tuple.Create("FirstName", SemanticClassification.Column),
                             Tuple.Create("LastName", SemanticClassification.Column),
                             Tuple.Create("FullName", SemanticClassification.Column),
                             Tuple.Create("Emps", SemanticClassification.CommonTableExpression),
                             Tuple.Create("d", SemanticClassification.DerivedTable),
                             Tuple.Create("d", SemanticClassification.DerivedTable),
                             Tuple.Create("ReportsTo", SemanticClassification.Column),
                             Tuple.Create("@Manager", SemanticClassification.Variable),
                             Tuple.Create("LEN", SemanticClassification.Function),
                             Tuple.Create("LastName", SemanticClassification.Column),
                             Tuple.Create("LastName", SemanticClassification.Column),
                             Tuple.Create("Length", SemanticClassification.Property),
                             Tuple.Create("LastName", SemanticClassification.Column),
                             Tuple.Create("Substring", SemanticClassification.Method),
                             Tuple.Create("ReportsTo", SemanticClassification.Column),
                         };

            var compilation = CompilationFactory.CreateQuery(query);
            var dataContext = compilation.DataContext.AddVariables(new VariableSymbol("Manager", typeof (int)));
            compilation = compilation.WithDataContext(dataContext);

            var syntaxTree = compilation.SyntaxTree;
            var semanticModel = compilation.GetSemanticModel();
            var classificationSpans = syntaxTree.Root.ClassifySemantics(semanticModel).ToArray();

            Assert.AreEqual(pieces.Length, classificationSpans.Length);

            for (var i = 0; i < pieces.Length; i++)
            {
                var piecce = pieces[i];
                var pieceText = piecce.Item1;
                var classification = piecce.Item2;
                var classificationText = query.Substring(classificationSpans[i].Span);

                Assert.AreEqual(pieceText, classificationText);
                Assert.AreEqual(classification, classificationSpans[i].Classification);
            }
        }
    }
}