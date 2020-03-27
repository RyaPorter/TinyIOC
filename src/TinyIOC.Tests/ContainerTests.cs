using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyIOC.Tests.TestClasses;
using TinyIOC;

namespace TinyIOC.Tests
{
    [TestClass]
    public class ContainerTests
    {

        [TestMethod]
        public void RegisterSingleton_WithInstanceTest()
        {
            var container = new TinyContainer();
            var typeInstance = new EmptyConreteType();
            container.RegisterSingleton(typeInstance);

            var instance = container.ResolveService<EmptyConreteType>();

            Assert.AreEqual(instance, typeInstance);
        }

        [TestMethod]
        public void RegisterService_MultipleConstructorParams()
        {
            var container = new TinyContainer();
            var typeInstance = new EmptyConreteType();

            container.RegisterService<FirstParam>();
            container.RegisterService<SecondParam>();
            container.RegisterService<IInterfaceParam, InterfaceParam>();
            container.RegisterService<ConcreteTypeMultiParam>();

            var instance = container.ResolveService<ConcreteTypeMultiParam>();

            Assert.AreEqual(true, instance != null);
        }


        [TestMethod]
        public void RegisterService_OnlyOneConstructorValid()
        {
            var container = new TinyContainer();
            var typeInstance = new EmptyConreteType();

            container.RegisterService<SecondParam>();
            container.RegisterService<IInterfaceParam, InterfaceParam>();
            container.RegisterService<ConcreteTypeMultiParam>();

            var instance = container.ResolveService<ConcreteTypeMultiParam>();

            Assert.AreEqual(true, instance != null);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void RegisterService_NoConstructorValid()
        {
            var container = new TinyContainer();
            var typeInstance = new EmptyConreteType();

            container.RegisterService<IInterfaceParam, InterfaceParam>();
            container.RegisterService<ConcreteTypeMultiParam>();

            var instance = container.ResolveService<ConcreteTypeMultiParam>();
        }
    }
}
