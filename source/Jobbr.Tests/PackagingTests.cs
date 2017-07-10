using System.Reflection;
using Jobbr.DevSupport.ReferencedVersionAsserter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests
{
    [TestClass]
    public class PackagingTests
    {
        private readonly bool isPre = Assembly.GetExecutingAssembly().GetInformalVersion().Contains("-");

        [TestMethod]
        public void Feature_NuSpec_IsCompliant()
        {
            var asserter = new Asserter(Asserter.ResolvePackagesConfig("Jobbr.Server"), Asserter.ResolveRootFile("Jobbr.Server.nuspec"));

            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.ArtefactStorage"));
            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.Execution"));
            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.JobStorage"));
            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.Management"));
            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.Registration"));

            if (this.isPre)
            {
                // This rule is only valid for Pre-Release versions because we only need exact match on PreRelease Versions
                asserter.Add(new ExactVersionMatchRule("Jobbr.ComponentModel.*"));
            }
            else
            {
                asserter.Add(new AllowNonBreakingChanges("Jobbr.ComonentModel.*"));
            }

            asserter.Add(new VersionIsIncludedInRange("Jobbr.ComponentModel.*"));
            asserter.Add(new NoMajorChangesInNuSpec("Jobbr.*"));

            var result = asserter.Validate();

            Assert.IsTrue(result.IsSuccessful, result.Message);
        }
    }
}
