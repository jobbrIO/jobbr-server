using Jobbr.Server.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jobbr.Tests.UnitTests.Storage
{
    [TestClass]
    public class ExtensionMethodsTests
    {
        [TestMethod]
        public void CloneNullObject()
        {
            // Arrange
            object data = null;

            // Act
            var dataClone = ExtensionMethods.Clone(data);

            // Assert
            Assert.IsNull(dataClone);
        }

        [TestMethod]
        public void CloneObject()
        {
            // Arrange
            var data = new TestClass("reference", 5);

            // Act
            var dataClone = ExtensionMethods.Clone(data);

            // Assert
            Assert.IsNotNull(dataClone);

            Assert.AreEqual(data.Key, dataClone.Key);

            Assert.AreEqual(data.Value, dataClone.Value);

            Assert.AreNotEqual(data, dataClone);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(1.45)]
        [DataRow("test")]
        public void CloneValueTypes(object value)
        {
            // Arrange

            // Act
            var clone = ExtensionMethods.Clone(value);

            // Assert
            Assert.AreEqual(clone, value);
        }

        [TestMethod]
        public void CloneList()
        {
            List<TestClass> data = new() { new("value-1", 1), new("value-2", 2), new("value-3", 3) };

            // Act
            var dataClone = ExtensionMethods.Clone(data);

            // Assert
            Assert.IsNotNull(dataClone);

            Assert.AreEqual(data.Count, dataClone.Count);

            CollectionAssert.AllItemsAreNotNull(dataClone);

            CollectionAssert.AreNotEqual(data, dataClone);

            CollectionAssert.AreEqual(data, dataClone, comparer: new TestClassValueComparer());
        }

        [TestMethod]
        public void CloneThrowsSerializationException()
        {
            // Arrange
            object data = new { Key = "reference", Value = 2 };

            // Act & Arrange
            Assert.ThrowsException<SerializationException>(() => ExtensionMethods.Clone(data));
        }

        [Serializable]
        private class TestClass
        {
            public string Key { get; set; }
            public int Value { get; set; }

            public TestClass(string key, int value)
            {
                Key = key;
                Value = value;
            }
        }

        private class TestClassValueComparer : IComparer, IComparer<TestClass>
        {
            public int Compare(object x, object y)
            {
                if (x is not TestClass left || y is not TestClass right)
                {
                    throw new InvalidOperationException();
                }

                return Compare(left, right);
            }

            public int Compare(TestClass x, TestClass y)
            {
                if (x.Value == y.Value && x.Key == y.Key)
                {
                    return 0;
                }
                else if (x.Value < y.Value) // Ignore any value comparision for now
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}
