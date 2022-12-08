using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weighted_Randomizer;

namespace Station
{
    public static class LootUtils
    {
        private static System.Random _staticRandom;
        public static Dictionary<string, long> GenerateCurrencies(string lootTableId)
        {
            Dictionary<string, long> currencies = new Dictionary<string, long>();

            var lootTableDb = GameInstance.GetDb<LootTableDb>();
            if (lootTableDb.HasKey(lootTableId))
            {
                var lootTableModel = lootTableDb.GetEntry(lootTableId);
                foreach (var model in lootTableModel.Currencies)
                {
                    float random = Random.Range(0, 100);
                    if (random <= model.Chance)
                    {
                        if (_staticRandom == null)
                        {
                            _staticRandom =  new System.Random();
                        }
                        long amount = _staticRandom.NextLong(model.AmountMin, model.AmountMax);
                        
                        currencies.Add(model.Currency.Key, amount);
                    }
                }
            }

       
            return currencies;
        }

    public static List<ItemStack> GenerateLootStack(IEnumerable<LootModel> modelList)
        {
            var generate = new List<ItemStack>();
            foreach (var model in modelList)
            {
                float random = Random.Range(0, 100);
                if (random <= model.Chance)
                {
                    var stack = new ItemStack
                    {
                        ItemId = model.ItemId, ItemCount = Random.Range(model.QuantityMin, model.QuantityMax)
                    };
                    generate.Add(stack);
                }
            }
            
            return generate;
        }
        
        public static List<ItemStack> GenerateLootStack(string lootTableId)
        {
            var generate = new List<ItemStack>();
            var lootTableDb = GameInstance.GetDb<LootTableDb>();
            if (lootTableDb.HasKey(lootTableId))
            {
                var lootTableModel = lootTableDb.GetEntry(lootTableId);
                foreach (var model in lootTableModel.Loots)
                {
                    float random = Random.Range(0, 100);
                    if (random <= model.Chance)
                    {
                        var stack = new ItemStack
                        {
                            ItemId = model.ItemId, ItemCount = Random.Range(model.QuantityMin, model.QuantityMax)
                        };
                        generate.Add(stack);
                    }
                }
            }

            return generate;
        }
    }

}

