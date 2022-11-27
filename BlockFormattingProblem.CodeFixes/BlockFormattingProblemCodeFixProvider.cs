using BlockFormattingProblem.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace BlockFormattingProblem.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BlockFormattingProblemCodeFixProvider)), Shared]
    public class BlockFormattingProblemCodeFixProvider : CodeFixProvider
    {
        private readonly string Title = "CodeFix Title";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(BlockFormattingProblemAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;
                var statement = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>().First();

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Title,
                        c => GetTransformedDocumentAsync(context.Document, root, statement),
                        equivalenceKey: Title),
                    diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, StatementSyntax statement)
        {
            var newRoot = root.ReplaceNode(statement, SyntaxFactory.Block(statement));
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}
