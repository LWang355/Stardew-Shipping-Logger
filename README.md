# Stardew Shipping Logger
 A mod for Stardew Valley that records shipped items daily

!!! Lots of things here are WIP and TBD. Please be patient, this will be my first Stardew Valley SMAPI mod. !!!

At the end of every game day, this mod will record the sold contents of the shipping bin as a separate file. A player can go through these files to look through their play history. I will also be writing something that can read these files and produce spreadsheets, but it might just be only for me.

## Log File Contents
A header (at beginning of log file) contains:
-Player Name
-Farm Name
-Unique IDs
Daily entries contain:
-Game Day (starting at Day 1 = Spr 1 of Yr 1)
-Season, SeasonDay, Year, Weekday (for convenience)
-Money at start of day
-Money at end of day
-Shipping bin revenue
-Overall daily profit
Then, for each item in the shipping bin:
-Item Name - localized name of item
-Category - according to this: https://stardewvalleywiki.com/Modding:Items#Categories
-Quantity 
-Quality - Normal/Silver/Gold/Iridium, abbrievated (Ns - no star,Ag,Au,Ir) 
-Price
-For preserves items, attempts to resolve the base item (eg. Wine) and preserved item (eg. Starfruit)

Each daily log is saved on day end, in a separate JSON file.
The filename is constructed as (TBD):
-Stonecoil-0100-00030127-20220418-1516
-Farm name
-Day (4 digits)
-Year (4 digits)
-Season (1= Spring, 4=Winter)
-Day in Season (2 digits)
-Timestamp "yyyyMMdd-HHmm"
