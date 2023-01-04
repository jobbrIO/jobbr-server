using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Server.IntegrationTests.Integration
{
    /// <summary>
    /// Tests that confirm that all members if JobRunStates do have same values if their name are equal
    /// In addition, it's ensured that all core members are known in component models and vice-versa.
    /// </summary>
    [TestClass]
    public class JobRunEnumMappingTests
    {
        private readonly IEnumerable<Type> _allComponentModelJobRunEnumTypes;
        private readonly Type _coreType;
        private readonly Func<Type, bool> _enumTypeMatcher = t => t.Namespace != null && t.IsEnum && t.Namespace.Contains("ComponentModel") && t.Name.Contains("JobRun");

        public JobRunEnumMappingTests()
        {
            var componentModelAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(a => a.Name.Contains("ComponentModel"));
            foreach (var assemblyName in componentModelAssemblies)
            {
                Assembly.Load(assemblyName);
            }

            _allComponentModelJobRunEnumTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes().Where(_enumTypeMatcher));
            _coreType = typeof(Server.Core.Models.JobRunStates);
        }

        [TestMethod]
        public void InfrastructureTest_GetComponentModelJobRunEnumTypes_HasSome()
        {
            Assert.IsTrue(_allComponentModelJobRunEnumTypes.Any(), "There should be some JobRun Enum Types in the Component Models");
        }

        [TestMethod]
        public void CoreRunStateNames_IfExistInComponentModels_NameHasSameValue()
        {
            var errors = new List<string>();

            foreach (var componentModelEnumType in _allComponentModelJobRunEnumTypes)
            {
                var differences = FindDifferentValues(_coreType, componentModelEnumType);

                errors.AddRange(differences);
            }

            Assert.AreEqual(0, errors.Count, $"Found different values while comparing core model enum '{_coreType}' with component models\n\n" + string.Join("\n", errors));
        }

        [TestMethod]
        public void ComponentModelRunStateNames_IfExistInCore_NameHasSameValue()
        {
            var errors = new List<string>();

            foreach (var componentModelEnumType in _allComponentModelJobRunEnumTypes)
            {
                var differences = FindDifferentValues(componentModelEnumType, _coreType);

                errors.AddRange(differences);
            }

            Assert.AreEqual(0, errors.Count, $"Found different values for same enum names while comparing all component model with '{_coreType}'\n\n" + string.Join("\n", errors));
        }

        [TestMethod]
        public void ComponentModelRunStateNames_ComparedToAllComponentModels_HaveSameValue()
        {
            var errors = new List<string>();

            foreach (var master in _allComponentModelJobRunEnumTypes)
            {
                foreach (var target in _allComponentModelJobRunEnumTypes)
                {
                    var differences = FindDifferentValues(master, target);
                    errors.AddRange(differences);
                }
            }

            Assert.AreEqual(0, errors.Count, $"Found different values for same enum names while comparing all component model against each other\n\n" + string.Join("\n", errors));
        }

        [TestMethod]
        public void CoreRunStateNames_ForAllComponentModel_ShouldBeKnown()
        {
            var masterMembers = Enum.GetNames(_coreType);

            var errors = new List<string>();

            foreach (var cmEnumType in _allComponentModelJobRunEnumTypes)
            {
                var notFound = masterMembers.Except(Enum.GetNames(cmEnumType));

                errors.AddRange(notFound.Select(e => $"{e} was not found on enum type {cmEnumType.FullName}"));
            }

            Assert.AreEqual(0, errors.Count, "There where Enum members that where unknown in component models! \n\n- " + string.Join("\n- ", errors));
        }

        [TestMethod]
        public void ComponentModelStateNames_ComparedToCore_ShouldBeKnown()
        {
            var masterMembers = Enum.GetNames(_coreType);

            var errors = new List<string>();

            foreach (var cmEnumType in _allComponentModelJobRunEnumTypes)
            {
                var notFound = Enum.GetNames(cmEnumType).Except(masterMembers);

                errors.AddRange(notFound.Select(e => $"{e} was not found on enum type {cmEnumType.FullName}"));
            }

            Assert.AreEqual(0, errors.Count, "There where Enum members that where unknown in core, but available in component models! \n\n- " + string.Join("\n- ", errors));
        }

        private static IEnumerable<string> FindDifferentValues(Type masterType, Type remoteType)
        {
            var remoteMembers = Enum.GetNames(remoteType);

            foreach (var name in Enum.GetNames(masterType))
            {
                // Only check those member that also exist in remoteType
                if (!remoteMembers.Contains(name))
                {
                    continue;
                }

                // AutoMapper maps by using the index/value of the enum so we do
                var masterValue = (int)Enum.Parse(masterType, name);
                var remoteValue = (int)Enum.Parse(remoteType, name);

                if (remoteValue != masterValue)
                {
                    var err = $"Remote {remoteType.FullName}.{name} (Value: {remoteValue}) does not match with {masterType.FullName}.{name} (Value: {masterValue})";

                    yield return err;
                }
            }
        }
    }
}
