using System.Collections.Generic;
using System.IO;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.ArtefactStorage.Model;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Builder;
using Jobbr.Tests.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Registration
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public void RegisterCustomStorageProvider_AfterCreation_CorrectTypeIsActivated()
        {
            var builder = new JobbrBuilder();

            builder.Register<IArtefactsStorageProvider>(typeof(CustomArtefactStorageAdapter));
            builder.Register<IJobbrComponent>(typeof(ExposeAllServicesComponent));

            var server = builder.Create();

            Assert.IsNotNull(ExposeAllServicesComponent.Instance.ArtefactsStorageProvider);
            Assert.AreEqual(typeof(CustomArtefactStorageAdapter), ExposeAllServicesComponent.Instance.ArtefactsStorageProvider.GetType());
        }
    }

    public class CustomArtefactStorageAdapter : IArtefactsStorageProvider
    {
        public void Save(string container, string fileName, Stream content)
        {
        }

        public Stream Load(string container, string fileName)
        {
            return null;
        }

        public List<JobbrArtefact> GetArtefacts(string container)
        {
            return null;
        }
    }
}
