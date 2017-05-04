using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.SysIo;

namespace Test400
{
    [TestClass]
    public class TestDirVector
    {
        string root = "Targets";
        string rootD1 = "Targets\\D1";
        string[] expectedL = { "12.txt", "8A.txt" };
        string[] expectedN = { "8A.txt", "12.txt" };
        string[] expectedD = { "", "D1" };

        [TestMethod]
        public void TestFiles1()
        {
            int ix = 0;
            foreach (string lx in DirNode.Vector.EnumerateFiles (rootD1))
            {
                Assert.IsTrue (ix < expectedL.Length);
                Assert.IsTrue (lx.EndsWith (rootD1 + "\\" + expectedL[ix]));
                ++ix;
            }

            Assert.AreEqual (expectedL.Length, ix);
        }


        [TestMethod]
        public void TestFiles2()
        {
            int ix = 0;
            foreach (string lx in DirNode.Vector.EnumerateFiles (rootD1, Ordering.Natural))
            {
                Assert.IsTrue (ix < expectedL.Length);
                Assert.IsTrue (lx.EndsWith (rootD1 + "\\" + expectedN[ix]));
                ++ix;
            }

            Assert.AreEqual (expectedN.Length, ix);
        }


        [TestMethod]
        public void TestDirs1()
        {
            int ix = 0;
            foreach (string lx in DirNode.Vector.EnumerateDirectories (root, Ordering.Lexical))
            {
                Assert.IsTrue (ix < expectedD.Length);
                if (ix == 0)
                    Assert.IsTrue (lx.EndsWith ("\\" + root));
                else
                    Assert.IsTrue (lx.EndsWith ("\\" + root + "\\" + expectedD[ix]));
                ++ix;
            }

            Assert.AreEqual (expectedN.Length, ix);
        }
    }
}
