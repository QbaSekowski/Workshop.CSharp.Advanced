using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Workshop.CSharp.Advanced.Tests
{
    [TestClass]
    public class ContainerTests
    {
        static ContainerTests()
        {
            Container.Default.Register(typeof(IDateTimeService), new DateTimeService());
        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var o = Container.Default.Resolve<A>();
            Assert.IsNotNull(o);
            Assert.IsNotNull(o.DateTimeService);
        }

        [TestMethod]
        public void DefaultConstructorReadOnlyPropertyTest()
        {
            var o = Container.Default.Resolve<B>();
            Assert.IsNotNull(o);
            Assert.IsNull(o.DateTimeService);
        }

        [TestMethod]
        public void ManyConstructorsTest()
        {
            var o = Container.Default.Resolve<C>();
            Assert.IsNotNull(o);
            Assert.IsNotNull(o.DateTimeService);
            Assert.IsTrue(o.WasDefaultCalled);
        }

        [TestMethod]
        public void NonDefaultTest()
        {
            var o = Container.Default.Resolve<D>();
            Assert.IsNotNull(o);
            Assert.IsNull(o.DateTimeService);
            Assert.IsNotNull(o.Service2);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void NonDefaultWithNotExistingServicesTest()
        {
            var o = Container.Default.Resolve<E>();
        }

        public class A
        {
            public IDateTimeService DateTimeService { get; set; }
        }
        public class B
        {
            public IDateTimeService DateTimeService { get; private set; }
        }
        public class C
        {
            public bool WasDefaultCalled { get; set; }
            public IDateTimeService DateTimeService { get; set; }

            public C()
            {
                WasDefaultCalled = true;
            }
            public C(IDateTimeService a, IDateTimeService b)
            {

            }
        }
        public class D
        {
            public IDateTimeService DateTimeService { get; set; }
            public IDateTimeService Service2 { get; private set; }

            public D(IDateTimeService dateTimeService)
            {
                Service2 = dateTimeService;
            }
        }
        public class E
        {
            public E(IDateTimeService dateTimeService, string s)
            {

            }
        }
        public interface IDateTimeService
        {
            DateTime GetCurrentDate();
        }
        public class DateTimeService : IDateTimeService
        {
            public DateTime GetCurrentDate()
            {
                return DateTime.Now;
            }
        }
    }
}