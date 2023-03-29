using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Workshop.CSharp.Advanced.Tests
{
    [TestClass]
    public class ExpressionTreeMapperTests : MapperTests<ExpressionTreeMapper> { }

    [TestClass]
    public class ReflectionMapperMapperTests : MapperTests<ReflectionMapper> { }

    public abstract class MapperTests<T>
        where T : IMapper, new()
    {
        private IMapper _mapper;

        [TestInitializeAttribute]
        public void BeforeEachTest()
        {
            _mapper = new T();
        }

        [TestMethod]
        [ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void NullArgumetsThrowExceptionTest()
        {
            _mapper.Map(null, null);
        }

        [TestMethod]
        public void ManyPropertiesTest()
        {
            var to = new SampleDestinationClass();
            var from = new
            {
                String1 = "",
                String2 = "",
                Int1 = 0,
            };

            _mapper.Map(from, to);

            Assert_TheSameProperties(to, new SampleDestinationClass()
            {
                String1 = from.String1,
                String2 = from.String2,
                Int1 = from.Int1,
            });
        }

        [TestMethod]
        public void DifferentNamesTest()
        {
            var to = new SampleDestinationClass();
            var from = new
            {
                String1 = "",

                String3 = "",
                Int2 = 0,
            };

            _mapper.Map(from, to);

            Assert_TheSameProperties(to, new SampleDestinationClass() { String1 = from.String1 });
        }

        [TestMethod]
        public void DifferentTypesTest()
        {
            var to = new SampleDestinationClass();
            var from = new
            {
                String1 = "",

                String2 = 123,
                Int1 = "",
            };

            _mapper.Map(from, to);

            Assert_TheSameProperties(to, new SampleDestinationClass() { String1 = from.String1 });
        }

        [TestMethod]
        public void ToWithGetterOnlyTest()
        {
            var to = new SampleDestinationClass();
            var from = new
            {
                StringGetOnly = "",
            };

            _mapper.Map(from, to);

            Assert_IsUntouched(to);
        }

        [TestMethod]
        public void ToWithSetterOnlyTest()
        {
            var to = new SampleDestinationClass();
            var from = new
            {
                StringSetOnly = "",
            };

            _mapper.Map(from, to);

            Assert_TheSameProperties(to, new SampleDestinationClass() { _stringSetOnly = from.StringSetOnly });
        }

        [TestMethod]
        public void FromWithSetterOnlyTest()
        {
            var to = new SampleDestinationClass();
            var from = new SampleDestinationClass
            {
                StringSetOnly = "",
            };

            _mapper.Map(from, to);

            Assert_IsUntouched(to);
        }

        [TestMethod]
        public void CacheTest()
        {
            var to1 = new SampleDestinationClass();
            var to2 = new SampleDestinationClass();
            var from = new SampleDestinationClass
            {
                String1 = "",
                String2 = "",
                StringSetOnly = "",
                _stringGetOnly = "",
                Int1 = 123
            };

            _mapper.Map(from, to1);
            _mapper.Map(to1, to2);

            Assert_TheSameProperties(to2,
                new SampleDestinationClass
                {
                    String1 = from.String1,
                    String2 = from.String2,
                    Int1 = 123
                });
        }


        private void Assert_TheSameProperties(SampleDestinationClass entity1, SampleDestinationClass entity2)
        {
            Assert.AreEqual(entity1.String1, entity2.String1);
            Assert.AreEqual(entity1.String2, entity2.String2);
            Assert.AreEqual(entity1.Int1, entity2.Int1);
            Assert.AreEqual(entity1.StringGetOnly, entity2.StringGetOnly);
            Assert.AreEqual(entity1._stringSetOnly, entity2._stringSetOnly);
        }

        private void Assert_IsUntouched(SampleDestinationClass entity)
        {
            Assert_TheSameProperties(entity, new SampleDestinationClass());
        }

        public class SampleDestinationClass
        {
            public string String1 { get; set; }
            public string String2 { get; set; }

            public string _stringGetOnly;
            public string StringGetOnly
            {
                get { return _stringGetOnly; }
                private set { _stringGetOnly = value; }
            }

            public string _stringSetOnly;
            public string StringSetOnly
            {
                private get { return _stringSetOnly; }
                set { _stringSetOnly = value; }
            }

            public int Int1 { get; set; }

            public SampleDestinationClass()
            {
                String1 = "String1";
                String2 = "String2";
                StringGetOnly = "StringGetOnly";
                StringSetOnly = "StringSetOnly";
                Int1 = -1;
            }
        }
    }

}
