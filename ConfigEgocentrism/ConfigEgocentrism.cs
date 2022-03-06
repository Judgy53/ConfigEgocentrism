using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigEgocentrism
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [R2APISubmoduleDependency(nameof(ItemAPI))]
	
	public class ExamplePlugin : BaseUnityPlugin
	{
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Judgy";
        public const string PluginName = "ConfigEgocentrism";
        public const string PluginVersion = "1.0.0";

        private static ConfigEntry<bool> ConfigProjectilesEnabled { get; set; }
        private static ConfigEntry<float> ConfigProjectilesInterval { get; set; }
        private static ConfigEntry<float> ConfigProjectilesDamage { get; set; }
        private static ConfigEntry<int> ConfigProjectilesMaxAmountBase { get; set; }
        private static ConfigEntry<int> ConfigProjectilesMaxAmountStack { get; set; }

        private static ConfigEntry<bool> ConfigTransformEnabled { get; set; }
        private static ConfigEntry<float> ConfigTransformInterval { get; set; }
        private static ConfigEntry<int> ConfigTransformCount { get; set; }

        public void Awake()
        {
            Log.Init(Logger);

			ConfigProjectilesEnabled = Config.Bind<bool>("ConfigEgocentrismProjectiles", "ProjectilesEnabled", true, "Enables the generation of projectiles.");
			ConfigProjectilesInterval = Config.Bind<float>("ConfigEgocentrismProjectiles", "ProjectilesInterval", 3.0f, "Sets the interval between each generation of projectiles.");
			ConfigProjectilesDamage = Config.Bind<float>("ConfigEgocentrismProjectiles", "ProjectilesDamage", 3.6f, "Sets the damage multiplier of projectiles.");
			ConfigProjectilesMaxAmountBase = Config.Bind<int>("ConfigEgocentrismProjectiles", "ProjectilesMaxAmountBase", 2, "Sets the base max amount of projectiles.");
			ConfigProjectilesMaxAmountStack = Config.Bind<int>("ConfigEgocentrismProjectiles", "ProjectilesMaxAmountStack", 1, "Sets the max amount of projectiles per item stack.");

			ConfigTransformEnabled = Config.Bind<bool>("ConfigEgocentrismItemTransform", "TransformEnabled", true, "Enables the transformation of other items.");
			ConfigTransformInterval = Config.Bind<float>("ConfigEgocentrismItemTransform", "TransformInterval", 60.0f, "Sets the interval between each item transform.");
			ConfigTransformCount = Config.Bind<int>("ConfigEgocentrismItemTransform", "TransformCount", 1, "Sets the number of items to transform at each iteration.");

			SetupHooks();

			Log.LogInfo(nameof(Awake) + " done.");
        }

		private void Update()
        {
			/*
			if (Input.GetKeyDown(KeyCode.F2))
			{
				var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

				Log.LogInfo($"Player pressed F2. Spawning EgoCentrism at coordinates {transform.position}");
				PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("LunarSun")), transform.position, transform.forward * 20f);
			}
			*/
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
					if (!body.master.IsDeployableLimited(DeployableSlot.LunarSunBomb) && projectileTimer > ConfigProjectilesInterval.Value / (float)stack)
					{
						projectileTimer = 0f;
						FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
						{
							projectilePrefab = projectilePrefab,
							crit = body.RollCrit(),
							damage = body.damage * ConfigProjectilesDamage.Value,
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
					if (transformTimer > ConfigTransformInterval.Value)
					{
						transformTimer = 0f;
						if (body.master && body.inventory)
						{
							for(int i = 0; i < ConfigTransformCount.Value; i++)
							{
								List<ItemIndex> list = new List<ItemIndex>(body.inventory.itemAcquisitionOrder);
								ItemIndex itemIndex = ItemIndex.None;
								Util.ShuffleList<ItemIndex>(list, transformRng);
								foreach (ItemIndex itemIndex2 in list)
								{
									if (itemIndex2 != DLC1Content.Items.LunarSun.itemIndex)
									{
										ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex2);
										if (itemDef && itemDef.tier != ItemTier.NoTier)
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
			On.RoR2.LunarSunBehavior.GetMaxProjectiles += (orig, inventory) =>
			{
				return ConfigProjectilesMaxAmountBase.Value + inventory.GetItemCount(DLC1Content.Items.LunarSun) * ConfigProjectilesMaxAmountStack.Value;
			};
		}
	}
}
