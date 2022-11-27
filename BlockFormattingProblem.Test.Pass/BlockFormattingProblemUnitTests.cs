using Microsoft.CodeAnalysis.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using VerifyCS = BlockFormattingProblem.Test.CSharpCodeFixVerifier<
    BlockFormattingProblem.Analyzers.BlockFormattingProblemAnalyzer,
    BlockFormattingProblem.CodeFixes.BlockFormattingProblemCodeFixProvider>;

namespace BlockFormattingProblem.Test.Pass
{
    [TestClass]
    public class BlockFormattingProblemUnitTest
    {
        [TestMethod]
        public async Task TestCodeFixProviderWithAlternateIndentationAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
 public void Bar(int i)
 {
  if (i == 0)
   Debug.Assert(true);
 }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
 public void Bar(int i)
 {
  if (i == 0)
  {
   Debug.Assert(true);
  }
 }
}";

            await new Test
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    VerifyCS.Diagnostic().WithLocation(7, 4),
                },
                FixedCode = fixedTestCode,
                IndentationSize = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }

    public class Test : VerifyCS.Test
    {
        public Test()
        {
            this.OptionsTransforms.Add(options =>
                options
                .WithChangedOption(FormattingOptions.IndentationSize, this.Language, this.IndentationSize));
        }

        public int IndentationSize = 4;
    }
}
