using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Workshop.CSharp.Advanced.Tests
{
    [TestClass]
    public class GenericsTests
    {
        [TestMethod]
        public void ZipCollectionsOfTheSameLengthTest()
        {
            List<string> collection1 = new List<string> { "a", "b", "c" };
            List<int> collection2 = new List<int> { 1, 2, 3 };

            var zipped = Generics.Zip(collection1, collection2);

            Assert.AreEqual("a-1,b-2,c-3", string.Join(',', zipped));
        }

        [TestMethod]
        public void ZipCollectionsOfDifferentLengthTest()
        {
            List<string> collection1 = new List<string> { "a" };
            List<int> collection2 = new List<int> { 1, 2, 3 };

            var zipped = Generics.Zip(collection1, collection2);

            Assert.AreEqual("a-1", string.Join(',', zipped));
        }

        [TestMethod]
        public void UnzipCollectionTest()
        {
            var collection = new List<Pair<string, int>> { new Pair<string, int>("a", 1), new Pair<string, int>("b", 2) };

            var unzipped = Generics.Unzip(collection);

            Assert.AreEqual("a,b", string.Join(',', unzipped.Value1));
            Assert.AreEqual("1,2", string.Join(',', unzipped.Value2));
        }
    }
}