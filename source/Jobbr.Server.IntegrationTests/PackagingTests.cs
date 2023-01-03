using Jobbr.DevSupport.ReferencedVersionAsserter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Server.IntegrationTests
{
    [TestClass]
    public class PackagingTests
    {
        [TestMethod]
        public void Feature_NuSpec_IsCompliant()
        {
            var asserter = new Asserter(Asserter.ResolveProjectFile("Jobbr.Server", "Jobbr.Server.csproj"), Asserter.ResolveRootFile("Jobbr.Server.nuspec"));

            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.ArtefactStorage"));
            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.Execution"));
            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.JobStorage"));
            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.Management"));
            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.Registration"));

            asserter.Add(new PackageExistsInBothRule("AutoMapper"));
            asserter.Add(new PackageExistsInBothRule("Microsoft.Extensions.Configuration.Abstractions"));
            asserter.Add(new PackageExistsInBothRule("Microsoft.Extensions.DependencyInjection"));
            asserter.Add(new PackageExistsInBothRule("Microsoft.Extensions.Logging.Abstractions"));
            asserter.Add(new PackageExistsInBothRule("NCrontab"));
            asserter.Add(new PackageExistsInBothRule("SimpleInjector"));
            asserter.Add(new PackageExistsInBothRule("TinyMessenger"));

            asserter.Add(new VersionIsIncludedInRange("Jobbr.ComponentModel.*"));
            asserter.Add(new VersionIsIncludedInRange("AutoMapper"));
            asserter.Add(new VersionIsIncludedInRange("Microsoft.Extensions.Configuration.Abstractions"));
            asserter.Add(new VersionIsIncludedInRange("Microsoft.Extensions.DependencyInjection"));
            asserter.Add(new VersionIsIncludedInRange("Microsoft.Extensions.Logging.Abstractions"));
            asserter.Add(new VersionIsIncludedInRange("NCrontab"));
            asserter.Add(new VersionIsIncludedInRange("SimpleInjector"));
            asserter.Add(new VersionIsIncludedInRange("TinyMessenger"));

            asserter.Add(new NoMajorChangesInNuSpec("Jobbr.*"));

            var result = asserter.Validate();

            Assert.IsTrue(result.IsSuccessful, result.Message);
        }
    }
}
