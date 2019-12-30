using System;
using System.Collections.Generic;
using System.Collections;

namespace Weighted_Randomizer
{
    /// <summary>
    /// A implementation of a weighted randomizer which uses (for lack of a better term) a weighted self-balancing binary tree.
    /// Adding/removing/updating items, and calls to both NextWithRemoval() and NextWithReplacement(), and are relatively fast (O(log n))
    /// </summary>
    /// <typeparam name="TKey">The type of the objects to choose at random</typeparam>
    public class DynamicWeightedRandomizer<TKey> : IWeightedRandomizer<TKey>
        where TKey : IComparable<TKey>
    {
        private readonly Node _sentinel;
        private readonly ThreadAwareRandom _random;
        private Node _root;
        private Node _deleted;

        /// <summary>
        /// Create a new DynamicWeightedRandomizer
        /// </summary>
        public DynamicWeightedRandomizer()
        {
            _root = _sentinel = new Node();
            _deleted = null;
            _random = new ThreadAwareRandom();
        }

        /// <summary>
        /// Create a new DynamicWeightedRandomizer with the given seed
        /// </summary>
        public DynamicWeightedRandomizer(int seed)
        {
            _root = _sentinel = new Node();
            _deleted = null;
            _random = new ThreadAwareRandom(seed);
        }

        #region Node class
        private class Node
        {
            // AA-tree data
            internal int level;
            internal Node left;
            internal Node right;

            //Weighted Randomizer data
            internal TKey key;
            internal int weight;
            internal long subtreeWeight;

            // constuctor for the sentinel node
            internal Node()
            {
                this.level = 0;
                this.left = this;
                this.right = this;
                this.weight = 0;
                this.subtreeWeight = 0;
            }

            // constuctor for regular nodes (that all start life as leaf nodes)
            internal Node(TKey key, int weight, Node sentinel)
            {
                this.level = 1;
                this.left = sentinel;
                this.right = sentinel;
                this.key = key;
                this.weight = weight;
                this.subtreeWeight = weight;
            }
        }
        #endregion

        #region AA-Tree code
        //This weighted randomizer can be used with any self-balancing tree.  I chose an AA-tree written by Aleksey Demakov
        //(with his permission of course), found here:  http://demakov.com/snippets/aatree.html
        //I chose this one because it was easy to understand and well-written.

        private void RotateRight(ref Node node)
        {
            if(node.level == node.left.level)
            {
                //Update the subtreeWeights before moving the nodes
                long oldSubtreeWeight = node.subtreeWeight;
                if(node != _sentinel)
                    node.subtreeWeight = oldSubtreeWeight - node.left.subtreeWeight + node.left.right.subtreeWeight;
                if(node.left != _sentinel)
                    node.left.subtreeWeight = oldSubtreeWeight;

                // rotate right
                Node left = node.left;
                node.left = left.right;
                left.right = node;
                node = left;
            }
        }

        private void RotateLeft(ref Node node)
        {
            if(node.right.right.level == node.level)
            {
                //Update the subtreeWeights before moving the nodes
                long oldSubtreeWeight = node.subtreeWeight;
                if(node != _sentinel)
                    node.subtreeWeight = oldSubtreeWeight - node.right.subtreeWeight + node.right.left.subtreeWeight;
                if(node.right != _sentinel)
                    node.right.subtreeWeight = oldSubtreeWeight;

                // rotate left
                Node right = node.right;
                node.right = right.left;
                right.left = node;
                node = right;
                node.level++;
            }
        }

        private void InsertNode(ref Node node, TKey key, int weight)
        {
            if(weight < 0)
            {
                throw new ArgumentOutOfRangeException("weight", weight, "Cannot add a key with weight < 0!");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key", "Cannot add a null key");
            }

            if(node == _sentinel)
            {
                node = new Node(key, weight, _sentinel);
                UpdateSubtreeWeightsForInsertion(node);
                Count++;
                return;
            }

            int compare = key.CompareTo(node.key);
            if(compare < 0)
            {
                InsertNode(ref node.left, key, weight);
            }
            else if(compare > 0)
            {
                InsertNode(ref node.right, key, weight);
            }
            else
            {
                throw new ArgumentException("Key already exists in DynamicWeightedRandomizer: " + key.ToString());
            }

            RotateRight(ref node);
            RotateLeft(ref node);
        }

        private bool DeleteNode(ref Node node, TKey key)
        {
            if(node == _sentinel)
            {
                return (_deleted != null);
            }

            int compare = key.CompareTo(node.key);
            if(compare < 0)
            {
                if(!DeleteNode(ref node.left, key))
                {
                    return false;
                }
            }
            else
            {
                if(compare == 0)
                {
                    _deleted = node;
                }
                if(!DeleteNode(ref node.right, key))
                {
                    return false;
                }
            }

            if(_deleted != null)
            {
                UpdateSubtreeWeightsForDeletion(_deleted, node);
                _deleted.key = node.key;
                _deleted.weight = node.weight;
                _deleted = null;
                node = node.right;
                Count--;
            }
            else if(node.left.level < node.level - 1
                     || node.right.level < node.level - 1)
            {
                --node.level;
                if(node.right.level > node.level)
                {
                    node.right.level = node.level;
                }
                RotateRight(ref node);
                RotateRight(ref node.right);
                RotateRight(ref node.right.right);
                RotateLeft(ref node);
                RotateLeft(ref node.right);
            }

            return true;
        }

        private Node FindNode(Node node, TKey key)
        {
            while(node != _sentinel)
            {
                int compare = key.CompareTo(node.key);
                if(compare < 0)
                {
                    node = node.left;
                }
                else if(compare > 0)
                {
                    node = node.right;
                }
                else
                {
                    return node;
                }
            }

            return null;
        }
        #endregion

        #region Updating weights
        //When inserting, all the subtree's parent subtrees go up by insertedNode.weight
        //Should be called AFTER the insertion
        private void UpdateSubtreeWeightsForInsertion(Node insertedNode)
        {
            //Search down the tree for the inserted node, updating the weights as we go
            Node currentNode = _root;
            while(currentNode != insertedNode)
            {
                currentNode.subtreeWeight += insertedNode.weight;

                int compare = insertedNode.key.CompareTo(currentNode.key);
                currentNode = (compare < 0 ? currentNode.left : currentNode.right);
            }
        }

        //When deleting a node, we swap it with its leftmost-descendent in the RIGHT subtree, then remove it.
        //Thus, the weight of deleteMe's subtree (and all parent subtrees) goes down by deleteMe.weight,
        //while all the children subtrees go that contain leftmostRightDescendent go down by leftmostRightDescendent.weight
        //Should be called BEFORE either the swap or deletion
        private void UpdateSubtreeWeightsForDeletion(Node deletedNode, Node leftmostRightDescendent)
        {
            //Search down the tree for the deletedNode, updating the weights as we go
            Node currentNode = _root;
            while(currentNode != deletedNode)
            {
                currentNode.subtreeWeight -= deletedNode.weight;

                int compare = deletedNode.key.CompareTo(currentNode.key);
                currentNode = (compare < 0 ? currentNode.left : currentNode.right);
            }

            //Right now, currentNode == deletedNode
            currentNode.subtreeWeight -= deletedNode.weight;

            //Deleted node gets replaced by leftmostRightDescendent, so we need to continue searching the subtree for it instead,
            //again updating the nodes' weights as we go
            while(currentNode != leftmostRightDescendent && currentNode != _sentinel)
            {
                int compare = leftmostRightDescendent.key.CompareTo(currentNode.key);
                currentNode = (compare < 0 ? currentNode.left : currentNode.right);

                currentNode.subtreeWeight -= leftmostRightDescendent.weight;
            }

            //At this point, the subtreeWeights have been correctly subtracted from, but the
            //actual node values haven't been swapped yet
        }
        #endregion

        #region ICollection<T> stuff
        /// <summary>
        /// Returns the number of items currently in the list
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Remove all items from the list
        /// </summary>
        public void Clear()
        {
            _root = _sentinel;
            Count = 0;
        }

        /// <summary>
        /// Returns false.  Necessary for the ICollection&lt;T&gt; interface.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Copies the keys to an array, in order
        /// </summary>
        public void CopyTo(TKey[] array, int startingIndex)
        {
            int currentIndex = startingIndex;
            foreach(TKey key in this)
            {
                array[currentIndex] = key;
                currentIndex++;
            }
        }

        /// <summary>
        /// Returns true if the given item has been added to the list; false otherwise
        /// </summary>
        public bool Contains(TKey key)
        {
            Node node = FindNode(_root, key);
            return (node != null && node != _sentinel);
        }

        /// <summary>
        /// Adds the given item with a default weight of 1
        /// </summary>
        public void Add(TKey key)
        {
            InsertNode(ref _root, key, 1);
        }

        /// <summary>
        /// Adds the given item with the given weight.  Higher weights are more likely to be chosen.
        /// </summary>
        public void Add(TKey key, int weight)
        {
            InsertNode(ref _root, key, weight);
        }

        /// <summary>
        /// Remoevs the given item from the list.
        /// </summary>
        /// <returns>Returns true if the item was successfully deleted, or false if it was not found</returns>
        public bool Remove(TKey key)
        {
            _deleted = null;
            return DeleteNode(ref _root, key);
        }
        #endregion

        #region IEnumerable<T> stuff
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            return InorderTraversal(_root);
        }

