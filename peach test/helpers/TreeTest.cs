using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace peach.helpers {
    [TestClass]
    public class TreeTest {

        private Tree<MyData> _tree = new Tree<MyData>();

        [TestMethod]
        public void TestInsert() {
            BuildTree();

            string stringRepresentation = _tree.ToString();
            AssertTree(
@"ROOT: 70
    L: 50
        L: 40
            L: 20
                L: 10
                R: 30
        R: 60
    R: 90
        L: 80
        R: 120
            L: 100
                R: 110
            R: 130
");
        }

        [TestMethod]
        public void TestFindExact() {
            BuildTree();

            for (int ii = 10; ii <= 130; ii += 10) {
                MyData data = _tree.FindItemWithMinSize(ii);
                Assert.AreEqual(ii, data.OrderBy);
            }
        }

        [TestMethod]
        public void TestFindNextLargest() {
            BuildTree();

            for (int ii = 0; ii <= 120; ii += 10) {
                int requestedSize = ii + 5;
                MyData data = _tree.FindItemWithMinSize(requestedSize);
                Assert.AreEqual(ii + 10, data.OrderBy, "Requested size was: " + requestedSize);
            }
        }

        [TestMethod]
        public void TestFindTooLarge() {
            BuildTree();
            Assert.IsNull(_tree.FindItemWithMinSize(131));
        }

        [TestMethod]
        public void TestDeleteChildlessNodeOnLeft() {
            BuildTree();
            _tree.Delete(FindNode(10));

            AssertTree(
@"ROOT: 70
    L: 50
        L: 40
            L: 20
                R: 30
        R: 60
    R: 90
        L: 80
        R: 120
            L: 100
                R: 110
            R: 130
");
        }

        [TestMethod]
        public void TestDeleteChildlessNodeOnRight() {
            BuildTree();
            _tree.Delete(FindNode(30));

            AssertTree(
@"ROOT: 70
    L: 50
        L: 40
            L: 20
                L: 10
        R: 60
    R: 90
        L: 80
        R: 120
            L: 100
                R: 110
            R: 130
");
        }

        [TestMethod]
        public void TestDeleteNodeWithLeftChild() {
            BuildTree();
            _tree.Delete(FindNode(40));    

            AssertTree(
@"ROOT: 70
    L: 50
        L: 20
            L: 10
            R: 30
        R: 60
    R: 90
        L: 80
        R: 120
            L: 100
                R: 110
            R: 130
");
        }

        [TestMethod]
        public void TestDeleteNodeWithRightChild() {
            BuildTree();
            _tree.Delete(FindNode(100));

            AssertTree(
@"ROOT: 70
    L: 50
        L: 40
            L: 20
                L: 10
                R: 30
        R: 60
    R: 90
        L: 80
        R: 120
            L: 110
            R: 130
");
        }

        [TestMethod]
        public void TestDeleteNodeWithTwoChildren() {
            BuildTree();
            _tree.Delete(FindNode(90));

            AssertTree(
@"ROOT: 70
    L: 50
        L: 40
            L: 20
                L: 10
                R: 30
        R: 60
    R: 100
        L: 80
        R: 120
            L: 110
            R: 130
");
        }

        [TestMethod]
        public void TestDeleteRootNode() {
            BuildTree();
            _tree.Delete(FindNode(70));

            AssertTree(
@"ROOT: 80
    L: 50
        L: 40
            L: 20
                L: 10
                R: 30
        R: 60
    R: 90
        R: 120
            L: 100
                R: 110
            R: 130
");
        }

        // ROOT: 70
        //  L: 50
        //      L: 40
        //          L: 20
        //              L: 10
        //              R: 30
        //      R: 60
        //  R: 90
        //      L: 80
        //      R: 120
        //          L: 100
        //              R: 110
        //          R: 130
        private void BuildTree() {
            _tree.Insert(new MyData(70));
            _tree.Insert(new MyData(50));
            _tree.Insert(new MyData(90));
            _tree.Insert(new MyData(40));
            _tree.Insert(new MyData(60));
            _tree.Insert(new MyData(80));
            _tree.Insert(new MyData(120));
            _tree.Insert(new MyData(20));
            _tree.Insert(new MyData(100));
            _tree.Insert(new MyData(130));
            _tree.Insert(new MyData(10));
            _tree.Insert(new MyData(30));
            _tree.Insert(new MyData(110));
        }

        #region Helper Methods
        private MyData FindNode(int data) {
            return _tree.EnumerateDepthFirst().Single(x => x.OrderBy == data);
        }

        private void AssertTree(string expected) {
            string actual = _tree.ToString();
            Assert.AreEqual(expected, actual);
        }
        #endregion
    }

    #region Helper Classes
    class MyData : ISortOrder {
        public int OrderBy { get; private set; }
        internal MyData(int id) {
            OrderBy = id;
        }

        public override string ToString() {
            return OrderBy.ToString();
        }
    }
    #endregion
}
