using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Jobbr.Server.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Jobbr.Server.UnitTests.Storage
{
    [TestClass]
    public class ExtensionMethodsTests
    {
        [TestMethod]
        public void CloneNullObject_ShouldNotThrow()
        {
            // Arrange
            object data = null;

            // Act
            var dataClone = ExtensionMethods.Clone(data);

            // Assert
            dataClone.ShouldBeNull();
        }

        [TestMethod]
        public void CloneObject_ShouldCreateDeepClone()
        {
            // Arrange
            var data = new TestClass("reference", 5);

            // Act
            var dataClone = ExtensionMethods.Clone(data);

            // Assert
            dataClone.ShouldNotBeNull();
            dataClone.Key.ShouldBe(data.Key);
            dataClone.Value.ShouldBe(data.Value);
            dataClone.ShouldNotBe(data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(1.45)]
        [DataRow("test")]
        public void CloneValueTypes_ShouldReturnCopy(object value)
        {
            // Act
            var clone = ExtensionMethods.Clone(value);

            // Assert
            clone.ShouldBe(value);
        }

        [TestMethod]
        public void CloneList_ShouldDeepCloneAllEntries()
        {
            var data = new List<TestClass>
            {
                new TestClass("value-1", 1),
                new TestClass("value-2", 2),
                new TestClass("value-3", 3),
            };

            // Act
            var dataClone = ExtensionMethods.Clone(data);

            // Assert
            dataClone.ShouldNotBeNull();
            dataClone.Count.ShouldBe(data.Count);
            dataClone.ShouldNotBeEmpty();
            dataClone.ShouldNotBe(data);
            dataClone.ShouldBe(data, comparer: new TestClassValueComparer());
        }

        [TestMethod]
        public void CloneObject_NoSerializableFlag_ThrowsSerializationException()
        {
            // Arrange
            var data = new { Key = "reference", Value = 2 };

            // Act & Arrange
            Should.Throw<SerializationException>(() => ExtensionMethods.Clone(data));
        }

        [Serializable]
        private class TestClass
        {
            public TestClass(string key, int value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; set; }

            public int Value { get; set; }
        }

        private class TestClassValueComparer : IEqualityComparer<TestClass>
        {
            public bool Equals(TestClass x, TestClass y)
            {
                return x is not null && y is not null && x.Value == y.Value && x.Key == y.Key;
            }

            public int GetHashCode([DisallowNull] TestClass obj)
            {
                return HashCode.Combine(obj.Key, obj.Value);
            }
        }
    }
}
