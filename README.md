# Stardew Shipping Logger
 A mod for Stardew Valley that records shipped items daily

!!! Lots of things here are WIP and TBD. Please be patient, this will be my first Stardew Valley SMAPI mod. !!!

At the end of every game day, this mod will record the sold contents of the shipping bin as a separate file. A player can go through these files to look through their play history. I will also be writing something that can read these files and produce spreadsheets, but it might just be only for me.
## To Use

After building this mod, it creates a folder "Shipping Logger" in the project folder. Put that folder into the Mods folder in Stardew Valley (mod does not auto-deploy when building).
I have not published this mod to any website yet. I will probably do so when I finish writing a companion app or script that digests the logs into a spreadsheet or charts (no estimates on how long that will take, sorry).

## Log File Contents
A header (at beginning of log file) contains:
- Player Name
- Farm Name
- Unique IDs

Daily entries contain:
- Game Day (starting at Day 1 = Spr 1 of Yr 1)
- Season, SeasonDay, Year (for convenience)
- Money at start of day
- Money at end of day
- Shipping bin revenue
- Other revenue (change in money over the day, without bin profit)

Then, for each item in the shipping bin:
- Item Name - localized name of item
- Item ID - the index in the object data
- Category - according to this: https://stardewvalleywiki.com/Modding:Items#Categories, as both a (negative) number and a string
- Quantity 
- Quality
- Price
- For preserves items, lists the preserved item ID, and attempts to resolve the base item (eg. Wine) and preserved item name and ID (eg. Starfruit)

Each daily log is saved on day end, in a separate JSON file.
The filename is constructed as:
- Player Name
- Timestamp "yyyyMMdd-HHmm"

The folder is named for the farm name and ID.
