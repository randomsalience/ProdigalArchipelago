using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace ProdigalArchipelago
{
    class MapTracker : MonoBehaviour
    {
        public static GameObject Instance;
        public static GameObject Canvas;
        public List<TrackerDot> Dots;

        public static void Create()
        {
            GameObject.Find("Main Camera90").AddComponent<Physics2DRaycaster>();

            Instance = new("MapTracker");
            Instance.transform.SetParent(GameMaster.GM.UI.transform.GetChild(2).GetChild(1));
            Instance.AddComponent<EventSystem>();
            Instance.AddComponent<InputSystemUIInputModule>();

            Canvas = new("MapTrackerCanvas");
            Canvas.transform.SetParent(Instance.transform);
            var canvas = Canvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 11;

            var tracker = Instance.AddComponent<MapTracker>();
            tracker.SetupDots();
        }

        public static void SetupTracker()
        {
            Instance.GetComponent<MapTracker>().Setup();
        }

        private void Setup()
        {
            foreach (TrackerDot dot in Dots)
            {
                foreach (TrackerLocation location in dot.Locations)
                {
                    if (location.ID == 223)
                    {
                        location.Name = Archipelago.AP.Settings.TradingQuest == ArchipelagoSettings.TradingQuestOption.Shuffle ? "Ulni Trade" : "Complete Trading Quest";
                    }
                }
            }
        }

        private void OnEnable()
        {
            foreach (var dot in Dots)
            {
                dot.gameObject.SetActive(Archipelago.Enabled);
                dot.ClearTextBox();
                if (Archipelago.Enabled)
                    dot.SetColor();
            }
        }

        private void SetupDots()
        {
            Dots = new()
            {
                TrackerDot.NewDot("Warehouse", -11, -45, false, () => true, new() {
                    new TrackerLocation("Warehouse Freestanding", 183, () => true),
                }),
                TrackerDot.NewDot("Pond", 45, -15, false, () => true, new() {
                    new TrackerLocation("Pond Freestanding", 182, () => HasPick() || HasLariat() || HasKnuckle()),
                }),
                TrackerDot.NewDot("Island", 68, 1, false, () => true, new() {
                    new TrackerLocation("Island Freestanding", 181, () => HasLariat()),
                }),
                TrackerDot.NewDot("Mountain", 52, 20, false, () => true, new() {
                    new TrackerLocation("Mountain Freestanding", 180, () => (HasKnuckle() && HasLariat()) || (CanClimb() && (Skips() || CanHit()))),
                }),
                TrackerDot.NewDot("Pa's House", 26, -23, false, () => true, new() {
                    new TrackerLocation("Pa's Desk", 227, () => true),
                    new TrackerLocation("Light Spirit", 237, () => TimeOut2() && HasKnuckle() && HasLariat() && HasKey(Key.TimeOut, 3),
                        () => TimeOut2() && HasKnuckle() && HasLariat() && (CanOpen(Key.TimeOut, 36, 37) || (HasFlare() && CanOpen(Key.TimeOut, 36)))),
                }),
                TrackerDot.NewDot("Old House", 11, 49, false, () => true, new() {
                    new TrackerLocation("Music Box", 228, () => CanClimb() && Item.Harmonica.Acquired()),
                }),
                TrackerDot.NewDot("Flooded Shack", 71, -54, false, () => true, new() {
                    new TrackerLocation("Drowned Gift", 88, () => HasCoins() && CanHit() && (Item.AnchorGreaves.Acquired() || Item.BootsOfGraile.Acquired())),
                }),
                TrackerDot.NewDot("Near Mine", -74, -30, false, () => true, new() {
                    new TrackerLocation("Heart Ore", 146, () => HasPick() && (HasKnuckle() || (Skips() && Item.WeaponChain.Acquired()))),
                }),
                TrackerDot.NewDot("Near Siska", -37, 40, false, () => true, new() {
                    new TrackerLocation("Heart Ore", 147, () => HasPick() && (HasLariat() || (Skips() && HasFlare()))),
                }),
                TrackerDot.NewDot("Near Smithy", 29, -32, false, () => true, new() {
                    new TrackerLocation("Heart Ore", 148, () => HasPick() && HasLariat()),
                }),
                TrackerDot.NewDot("Near Pond", 66, -30, false, () => true, new() {
                    new TrackerLocation("Heart Ore", 145, () => HasPick()),
                }),
                TrackerDot.NewDot("Near Magma Heart", 67, 56, false, () => true, new() {
                    new TrackerLocation("Heart Ore", 144, () => HasPick() && (CanClimb() || (HasLariat() && HasKnuckle()))),
                }),
                TrackerDot.NewDot("Casino", 38, 5, false, () => true, new() {
                    new TrackerLocation("Tara Reward", 200, () => true),
                    new TrackerLocation("Crocodile", 209, () => true),
                    new TrackerLocation("Cactus Hidden Item", 175, () => true),
                }),
                TrackerDot.NewDot("Boot Shop", 12, -12, false, () => true, new() {
                    new TrackerLocation("Tess", 201, () => true),
                    new TrackerLocation("Tess Boots", 210, () => Has4Boots() && Item.OldHairpin.Acquired()),
                    new TrackerLocation("Tess Trade", 206, () => Item.LostShipment.Acquired()),
                }),
                TrackerDot.NewDot("Hackett", -53, -7, false, () => true, new() {
                    new TrackerLocation("Purchase Item", 202, () => CanHit()),
                }),
                TrackerDot.NewDot("Grant", -49, -13, false, () => true, new() {
                    new TrackerLocation("Stealth Mission", 203, () => Item.CursedBones.Acquired() && Item.CursedPick.Acquired()),
                }),
                TrackerDot.NewDot("Arena", -26, -41, false, () => true, new() {
                    new TrackerLocation("Mariana", 204, () => CanHit()),
                }),
                TrackerDot.NewDot("Red's Inn", 7, -52, false, () => true, new() {
                    new TrackerLocation("Keaton Fishing Gift", 205, () => HasLariat()),
                }),
                TrackerDot.NewDot("Church", -8, -14, false, () => true, new() {
                    new TrackerLocation("Xavier Blessing", 208, () => Item.CursedBones.Acquired() && Item.CursedPick.Acquired()),
                }),
                TrackerDot.NewDot("Lynn", 3, -40, false, () => true, new() {
                    new TrackerLocation("Gift", 211, () => CanReturnStolenItems()),
                }),
                TrackerDot.NewDot("Bolivar", 19, -24, false, () => true, new() {
                    new TrackerLocation("Gift", 212, () => Item.Shaedrite.Acquired() && Item.DrownedOre.Acquired() && Item.MiasmicExtract.Acquired() && Item.BrokenSword.Acquired()),
                }),
                TrackerDot.NewDot("Hooded Figure", 15, 45, false, () => true, new() {
                    new TrackerLocation("Reward", 213, () => Item.EerieMask.Acquired() && HasHand2() && CanClimb() && CanHit()),
                }),
                TrackerDot.NewDot("Farmhouse", 11, -40, false, () => true, new() {
                    new TrackerLocation("Quinlan Trade", 207, () => Item.TheCarrotCake.Acquired()),
                }),
                TrackerDot.NewDot("On Farm", 0, -40, false, () => true, new() {
                    new TrackerLocation("Bush Hidden Item", 141, () => true),
                }),
                TrackerDot.NewDot("Near Bridge", 17, -3, false, () => true, new() {
                    new TrackerLocation("Bush Hidden Item", 137, () => true),
                }),
                TrackerDot.NewDot("Near Bench", -46, -45, false, () => true, new() {
                    new TrackerLocation("Bush Hidden Item", 139, () => true),
                }),
                TrackerDot.NewDot("Near River's House", 73, -19, false, () => true, new() {
                    new TrackerLocation("Bush Hidden Item", 162, () => CanHit() || HasLariat()),
                }),
                TrackerDot.NewDot("Near Old House", 16, 42, false, () => true, new() {
                    new TrackerLocation("Bush Hidden Item", 0, () => CanClimb()),
                }),
                TrackerDot.NewDot("Near Boot Shop", 7, -11, false, () => true, new() {
                    new TrackerLocation("Crate Hidden Item", 140, () => true),
                }),
                TrackerDot.NewDot("Near Tidal Mines", 53, -52, false, () => true, new() {
                    new TrackerLocation("Crate Hidden Item", 138, () => CanHit()),
                }),
                TrackerDot.NewDot("Hidden Dock", 61, -59, false, () => true, new() {
                    new TrackerLocation("Chest Hidden Item", 176, () => CanHit()),
                }),
                TrackerDot.NewDot("Ashwood's Manor", -24, -13, false, () => true, new() {
                    new TrackerLocation("Plant Hidden Item", 184, () => true),
                }),
                TrackerDot.NewDot("Colorless Void", 52, -7, true, () => true, new() {
                    new TrackerLocation("Near Portal Heart Ore", 165, () => HasPick()),
                    new TrackerLocation("Near Burg Heart Ore", 166, () => HasPick()),
                    new TrackerLocation("Near Tavern Heart Ore", 167, () => HasPick() && HasLariat()),
                    new TrackerLocation("Near Waterfall Heart Ore", 168, () => HasPick()),
                    new TrackerLocation("Vulhara Trade", 214, () => Item.Coffee.Acquired() && HasLariat()),
                    new TrackerLocation("Reskel Trade", 215, () => Item.TatteredCape.Acquired() && HasLariat()),
                    new TrackerLocation("Mynir Trade", 216, () => Item.BallOfYarn.Acquired() && HasLariat()),
                    new TrackerLocation("Orima Trade", 217, () => Item.SlimeSoap.Acquired()),
                    new TrackerLocation("Wren Trade", 218, () => Item.SerpentBracelet.Acquired()),
                    new TrackerLocation("Leer Trade", 219, () => Item.HuntingBow.Acquired()),
                    new TrackerLocation("Burg Trade", 220, () => Item.DownPillow.Acquired()),
                    new TrackerLocation("Crelon Trade", 221, () => Item.GiantsMonocle.Acquired() && HasLariat()),
                    new TrackerLocation("Tedra Trade", 222, () => Item.ForbiddenBook.Acquired()),
                    new TrackerLocation("Ulni Trade", 223, () => CanTradeWithUlni()),
                }),
                TrackerDot.NewDot("Abandoned Mine", -72, -35, false, () => true, new() {
                    new TrackerLocation("Iron Pick Chest", 2, () => true),
                    new TrackerLocation("Lower Chest", 44, () => HasLariat() && HasKnuckle()),
                }),
                TrackerDot.NewDot("Waterfall Cave", 55, 15, false, () => CanClimb(), new() {
                    new TrackerLocation("Skeleton", 135, () => (HasPick() || (Skips() && HasKnuckle())) && HasLariat()),
                }),
                TrackerDot.NewDot("Celina's Mine", 45, 34, false, () => Item.Bandana.Acquired() && ((CanClimb() && CanHit()) || (HasKnuckle() && HasLariat())), new() {
                    new TrackerLocation("Mine Treasure", 179, () => HasPick()),
                }),
                TrackerDot.NewDot("Cursed Grave", -39, 11, false, () => Item.CursedBones.Acquired(), new() {
                    new TrackerLocation("Top Chest", 49, () => CanHit()),
                    new TrackerLocation("Center Chest", 48, () => CanHit()),
                    new TrackerLocation("Bottom Chest", 47, () => CanHit()),
                    new TrackerLocation("Lariat Target Chest", 46, () => CanHit() && (HasLariat() || HasFlare())),
                    new TrackerLocation("Biggun", 45, () => CanHit() && (HasLariat() || HasFlare())),
                }),
                TrackerDot.NewDot("Various Locations", -70, 63, false, () => true, new() {
                    new TrackerLocation("Grelin Drop 1", 240, () => CanKillGrelins()),
                    new TrackerLocation("Grelin Drop 2", 241, () => CanKillGrelins()),
                    new TrackerLocation("Grelin Drop 3", 242, () => CanKillGrelins()),
                    new TrackerLocation("Grelin Drop 4", 243, () => CanKillGrelins()),
                }),
                TrackerDot.NewDot("Boneyard", -66, 19, true, () => CanHit(), new() {
                    new TrackerLocation("Ball Chest", 5, () => HasKnuckle() && (HasLariat() || HasFlare() || HasHand2() || Skips())),
                    new TrackerLocation("Right Side Heart Ore", 163, () => HasPick() && (HasKnuckle() || (Skips() && (HasLariat() || HasHand2())))),
                    new TrackerLocation("Left Hidden Chest", 7, () => CanHit()),
                    new TrackerLocation("Right Hidden Chest", 8, () => CanHit()),
                    new TrackerLocation("Bottom Hidden Chest", 6, () => CanHit()),
                    new TrackerLocation("Bats Chest", 3, () => HasPick() || (Skips() && HasKnuckle() && HasLariat())),
                    new TrackerLocation("Dread Hand Chest", 1, () => HasKey(Key.Boneyard, 1) && (HasPick() || (HasLariat() && HasKnuckle())),
                        () => CanOpen(Key.Boneyard, 0) && (HasPick() || (HasLariat() && HasKnuckle()))),
                    new TrackerLocation("Near Boss Heart Ore", 164, () => HasPick() && HasHand()),
                    new TrackerLocation("Roller Chest", 4, () => HasHand() && (HasPick() || (HasLariat() && HasKnuckle()))),
                    new TrackerLocation("Lariat Target Chest", 53, () => HasHand() && (HasLariat() || (HasPick() && CanLongJump()))),
                    new TrackerLocation("Boss Key Chest", 9, () => HasHand() && (HasPick() || (HasLariat() && HasKnuckle()))),
                    new TrackerLocation("Vulture", 142, () => Item.BossKey.Acquired() && HasPick() && HasHand()),
                }),
                TrackerDot.NewDot("Tidal Mines", 43, -52, true, () => Item.RustyKey.Acquired(), new() {
                    new TrackerLocation("Secret Shop - Item 1", 245, () => CanReachZaegul()),
                    new TrackerLocation("Secret Shop - Item 2", 246, () => CanReachZaegul()),
                    new TrackerLocation("Secret Shop - Item 3", 247, () => CanReachZaegul()),
                    new TrackerLocation("Barrel Puzzle Heart Ore", 151, () => CanEnterTidalMines() && (HasLariat() || CanLongJump()) && HasPick()),
                    new TrackerLocation("Rocks Chest", 13, () => CanEnterTidalMines() && CanHit()),
                    new TrackerLocation("Lariat Chest", 15, () => CanEnterTidalMines() && (HasLariat() || HasKey(Key.TidalMines, 4)),
                        () => CanEnterTidalMines() && (HasLariat() || CanOpen(Key.TidalMines, 1))),
                    new TrackerLocation("Left Hidden Chest", 12, () => CanEnterTidalMines() && (HasLariat() || CanLongJump())),
                    new TrackerLocation("Right Hidden Chest", 11, () => CanEnterTidalMines() && (HasLariat() || CanLongJump())),
                    new TrackerLocation("Near Boss Chest", 16, () => HasLariat() && (HasPick() || HasFlare() || (HasKnuckle() && Skips()))),
                    new TrackerLocation("Barrel Cage Chest", 10, () => HasLariat() && (HasPick() || HasFlare() || (HasKnuckle() && Skips()))),
                    new TrackerLocation("Islands Chest", 14, () => HasLariat() && HasKey(Key.TidalMines, 4) && (HasPick() || HasFlare() || Skips()),
                        () => HasLariat() && CanOpen(Key.TidalMines, 2) && (HasPick() || HasFlare() || Skips())),
                    new TrackerLocation("Islands Heart Ore", 152, () => HasLariat() && HasPick() && HasKey(Key.TidalMines, 4),
                        () => HasLariat() && HasPick() && CanOpen(Key.TidalMines, 2)),
                    new TrackerLocation("Tidal Frog", 170, () => HasLariat() && HasPick()),
                    new TrackerLocation("Deep - Barrel Chest", 96, () => HasHand2() && (CanLongJump() || (HasLariat() && HasKnuckle()))),
                    new TrackerLocation("Deep - Turtles Chest", 97, () => HasHand2() && HasLariat() && HasPick() && HasFlare() && HasKey(Key.TidalMines, 3),
                        () => HasHand2() && HasLariat() && HasPick() && HasFlare() && CanOpen(Key.TidalMines, 40)),
                    new TrackerLocation("Deep - Water Blessing", 230, () => HasHand2() && HasLariat() && HasPick() && HasFlare() && HasKey(Key.TidalMines, 4),
                        () => HasHand2() && HasLariat() && HasPick() && HasFlare() && CanOpen(Key.TidalMines, 40, 41)),
                }),
                TrackerDot.NewDot("Dry Fountain", -28, -19, true, () => true, new() {
                    new TrackerLocation("Rust Knuckle Chest", 20, () => (HasLariat() || CanLongJump()) && CanHit()),
                    new TrackerLocation("Left Side Heart Ore", 160, () => HasLariat() && HasPick() && HasKnuckle() && HasHand()),
                    new TrackerLocation("Central Room Chest", 21, () => (HasLariat() && HasKnuckle()) || CanLongJump()),
                    new TrackerLocation("Barrel Bridge Chest", 19, () => HasLariat() && HasPick() && HasKnuckle() && HasHand()),
                    new TrackerLocation("Right Side Heart Ore", 159, () => HasLariat() && HasPick() && HasKnuckle() && HasHand()),
                    new TrackerLocation("Left Hidden Chest", 17, () => (HasPick() || Skips()) && HasLariat() && HasKnuckle()),
                    new TrackerLocation("Center Hidden Chest", 69, () => (HasPick() || Skips()) && HasLariat() && HasKnuckle()),
                    new TrackerLocation("Right Hidden Chest", 18, () => (HasPick() || Skips()) && HasLariat() && HasKnuckle()),
                    new TrackerLocation("Rat Potion", 63, () => HasLariat() && HasPick() && HasKnuckle() && HasHand()),
                }),
                TrackerDot.NewDot("Crocasino", 36, 10, true, () => HasKnuckle(), new() {
                    new TrackerLocation("Gator Key", 232, () => HasLariat() || CanLongJump()),
                    new TrackerLocation("Jail Chest", 50, () => Item.BunnyKey.Acquired() && (HasLariat() || HasFlare()) && (Item.GatorKey.Acquired() || (HasKnuckle() && (HasLariat() || CanLongJump())))),
                    new TrackerLocation("Hidden Chest", 68, () => (HasLariat() || HasFlare()) && ((Item.GatorKey.Acquired() && HasPick()) || (HasKnuckle() && (HasLariat() || CanLongJump())))),
                    new TrackerLocation("Turtle Chest", 22, () => HasLariat() && HasKnuckle()),
                    new TrackerLocation("Block Push Chest", 23, () => HasLariat() && HasKnuckle() && HasKey(Key.Crocasino, 1),
                        () => HasLariat() && HasKnuckle() && CanOpen(Key.Crocasino, 3)),
                    new TrackerLocation("Heart Ore", 161, () => HasLariat() && HasKnuckle() && HasKey(Key.Crocasino, 2) && HasPick(),
                        () => HasLariat() && HasKnuckle() && CanOpen(Key.Crocasino, 3, 5) && HasPick()),
                    new TrackerLocation("Wren", 62, () => HasLariat() && HasKnuckle() && HasKey(Key.Crocasino, 2) && Item.BunnyKey.Acquired(),
                        () => HasLariat() && HasKnuckle() && CanOpen(Key.Crocasino, 3, 5) && Item.BunnyKey.Acquired()),
                }),
                TrackerDot.NewDot("Howling Bjerg", -11, -58, true, () => true, new() {
                    new TrackerLocation("Outside Heart Ore", 157, () => HasPick() && HasKnuckle() && HasLariat()),
                    new TrackerLocation("Hidden Chest", 70, () => HasLariat() && HasKnuckle()),
                    new TrackerLocation("Ball Chest", 24, () => HasLariat() && HasKnuckle()),
                    new TrackerLocation("Inside Heart Ore", 158, () => HasPick() && HasKnuckle() && HasLariat()),
                    new TrackerLocation("Ice Chest", 25, () => HasLariat() && HasKnuckle()),
                    new TrackerLocation("Yhote", 233, () => HasLariat() && HasKnuckle() && HasKey(Key.HowlingBjerg, 1),
                        () => HasLariat() && HasKnuckle() && CanOpen(Key.HowlingBjerg, 6)),
                }),
                TrackerDot.NewDot("Castle Vann", -12, 32, true, () => Item.HallowedKey.Acquired(), new() {
                    new TrackerLocation("Entry Heart Ore", 154, () => HasPick()),
                    new TrackerLocation("Main - Upper Chest", 52, () => true),
                    new TrackerLocation("Main - Upper Right Chest", 30, () => HasLariat() && CanHit() && HasKey(Key.CastleVann, 4),
                        () => HasLariat() && CanHit() && CanOpen(Key.CastleVann, 7)),
                    new TrackerLocation("Main - Left Chest", 31, () => HasLariat() && (Skips() || (CanHit() && HasKey(Key.CastleVann, 4))),
                        () => HasLariat() && (Skips() || (CanHit() && CanOpen(Key.CastleVann, 8)))),
                    new TrackerLocation("Main - Lower Right Chest", 32, () => HasFlare() || HasLariat() || Skips()),
                    new TrackerLocation("West - Ball Puzzle Chest", 51, () => (HasLariat() || CanLongJump()) && (Skips() || HasKnuckle())),
                    new TrackerLocation("West - After Ball Puzzle Chest", 34, () => HasKnuckle() && (HasLariat() || CanLongJump())),
                    new TrackerLocation("West - Turtle Chest", 29, () => CanLongJump() || (CanHit() && HasKey(Key.CastleVann, 4)),
                        () => CanLongJump() || (HasPick() && CanOpen(Key.CastleVann, 8))),
                    new TrackerLocation("West - Black Hole Chest", 35, () => CanLongJump() || (HasKnuckle() && HasLariat() && HasKey(Key.CastleVann, 4)),
                        () => CanLongJump() || (HasKnuckle() && HasLariat() && CanOpen(Key.CastleVann, 8))),
                    new TrackerLocation("East - Floor Switches Chest", 33, () => HasLariat() && CanHit() && HasKey(Key.CastleVann, 4),
                        () => HasLariat() && CanHit() && CanOpen(Key.CastleVann, 7)),
                    new TrackerLocation("East - Block Push Heart Ore", 153, () => HasLariat() && HasPick() && HasKey(Key.CastleVann, 4),
                        () => HasLariat() && HasPick() && CanOpen(Key.CastleVann, 7)),
                    new TrackerLocation("Hidden Chest", 87, () => HasCrest() && CanHit()),
                    new TrackerLocation("Spirit of Vann", 234, () => HasCrest() && (HasLariat() || CanLongJump()) && CanHit()),
                    new TrackerLocation("Basement - Crumbling Floor Chest", 94, () => HasCrest() && Item.DustyKey.Acquired() && (HasLariat() || HasFlare())),
                    new TrackerLocation("Basement - Puzzle Chest", 95, () => HasCrest() && Item.DustyKey.Acquired() && HasLariat() && HasPick() && HasKey(Key.CastleVann, 3),
                        () => HasCrest() && Item.DustyKey.Acquired() && HasLariat() && HasPick() && CanOpen(Key.CastleVann, 39)),
                    new TrackerLocation("Basement - Ram Wraith", 171, () => HasCrest() && Item.DustyKey.Acquired() && HasPick() && HasKey(Key.CastleVann, 4),
                        () => HasCrest() && Item.DustyKey.Acquired() && HasPick() && CanOpen(Key.CastleVann, 38, 39)),
                }),
                TrackerDot.NewDot("Magma Heart", 56, 44, true, () => (HasKnuckle() && HasLariat()) || CanClimb(), new() {
                    new TrackerLocation("Hidden Chest", 71, () => CanHitFire()),
                    new TrackerLocation("Main Room Left Chest", 27, () => CanHitFire() && (Skips() || HasKnuckle())),
                    new TrackerLocation("Main Room Right Chest", 26, () => CanHitFire() && HasKnuckle()),
                    new TrackerLocation("Main Room Heart Ore", 155, () => HasPick() && HasKnuckle()),
                    new TrackerLocation("Near Boss Chest", 28, () => CanHitFire() && HasKnuckle() && (HasLariat() || CanLongJump())),
                    new TrackerLocation("Near Boss Heart Ore", 156, () => HasPick() && HasKnuckle() && (HasLariat() || CanLongJump())),
                    new TrackerLocation("Loomagnos", 169, () => HasPick() && HasKnuckle() && (HasLariat() || CanLongJump())),
                    new TrackerLocation("Deep - Spike Balls Chest", 91, () => HasHand2() && ((HasKnuckle() && HasLariat()) || HasFlare()) && CanHitFire()),
                    new TrackerLocation("Deep - Barrel Puzzle Chest", 92, () => HasHand2() && CanHitFire() && HasLariat() && HasKey(Key.MagmaHeart, 1),
                        () => HasHand2() && CanHitFire() && HasLariat() && CanOpen(Key.MagmaHeart, 34)),
                    new TrackerLocation("Deep - Earth Blessing", 235, () => HasHand2() && HasPick() && (HasLariat() || CanLongJump()) && HasKey(Key.MagmaHeart, 2),
                        () => HasHand2() && HasPick() && (HasLariat() || CanLongJump()) && CanOpen(Key.MagmaHeart, 33, 34)),
                }),
                TrackerDot.NewDot("Time Out", 52, -3, true, () => true, new() {
                    new TrackerLocation("West - First Chest", 37, () => TimeOut1() && CanHit()),
                    new TrackerLocation("West - Pits Heart Ore", 150, () => TimeOut1() && HasPick() && HasLariat() && HasKnuckle()),
                    new TrackerLocation("West - Left Hidden Chest", 38, () => TimeOut1() && HasPick() && HasLariat() && HasKnuckle()),
                    new TrackerLocation("West - Right Hidden Chest", 39, () => TimeOut1() && HasPick() && HasLariat() && HasKnuckle()),
                    new TrackerLocation("West - Underground Chest", 36, () => TimeOut1() && HasPick() && HasLariat() && HasKnuckle()),
                    new TrackerLocation("West - Invisible Item", 123, () => TimeOut1() && HasPick() && HasLariat() && HasFlare()),
                    new TrackerLocation("West - Blocks Heart Ore", 149, () => TimeOut1() && HasPick() && HasLariat() && HasKnuckle()),
                    new TrackerLocation("West - Near Boss Chest", 41, () => TimeOut1() && HasPick() && HasLariat() && HasKnuckle() && HasKey(Key.TimeOut, 3),
                        () => TimeOut1() && HasPick() && HasLariat() && HasKnuckle() && CanOpen(Key.TimeOut, 9)),
                    new TrackerLocation("East - Ball Push Chest", 40, () => TimeOut2() && ((HasLariat() && HasKnuckle()) || CanLongJump())),
                    new TrackerLocation("East - Invisible Floor Chest", 93, () => TimeOut2() && HasKnuckle() && (HasKey(Key.TimeOut, 2) || HasFlare()),
                        () => TimeOut2() && HasKnuckle() && (CanOpen(Key.TimeOut, 37) || HasFlare())),
                    new TrackerLocation("Color Correction", 236, () => TimeOut1() && HasPick() && HasLariat() && HasKnuckle() && HasKey(Key.TimeOut, 3),
                        () => TimeOut1() && HasPick() && HasLariat() && HasKnuckle() && CanOpen(Key.TimeOut, 9)),
                    new TrackerLocation("Colorgrave Gift", 224, () => Archipelago.AP.ColorCount() >= 5),
                }),
                TrackerDot.NewDot("Lighthouse", -56, -51, true, () => GameMaster.GM.Save.Data.Recolored, new() {
                    new TrackerLocation("Library Chest", 43, () => Skips() || (HasPick() && HasKnuckle() && HasLariat())),
                    new TrackerLocation("Junk Room Chest", 42, () => Skips() || (HasPick() && HasKnuckle() && HasLariat())),
                }),
                TrackerDot.NewDot("Crystal Caves", 7, 31, true, () => HasKnuckle(), new() {
                    new TrackerLocation("East - Three Barrels Chest", 110, () => CanEnterEastCrystalCaves()),
                    new TrackerLocation("East - Across Ice Chest", 136, () => CanEnterEastCrystalCaves() && HasPick()),
                    new TrackerLocation("East - Center Room Chest", 111, () => CanEnterEastCrystalCaves() && (HasCleats() || (HasPick() && (Skips() || HasHand()) && HasLariat())) && CanRemoveBoulders() && HasKey(Key.CrystalCaves, 2),
                        () => CanEnterEastCrystalCaves() && (HasCleats() || (HasPick() && (Skips() || HasHand()) && HasLariat())) && CanRemoveBoulders() && CanOpen(Key.CrystalCaves, 68)),
                    new TrackerLocation("East - Trapped Chest", 109, () => CanEnterEastCrystalCaves() && CanRemoveBoulders() && HasKey(Key.CrystalCaves, 2),
                        () => CanEnterEastCrystalCaves() && CanRemoveBoulders() && CanOpen(Key.CrystalCaves, 68)),
                    new TrackerLocation("East - Yhortes Chest", 113, () => CanEnterNortheastCrystalCaves(),
                        () => CanEnterNortheastCrystalCavesAlt()),
                    new TrackerLocation("East - Rock Cross Chest", 115, () => CanEnterNortheastCrystalCaves() && (HasLariat() || Skips()),
                        () => CanEnterNortheastCrystalCavesAlt() && (HasLariat() || Skips())),
                    new TrackerLocation("East - Two Chest Room Left Chest", 112, () => CanEnterNortheastCrystalCaves() && ((HasPick() && HasCleats()) || HasLariat()),
                        () => CanEnterNortheastCrystalCavesAlt() && ((HasPick() && HasCleats()) || HasLariat())),
                    new TrackerLocation("East - Two Chest Room Right Chest", 114, () => CanEnterNortheastCrystalCaves() && (CanLongJump() || HasPick()),
                        () => CanEnterNortheastCrystalCavesAlt() && (CanLongJump() || HasPick())),
                    new TrackerLocation("East - Stindle", 238, () => HasFlare() && (HasLariat() || Skips()) && HasPick() && HasKey(Key.CrystalCaves, 3),
                        () => HasFlare() && (HasLariat() || Skips()) && HasPick() && CanOpen(Key.CrystalCaves, 68, 69)),
                    new TrackerLocation("West - Lariat Target Chest", 116, () => HasFlare() && HasPick() && (HasLariat() || CanLongJump())),
                    new TrackerLocation("West - Barrel Bridge Chest", 118, () => HasFlare() && HasPick() && (HasLariat() || HasCleats() || Skips())),
                    new TrackerLocation("West - Across Ice Chest", 117, () => HasFlare() && HasPick()),
                    new TrackerLocation("West - Behind Rocks Chest", 120, () => HasFlare() && HasPick()),
                    new TrackerLocation("West - Rock Puzzle Chest", 119, () => HasFlare() && HasPick()),
                    new TrackerLocation("West - Frozen Heart", 177, () => HasFlare() && (HasLariat() || HasCleats() || Skips()) && HasPick() && HasKey(Key.CrystalCaves, 3),
                        () => HasFlare() && (HasLariat() || HasCleats() || Skips()) && HasPick() && CanOpen(Key.CrystalCaves, 70)),
                }),
                TrackerDot.NewDot("Haunted Hall", -30, 15, true, () => Item.BoneKey.Acquired() && CanHit(), new() {
                    new TrackerLocation("Right Entry Chest", 66, () => true),
                    new TrackerLocation("Left Entry Chest", 65, () => CanHit()),
                    new TrackerLocation("Invisible Maze Chest", 67, () => CanHit() && HasHand() && HasKey(Key.HauntedHall, 1),
                        () => CanHit() && HasHand() && CanOpen(Key.HauntedHall, 21)),
                    new TrackerLocation("Crystal Chest", 72, () => HasFlare() && HasHand() && HasKey(Key.HauntedHall, 2) && (HasLariat() || CanLongJump() || HasIceKey()),
                        () => HasFlare() && HasHand() && CanOpen(Key.HauntedHall, 21, 22) && (HasLariat() || CanLongJump() || HasIceKey())),
                    new TrackerLocation("Killer", 229, () => CanHit() && HasHand() && HasKey(Key.HauntedHall, 2) && HasLariat(),
                        () => CanHit() && HasHand() && CanOpen(Key.HauntedHall, 21, 22) && HasLariat()),
                }),
                TrackerDot.NewDot("Siska's Workshop", -56, 42, true, () => HasLariat() && (HasPick() || Item.HallowedKey.Acquired()), new() {
                    new TrackerLocation("First Chest", 82, () => HasLariat() && CanHit()),
                    new TrackerLocation("Energy Orb Chest", 83, () => HasLariat() && CanHit() && HasKey(Key.SiskasWorkshop, 1),
                        () => HasLariat() && CanHit() && CanOpen(Key.SiskasWorkshop, 28)),
                    new TrackerLocation("Cannon Chest", 84, () => HasLariat() && CanHit() && HasKey(Key.SiskasWorkshop, 2),
                        () => HasLariat() && CanHit() && CanOpen(Key.SiskasWorkshop, 28, 29)),
                    new TrackerLocation("Mecha Vanns Chest", 86, () => HasLariat() && CanHit() && (HasKey(Key.SiskasWorkshop, 3) || HasFlare()) && HasKey(Key.SiskasWorkshop, 2),
                        () => HasLariat() && CanHit() && (CanOpen(Key.SiskasWorkshop, 28, 29, 30) || HasFlare()) && CanOpen(Key.SiskasWorkshop, 28, 29)),
                    new TrackerLocation("Crystal Chest", 85, () => HasLariat() && CanHit() && ((HasHand() && HasKey(Key.SiskasWorkshop, 3)) || HasFlare()) && HasKey(Key.SiskasWorkshop, 2),
                        () => HasLariat() && CanHit() && (((HasHand() || HasIceKey()) && CanOpen(Key.SiskasWorkshop, 28, 29, 30)) || HasFlare()) && CanOpen(Key.SiskasWorkshop, 28, 29)),
                    new TrackerLocation("Siska", 231, () => HasLariat() && CanHit() && (HasKey(Key.SiskasWorkshop, 3) || HasFlare()) && HasKey(Key.SiskasWorkshop, 2),
                        () => HasLariat() && CanHit() && (CanOpen(Key.SiskasWorkshop, 28, 29, 30) || HasFlare()) && CanOpen(Key.SiskasWorkshop, 28, 29)),
                }),
                TrackerDot.NewDot("Backrooms", 40, 10, true, () => true, new() {
                    new TrackerLocation("Entry Chest", 89, () => HasKnuckle()),
                    new TrackerLocation("Left Side Chest", 172, () => HasKnuckle() && HasLariat()),
                    new TrackerLocation("Hidden Chest", 174, () => HasKnuckle() && HasLariat()),
                    new TrackerLocation("Cannon Chest", 173, () => HasKnuckle() && HasKey(Key.Backrooms, 1) && HasLariat(),
                        () => HasKnuckle() && CanOpen(Key.Backrooms, 81) && HasLariat()),
                    new TrackerLocation("Ball Chest", 178, () => HasKnuckle() && HasKey(Key.Backrooms, 1) && HasLariat(),
                        () => HasKnuckle() && CanOpen(Key.Backrooms, 81) && HasLariat()),
                    new TrackerLocation("Near Cracked Wall Chest", 76, () => HasKnuckle() && HasKey(Key.Backrooms, 1) && HasLariat(),
                        () => HasKnuckle() && CanOpen(Key.Backrooms, 81) && HasLariat()),
                    new TrackerLocation("Crystal Chest", 81, () => HasKnuckle() && HasKey(Key.Backrooms, 1) && HasLariat() && (HasKey(Key.Backrooms, 2) || HasIceKey()),
                        () => HasKnuckle() && CanOpen(Key.Backrooms, 81) && HasLariat() && (CanOpen(Key.Backrooms, 81, 82) || HasIceKey())),
                    new TrackerLocation("Mechanized Slot Machine", 73, () => HasKnuckle() && (HasKey(Key.Backrooms, 2) || HasIceKey()) && HasPick() && HasLariat(),
                        () => HasKnuckle() && (CanOpen(Key.Backrooms, 81, 82) || HasIceKey()) && HasPick() && HasLariat()),
                }),
                TrackerDot.NewDot("Pirate's Pier", 61, -55, true, () => Item.StindlesMap.Acquired() && CanHit(), new() {
                    new TrackerLocation("Caroline", 225, () => true),
                    new TrackerLocation("Outside - First Chest", 134, () => true),
                    new TrackerLocation("Outside - Lariat Target Chest", 133, () => HasLariat()),
                    new TrackerLocation("Outside - Locked Chest", 132, () => HasLariat() && HasKey(Key.PiratesPier, 5),
                        () => HasLariat() && CanOpen(Key.PiratesPier, 75)),
                    new TrackerLocation("East - Shelled Nipper Chest", 121, () => CanHit()),
                    new TrackerLocation("East - Block Puzzle Chest", 122, () => HasKnuckle() && HasKey(Key.PiratesPier, 5),
                        () => HasKnuckle() && CanOpen(Key.PiratesPier, 72)),
                    new TrackerLocation("West - Spikes Chest", 131, () => HasKnuckle() && (HasFlare() || HasLariat())),
                    new TrackerLocation("West - Lariat Puzzle Chest", 130, () => HasKnuckle() && HasLariat()),
                    new TrackerLocation("West - Inkwell", 239, () => HasFlare() && HasKey(Key.PiratesPier, 5),
                        () => HasFlare() && CanOpen(Key.PiratesPier, 74)),
                    new TrackerLocation("Upstairs - Block Push Chest", 126, () => HasPick() && HasLariat() && (Skips() || HasFlare())),
                    new TrackerLocation("Upstairs - Barrel Switches Chest", 129, () => HasPick() && HasLariat() && ((Skips() && HasHand()) || HasHand2()) && HasKey(Key.PiratesPier, 5),
                        () => HasPick() && HasLariat() && ((Skips() && HasHand()) || HasHand2()) && CanOpen(Key.PiratesPier, 73)),
                    new TrackerLocation("Upstairs - Don't Drop Chest", 127, () => HasPick() && HasLariat()),
                    new TrackerLocation("Upstairs - Drop Chest", 128, () => HasPick() && HasLariat()),
                    new TrackerLocation("Upstairs - Kings Ring Chest", 125, () => HasPick() && HasLariat() && HasFlare() && HasKey(Key.PiratesPier, 5),
                        () => HasPick() && HasLariat() && HasFlare() && CanOpen(Key.PiratesPier, 78)),
                    new TrackerLocation("Revulan", 226, () => HasKnuckle() && Item.KingsRing.Acquired() && Archipelago.AP.BlessingCount() >= 2),
                }),
                TrackerDot.NewDot("Bjerg Castle", -21, -44, true, () => true, new() {
                    new TrackerLocation("Hype Chest 1", 56, () => true),
                    new TrackerLocation("Hype Chest 2", 60, () => true),
                    new TrackerLocation("Hype Chest 3", 59, () => true),
                    new TrackerLocation("Hype Chest 4", 57, () => true),
                    new TrackerLocation("Hype Chest 5", 61, () => true),
                    new TrackerLocation("Hype Chest 6", 58, () => true),
                    new TrackerLocation("Cannonball Chest", 54, () => HasLariat() && HasKnuckle()),
                    new TrackerLocation("Near Boss Chest", 55, () => HasLariat() && HasKnuckle() && HasKey(Key.BjergCastle, 1)),
                    new TrackerLocation("Captain Crossbones", 248, () => HasLariat() && HasKnuckle() && HasKey(Key.BjergCastle, 1)),
                }),
                TrackerDot.NewDot("Daemon's Dive", -30, 37, true, () => HasHand2() && Item.HallowedKey.Acquired() && Item.DaemonsKey.Acquired(), new() {
                    new TrackerLocation("1 - Barrel Puzzle Chest", 98, () => (HasLariat() || CanLongJump()) && CanHit()),
                    new TrackerLocation("1 - Lariat Puzzle Chest", 108, () => HasDivineKey(1) && HasHand2() && HasLariat()),
                    new TrackerLocation("2 - Skull Chest", 99, () => HasDivineKey(2) && (HasIceKey() || HasHand2()) && HasFlare() && HasHand() && HasLariat()),
                    new TrackerLocation("2 - Near Boss Chest", 100, () => HasDivineKey(2) && (HasIceKey() || HasHand2()) && HasFlare() && HasHand() && HasLariat()),
                    new TrackerLocation("3 - Cannon Chest", 101, () => HasDivineKey(2) && (HasIceKey() || (HasHand2() && HasDivineKey(4))) && HasFlare() && HasHand() && HasLariat() && HasPick(),
                        () => HasDivineKey(2) && (HasIceKey() || (HasHand2() && CanOpenDivine(46, 47, 51))) && HasFlare() && HasHand() && HasLariat() && HasPick()),
                    new TrackerLocation("3 - Turtles Chest", 102, () => HasDivineKey(5) && (HasIceKey() || HasHand2()) && HasFlare() && HasHand() && HasLariat() && HasPick(),
                        () => CanOpenDivine(46, 47, 52) && (HasIceKey() || (HasHand2() && CanOpenDivine(46, 47, 51, 52))) && HasFlare() && HasHand() && HasLariat() && HasPick()),
                    new TrackerLocation("4 - Turtles Chest", 103, () => HasDivineKey(6) && (HasIceKey() || HasHand2()) && HasFlare() && HasHand() && HasLariat() && HasPick(),
                        () => CanOpenDivine(46, 47, 52, 53) && (HasIceKey() || (HasHand2() && CanOpenDivine(46, 47, 51, 52, 53))) && HasFlare() && HasHand() && HasLariat() && HasPick()),
                    new TrackerLocation("5 - Many Switches Chest", 104, () => HasDivineKey(7) && (HasIceKey() || HasHand2()) && HasFlare() && HasHand() && HasLariat() && HasPick(),
                        () => CanOpenDivine(46, 47, 52, 53, 55) && (HasIceKey() || (HasHand2() && CanOpenDivine(46, 47, 51, 52, 53, 55))) && HasFlare() && HasHand() && HasLariat() && HasPick()),
                    new TrackerLocation("5 - Junk Pile Chest", 105, () => HasDivineKey(7) && (HasIceKey() || HasHand2()) && HasFlare() && HasHand() && HasLariat() && HasPick(),
                        () => CanOpenDivine(46, 47, 52, 53, 55) && (HasIceKey() || (HasHand2() && CanOpenDivine(46, 47, 51, 52, 53, 55))) && HasFlare() && HasHand() && HasLariat() && HasPick()),
                    new TrackerLocation("6 - Main Room Chest", 106, () => HasDivineKey(7) && (HasIceKey() || (HasHand2() && HasDivineKey(9))) && HasFlare() && HasHand() && HasLariat() && HasPick(),
                        () => CanOpenDivine(46, 47, 52, 53, 55) && (HasIceKey() || (HasHand2() && CanOpenDivine(46, 47, 51, 52, 53, 55, 61))) && HasFlare() && HasHand() && HasLariat() && HasPick()),
                    new TrackerLocation("7 - Crystal Key Chest", 124, () => HasDivineKey(10) && HasHand2() && HasFlare() && HasHand() && HasLariat() && HasPick(),
                        () => CanOpenDivine(46, 47, 52, 53, 55, 59) && (HasIceKey() || CanOpenDivine(46, 47, 51, 52, 53, 55, 59, 61)) && HasHand2() && HasFlare() && HasHand() && HasLariat() && HasPick()),
                    new TrackerLocation("7 - Turtles Chest", 107, () => HasDivineKey(11) && (HasIceKey() || HasHand2()) && HasFlare() && HasHand() && HasLariat() && HasPick(),
                        () => CanOpenDivine(46, 47, 52, 53, 55, 59, 67) && (HasIceKey() || (HasHand2() && CanOpenDivine(46, 47, 51, 52, 53, 55, 59, 61, 67))) && HasFlare() && HasHand() && HasLariat() && HasPick()),
                    new TrackerLocation("Shadow Oran", 244, () => HasDivineKey(12) && (HasIceKey() || HasHand2()) && HasFlare() && HasHand() && HasLariat() && HasPick(),
                        () => CanOpenDivine(46, 47, 52, 53, 55, 59, 65, 67) && (HasIceKey() || (HasHand2() && CanOpenDivine(46, 47, 51, 52, 53, 55, 59, 61, 65, 67))) && HasFlare() && HasHand() && HasLariat() && HasPick()),
                }),
                TrackerDot.NewDot("Enlightenment", 11, 45, true, () => CanClimb() && HasHand2() && (HasLariat() || HasFlare()) && ((Item.GatorKey.Acquired() && HasPick()) || HasKnuckle() && (HasLariat() || CanLongJump())) && HasEnoughBlessings(), new() {
                    new TrackerLocation("1 - Right Side Chest", 64, () => HasFlare() && HasLariat() && HasPick()),
                    new TrackerLocation("1 - Left Side Chest", 77, () => HasFlare() && HasLariat() && HasPick() && HasHand2()),
                    new TrackerLocation("1 - Center Room Chest", 80, () => HasFlare() && HasLariat() && HasPick() && HasHand2()),
                    new TrackerLocation("2 - Crystal Key Chest", 74, () => HasFlare() && HasLariat() && HasPick() && HasHand2()),
                    new TrackerLocation("3 - Perilous Platforms Chest", 78, () => HasFlare() && HasLariat() && HasPick() && HasHand2()),
                    new TrackerLocation("4 - Roller Chest", 90, () => HasFlare() && HasLariat() && HasPick() && HasHand2()),
                    new TrackerLocation("4 - Spike Floor Chest", 75, () => HasFlare() && HasLariat() && HasPick() && HasHand2()),
                    new TrackerLocation("5 - Falling Floor Chest", 79, () => HasFlare() && HasLariat() && HasPick() && HasHand2()),
                }),
            };
        }

        private bool HasHand()
        {
            return Item.DreadHand.Acquired();
        }

        private bool HasHand2()
        {
            return Item.EmpoweredHand.Acquired();
        }

        private bool HasPick()
        {
            return Item.IronPick.Acquired();
        }

        private bool HasLariat()
        {
            return Item.Lariat.Acquired();
        }

        private bool HasKnuckle()
        {
            return Item.RustKnuckle.Acquired();
        }

        private bool HasFlare()
        {
            return Item.FlareKnuckle.Acquired();
        }
        
        private bool HasCleats()
        {
            return Item.CleatedBoots.Acquired() || Item.BootsOfGraile.Acquired();
        }

        private bool CanHit()
        {
            return HasPick() || HasKnuckle();
        }

        private bool CanHitFire()
        {
            return HasPick() || HasFlare();
        }

        private bool CanClimb()
        {
            return Item.ClimbingGear.Acquired();
        }

        private bool CanReachZaegul()
        {
            return Item.RustyKey.Acquired() && Item.Harmonica.Acquired() && (HasPick() || CanLongJump() || (HasLariat() && (Skips() || HasFlare())));
        }

        private bool Skips()
        {
            return Archipelago.AP.Settings.SkipsInLogic;
        }

        private bool CanLongJump()
        {
            return (Skips() || Archipelago.AP.Settings.LongJumpsInLogic) && HasFlare();
        }

        private bool CanRemoveBoulders()
        {
            return HasKnuckle() || (Skips() && HasHand2());
        }

        private bool HasKey(Key key, int count)
        {
            return Archipelago.AP.Settings.SpecificKeys ? Archipelago.AP.Data.KeyTotals[key.ID] >= count : CanReachZaegul();
        }

        private bool HasIceKey()
        {
            return CanReachZaegul();
        }

        private bool HasDivineKey(int count)
        {
            return Item.DivineKey.Count() >= count;
        }

        private bool HasCrest()
        {
            return Item.CrestFragment.Count() >= Archipelago.AP.Settings.CrestFragmentsRequired;
        }

        private bool HasCoins()
        {
            return Item.CoinOfCrowl.Count() >= Archipelago.AP.Settings.CoinsOfCrowlRequired;
        }

        private bool TimeOut1()
        {
            return Archipelago.AP.ColorCount() >= 2;
        }

        private bool TimeOut2()
        {
            return Archipelago.AP.ColorCount() >= 2;
        }

        private bool Has4Boots()
        {
            return ItemExtension.AllBoots().Count(boots => boots.Acquired()) >= 4;
        }

        private bool CanKillGrelins()
        {
            return ((HasPick() || (Skips() && HasKnuckle())) && CanClimb() && HasLariat()) || ((CanClimb() || (HasLariat() && HasKnuckle())) && CanHitFire() && (Skips() || HasKnuckle()));
        }

        private bool HasEnoughBlessings()
        {
            return Archipelago.AP.BlessingCount() >= Archipelago.AP.Settings.BlessingsRequired;
        }

        private bool CanOpen(Key key, params int[] locks)
        {
            int keysRequired = locks.Count(id => !GameMaster.GM.Save.Data.UnlockedDoors.Contains(id));
            if (!Archipelago.AP.Settings.SpecificKeys)
            {
                return CanReachZaegul() || Item.NormalKey.Count() >= keysRequired;
            }
            return key.Count >= keysRequired;
        }

        private bool CanOpenDivine(params int[] locks)
        {
            int keysRequired = locks.Count(id => !GameMaster.GM.Save.Data.UnlockedDoors.Contains(id));
            return Item.DivineKey.Count() >= keysRequired;
        }

        private bool CanTradeWithUlni()
        {
            return Archipelago.AP.Settings.TradingQuest == ArchipelagoSettings.TradingQuestOption.Shuffle ?
                Item.KelpRolls.Acquired() :
                Item.LostShipment.Acquired() && HasLariat();
        }

        private bool CanReturnStolenItems()
        {
            return Archipelago.AP.Settings.ShuffleGrelinDrops ?
                Item.HolyRelic.Acquired() && Item.WeddingRing.Acquired() && Item.SilverMirror.Acquired() && Item.Painting.Acquired() :
                CanKillGrelins();
        }

        private bool CanEnterTidalMines()
        {
            return (HasHand() || HasLariat()) && (HasPick() || (HasLariat() && (HasFlare() || Skips())) || CanLongJump());
        }

        private bool CanEnterEastCrystalCaves()
        {
            return (HasFlare() || HasLariat()) && (CanLongJump() || (HasPick() && (Skips() || HasLariat() || HasCleats())));
        }

        private bool CanEnterNortheastCrystalCaves()
        {
            return CanEnterEastCrystalCaves() && HasFlare() && (CanLongJump() || HasPick() || HasLariat()) && HasKey(Key.CrystalCaves, 2);
        }

        private bool CanEnterNortheastCrystalCavesAlt()
        {
            return CanEnterEastCrystalCaves() && HasFlare() && (CanLongJump() || HasPick() || HasLariat()) && CanOpen(Key.CrystalCaves, 68);
        }
    }
}