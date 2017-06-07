using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration
{
    /// <summary>
    /// Tests that confirm that all members if JobRunStates do have same values if their name are equal
    /// 
    /// Note: There is no rule that the Server.Core.Model.JobRunStates-Enum is a superset of all known states available in all Component Models. 
    /// </summary>
    [TestClass]
    public class JobRunEnumMappingTests
    {
        private readonly IEnumerable<Type> allComponentModelJobRunEnumTypes;
        private readonly Type coreType;

        private readonly Func<Type, bool> enumTypeMatcher = t => t.Namespace != null && t.IsEnum && t.Namespace.Contains("ComponentModel") && t.Name.Contains("JobRun");

        public JobRunEnumMappingTests()
        {
            this.allComponentModelJobRunEnumTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes().Where(this.enumTypeMatcher));
            this.coreType = typeof(Server.Core.Models.JobRunStates);
        }

        [TestMethod]
        public void InfrastructureTest_GetComponenModelJobRunEnumTypes_HasSome()
        {
            Assert.IsTrue(this.allComponentModelJobRunEnumTypes.Any(), "There should be some JobRun Enum Types in the Component Models");
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

            Assert.AreEqual(0, errors.Count, $"Found different values while comparing core model enum '{this.coreType}' with component models\n\n" + string.Join("\n", errors));
        }

        [TestMethod]
        public void ComponentModelRunStateNames_IfExistInCore_NameHasSameValue()
        {
            var errors = new List<string>();

            foreach (var componentModelEnumType in this.allComponentModelJobRunEnumTypes)
            {
                var differences = FindDifferentValues(componentModelEnumType, this.coreType);

                errors.AddRange(differences);
            }

            Assert.AreEqual(0, errors.Count, $"Found different values for same enum names while comparing all component model with '{this.coreType}'\n\n" + string.Join("\n", errors));
        }

        [TestMethod]
        public void ComponentModelRunStateNames_ComparedToAllComponentModels_HaveSameValue()
        {
            var errors = new List<string>();

            foreach (var master in this.allComponentModelJobRunEnumTypes)
            {
                foreach (var target in this.allComponentModelJobRunEnumTypes)
                {
                    var differences = FindDifferentValues(master, target);
                    errors.AddRange(differences);
                }
            }

            Assert.AreEqual(0, errors.Count, $"Found different values for same enum names while comparing all component model against each other\n\n" + string.Join("\n", errors));
        }

        private static IEnumerable<string> FindDifferentValues(Type masterType, Type remoteType)
        {
            var remoteMembers = Enum.GetNames(remoteType);

            foreach (var name in Enum.GetNames(masterType))
            {
                // Only check those member that also exist in remoteType
                if (!remoteMembers.Contains(name)) continue;

                // AutoMapper maps by using the index/value of the enum so we do
                var masterValue = (int) Enum.Parse(masterType, name);
                var remoteVaue = (int) Enum.Parse(remoteType, name);

                if (remoteVaue != masterValue)
                {
                    var err = $"Remote {remoteType.FullName}.{name} (Value: {remoteVaue}) does not match with {masterType.FullName}.{name} (Value: {masterValue})";

                    yield return err;
                }
            }
        }
    }
}
