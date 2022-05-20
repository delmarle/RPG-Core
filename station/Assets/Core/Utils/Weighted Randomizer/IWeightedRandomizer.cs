using System;
using System.Collections.Generic;

namespace Weighted_Randomizer
{
    /// <summary>
    /// Represents a class which can choose weighted items at random; that is, it can randomly choose items from a list, giving some items higher
    /// probability of being chosen than others.  It supports both choosing with replacement (so the same item can be chosen multiple times) and
    /// choosing with removal (so each item can be chosen only once).
    /// 
    /// Note that though this interface is enumerable, the enumeration is not necessarily ordered by anything.
    /// </summary>
    /// <typeparam name="TKey">The type of the objects to choose at random</typeparam>
    public interface IWeightedRandomizer<TKey> : ICollection<TKey>
    {
        /// <summary>
        /// The total weight of all the items added so far
        /// </summary>
        long TotalWeight { get; }

        /// <summary>
        /// Returns an item chosen randomly by weight (higher weights are more likely),
        /// and replaces it so that it can be chosen again
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the collection is empty or has only 0-weight items
        /// </exception>
        TKey NextWithReplacement();

        /// <summary>
        /// Returns an item chosen randomly by weight (higher weights are more likely),
        /// and removes it so it cannot be chosen again
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the collection is empty or has only 0-weight items
        /// </exception>
        TKey NextWithRemoval();

        /// <summary>
        /// Adds the given item with the given weight.  Higher weights are more likely to be chosen.
        /// If the key already exists in the collection, an exception is thrown.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if weight &lt; 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the key already exists in the collection
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the key is null
        /// </exception>
        void Add(TKey key, int weight);

        /// <summary>
        /// Shortcut syntax to add, remove, and update an item.  Higher weights are more likely to be chosen.
        /// </summary>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if attempting to retrieve a key which does not exist in the collection
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if attempting to set the weight to a value &lt; 0.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the key is null
        /// </exception>
        int this[TKey key] { get; set; }

        /// <summary>
        /// Returns the weight of the given item.  Throws an exception if the item is not added
        /// (use .Contains to check first if unsure)
        /// </summary>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if the key does not exist in the collection
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the key is null
        /// </exception>
        int GetWeight(TKey key);

        /// <summary>
        /// Updates the weight of the given item, or adds it to the collection if it has not already been added.
        /// Higher weights are more likely to be chosen.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if attempting to set the weight to a value &lt; 0.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the key is null
        /// </exception>
        void SetWeight(TKey key, int weight);
    }
}
