using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using System.Collections.Generic;

namespace StardewShippingLogger
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            // Set event responses
            helper.Events.GameLoop.SaveLoaded += this.OnLoad;
            helper.Events.GameLoop.DayEnding += this.OnDayEnd;
            helper.Events.GameLoop.DayStarted += this.OnDayStart;
        }

        private ShippingLog ShippingLog = new();

        private void OnLoad(object sender, EventArgs e)
        {
            ShippingLog.PlayerInfo = new PlayerInfo(Game1.player);
        }

        private void OnDayStart(object sender, EventArgs e)
        {
            SDate date = SDate.Now();
            ShippingLog.DayInfo = new DayInfo(date)
            {
                MoneyAtStart = Game1.player.Money
            };
            ShippingLog.ClearBoxContents();

        }

        private void OnDayEnd(object sender, EventArgs e)
        {
            // Iterate through items in shipping bin, added them to log
            foreach (Item item in Game1.getFarm().getShippingBin(Game1.player))
            {
                ShippingLog.AddItemStack(item);
            }

            // Will not write a log on the first playable day if the intro was skipped
            if (ShippingLog.DayInfo != null)
            {
                ShippingLog.DayInfo.MoneyAtEnd = Game1.player.Money;
                ShippingLog.calculateProfits();
            

                // Write a Shipping Log file
                string fileName = $"Logs/{Constants.SaveFolderName}/";
                fileName += ShippingLog.PlayerInfo.PlayerName;
                fileName += "-"+ShippingLog.DayInfo.GetDateAsString();
                fileName += "-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".json";

                this.Helper.Data.WriteJsonFile(fileName, ShippingLog);
            }
        }

    }
    class ShippingLog
    {
        public PlayerInfo PlayerInfo;
        public DayInfo DayInfo;
        public IList<StackInBox> boxContents = new List<StackInBox>();

        public void calculateProfits()
        {
            this.DayInfo.OtherRevenue = this.DayInfo.MoneyAtEnd - this.DayInfo.MoneyAtStart;
            this.DayInfo.BinRevenue = this.CalculateBinRevenue();
        }

        public int CalculateBinRevenue()
        {
            int total = 0;
            foreach (StackInBox binItem in this.boxContents)
            {
                total += binItem.ItemQuantity * binItem.ItemUnitPrice;
            }
            return total;
        }

        public void AddItemStack(Item item)
        {
            StackInBox newStack = new StackInBox(item);
            this.boxContents.Add(newStack);
        }

        public void ClearBoxContents()
        {
            this.boxContents.Clear();
        }
    }

    class DayInfo 
    {
        public int RunDay { get; set; }
        public int SeasonIndex { get; set; }
        public string Season { get; set; }
        public int DayOfSeason { get; set; }
        public int Year { get; set; }
        public int MoneyAtStart { get; set; }
        public int MoneyAtEnd { get; set; }
        public int BinRevenue { get; set; }
        public int OtherRevenue { get; set; }

        public DayInfo(SDate sDate)
        {
            this.RunDay = sDate.DaysSinceStart;
            this.SeasonIndex = sDate.SeasonIndex;
            this.Season = sDate.Season;
            this.DayOfSeason = sDate.Day;
            this.Year = sDate.Year;
        }

        public string GetDateAsString()
        {
            //String filename = String.Format("{0,4}-D{1,4}S{2,1}Y{3,2}", this.RunDay.ToString("D4"),this.Year.ToString("D4"), this.SeasonIndex, this.DayOfSeason);
            String filename = String.Format("{0,4}", this.RunDay.ToString("D4"));
            return filename;
        }
    }
    class PlayerInfo
    {
        public string PlayerName { get; set; }
        public string FarmName { get; set; }
        public long UniqueId { get; set; }

        public PlayerInfo(Farmer farmer)
        {
            this.PlayerName = farmer.Name;
            this.FarmName = farmer.farmName.Value;
            this.UniqueId = farmer.UniqueMultiplayerID;
        }
    }

    class StackInBox
    {
        public string ItemDisplayName { get; set; }
        public string ItemName { get; set; }
        public int ItemID { get; set; }
        public string ItemCategory { get; set; }
        public int ItemCategoryNum { get; set; }
        public int ItemQuantity { get; set; }
        public int ItemQuality { get; set; }
        public int ItemUnitPrice { get; set; }
        public int StackTotalPrice { get; set; }

        // Is this item stack a "preserve" item such as jelly, pickles, wine
        public bool ItemIsPreserve { get; set; } = false;
        // "Starfruit Wine"
        // If not, these fields are left as defaults
        public string BaseItemName { get; set; } = ""; // "Wine"
        public string PreservedItemName { get; set; } = ""; // "Starfruit"
        public int PreservedItemID { get; set; } = 0;
        public bool IsEdible { get; set; }

        public StackInBox(Item item)
        {
            this.ItemDisplayName = item.DisplayName;
            this.ItemName = item.Name;
            this.ItemCategory = item.getCategoryName();
            this.ItemCategoryNum = item.Category;
            this.ItemQuantity = item.Stack;

            SObject asObj = item.getOne() as SObject;
            this.ItemID = asObj.ParentSheetIndex;
            this.ItemQuality = asObj.Quality;
            this.ItemUnitPrice = Utility.getSellToStorePriceOfItem(item, false);
            //this.ItemUnitPrice = asObj.sellToStorePrice(Game1.player.UniqueMultiplayerID);
            this.StackTotalPrice = this.ItemQuantity * this.ItemUnitPrice;

            int preservedItemIndex = asObj.preservedParentSheetIndex.Value;
            this.PreservedItemID = preservedItemIndex;

            if (asObj.preservedParentSheetIndex.Value != 0)
            {
                this.ItemIsPreserve = true;
                this.PreservedItemName = Game1.objectInformation[preservedItemIndex].Split('/')[4];
                this.BaseItemName = Game1.objectInformation[item.ParentSheetIndex].Split('/')[4];
            }

            this.IsEdible = asObj.Edibility > 0;
        }
    }

}
