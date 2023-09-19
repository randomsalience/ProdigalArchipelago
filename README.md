# Prodigal Archipelago

This is a mod for the game [Prodigal](https://store.steampowered.com/app/1393820/Prodigal/) allowing it to be incorporated into the multiworld randomizer [Archipelago](https://archipelago.gg/).

## Installation

1. Download [BepInEx 5](https://github.com/BepInEx/BepInEx/releases/) -- make sure not to use the pre-release version 6.
2. Extract BepInEx into the folder containing the Prodigal executable.
3. Run Prodigal once to finish installing BepInEx, then close the game once the menu loads.
4. Download this mod, which contains an `Archipelago` folder, a `Prodigal.yaml` file, and a `prodigal.apworld` file.
5. Place the `Archipelago` folder in `BepInEx/plugins`.
6. Install Archipelago.
7. Place `prodigal.apworld` in the `lib/worlds` folder of your Archipelago installation.

## Joining an Archipelago Game

1. Edit the settings `Prodigal.yaml` file to your liking. Use this file when generating a seed according to the Archipelago instructions.
2. Start a new game of Prodigal, and select Archipelago.
3. Enter the server address, port number, slot name, and optionally password, then select Start.

## Gameplay Tips

This section describes changes which have been made to the randomizer, as well as information about the requirements for some of the less obvious checks.

- The game starts in Act 2. So the portal to the Colorless Void is open, Hackett appears, the sailor will take you to the Bjerg, etc. The Backrooms and Siska's Workshop are open from the start. Bolivar remains in town so you can obtain his check.
- If "Specific Keys" is turned on, keys are specific to each dungeon. If this option is on, two new pages are added to the inventory menu showing the number of keys for each dungeon.
- Buying ice keys from Zaegul is in logic if you have the Harmonica, access to the secret shop in the Tidal Mines, and a means of making money. If "Specific Keys" is turned off, buying normal keys from Zaegul is also in logic.
- A warp to town button has been added to the menu. Besides being convenient, this allows you to obtain checks that would otherwise softlock you.
- The map in the menu now includes a tracker. Locations are green if all items can be obtained, yellow if some can be obtained, or red if none can be obtained. Orange means that items can be obtained out of logic by spending keys. Hover over a location with the mouse to see a list of all checks at that location.
- Extra checks are added on Yhorte, Siska, and Inkwell, as well as Color Correction (beating Me?).
- To enter the lighthouse, you must collect a certain number of Colors (Shattered Soul, Fury Heart, Frozen Heart, Red Crystal, Sunset Painting).
- To obtain the Hero's Soul, you must collect a certain number of Blessings.
- To enter Enlightenment, you must obtain the Hero's Soul and the Empowered Hand and absorb the tar in the hidden room in the Crocasino.
- The lighthouse door will tell you the number of colors required. The hero's grave will tell you the number of blessings required.
- Time Out 2 now opens at the same time as Time Out 1 (when you give two colors to Colorgrave).
- You know the harmonica tune from the start, so there is no need to do the Reflective Crypt.
- The Empowered Hand is required to make the Hooded Figure appear, to open the warp stone to the hero's grave, and to open paths in the Tidal Mines and Magma Heart.
- The number of Crest Fragments required to pass Alexan and the number of Coins of Crowl needed to pass Kaelum can be changed. The Blessed Pick is no longer required for the underwater item.
- Fireballs may be hit with either the pick or the flare knuckle (but not the rust knuckle).
- If you accidentally make Grant leave town, you can bring him back by talking to Diana at the ring shop.
- To get the item from Bolivar, give him all four materials (Shaedrite, Drowned Ore, Miasmic Extract, Broken Sword).
- To get the item from Lynn, return the four stolen items (Holy Relic, Wedding Ring, Silver Mirror, Painting).
- To get the item from Keaton, collect 999 points worth of fish (100 if fast fishing is turned on).
- To get the boots check from Tess, obtain any four pairs of boots and the Hairpin.
- To get the item from the Light Spirit, sleep after defeating Amadeus.
- To get the item from Caroline, get captured by the pirates and inspect the lock.