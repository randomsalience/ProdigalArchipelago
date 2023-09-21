using System;
using System.Collections.Generic;

namespace ProdigalArchipelago
{
    public class ArchipelagoSettings
    {
        public enum GoalOption
        {
            Var = 0,
            Rest = 1,
            Shadow = 2,
            Torran = 3,
            Any = 4,
        }

        public enum TradingQuestOption
        {
            Vanilla = 0,
            Skip = 1,
            Shuffle = 2,
        }

        public bool SpecificKeys;
        public int ColorsRequired;
        public int BlessingsRequired;
        public int CrestFragmentsRequired;
        public int CoinsOfCrowlRequired;
        public GoalOption Goal;
        public TradingQuestOption TradingQuest;
        public bool ShuffleGrelinDrops;
        public bool ShuffleHiddenItems;
        public bool ShuffleDaemonsDive;
        public bool ShuffleEnlightenment;
        public bool ShuffleSecretShop;
        public bool LongJumpsInLogic;
        public bool StartWithSpicedHam;
        public bool SkipOneSmallFavor;
        public bool FakeDreadHand;
        public bool FastFishing;

        public ArchipelagoSettings(Dictionary<string, object> slotData)
        {
            SpecificKeys = GetOrDefault(slotData, "specific_keys", 1) != 0;
            ColorsRequired = GetOrDefault(slotData, "colors_required", 5);
            BlessingsRequired = GetOrDefault(slotData, "blessings_required", 0);
            CrestFragmentsRequired = GetOrDefault(slotData, "crest_fragments_required", 5);
            CoinsOfCrowlRequired = GetOrDefault(slotData, "coins_of_crowl_required", 13);
            Goal = (GoalOption)GetOrDefault(slotData, "goal", 0);
            TradingQuest = (TradingQuestOption)GetOrDefault(slotData, "trading_quest", 0);
            ShuffleGrelinDrops = GetOrDefault(slotData, "shuffle_grelin_drops", 0) != 0;
            ShuffleHiddenItems = GetOrDefault(slotData, "shuffle_hidden_items", 0) != 0;
            ShuffleDaemonsDive = GetOrDefault(slotData, "shuffle_daemons_dive", 0) != 0;
            ShuffleEnlightenment = GetOrDefault(slotData, "shuffle_enlightenment", 0) != 0;
            ShuffleSecretShop = GetOrDefault(slotData, "shuffle_secret_shop", 0) != 0;
            LongJumpsInLogic = GetOrDefault(slotData, "long_jumps_in_logic", 0) != 0;
            StartWithSpicedHam = GetOrDefault(slotData, "start_with_spiced_ham", 0) != 0;
            SkipOneSmallFavor = GetOrDefault(slotData, "skip_one_small_favor", 0) != 0;
            FakeDreadHand = GetOrDefault(slotData, "fake_dread_hand", 0) != 0;
            FastFishing = GetOrDefault(slotData, "fast_fishing", 0) != 0;
        }

        public bool GoalVar()
        {
            return Goal == GoalOption.Var || Goal == GoalOption.Any;
        }

        public bool GoalRest()
        {
            return Goal == GoalOption.Rest || Goal == GoalOption.Any;
        }

        public bool GoalShadow()
        {
            return Goal == GoalOption.Shadow || Goal == GoalOption.Any;
        }

        public bool GoalTorran()
        {
            return Goal == GoalOption.Torran || Goal == GoalOption.Any;
        }

        public static int GetOrDefault(Dictionary<string, object> slotData, string key, int dflt)
        {
            if (!slotData.TryGetValue(key, out object value))
            {
                return dflt;
            }
            else
            {
                try
                {
                    return (int)(long)value;
                }
                catch (InvalidCastException)
                {
                    return dflt;
                }
            }
        }
    }
}