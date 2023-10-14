using System;
using System.Collections.Generic;

namespace ProdigalArchipelago
{
    public class ArchipelagoSettings(Dictionary<string, object> slotData)
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

        public bool SpecificKeys = GetOrDefault(slotData, "specific_keys", 1) != 0;
        public int ColorsRequired = GetOrDefault(slotData, "colors_required", 5);
        public int BlessingsRequired = GetOrDefault(slotData, "blessings_required", 0);
        public int CrestFragmentsRequired = GetOrDefault(slotData, "crest_fragments_required", 5);
        public int CoinsOfCrowlRequired = GetOrDefault(slotData, "coins_of_crowl_required", 13);
        public GoalOption Goal = (GoalOption)GetOrDefault(slotData, "goal", 0);
        public TradingQuestOption TradingQuest = (TradingQuestOption)GetOrDefault(slotData, "trading_quest", 0);
        public bool ShuffleGrelinDrops = GetOrDefault(slotData, "shuffle_grelin_drops", 0) != 0;
        public bool ShuffleHiddenItems = GetOrDefault(slotData, "shuffle_hidden_items", 0) != 0;
        public bool ShuffleBjergCastle = GetOrDefault(slotData, "shuffle_bjerg_castle", 0) != 0;
        public bool ShuffleDaemonsDive = GetOrDefault(slotData, "shuffle_daemons_dive", 0) != 0;
        public bool ShuffleEnlightenment = GetOrDefault(slotData, "shuffle_enlightenment", 0) != 0;
        public bool ShuffleSecretShop = GetOrDefault(slotData, "shuffle_secret_shop", 0) != 0;
        public bool LongJumpsInLogic = GetOrDefault(slotData, "long_jumps_in_logic", 0) != 0; // for backward compatibility
        public bool SkipsInLogic = GetOrDefault(slotData, "skips_in_logic", 0) != 0;
        public bool StartWithSpicedHam = GetOrDefault(slotData, "start_with_spiced_ham", 0) != 0;
        public bool StartWithWingedBoots = GetOrDefault(slotData, "start_with_winged_boots", 0) != 0;
        public bool SkipOneSmallFavor = GetOrDefault(slotData, "skip_one_small_favor", 0) != 0;
        public bool FakeDreadHand = GetOrDefault(slotData, "fake_dread_hand", 0) != 0;
        public bool FastFishing = GetOrDefault(slotData, "fast_fishing", 0) != 0;
        public bool AltarToVar = GetOrDefault(slotData, "altar_to_var", 0) != 0;
        public bool AltarToZolei = GetOrDefault(slotData, "altar_to_zolei", 0) != 0;
        public bool AltarToRaem = GetOrDefault(slotData, "altar_to_raem", 0) != 0;
        public bool AltarToHate = GetOrDefault(slotData, "altar_to_hate", 0) != 0;
        public bool CurseOfFrailty = GetOrDefault(slotData, "curse_of_frailty", 0) != 0;
        public bool CurseOfFamine = GetOrDefault(slotData, "curse_of_famine", 0) != 0;
        public bool CurseOfRust = GetOrDefault(slotData, "curse_of_rust", 0) != 0;
        public bool CurseOfWind = GetOrDefault(slotData, "curse_of_wind", 0) != 0;
        public bool CurseOfBlooms = GetOrDefault(slotData, "curse_of_blooms", 0) != 0;
        public bool CurseOfCrowns = GetOrDefault(slotData, "curse_of_crowns", 0) != 0;
        public bool CurseOfHorns = GetOrDefault(slotData, "curse_of_horns", 0) != 0;
        public bool CurseOfFlames = GetOrDefault(slotData, "curse_of_flames", 0) != 0;

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