        private IEnumerator<TKey> InorderTraversal(Node node)
        {
            //The obvious way of doing this - calling itself recursively - ends up creating an enormous number
            // of iterators (as many as there are items in the tree).  So, we have to emulate recursion manually >_<
            Stack<Node> stack = new Stack<Node>();
            while(stack.Count != 0 || node != _sentinel)
            {
                if(node != _sentinel)
                {
                    stack.Push(node);
                    node = node.left;
                }
                else
                {
                    node = stack.Pop();
                    yield return node.key;
                    node = node.right;
                }
            }
        }
        #endregion

        #region IWeightedRandomizer<T> stuff
        /// <summary>
        /// The total weight of all the items added so far
        /// </summary>
        public long TotalWeight
        {
            get
            {
                return _root.subtreeWeight;
            }
        }

        /// <summary>
        /// Returns an item chosen randomly by weight (higher weights are more likely),
        /// and replaces it so that it can be chosen again
        /// </summary>
        public TKey NextWithReplacement()
        {
            VerifyHaveItemsToChooseFrom();

            Node currentNode = _root;
            long randomNumber = _random.NextLong(0, TotalWeight) + 1;  //[1, TotalWeight] inclusive

            while(true)
            {
                if(currentNode.left.subtreeWeight >= randomNumber)
                {
                    currentNode = currentNode.left;
                }
                else
                {
                    randomNumber -= currentNode.left.subtreeWeight;
                    if(currentNode.right.subtreeWeight >= randomNumber)
                    {
                        currentNode = currentNode.right;
                    }
                    else
                    {
                        return currentNode.key;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an item chosen randomly by weight (higher weights are more likely),
        /// and removes it so it cannot be chosen again
        /// </summary>
        public TKey NextWithRemoval()
        {
            VerifyHaveItemsToChooseFrom();

            TKey randomKey = NextWithReplacement();
            Remove(randomKey);
            return randomKey;
        }

        /// <summary>
        /// Throws an exception if the Count or TotalWeight are 0, meaning that are no items to choose from.
        /// </summary>
        private void VerifyHaveItemsToChooseFrom()
        {
            if (Count <= 0)
                throw new InvalidOperationException("There are no items in the DynamicWeightedRandomizer");
            if (TotalWeight <= 0)
                throw new InvalidOperationException("There are no items with positive weight in the DynamicWeightedRandomizer");
        }

        /// <summary>
        /// Shortcut syntax to add, remove, and update an item
        /// </summary>
        public int this[TKey key]
        {
            get
            {
                return GetWeight(key);
            }
            set
            {
                SetWeight(key, value);
            }
        }

        /// <summary>
        /// Returns the weight of the given item.  Throws an exception if the item is not added
        /// (use .Contains to check first if unsure)
        /// </summary>
        public int GetWeight(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key", "key cannot be null");

            Node node = FindNode(_root, key);
            if(node == null)
                throw new KeyNotFoundException("Key not found in DynamicWeightedRandomizer: " + key);

            return node.weight;
        }

        /// <summary>
        /// Updates the weight of the given item, or adds it if it has not already been added.
        /// If weight &lt;= 0, the item is removed.
        /// </summary>
        public void SetWeight(TKey key, int weight)
        {
            if(weight < 0)
            {
                throw new ArgumentOutOfRangeException("weight", weight, "Cannot add a weight with value < 0");
            }

            Node node = FindNode(_root, key);
            if(node == null)
            {
                Add(key, weight);
            }
            else
            {
                int weightDelta = weight - node.weight;

                //This is a hack.  The point is to update this node's and all it's ancestors' subtreeWeights.
                //We already have a method that will do that; however, it uses the value of node.weight, rather
                //than a parameter.
                node.weight = weightDelta;
                UpdateSubtreeWeightsForInsertion(node);

                //Finally, set the node.weight to what it should be
                node.weight = weight;
                node.subtreeWeight += weightDelta;
            }
        }
        #endregion

        #region Debugging code
        /// <summary>
        /// Returns the height of the tree (very slow)
        /// </summary>
        private int Height
        {
            get
            {
                return GetNumLayers(_root);
            }
        }

        private int GetNumLayers(Node node)
        {
            if(node == null || node == _sentinel)
                return 0;
            return Math.Max(GetNumLayers(node.left), GetNumLayers(node.right)) + 1;
        }

        /// <summary>
        /// Quick hack to write quick tests
        /// </summary>
        private void Assert(bool condition)
        {
            if(!condition)
                throw new ArgumentException("Test case failed");
        }

        /// <summary>
        /// Make sure the entire tree is valid (correct subtreeWeights, valid BST, that sort of thing)
        /// </summary>
        private void DebugCheckTree()
        {
            DebugCheckNode(_root);
            Assert(Count == 0 || Height <= 2 * Math.Ceiling(Math.Log(Count, 2) + 1));
        }

        private void DebugCheckNode(Node node)
        {
            if(node == null || node == _sentinel)
                return;

            Assert(node.left == null || node.left == _sentinel || node.left.key.CompareTo(node.key) < 0);
            Assert(node.right == null || node.right == _sentinel || node.right.key.CompareTo(node.key) > 0);
            Assert(node.left.subtreeWeight + node.right.subtreeWeight + node.weight == node.subtreeWeight);

            DebugCheckNode(node.left);
            DebugCheckNode(node.right);
        }
        #endregion
    }
}