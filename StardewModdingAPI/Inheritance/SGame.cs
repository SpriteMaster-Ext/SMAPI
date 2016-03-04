﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using StardewModdingAPI.Events;

namespace StardewModdingAPI.Inheritance
{
    public class SGame : Game1
    {
        public static List<SGameLocation> ModLocations = new List<SGameLocation>();
        public static SGameLocation CurrentLocation { get; internal set; }
        public static Dictionary<Int32, SObject> ModItems { get; private set; }
        public const Int32 LowestModItemID = 1000;

        public static FieldInfo[] StaticFields { get { return GetStaticFields(); } }

        public static FieldInfo[] GetStaticFields()
        {
            return typeof(Game1).GetFields();
        }

        public KeyboardState KStateNow { get; private set; }
        public KeyboardState KStatePrior { get; private set; }

        public MouseState MStateNow { get; private set; }
        public MouseState MStatePrior { get; private set; }

        public Keys[] CurrentlyPressedKeys { get; private set; }
        public Keys[] PreviouslyPressedKeys { get; private set; }

        public Keys[] FramePressedKeys 
        { 
            get { return CurrentlyPressedKeys.Where(x => !PreviouslyPressedKeys.Contains(x)).ToArray(); }
        }

        public int PreviousGameLocations { get; private set; }
        public int PreviousLocationObjects { get; private set; }
        public int PreviousItems_ { get; private set; }
        public Dictionary<Item, int> PreviousItems { get; private set; }

        public int PreviousCombatLevel { get; private set; }
        public int PreviousFarmingLevel { get; private set; }
        public int PreviousFishingLevel { get; private set; }
        public int PreviousForagingLevel { get; private set; }
        public int PreviousMiningLevel { get; private set; }
        public int PreviousLuckLevel { get; private set; }

        public GameLocation PreviousGameLocation { get; private set; }
        public IClickableMenu PreviousActiveMenu { get; private set; }

        public Int32 PreviousTimeOfDay { get; private set; }
        public Int32 PreviousDayOfMonth { get; private set; }
        public String PreviousSeasonOfYear { get; private set; }
        public Int32 PreviousYearOfGame { get; private set; }

        public Farmer PreviousFarmer { get; private set; }

        private static SGame instance;
        public static SGame Instance { get { return instance; } }

        public Farmer CurrentFarmer { get { return player; } }

        public SGame()
        {
            instance = this;

            if (Program.debug)
            {
                SaveGame.serializer = new XmlSerializer(typeof (SaveGame), new Type[28]
                {
                    typeof (Tool),
                    typeof (GameLocation),
                    typeof (Crow),
                    typeof (Duggy),
                    typeof (Bug),
                    typeof (BigSlime),
                    typeof (Fireball),
                    typeof (Ghost),
                    typeof (Child),
                    typeof (Pet),
                    typeof (Dog),
                    typeof (StardewValley.Characters.Cat),
                    typeof (Horse),
                    typeof (GreenSlime),
                    typeof (LavaCrab),
                    typeof (RockCrab),
                    typeof (ShadowGuy),
                    typeof (SkeletonMage),
                    typeof (SquidKid),
                    typeof (Grub),
                    typeof (Fly),
                    typeof (DustSpirit),
                    typeof (Quest),
                    typeof (MetalHead),
                    typeof (ShadowGirl),
                    typeof (Monster),
                    typeof (TerrainFeature),
                    typeof (SObject)
                });
            }
        }

        protected override void Initialize()
        {
            Program.Log("XNA Initialize");
            ModItems = new Dictionary<Int32, SObject>();
            PreviouslyPressedKeys = new Keys[0];
            base.Initialize();
            Events.GameEvents.InvokeInitialize();
        }

        protected override void LoadContent()
        {
            Program.Log("XNA LoadContent");
            base.LoadContent();
            Events.GameEvents.InvokeLoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateEventCalls();

            try
            {
                base.Update(gameTime);
            }
            catch (Exception ex)
            {
                Program.LogError("An error occured in the base update loop: " + ex);
                Console.ReadKey();
            }

            Events.GameEvents.InvokeUpdateTick();

            PreviouslyPressedKeys = CurrentlyPressedKeys;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Events.GraphicsEvents.InvokeDrawTick();

            if (false)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                if (CurrentLocation != null)
                    CurrentLocation.draw(spriteBatch);

                if (player != null && player.position != null)
                    spriteBatch.DrawString(dialogueFont, player.position.ToString(), new Vector2(0, 180), Color.Orange);

                spriteBatch.End();
            }
        }

