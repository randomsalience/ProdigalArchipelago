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
        public bool ShuffleBjergCastle;
        public bool ShuffleDaemonsDive;
        public bool ShuffleEnlightenment;
        public bool ShuffleSecretShop;
        public bool LongJumpsInLogic; // for backward compatibility
        public bool SkipsInLogic;
        public bool StartWithSpicedHam;
        public bool SkipOneSmallFavor;
        public bool FakeDreadHand;
        public bool FastFishing;
        public bool AltarToVar;
        public bool AltarToZolei;
        public bool AltarToRaem;
        public bool AltarToHate;
        public bool CurseOfFrailty;
        public bool CurseOfFamine;
        public bool CurseOfRust;
        public bool CurseOfWind;
        public bool CurseOfBlooms;
        public bool CurseOfCrowns;
        public bool CurseOfHorns;
        public bool CurseOfFlames;

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
            ShuffleBjergCastle = GetOrDefault(slotData, "shuffle_bjerg_castle", 0) != 0;
            ShuffleDaemonsDive = GetOrDefault(slotData, "shuffle_daemons_dive", 0) != 0;
            ShuffleEnlightenment = GetOrDefault(slotData, "shuffle_enlightenment", 0) != 0;
            ShuffleSecretShop = GetOrDefault(slotData, "shuffle_secret_shop", 0) != 0;
            LongJumpsInLogic = GetOrDefault(slotData, "long_jumps_in_logic", 0) != 0;
            SkipsInLogic = GetOrDefault(slotData, "skips_in_logic", 0) != 0;
            StartWithSpicedHam = GetOrDefault(slotData, "start_with_spiced_ham", 0) != 0;
            SkipOneSmallFavor = GetOrDefault(slotData, "skip_one_small_favor", 0) != 0;
            FakeDreadHand = GetOrDefault(slotData, "fake_dread_hand", 0) != 0;
            FastFishing = GetOrDefault(slotData, "fast_fishing", 0) != 0;
            AltarToVar = GetOrDefault(slotData, "altar_to_var", 0) != 0;
            AltarToZolei = GetOrDefault(slotData, "altar_to_zolei", 0) != 0;
            AltarToRaem = GetOrDefault(slotData, "altar_to_raem", 0) != 0;
            AltarToHate = GetOrDefault(slotData, "altar_to_hate", 0) != 0;
            CurseOfFrailty = GetOrDefault(slotData, "curse_of_frailty", 0) != 0;
            CurseOfFamine = GetOrDefault(slotData, "curse_of_famine", 0) != 0;
            CurseOfRust = GetOrDefault(slotData, "curse_of_rust", 0) != 0;
            CurseOfWind = GetOrDefault(slotData, "curse_of_wind", 0) != 0;
            CurseOfBlooms = GetOrDefault(slotData, "curse_of_blooms", 0) != 0;
            CurseOfCrowns = GetOrDefault(slotData, "curse_of_crowns", 0) != 0;
            CurseOfHorns = GetOrDefault(slotData, "curse_of_horns", 0) != 0;
            CurseOfFlames = GetOrDefault(slotData, "curse_of_flames", 0) != 0;
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