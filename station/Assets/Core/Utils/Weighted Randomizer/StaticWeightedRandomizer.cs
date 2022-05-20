using System;
using System.Collections;
using System.Collections.Generic;

namespace Weighted_Randomizer
{
    /// <summary>
    /// A weighted randomizer implementation which uses Vose's alias method.  It is very fast when doing many contiguous calls to NextWithReplacement().
    /// It is slow when making making calls to NextWithRemoval(), or when adding/removing/updating items often between calls to NextWithReplacement().
    /// </summary>
    /// <typeparam name="TKey">The type of the objects to choose at random</typeparam>
    public class StaticWeightedRandomizer<TKey> : IWeightedRandomizer<TKey>
    {
        private readonly ThreadAwareRandom _random;
        private readonly Dictionary<TKey, int> _weights;
        private bool _listNeedsRebuilding;

        private readonly IList<ProbabilityBox> _probabilityBoxes;
        private long _heightPerBox;

        /// <summary>
        /// The discrete boxes used to hold the keys/aliases in Vose's alias method.  Since we're using integers rather than floating-point
        /// probabilities, I've chosen the word "balls" for the value of the coin-flip used to determine whether to choose the key or the alias
        /// from the box.  If the number of balls chosen (taken from 1 to _heightPerBox) is <= NumBallsInBox, we choose the Key; otherwise,
        /// we choose the Alias.  Thus, there is exactly a NumBallsInBox/_heightPerBox probability of choosing the Key.
        /// </summary>
        private struct ProbabilityBox
        {
            public TKey Key { get; private set; }
            public TKey Alias { get; private set; }
            public long NumBallsInBox { get; private set; }

            public ProbabilityBox(TKey key, TKey alias, long numBallsInBox)
                : this()
            {
                Key = key;
                Alias = alias;
                NumBallsInBox = numBallsInBox;
            }
        }

        /// <summary>
        /// Create a new StaticWeightedRandomizer
        /// </summary>
        public StaticWeightedRandomizer() : this(new ThreadAwareRandom()) { }

        /// <summary>
        /// Create a new StaticWeightedRandomizer with the given seed
        /// </summary>
        public StaticWeightedRandomizer(int seed) : this(new ThreadAwareRandom(seed)) {}

        /// <summary>
        /// A quick hack that allows me to share code between the two constructors but still set
        /// readonly fields
        /// </summary>
        private StaticWeightedRandomizer(ThreadAwareRandom random)
        {
            _random = random;
            _weights = new Dictionary<TKey, int>();
            _listNeedsRebuilding = true;
            TotalWeight = 0;

            _probabilityBoxes = new List<ProbabilityBox>();
            _heightPerBox = 0;
        }

        #region ICollection<T> stuff
        /// <summary>
        /// Returns the number of items currently in the list
        /// </summary>
        public int Count { get { return _weights.Keys.Count; } }

        /// <summary>
        /// Remove all items from the list
        /// </summary>
        public void Clear()
        {
            _weights.Clear();
            _listNeedsRebuilding = true;
            TotalWeight = 0;
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
            return _weights.ContainsKey(key);
        }

        /// <summary>
        /// Adds the given item with a default weight of 1
        /// </summary>
        public void Add(TKey key)
        {
            Add(key, 1);
        }

        /// <summary>
        /// Adds the given item with the given weight.  Higher weights are more likely to be chosen.
        /// </summary>
        public void Add(TKey key, int weight)
        {
            if(weight < 0)
            {
                throw new ArgumentOutOfRangeException("weight", weight, "Cannot add a key with weight < 0!");
            }

            _weights.Add(key, weight);
            _listNeedsRebuilding = true;
            TotalWeight += weight;
        }

        /// <summary>
        /// Remoevs the given item from the list.
        /// </summary>
        /// <returns>Returns true if the item was successfully deleted, or false if it was not found</returns>
        public bool Remove(TKey key)
        {
            int weight;
            if(!_weights.TryGetValue(key, out weight))
            {
                return false;
            }

            TotalWeight -= weight;
            _listNeedsRebuilding = true;

            //Preemptively clear the _probabilityBoxes list, so we don't unnecessarily hold on to unused references
            _probabilityBoxes.Clear();

            return _weights.Remove(key);
        }
        #endregion

        #region IEnumerable<T> stuff
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            return _weights.Keys.GetEnumerator();
        }
        #endregion

        #region IWeightedRandomizer<T> stuff
        /// <summary>
        /// The total weight of all the items added so far
        /// </summary>
        public long TotalWeight { get; private set; }

