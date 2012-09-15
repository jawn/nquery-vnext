﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NQuery.Language.UnitTests
{
    [TestClass]
    public class DerivedTableTests
    {
        [TestMethod]
        public void DerivedTables_CannotBindToRowObject()
        {
            var syntaxTree = SyntaxTree.ParseQuery("SELECT D FROM (SELECT 'foo') AS D");
            var compilation = Compilation.Empty.WithSyntaxTree(syntaxTree);
            var semanticModel = compilation.GetSemanticModel();
            var diagnostics = syntaxTree.GetDiagnostics().Concat(semanticModel.GetDiagnostics()).ToArray();

            Assert.AreEqual(1, diagnostics.Length);
            Assert.AreEqual(DiagnosticId.InvalidRowReference, diagnostics[0].DiagnosticId);
        }
    }
}