        public static Int32 RegisterModItem(SObject modItem)
        {
            if (modItem.HasBeenRegistered)
            {
                Program.LogError("The item {0} has already been registered with ID {1}", modItem.Name, modItem.RegisteredId);
                return modItem.RegisteredId;
            }
            Int32 newId = LowestModItemID;
            if (ModItems.Count > 0)
                newId = Math.Max(LowestModItemID, ModItems.OrderBy(x => x.Key).First().Key + 1);
            ModItems.Add(newId, modItem);
            modItem.HasBeenRegistered = true;
            modItem.RegisteredId = newId;
            return newId;
        }

        public static SObject PullModItemFromDict(Int32 id, bool isIndex)
        {
            if (isIndex)
            {
                if (ModItems.ElementAtOrDefault(id).Value != null)
                {
                    return ModItems.ElementAt(id).Value.Clone();
                }
                Program.LogError("ModItem Dictionary does not contain index: " + id.ToString());
                return null;
            }
            if (ModItems.ContainsKey(id))
            {
                return ModItems[id].Clone();
            }
            Program.LogError("ModItem Dictionary does not contain ID: " + id.ToString());
            return null;
        }

        public static SGameLocation GetLocationFromName(String name)
        {
            return ModLocations.FirstOrDefault(n => n.name == name);
        }

        public static SGameLocation LoadOrCreateSGameLocationFromName(String name)
        {
            if (GetLocationFromName(name) != null)
                return GetLocationFromName(name);
            GameLocation gl = locations.FirstOrDefault(x => x.name == name);
            if (gl != null)
            {
                Program.LogDebug("A custom location was created for the new name: " + name);
                SGameLocation s = SGameLocation.ConstructFromBaseClass(gl);
                ModLocations.Add(s);
                return s;
            }
            if (currentLocation != null && currentLocation.name == name)
            {
                gl = currentLocation;
                Program.LogDebug("A custom location was created from the current location for the new name: " + name);
                SGameLocation s = SGameLocation.ConstructFromBaseClass(gl);
                ModLocations.Add(s);
                return s;
            }

            Program.LogDebug("A custom location could not be created for: " + name);
            return null;
        }
        
