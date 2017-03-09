using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NeuroSpeech.CoreDI.Tests
{
    [TestClass]
    public class DITests
    {

        [TestInitialize]
        public void Init() {

            DI.RegisterGlobal<IGlobalRoot, GlobalRoot>();

            DI.RegisterGlobal<IGlobalChild, GlobalChild>();

            DI.RegisterScoped<IRoot, Root>();
            DI.RegisterScoped<IChild, Child>();

        }

        [TestCleanup]
        public void Dispose() {
            DI.Clear();
        }


        [TestMethod]
        public void Exceptions()
        {
            Assert.ThrowsException<KeyNotFoundException>(() => {
                DI.Get<DITests>();
            });

            Assert.ThrowsException<ArgumentNullException>(() => {
                DI.Get<IRoot>();
            });


            Assert.ThrowsException<ArgumentNullException>(() => {
                DI.Get<IChild>();
            });

        }

        [TestMethod]
        public void Globals() {

            var child = DI.Get<IGlobalChild>();
            Assert.IsInstanceOfType(child, typeof(GlobalChild));

            var child2 = DI.Get<IGlobalChild>();

            Assert.AreEqual(child, child2);

            var root = DI.Get<IGlobalRoot>();
            Assert.IsInstanceOfType(root, typeof(IGlobalRoot));

            var root2 = DI.Get<IGlobalRoot>();

            Assert.AreEqual(child, child2);

            Assert.AreNotEqual(child, root);
        }

        [TestMethod]
        public void Scopes() {

            var scope = DI.NewScope();


            var scope2 = DI.NewScope();


            var child = DI.Get<IChild>(scope);

            var child2 = DI.Get<IChild>(scope2);

            Assert.AreNotEqual(child, child2);

        }
    }




    public interface IGlobalRoot {
    }

    public interface IGlobalChild: IGlobalRoot {
    }


    public class GlobalRoot: IGlobalRoot {
    }

    public class GlobalChild : GlobalRoot, IGlobalChild {
    }

    public interface IRoot
    {
    }

    public interface IChild : IRoot
    {
    }


    public class Root : IRoot
    {
    }

    public class Child : Root, IChild
    {
    }


}
