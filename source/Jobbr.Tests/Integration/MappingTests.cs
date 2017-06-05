using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration
{
    [TestClass]
    public class MappingTests
    {
        [TestMethod]
        public void CoreRunStateNames_IfExistInComponentModels_NameHasSameValue()
        {
            var coreType = typeof(Jobbr.Server.Core.Models.JobRunStates);
            var coreMembers = Enum.GetNames(coreType);

            var errors = new List<string>();

            var allJobJobRunEnumTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes().Where(t => t.Namespace != null && t.IsEnum && t.Namespace.Contains("ComponentModel")));

            foreach (var enumType in allJobJobRunEnumTypes)
            {
                var remoteMembers = Enum.GetNames(enumType);

                foreach (var coreName in coreMembers)
                {
                    if (remoteMembers.Contains(coreName))
                    {
                        // AutoMapper maps by using the index/value of the enum so we do
                        var coreValue = (int)Enum.Parse(coreType, coreName);
                        var remoteVaue = (int)Enum.Parse(enumType, coreName);

                        if (remoteVaue != coreValue)
                        {
                            var err = $"{enumType.FullName} contains a member for {coreName} but its value is different. CoreValue: {coreValue}, RemoteValue: {remoteVaue}";
                            errors.Add(err);
                        }
                    }
                }
            }

            Assert.AreEqual(0, errors.Count, $"One or more errors found while comparing component model values for {coreType}\n\n" + string.Join("\n", errors));

        }
    }
}