        /// <summary>
        /// Returns an item chosen randomly by weight (higher weights are more likely),
        /// and replaces it so that it can be chosen again
        /// </summary>
        public TKey NextWithReplacement()
        {
            VerifyHaveItemsToChooseFrom();

            if(_listNeedsRebuilding)
            {
                RebuildProbabilityList();
            }

            //Choose a random box, then flip a biased coin (represented by choosing a number of balls within the box)
            int randomIndex = _random.Next(_probabilityBoxes.Count);
            long randomNumBalls = _random.NextLong(_heightPerBox) + 1;

            if(randomNumBalls <= _probabilityBoxes[randomIndex].NumBallsInBox)
            {
                return _probabilityBoxes[randomIndex].Key;
            }
            else
            {
                return _probabilityBoxes[randomIndex].Alias;
            }
        }

        private struct KeyBallsPair
        {
            public TKey Key;
            public long NumBalls;
        }

        private void RebuildProbabilityList()
        {
            long gcd = GreatestCommonDenominator(Count, TotalWeight);
            long weightMultiplier = Count / gcd;
            _heightPerBox = TotalWeight / gcd;

            Stack<KeyBallsPair> smallStack = new Stack<KeyBallsPair>();
            Stack<KeyBallsPair> largeStack = new Stack<KeyBallsPair>();

            DistributeKeysIntoStacks(weightMultiplier, largeStack, smallStack);
            CreateSplitProbabilityBoxes(largeStack, smallStack);
            AddRemainingProbabilityBoxes(smallStack);

            _listNeedsRebuilding = false;
        }

        /// <summary>
        /// Step one:  Load the small list with all items whose total weight is less than _heightPerBox (after scaling)
        /// the large list with those that are greater.
        /// </summary>
        private void DistributeKeysIntoStacks(long weightMultiplier, Stack<KeyBallsPair> largeStack, Stack<KeyBallsPair> smallStack)
        {
            _probabilityBoxes.Clear();
            foreach (TKey item in _weights.Keys)
            {
                long newWeight = _weights[item]*weightMultiplier;
                if (newWeight > _heightPerBox)
                {
                    largeStack.Push(new KeyBallsPair {Key = item, NumBalls = newWeight});
                }
                else
                {
                    smallStack.Push(new KeyBallsPair {Key = item, NumBalls = newWeight});
                }
            }
        }

        /// <summary>
        /// Step two:  Pair up each item in the large/small lists and create a probability box for them
        /// </summary>
        private void CreateSplitProbabilityBoxes(Stack<KeyBallsPair> largeStack, Stack<KeyBallsPair> smallStack)
        {
            while (largeStack.Count != 0)
            {
                KeyBallsPair largeItem = largeStack.Pop();
                KeyBallsPair smallItem = smallStack.Pop();
                _probabilityBoxes.Add(new ProbabilityBox(smallItem.Key, largeItem.Key, smallItem.NumBalls));

                //Set the new weight for the largeList item, and move it to smallList if necessary
                long difference = _heightPerBox - smallItem.NumBalls;
                largeItem.NumBalls = largeItem.NumBalls - difference;
                if (largeItem.NumBalls > _heightPerBox)
                {
                    largeStack.Push(largeItem);
                }
                else
                {
                    smallStack.Push(largeItem);
                }
            }
        }

        /// <summary>
        /// Step three:  All the remining items in smallList necessarily have probability of 100%
        /// </summary>
        private void AddRemainingProbabilityBoxes(Stack<KeyBallsPair> smallStack)
        {
            while (smallStack.Count != 0)
            {
                KeyBallsPair smallItem = smallStack.Pop();
                _probabilityBoxes.Add(new ProbabilityBox(smallItem.Key, smallItem.Key, _heightPerBox));
            }
        }

        private static long GreatestCommonDenominator(long a, long b)
        {
            while(b > 0)
            {
                long remainder = a % b;
                if(remainder == 0)
                    return b;
                a = b;
                b = remainder;
            }

            return a;
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
                throw new InvalidOperationException("There are no items in the StaticWeightedRandomizer");
            if (TotalWeight <= 0)
                throw new InvalidOperationException("There are no items with positive weight in the StaticWeightedRandomizer");
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

            int weight;
            if(!_weights.TryGetValue(key, out weight))
                throw new KeyNotFoundException("Key not found in StaticWeightedRandomizer: " + key);

            return weight;
        }

        /// <summary>
        /// Updates the weight of the given item, or adds it if it has not already been added.
        /// If weight &lt;= 0, the item is removed.
        /// </summary>
        public void SetWeight(TKey key, int weight)
        {
            if (weight < 0)
            {
                throw new ArgumentOutOfRangeException("weight", weight, "Cannot add a weight with value < 0");
            }

            if(Contains(key))
            {
                TotalWeight += (weight - _weights[key]);
                _weights[key] = weight;
            }
            else
            {
                Add(key, weight);
            }
            _listNeedsRebuilding = true;
        }
        #endregion
    }
}
