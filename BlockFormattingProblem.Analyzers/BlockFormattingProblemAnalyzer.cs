using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace BlockFormattingProblem.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlockFormattingProblemAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BlockFormattingProblem";

        private const string Title = "Analyzer title";
        private const string MessageFormat = "Analyzer message";
        private const string Description = "Analyzer description.";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            var statement = ifStatement.Statement;
            if (statement == null)
            {
                return;
            }

            var blockStatement = statement as BlockSyntax;
            if (blockStatement != null)
            {
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, statement.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
