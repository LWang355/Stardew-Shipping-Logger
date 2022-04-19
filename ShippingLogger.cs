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

        private ShippingLog shippingLog;

        private void OnLoad(object sender, EventArgs e)
        {
            shippingLog.extractPlayerInfo(Game1.player);
        }

        private void OnDayStart(object sender, EventArgs e)
        {
            SDate date = SDate.Now();
            shippingLog.dayInfo.moneyAtStart = Game1.player.Money;

        }

        private void OnDayEnd(object sender, EventArgs e)
        {
            // TODO:
            // The part that actually records the contents of the bin
            shippingLog.dayInfo.moneyAtEnd = Game1.player.Money;
            shippingLog.calculateProfits();

            // TODO:
            // The part that writes out to a file
        }

    }
    class ShippingLog
    {
        public PlayerInfo playerInfo;
        public DayInfo dayInfo;
        public IList<StackInBox> boxContents = new List<StackInBox>();

        public void calculateProfits()
        {
            this.dayInfo.dayProfit = this.dayInfo.moneyAtStart - this.dayInfo.moneyAtEnd;
            this.dayInfo.binRevenue = this.CalculateBinRevenue();
        }

        public int CalculateBinRevenue()
        {
            int total = 0;
            foreach (StackInBox binItem in this.boxContents)
            {
                total += binItem.quantity * binItem.unitPrice;
            }
            return total;
        }

        public void extractPlayerInfo(Farmer farmer)
        {

        }
    }

    class DayInfo 
    {
        public int runDay { get; set; }
        public string season { get; set; }
        public int dayOfSeason { get; set; }
        public int year { get; set; }
        public int moneyAtStart { get; set; }
        public int moneyAtEnd { get; set; }
        public int binRevenue { get; set; }
        public int dayProfit { get; set; }

        public DayInfo(SDate sDate)
        {
            runDay = sDate.DaysSinceStart;
            season = sDate.Season;
            dayOfSeason = sDate.Day;
            year = sDate.Year;
        }
    }
    class PlayerInfo
    {
        public string playerName { get; set; }
        public string farmName { get; set; }
        public int uniqueId { get; set; }
    }

    class StackInBox
    {
        public string itemName { get; set; }
        public int category { get; set; }
        public int quantity { get; set; }
        public int unitPrice { get; set; }
        public int totalPrice { get; set; }

        // Is this item stack a "preserve" item such as jelly, pickles, wine
        public Boolean isPreserve { get; set; } = false;
        // "Starfruit Wine"
        // If false, these fields are ignored
        public string baseItem { get; set; } = ""; // "Wine"
        public string preservedItem { get; set; } = ""; // "Starfruit"
        

    }
}
