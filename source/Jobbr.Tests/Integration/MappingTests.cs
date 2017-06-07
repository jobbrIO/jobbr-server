using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration
{
    [TestClass]
    public class MappingTests
    {
        private readonly IEnumerable<Type> allComponentModelJobRunEnumTypes;
        private readonly Type coreType;

        public MappingTests()
        {
            this.allComponentModelJobRunEnumTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes().Where(t => t.Namespace != null && t.IsEnum && t.Namespace.Contains("ComponentModel")));
            this.coreType = typeof(Jobbr.Server.Core.Models.JobRunStates);
        }

        [TestMethod]
        public void CoreRunStateNames_IfExistInComponentModels_NameHasSameValue()
        {
            var errors = new List<string>();

            foreach (var componentModelEnumType in this.allComponentModelJobRunEnumTypes)
            {
                var differences = FindDifferentValues(this.coreType, componentModelEnumType);

                errors.AddRange(differences);
            }

            Assert.AreEqual(0, errors.Count, $"One or more errors found while comparing component model values for {this.coreType}\n\n" + string.Join("\n", errors));

        }

        private static IEnumerable<string> FindDifferentValues(Type masterType, Type remoteType)
        {
            var remoteMembers = Enum.GetNames(remoteType);

            foreach (var coreName in Enum.GetNames(masterType))
            {
                // Only check those member that also exist in remoteType
                if (!remoteMembers.Contains(coreName)) continue;

                // AutoMapper maps by using the index/value of the enum so we do
                var coreValue = (int) Enum.Parse(masterType, coreName);
                var remoteVaue = (int) Enum.Parse(remoteType, coreName);

                if (remoteVaue != coreValue)
                {
                    var err = $"{remoteType.FullName} contains a member for {coreName} but its value is different. CoreValue: {coreValue}, RemoteValue: {remoteVaue}";

                    yield return err;

                }
            }
        }
    }
}
