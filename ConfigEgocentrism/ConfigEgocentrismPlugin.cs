using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace ConfigEgocentrism
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    
    public class ConfigEgocentrismPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Judgy";
        public const string PluginName = "ConfigEgocentrism";
        public const string PluginVersion = "1.2.0";

        #region Config Definitions
        #region Config Projectiles
        private static ConfigEntry<bool> ConfigProjectilesEnabled { get; set; }

        private static ConfigEntry<float> ConfigProjectilesIntervalBase { get; set; }
        private static ConfigEntry<float> ConfigProjectilesIntervalStackMult { get; set; }
        private static ConfigEntry<float> ConfigProjectilesIntervalStackExponent { get; set; }

        private static ConfigEntry<float> ConfigProjectilesDamageBase { get; set; }
        private static ConfigEntry<float> ConfigProjectilesDamageStackMult { get; set; }
        private static ConfigEntry<float> ConfigProjectilesDamageStackExponent { get; set; }

        private static ConfigEntry<float> ConfigProjectilesMaxAmountBase { get; set; }
        private static ConfigEntry<float> ConfigProjectilesMaxAmountStackMult { get; set; }
        private static ConfigEntry<float> ConfigProjectilesMaxAmountStackExponent { get; set; }
        private static ConfigEntry<string> ConfigProjectilesMaxAmountRoundingMode { get; set; }

        private static ConfigEntry<float> ConfigProjectilesRangeBase { get; set; }
        private static ConfigEntry<float> ConfigProjectilesRangeStackMult { get; set; }
        private static ConfigEntry<float> ConfigProjectilesRangeStackExponent { get; set; }
        #endregion
        #region Config Transform
        private static ConfigEntry<bool> ConfigTransformEnabled { get; set; }

        private static ConfigEntry<float> ConfigTransformIntervalBase { get; set; }
        private static ConfigEntry<float> ConfigTransformIntervalStackMult { get; set; }
        private static ConfigEntry<float> ConfigTransformIntervalStackExponent { get; set; }

        private static ConfigEntry<float> ConfigTransformAmountBase { get; set; }
        private static ConfigEntry<float> ConfigTransformAmountStackMult { get; set; }
        private static ConfigEntry<float> ConfigTransformAmountStackExponent { get; set; }
        private static ConfigEntry<string> ConfigTransformAmountRoundingMode { get; set; }

        private static ConfigEntry<string> ConfigTransformItemFilter { get; set; }
        #endregion
        #region Config Misc
        private static ConfigEntry<string> ConfigPluginVersion { get; set; }
        #endregion
        #endregion

        private List<ItemTier> TransformInvalidItemTiers;

        public void Awake()
        {
            Log.Init(Logger);

            CreateConfig();

            BuildItemFilters();

            SetupHooks();

            Log.LogInfo(nameof(Awake) + " done.");
        }

        /* Debug Item Spawns
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                Log.LogInfo($"Player pressed F2. Spawning EgoCentrism at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("LunarSun")), transform.position, transform.forward * 5f);

                //Spawn some items to test out filters
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ArmorPlate")), transform.position, transform.forward * 10f);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("AttackSpeedOnCrit")), transform.position, transform.forward * 15f);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("BarrierOnOverHeal")), transform.position, transform.forward * 20f);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("FocusConvergence")), transform.position, transform.forward * 25f);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("BeetleGland")), transform.position, transform.forward * 30f);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("BleedOnHitVoid")), transform.position, transform.right * 5f);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ChainLightningVoid")), transform.position, transform.right * 10f);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ExtraLifeVoid")), transform.position, transform.right * 15f);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("VoidMegaCrabItem")), transform.position, transform.right * 20f);
            }
        }
        //*/

        private void CreateConfig()
        {
            //Features Toggle
            ConfigProjectilesEnabled = Config.Bind("Feature Toggle", "Projectiles Enabled", true, "Enables the generation of projectiles.");
            ConfigTransformEnabled = Config.Bind("Feature Toggle", "Transform Enabled", true, "Enables the transformation of other items.");

            //Projectiles Interval
            ConfigProjectilesIntervalBase = Config.Bind("Projectiles Interval", "Interval Base", 3.0f, "Sets the base interval between each generation of projectiles (in seconds).\nFormula: _Base_ / (Stack * StackMult)^StackExponent");
            ConfigProjectilesIntervalStackMult = Config.Bind("Projectiles Interval", "Interval Stack Multiplier", 1.0f, "Sets the stack multiplier in projectiles interval formula.\nOnly accepts positive values or 0.0. Default value used if input is negative.\nFormula: Base / (Stack * _StackMult_)^StackExponent");
            ConfigProjectilesIntervalStackExponent = Config.Bind("Projectiles Interval", "Interval Stack Exponent", 1.0f, "Sets the stack exponent in projectiles interval formula.\nOnly accepts positive values or 0.0. Default value used if input is negative.\nFormula: Base / (Stack * StackMult)^_StackExponent_");

            //Projectiles Damage
            ConfigProjectilesDamageBase = Config.Bind("Projectiles Damage", "Damage Base", 3.6f, "Sets the base damage multiplier of projectiles.\nFormula: _Base_ + (Stack * StackMult)^StackExponent");
            ConfigProjectilesDamageStackMult = Config.Bind("Projectiles Damage", "Damage Stack Multiplier", 0.0f, "Sets the stack multiplier in projectiles damage formula.\nFormula: Base + (Stack * _StackMult_)^StackExponent");
            ConfigProjectilesDamageStackExponent = Config.Bind("Projectiles Damage", "Damage Stack Exponent", 1.0f, "Sets the stack exponent in projectiles damage formula.\nFormula: Base + (Stack * StackMult)^_StackExponent_");

            //Projectiles Max Amount
            ConfigProjectilesMaxAmountBase = Config.Bind("Projectiles Max Amount", "Max Amount Base", 2.0f, "Sets the base max amount of projectiles.\nFormula: _Base_ + (Stack * StackMult)^StackExponent");
            ConfigProjectilesMaxAmountStackMult = Config.Bind("Projectiles Max Amount", "Max Amount Stack Multiplier", 1.0f, "Sets the stack multiplier in projectiles max amount formula.\nFormula: Base + (Stack * _StackMult_)^StackExponent");
            ConfigProjectilesMaxAmountStackExponent = Config.Bind("Projectiles Max Amount", "Max Amount Stack Exponent", 1.0f, "Sets the stack multiplier in projectiles max amount formula.\nFormula: Base + (Stack * StackMult)^_StackExponent_");
            ConfigProjectilesMaxAmountRoundingMode = Config.Bind("Projectiles Max Amount", "Max Amount Rounding Mode", Utils.RoundingMode.AlwaysDown.ToString(), new ConfigDescription("Sets the rounding mode when calculating projectiles max amount.", new AcceptableValueList<string>(Enum.GetNames(typeof(Utils.RoundingMode)))));

            //Projectiles Range
            ConfigProjectilesRangeBase = Config.Bind("Projectiles Range", "Range Base", 15.0f, "Sets the base targeting range of projectiles.\nFormula: _Base_ + (Stack * StackMult)^StackExponent");
            ConfigProjectilesRangeStackMult = Config.Bind("Projectiles Range", "Range Stack Mutliplier", 0.0f, "Sets the stack multiplier in projectiles range formula.\nFormula: Base + (Stack * _StackMult_)^StackExponent");
            ConfigProjectilesRangeStackExponent = Config.Bind("Projectiles Range", "Range Stack Exponent", 1.0f, "Sets the stack exponent in projectiles range formula.\nFormula: Base + (Stack * StackMult)^_StackExponent_");

            //Transform Interval
            ConfigTransformIntervalBase = Config.Bind("Transform Interval", "Interval Base", 60.0f, "Sets the base interval between each item transform (in seconds).\nFormula: _Base_ / (Stack * StackMult)^StackExponent");
            ConfigTransformIntervalStackMult = Config.Bind("Transform Interval", "Interval Stack Multiplier", 0.0f, "Sets the stack multiplier in item transform interval formula.\nOnly accepts positive values or 0.0. Default value used if input is negative.\nFormula: Base / (Stack * _StackMult_)^StackExponent");
            ConfigTransformIntervalStackExponent = Config.Bind("Transform Interval", "Interval Stack Exponent", 0.0f, "Sets the stack exponent in item transform interval formula.\nOnly accepts positive values or 0.0. Default value used if input is negative.\nFormula: Base / (Stack * StackMult)^_StackExponent_");
            
            //Transform Item Amount
            ConfigTransformAmountBase = Config.Bind("Transform Item Amount", "Transform Amount Base", 1.0f, "Sets the base number of items transformed at each iteration.\nFormula: _Base_ + (Stack * StackMult)^StackExponent");
            ConfigTransformAmountStackMult = Config.Bind("Transform Item Amount", "Transform Amount Stack Multiplier", 0.0f, "Sets the stack multiplier in transform item amount formula.\nFormula: Base + (Stack * _StackMult_)^StackExponent");
            ConfigTransformAmountStackExponent = Config.Bind("Transform Item Amount", "Transform Amount Stack Exponent", 1.0f, "Sets the stack exponent in transform item amount formula.\nFormula: Base + (Stack * StackMult)^_StackExponent_");
            ConfigTransformAmountRoundingMode = Config.Bind("Transform Item Amount", "Transform Amount Rounding Mode", Utils.RoundingMode.AlwaysDown.ToString(), new ConfigDescription("Sets the rounding mode when calculating item transform amount.", new AcceptableValueList<string>(Enum.GetNames(typeof(Utils.RoundingMode)))));

            //Transform Item Filter
            ConfigTransformItemFilter = Config.Bind("Transform Item Filter", "Filter", "untiered", "Filters Item Tiers to NOT transform. Avoid removing `untiered` from that list to not lose character passives, artifacts and other hidden stuff.\r\nFormat : tier1,tier2,tier3\r\nValid Tiers: untiered,white,green,red,blue,yellow,voidwhite,voidgreen,voidred,voidyellow");
            ConfigTransformItemFilter.SettingChanged += RebuildItemFiltersEvent;

            //Config Version Check
            ConfigPluginVersion = Config.Bind("zMiscellaneous", "Plugin Version", "0.0.0", "Plugin Version, used for cleaning old configs entries.\nAvoid editing this unless you want duplicate config entries.");
            if (ConfigPluginVersion.Value == (string)ConfigPluginVersion.DefaultValue)
                ConfigCleanup();
            ConfigPluginVersion.Value = PluginVersion;
        }

        private void ConfigCleanup()
        {
            Dictionary<ConfigDefinition, string> orphanedEntries = Config.GetPropertyValue<Dictionary<ConfigDefinition, string>>("OrphanedEntries");
            orphanedEntries.Clear();

            Config.Save();
        }

        private void RebuildItemFiltersEvent(object s, EventArgs e) => BuildItemFilters();

        private void BuildItemFilters()
        {
            TransformInvalidItemTiers = new List<ItemTier>();

            string[] inputTiers = ConfigTransformItemFilter.Value.Split(',');

            foreach(string tierStr in inputTiers)
            {
                if (string.IsNullOrWhiteSpace(tierStr))
                    continue;
                if (Enum.TryParse(tierStr.Trim().ToLower(), out Utils.ItemTierLookup tier))
                    TransformInvalidItemTiers.Add((ItemTier)tier);
                else
                    Log.LogDebug($"TransformItemFilter: Invalid tier input `{tierStr}`");
            }

            Log.LogInfo($"TransformItemFilter: Filter List built ({TransformInvalidItemTiers.Count} filters)");
        }

        private void SetupHooks()
        {
            //Full rewrite of FixedUpdate to include our config values
            On.RoR2.LunarSunBehavior.FixedUpdate += (orig, self) =>
            {
                //Grab private variables first, makes the code readable
                CharacterBody body = self.GetFieldValue<CharacterBody>("body");
                GameObject projectilePrefab = self.GetFieldValue<GameObject>("projectilePrefab");
                int stack = self.GetFieldValue<int>("stack");
                float projectileTimer = self.GetFieldValue<float>("projectileTimer");
                float transformTimer = self.GetFieldValue<float>("transformTimer");
                Xoroshiro128Plus transformRng = self.GetFieldValue<Xoroshiro128Plus>("transformRng");

                //Configurable Projectile Fire
                if(ConfigProjectilesEnabled.Value)
                {
                    projectileTimer += Time.fixedDeltaTime;
                    if (!body.master.IsDeployableLimited(DeployableSlot.LunarSunBomb) && projectileTimer > GetProjectilesInterval(stack))
                    {
                        projectileTimer = 0f;
                        //Modify Projectile Range
                        ProjectileSphereTargetFinder targetFinder = projectilePrefab.GetComponent<ProjectileSphereTargetFinder>();
                        if (targetFinder)
                            targetFinder.lookRange = GetProjectilesRange(stack);
                        else
                            Log.LogError("LunarSunBehavior: Unable to modify projectile Range (ProjectileSphereTargetFinder component not found)");

                        FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                        {
                            projectilePrefab = projectilePrefab,
                            crit = body.RollCrit(),
                            damage = body.damage * GetProjectilesDamage(stack),
                            damageColorIndex = DamageColorIndex.Item,
                            force = 0f,
                            owner = self.gameObject,
                            position = body.transform.position,
                            rotation = Quaternion.identity
                        };

                        ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                    }
                }

                //Configurable Item Transform
                if(ConfigTransformEnabled.Value)
                {
                    transformTimer += Time.fixedDeltaTime;
                    if (transformTimer > GetTransformInterval(stack))
                    {
                        transformTimer = 0f;
                        if (body.master && body.inventory)
                        {
                            int amount = GetTransformAmount(stack);
                            for(int i = 0; i < amount; i++)
                            {
                                List<ItemIndex> list = new List<ItemIndex>(body.inventory.itemAcquisitionOrder);
                                ItemIndex itemIndex = ItemIndex.None;
                                Util.ShuffleList<ItemIndex>(list, transformRng);
                                foreach (ItemIndex itemIndex2 in list)
                                {
                                    if (itemIndex2 != DLC1Content.Items.LunarSun.itemIndex)
                                    {
                                        ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex2);
                                        if (itemDef && !TransformInvalidItemTiers.Contains(itemDef.tier))
                                        {
                                            itemIndex = itemIndex2;
                                            break;
                                        }
                                    }
                                }
                                if (itemIndex != ItemIndex.None)
                                {
                                    body.inventory.RemoveItem(itemIndex, 1);
                                    body.inventory.GiveItem(DLC1Content.Items.LunarSun, 1);
                                    CharacterMasterNotificationQueue.PushItemTransformNotification(body.master, itemIndex, DLC1Content.Items.LunarSun.itemIndex, CharacterMasterNotificationQueue.TransformationType.LunarSun);
                                }
                            }
                        }
                    }
                }

                //Update previously grabbed private variables as needed
                self.SetFieldValue<float>("projectileTimer", projectileTimer);
                self.SetFieldValue<float>("transformTimer", transformTimer);
            };

            //Rewrite of GetMaxProjectiles to include our config values
            On.RoR2.LunarSunBehavior.GetMaxProjectiles += (orig, inventory) => GetProjectilesMaxAmount(inventory.GetItemCount(DLC1Content.Items.LunarSun));
        }

        private float GetProjectilesInterval(int stack)
        {
            float baseInterval = ConfigProjectilesIntervalBase.Value;
            float stackMult = Utils.GetPositiveConfigFloat(ConfigProjectilesIntervalStackMult);
            float stackExponent = Utils.GetPositiveConfigFloat(ConfigProjectilesIntervalStackExponent);

            return Utils.GetDividingFormulaValue(baseInterval, stack, stackMult, stackExponent);
        }

        private float GetProjectilesDamage(int stack)
        {
            float baseDamage = ConfigProjectilesDamageBase.Value;
            float stackMult = ConfigProjectilesDamageStackMult.Value;
            float stackExponent = ConfigProjectilesDamageStackExponent.Value;

            return Utils.GetAdditionalFormulaValue(baseDamage, stack, stackMult, stackExponent);
        }

        private int GetProjectilesMaxAmount(int stack)
        {
            float baseAmount = ConfigProjectilesMaxAmountBase.Value;
            float stackMult = ConfigProjectilesMaxAmountStackMult.Value;
            float stackExponent = ConfigProjectilesMaxAmountStackExponent.Value;

            float result = Utils.GetAdditionalFormulaValue(baseAmount, stack, stackMult, stackExponent);
            return Utils.Round(result, ConfigProjectilesMaxAmountRoundingMode.Value);
        }

        private float GetProjectilesRange(int stack)
        {
            float baseRange = ConfigProjectilesRangeBase.Value;
            float stackMult = ConfigProjectilesRangeStackMult.Value;
            float stackExponent = ConfigProjectilesRangeStackExponent.Value;

            return Utils.GetAdditionalFormulaValue(baseRange, stack, stackMult, stackExponent);
        }

        private float GetTransformInterval(int stack)
        {
            float baseInterval = ConfigTransformIntervalBase.Value;
            float stackMult = Utils.GetPositiveConfigFloat(ConfigTransformIntervalStackMult);
            float stackExponent = Utils.GetPositiveConfigFloat(ConfigTransformIntervalStackExponent);

            return Utils.GetDividingFormulaValue(baseInterval, stack, stackMult, stackExponent);
        }

        private int GetTransformAmount(int stack)
        {
            float baseAmount = ConfigTransformAmountBase.Value;
            float stackMult = ConfigTransformAmountStackMult.Value;
            float stackExponent = ConfigTransformAmountStackExponent.Value;

            float result = Utils.GetAdditionalFormulaValue(baseAmount, stack, stackMult, stackExponent);
            return Utils.Round(result, ConfigTransformAmountRoundingMode.Value);
        }
    }
}