        public void UpdateEventCalls()
        {
            KStateNow = Keyboard.GetState();
            CurrentlyPressedKeys = KStateNow.GetPressedKeys();
            MStateNow = Mouse.GetState();

            foreach (Keys k in FramePressedKeys)
                Events.ControlEvents.InvokeKeyPressed(k);
            
            if (KStateNow != KStatePrior)
            {
                Events.ControlEvents.InvokeKeyboardChanged(KStatePrior, KStateNow);
                KStatePrior = KStateNow;
            }

            if (MStateNow != MStatePrior)
            {
                Events.ControlEvents.InvokeMouseChanged(MStatePrior, MStateNow);
                MStatePrior = MStateNow;
            }

            if (activeClickableMenu != null && activeClickableMenu != PreviousActiveMenu)
            {
                Events.MenuEvents.InvokeMenuChanged(PreviousActiveMenu, activeClickableMenu);
                PreviousActiveMenu = activeClickableMenu;
            }

            if (locations.GetHash() != PreviousGameLocations)
            {
                Events.LocationEvents.InvokeLocationsChanged(locations);
                PreviousGameLocations = locations.GetHash();
            }

            if (currentLocation != PreviousGameLocation)
            {
                Events.LocationEvents.InvokeCurrentLocationChanged(PreviousGameLocation, currentLocation);
                PreviousGameLocation = currentLocation;
            }

            if (player != null && player != PreviousFarmer)
            {
                Events.PlayerEvents.InvokeFarmerChanged(PreviousFarmer, player);
                PreviousFarmer = player;
            }

            if (player != null && player.combatLevel != PreviousCombatLevel)
            {
                Events.PlayerEvents.InvokeLeveledUp(EventArgsLevelUp.LevelType.Combat, player.combatLevel);
                PreviousCombatLevel = player.combatLevel;
            }

            if (player != null && player.farmingLevel != PreviousFarmingLevel)
            {
                Events.PlayerEvents.InvokeLeveledUp(EventArgsLevelUp.LevelType.Farming, player.farmingLevel);
                PreviousFarmingLevel = player.farmingLevel;
            }

            if (player != null && player.fishingLevel != PreviousFishingLevel)
            {
                Events.PlayerEvents.InvokeLeveledUp(EventArgsLevelUp.LevelType.Fishing, player.fishingLevel);
                PreviousFishingLevel = player.fishingLevel;
            }

            if (player != null && player.foragingLevel != PreviousForagingLevel)
            {
                Events.PlayerEvents.InvokeLeveledUp(EventArgsLevelUp.LevelType.Foraging, player.foragingLevel);
                PreviousForagingLevel = player.foragingLevel;
            }

            if (player != null && player.miningLevel != PreviousMiningLevel)
            {
                Events.PlayerEvents.InvokeLeveledUp(EventArgsLevelUp.LevelType.Mining, player.miningLevel);
                PreviousMiningLevel = player.miningLevel;
            }

            if (player != null && player.luckLevel != PreviousLuckLevel)
            {
                Events.PlayerEvents.InvokeLeveledUp(EventArgsLevelUp.LevelType.Luck, player.luckLevel);
                PreviousLuckLevel = player.luckLevel;
            }

            List<ItemStackChange> changedItems;            
            if (player != null && HasInventoryChanged(player.items, out changedItems))
            {
                Events.PlayerEvents.InvokeInventoryChanged(player.items, changedItems);
                PreviousItems = player.items.Where(n => n != null).ToDictionary(n => n, n => n.Stack);
            }            

            if(currentLocation != null && PreviousLocationObjects != currentLocation.objects.GetHash())
            {
                Events.LocationEvents.InvokeOnNewLocationObject(currentLocation.objects);
                PreviousLocationObjects = currentLocation.objects.GetHash();
            }

            if (timeOfDay != PreviousTimeOfDay)
            {
                Events.TimeEvents.InvokeTimeOfDayChanged(PreviousTimeOfDay, timeOfDay);
                PreviousTimeOfDay = timeOfDay;
            }

            if (dayOfMonth != PreviousDayOfMonth)
            {
                Events.TimeEvents.InvokeDayOfMonthChanged(PreviousDayOfMonth, dayOfMonth);
                PreviousDayOfMonth = dayOfMonth;
            }

            if (currentSeason != PreviousSeasonOfYear)
            {
                Events.TimeEvents.InvokeSeasonOfYearChanged(PreviousSeasonOfYear, currentSeason);
                PreviousSeasonOfYear = currentSeason;
            }

            if (year != PreviousYearOfGame)
            {
                Events.TimeEvents.InvokeYearOfGameChanged(PreviousYearOfGame, year);
                PreviousYearOfGame = year;
            }
        }

        private bool HasInventoryChanged(List<Item> items, out List<ItemStackChange> changedItems)
        {
            changedItems = new List<ItemStackChange>();
            IEnumerable<Item> actualItems = items.Where(n => n != null);
            foreach (var item in actualItems)
            {
                if (PreviousItems != null && PreviousItems.ContainsKey(item))
                {
                    if(PreviousItems[item] != item.Stack)
                    {
                        changedItems.Add(new ItemStackChange() { Item = item, StackChange = item.Stack - PreviousItems[item], ChangeType = ChangeType.StackChange });
                    }
                }
                else
                {
                    changedItems.Add(new ItemStackChange() { Item = item, StackChange = item.Stack, ChangeType = ChangeType.Added });
                }
            }

            if (PreviousItems != null)
            {
                changedItems.AddRange(PreviousItems.Where(n => !actualItems.Any(i => i == n.Key)).Select(n =>
                    new ItemStackChange() { Item = n.Key, StackChange = -n.Key.Stack, ChangeType = ChangeType.Removed }));
            }

            return (changedItems.Any());                        
        }
    }
}