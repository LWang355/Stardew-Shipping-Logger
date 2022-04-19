using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
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
            ShippingLog.PlayerInfo = new PlayerInfo(Game1.player, Game1.getFarm().Name);
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
            // 
            foreach (Item item in Game1.getFarm().getShippingBin(Game1.player))
            {
                ShippingLog.AddItemStack(item);
            }

            ShippingLog.DayInfo.MoneyAtEnd = Game1.player.Money;
            ShippingLog.calculateProfits();

            // Write a Shipping Log file
            string fileName = $"data/{Constants.SaveFolderName}";
            fileName += "-"+ShippingLog.PlayerInfo.PlayerName;
            fileName += "-" + ShippingLog.DayInfo.GetDateAsString();
            fileName += "-" + DateTime.Now.ToString("yyyyMMdd-HHmm");

            this.Helper.Data.WriteJsonFile(fileName, ShippingLog);
        }

    }
    class ShippingLog
    {
        public PlayerInfo PlayerInfo;
        public DayInfo DayInfo;
        public IList<StackInBox> boxContents = new List<StackInBox>();

        public void calculateProfits()
        {
            this.DayInfo.DayProfit = this.DayInfo.MoneyAtStart - this.DayInfo.MoneyAtEnd;
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
        public int DayOfSeason { get; set; }
        public int Year { get; set; }
        public int MoneyAtStart { get; set; }
        public int MoneyAtEnd { get; set; }
        public int BinRevenue { get; set; }
        public int DayProfit { get; set; }

        public DayInfo(SDate sDate)
        {
            RunDay = sDate.DaysSinceStart;
            SeasonIndex = sDate.SeasonIndex;
            DayOfSeason = sDate.Day;
            Year = sDate.Year;
        }

        public string GetDateAsString()
        {
            String filename = String.Format("-{0,4}", this.RunDay);
            filename += String.Format("-{0,4}{1,1}{2,2}", this.Year, this.SeasonIndex, this.DayOfSeason);
            return filename;
        }
    }
    class PlayerInfo
    {
        public string PlayerName { get; set; }
        public string FarmName { get; set; }
        public long UniqueId { get; set; }

        public PlayerInfo(Farmer farmer, string farmName)
        {
            this.PlayerName = farmer.Name;
            this.FarmName = farmName;
            this.UniqueId = farmer.UniqueMultiplayerID;
        }
    }

    class StackInBox
    {
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public int ItemQuantity { get; set; }
        public int ItemUnitPrice { get; set; }
        public int StackTotalPrice { get; set; }

        // Is this item stack a "preserve" item such as jelly, pickles, wine
        public Boolean ItemIsPreserve { get; set; } = false;
        // "Starfruit Wine"
        // If false, these fields are ignored
        public string BaseItem { get; set; } = ""; // "Wine"
        public string PreservedItem { get; set; } = ""; // "Starfruit"

        public StackInBox(Item item)
        {
            this.ItemName = item.DisplayName;
            this.ItemCategory = item.getCategoryName();
            this.ItemQuantity = item.Stack;
            this.ItemUnitPrice = item.salePrice();
            this.StackTotalPrice = StackTotalPrice * this.ItemUnitPrice;

            /*if (item. != 0)
            {
                item.Name
                this.ItemIsPreserve = true;
                string[]? fields = Game1.objectInformation[id]?.Split('/');
                this.PreservedItem = Game1.objectInformation[preservedItemIndex].Split('/')[4];
                this.BaseItem = Game1.objectInformation[item.ParentSheetIndex].Split('/')[4];

            } */
        }
    }
}
