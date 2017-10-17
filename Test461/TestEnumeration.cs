using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.SysIo;

namespace Test400
{
    [TestClass]
    public class TestDirVector
    {
        string root = "Targets";
        string rootD1 = "Targets\\D1";
        string[] expectedf1 = { "D1\\12.txt", "D1\\8A.txt" };
        string[] expectedL = { "12.txt", "8A.txt", "D2\\Doc01.rtf" };
        string[] expectedN = { "8A.txt", "12.txt", "D2\\Doc01.rtf" };
        string[] expectedD = { "", "D1", "D1\\D2" };

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
            foreach (string lx in DirNode.Vector.EnumerateFiles (rootD1, null, Ordering.Natural))
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
            foreach (string lx in DirNode.Vector.EnumerateDirectories (root, null, Ordering.Lexical))
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


        [TestMethod]
        public void TestFilter1_all()
        {
            int ix = 0;
            foreach (string lx in DirNode.Vector.EnumerateDirectories (root, "*", Ordering.Lexical))
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


        [TestMethod]
        public void TestFilter1_txt()
        {
            int ix = 0;
            foreach (string lx in DirNode.Vector.EnumerateFiles (root, "*.txt", Ordering.Lexical))
            {
                Assert.IsTrue (ix < expectedf1.Length);
                Assert.IsTrue (lx.EndsWith ("\\" + root + "\\" + expectedf1[ix]));
                ++ix;
            }

            Assert.AreEqual (expectedf1.Length, ix);
        }
    }
}
