using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace peach.helpers {

    internal interface ISortOrder {
        int OrderBy { get; }
    }

    internal class Tree<T> where T : class, ISortOrder {
        internal TreeNode<T> Root;
        internal int Count { get; private set; }

        internal void Insert(T item) {
            TreeNode<T> node = new TreeNode<T>(item);
            if (Root == null)
                Root = node;
            else
                Root.Insert(node);

            Count++;
        }

        internal T FindItemWithMinSize(int minSize) {
            if (Root == null)
                return null;
            else
                return Root.FindItemWithMinSize(minSize);
        }

        internal void Delete(T item) {
            Root.Delete(item, this);
            Count--;
        }

        internal IEnumerable<T> EnumerateDepthFirst() {
            List<T> list = new List<T>();

            if (Root != null)
                Root.EnumerateDepthFirst(list);

            return list;
        }

        public override string ToString() {
            if (Root == null)
                return "EMPTY";
            else {
                StringBuilder builder = new StringBuilder();
                Root.ToString(builder, "ROOT", 0);
                return builder.ToString();
            }
        }
    }

    internal class TreeNode<T> where T : class, ISortOrder {
        internal T Item;
        internal TreeNode<T> Left;
        internal TreeNode<T> Right;
        internal TreeNode<T> Parent;

        internal TreeNode(T item) {
            Item = item;
        }

        #region Insert
        internal void Insert(TreeNode<T> node) {
            if (node.Item.OrderBy < Item.OrderBy) {
                if (Left == null) {
                    Left = node;
                    node.Parent = this;
                } else
                    Left.Insert(node);
            } else {
                if (Right == null) {
                    Right = node;
                    node.Parent = this;
                } else
                    Right.Insert(node);
            }
        }
        #endregion

        #region Find
        internal T FindItemWithMinSize(int minSize) {

            if (Item.OrderBy == minSize)
                return Item;                // Found a perfect match

            if (minSize < Item.OrderBy) {           // minSize < current node... Descending Left
                if (Left == null)
                    return Item;            // There is nothing smaller below us, so return this as the best fit
                T result = Left.FindItemWithMinSize(minSize);
                if (result == null)
                    // (***) Since Left did not contain a sufficiently large item, but this item IS big enough, just return it.
                    return Item;
                return result;
            } else {                                // minSize >= current node... Descending Right
                if (Right == null)
                    // We need the next higher node, but there is no Right to descend into.
                    // If there was any place above us in the recursion where we descended into a Left node,
                    // the local Item will be retunred - See (***)
                    return null;
                return Right.FindItemWithMinSize(minSize);
            }
        }
        #endregion

        #region Delete
        internal void Delete(T item, Tree<T> tree) {
            TreeNode<T> node = FindNode(item);
            node.Delete(tree);
        }

        private TreeNode<T> FindNode(T item) {
            if (item == this.Item)
                return this;

            if (item.OrderBy < Item.OrderBy) {
                return Left.FindNode(item);
            } else {
                return Right.FindNode(item);
            }
        }

        private void Delete(Tree<T> tree) {

            // Case 1: Node to delete has no children, just remove it from parent
            if (Left == null && Right == null)
                AttachNodeToParent(null, tree);

            // Case 2: Only one child on Right... Hoist up this grandchild up to the Parent
            else if (Left == null && Right != null)
                AttachNodeToParent(Right, tree);

            // Case 3: Only one child on Left... (same as above)
            else if (Left != null && Right == null)
                AttachNodeToParent(Left, tree);

            // Case 4: Node has both children. Do the following:
            // a) Find the subsequent node
            // b) Copy that node's data into this node
            // c) Delete the subsequent node
            else {
                TreeNode<T> subsequent = FindSubsequentNode(this);
                this.Item = subsequent.Item;
                subsequent.Delete(tree);
            }
        }

        // Note that 'node' can be null, in which case this method set the parent's branch to null.
        private void AttachNodeToParent(TreeNode<T> node, Tree<T> tree) {
            if (Parent == null)
                tree.Root = node;
            else if (Parent.Left == this)
                Parent.Left = node;
            else if (Parent.Right == this)
                Parent.Right = node;
            else
                throw new Exception("Unexpected");

            // Now we have to set the new parent of the node (which is our current Parent)
            if (node != null)
                node.Parent = Parent;
        }

        // Given a node which is guaranteed to have a Right child, find the subsequent
        // node by first descending to the Right, and then all the way down to the Left
        private TreeNode<T> FindSubsequentNode(TreeNode<T> node) {
            node = node.Right;
            while (node.Left != null)
                node = node.Left;
            return node;
        }
        #endregion

        #region Helps for Testing and Debugging
        internal void EnumerateDepthFirst(List<T> list) {
            if (Left != null)
                Left.EnumerateDepthFirst(list);

            list.Add(Item);

            if (Right != null)
                Right.EnumerateDepthFirst(list);
        }

        internal void ToString(StringBuilder builder, string label, int depth) {
            builder.AppendLine(string.Format("{0}{1}: {2}", new string(' ', depth * 4), label, Item.OrderBy));

            if (Left != null)
                Left.ToString(builder, "L", depth + 1);
            if (Right != null)
                Right.ToString(builder, "R", depth + 1);
        }
        #endregion
    }
}
