using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace cs2go
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Analyzer1 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Analyzer1";
        internal static readonly LocalizableString Title = "Analyzer1 Title";
        internal static readonly LocalizableString MessageFormat = "Analyzer1 '{0}'";
        internal const string Category = "Analyzer1 Category";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.LocalDeclarationStatement);
        }
        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {

        }
    }
}
