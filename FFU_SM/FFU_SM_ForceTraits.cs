using MonoMod;
using System;
using System.Collections.Generic;
using TheLastStand.Controller.Item;
using TheLastStand.Controller.Meta;
using TheLastStand.Controller.Unit.Stat;
using TheLastStand.Database;
using TheLastStand.Database.Unit;
using TheLastStand.Definition.Item;
using TheLastStand.Definition.Meta;
using TheLastStand.Definition.Unit;
using TheLastStand.Definition.Unit.PlayableUnitGeneration;
using TheLastStand.Manager;
using TheLastStand.Manager.Item;
using TheLastStand.Manager.Meta;
using TheLastStand.Manager.Unit;
using TheLastStand.Model.Item;
using TheLastStand.Serialization.Unit;
using TheLastStand.View.CharacterSheet;
using TPLib;

namespace TheLastStand.Controller.Unit {
    public class patch_PlayableUnitController : PlayableUnitController {
        [MonoModIgnore] public patch_PlayableUnitController(SerializedPlayableUnit serializedPlayableUnit, int saveVersion, bool isDead) : base (serializedPlayableUnit, saveVersion, isDead) { }
        [MonoModIgnore] private bool CanOnlyHaveTwoHandsWeapons(PlayableUnitGenerationDefinition unitGenerationDefinition) { return false; }
		[MonoModIgnore] private void AddGeneratedTrait(string traitId, ref int currentTraitPoints, bool forceAdd = false) {
			if (AddTrait(traitId, forceAdd)) {
				currentTraitPoints -= PlayableUnitDatabase.UnitTraitDefinitions[traitId].Cost;
			}
		}
		private void Generate(int traitPoints, int level = 1) {
			string text = "--- PlayableUnit generation: " + PlayableUnit.Name + " ---";
			PlayableUnitGenerationDefinition playableUnitGenerationDefinition = PlayableUnitDatabase.PlayableUnitGenerationDefinitions[PlayableUnit.ArchetypeId];
			text += "\n---- Unit stats generation ----";
			base.Unit.UnitStatsController = new PlayableUnitStatsController(PlayableUnit);
			text += "\n---- End of unit stats generation ----";
			foreach (KeyValuePair<ItemSlotDefinition.E_ItemSlotId, UnitEquipmentSlotDefinition> unitEquipmentSlotDefinition in PlayableUnitDatabase.UnitEquipmentSlotDefinitions) {
				int i = 0;
				for (int @base = unitEquipmentSlotDefinition.Value.Base; i < @base; i++) {
					if (!PlayableUnit.EquipmentSlots.ContainsKey(unitEquipmentSlotDefinition.Key)) {
						PlayableUnit.EquipmentSlots.Add(unitEquipmentSlotDefinition.Key, new List<EquipmentSlot>());
					}
					PlayableUnit.EquipmentSlots[unitEquipmentSlotDefinition.Key].Add(new EquipmentSlotController(ItemDatabase.ItemSlotDefinitions[unitEquipmentSlotDefinition.Key], CharacterSheetPanel.EquipmentSlots[unitEquipmentSlotDefinition.Key][i], PlayableUnit).EquipmentSlot);
				}
			}
			text += "\n---- Unit traits generation ----";
			text += $"\nStarts with {traitPoints} trait points";
			bool flag = false;
			List<string> list = new List<string>(playableUnitGenerationDefinition.BackgroundTraitAvailableIds);
			//foreach (string backgroundTrait in playableUnitGenerationDefinition.BackgroundTraitAvailableIds) FFU_Shattered_Magic.ModLog.Warning($"Background Trait: {backgroundTrait}");
			//foreach (string secondaryTrait in PlayableUnitDatabase.SecondaryTraitIds) FFU_Shattered_Magic.ModLog.Warning($"Secondary Trait: {secondaryTrait}");
			bool flag2 = CanOnlyHaveTwoHandsWeapons(playableUnitGenerationDefinition);
			if (flag2) {
				list.Remove("One-Armed");
			}
			string[] lockedTraitsIds = TPSingleton<MetaUpgradesManager>.Instance.GetLockedTraitsIds();
			foreach (string item in lockedTraitsIds) {
				if (list.Contains(item)) {
					list.Remove(item);
				}
			}
			for (int num = list.Count - 1; num >= 0; num--) {
				string backgroundTraitId = RandomManager.GetRandomElement(this, list);
				text += $"\nTrying to add background trait: {backgroundTraitId} which costs {PlayableUnitDatabase.UnitTraitDefinitions[backgroundTraitId].Cost} points";
				List<string> list2 = new List<string>(PlayableUnitDatabase.SecondaryTraitIds);
				lockedTraitsIds = TPSingleton<MetaUpgradesManager>.Instance.GetLockedTraitsIds();
				foreach (string item2 in lockedTraitsIds) {
					if (list2.Contains(item2)) {
						list2.Remove(item2);
					}
				}
				if (flag2) {
					list2.Remove("One-Armed");
				}
				for (int num2 = list2.Count - 1; num2 >= 0; num2--) {
					string secondTraitId = RandomManager.GetRandomElement(this, list2);
					text += $"\nTrying to add second trait: {secondTraitId} which costs {PlayableUnitDatabase.UnitTraitDefinitions[secondTraitId].Cost} points";
					if (!PlayableUnitDatabase.UnitTraitDefinitions[backgroundTraitId].Incompatibilities.Contains(secondTraitId)) {
						int remainingCost = traitPoints - PlayableUnitDatabase.UnitTraitDefinitions[backgroundTraitId].Cost - PlayableUnitDatabase.UnitTraitDefinitions[secondTraitId].Cost;
						if (!PlayableUnitDatabase.SecondaryTraitCost.Contains(remainingCost)) {
							text = text + "\nFAILED --> Can't find any matching third trait for " + backgroundTraitId + " & " + secondTraitId + " because there are all in first traits Incompatibilities or the trait points will not be all spent";
						} else {
							List<string> list3 = list2.FindAll((string id) => id != secondTraitId && !PlayableUnitDatabase.UnitTraitDefinitions[id].Incompatibilities.Contains(backgroundTraitId) && !PlayableUnitDatabase.UnitTraitDefinitions[id].Incompatibilities.Contains(secondTraitId) && PlayableUnitDatabase.UnitTraitDefinitions[id].Cost == remainingCost);
							if (list3.Count != 0) {
								string randomElement = RandomManager.GetRandomElement(this, list3);
								AddGeneratedTrait("Hero", ref traitPoints, forceAdd: true);
								AddGeneratedTrait("Energetic", ref traitPoints, forceAdd: true);
								AddGeneratedTrait("DemonBlood", ref traitPoints, forceAdd: true);
								text += $"\nSUCCESS --> Third trait picked: {randomElement} which costs {PlayableUnitDatabase.UnitTraitDefinitions[randomElement].Cost} points";
								flag = true;
								break;
							}
							text = text + "\nFAILED --> Can't find any matching third trait for " + backgroundTraitId + " & " + secondTraitId + " because there are all in first traits Incompatibilities or the trait points will not be all spent";
						}
					} else {
						text = text + "\nFAILED --> " + secondTraitId + " is in " + backgroundTraitId + " Incompatibilities";
					}
					list2.Remove(backgroundTraitId);
				}
				if (flag) {
					break;
				}
				list.Remove(backgroundTraitId);
			}
			text += "\n---- End of unit traits generation ----";
			List<string> list4 = new List<string>();
			if (MetaUpgradeEffectsController.TryGetEffectsOfType<UnlockEquipmentGenerationMetaEffectDefinition>(out var effects, MetaUpgradesManager.E_MetaState.Activated)) {
				for (int k = 0; k < effects.Length; k++) {
					list4.Add(effects[k].Id);
				}
			}
			foreach (KeyValuePair<ItemSlotDefinition.E_ItemSlotId, EquipmentGenerationDefinition> equipmentGenerationDefinition in playableUnitGenerationDefinition.EquipmentGenerationDefinitions) {
				ItemSlotDefinition.E_ItemSlotId key = equipmentGenerationDefinition.Key;
				if (!PlayableUnit.EquipmentSlots.ContainsKey(key)) {
					continue;
				}
				EquipmentGenerationDefinition value = equipmentGenerationDefinition.Value;
				int num3 = value.TotalWeight;
				List<Tuple<int, EquipmentGenerationDefinition.ItemGenerationData>> list5 = new List<Tuple<int, EquipmentGenerationDefinition.ItemGenerationData>>(value.ItemsPerWeight);
				for (int l = 0; l < list5.Count; l++) {
					if (list5[l].Item2.ItemId != string.Empty && !list4.Contains(list5[l].Item2.ItemId)) {
						num3 -= list5[l].Item1;
						list5.RemoveAt(l--);
					}
				}
				int num4 = RandomManager.GetRandomRange(this, 0, num3);
				foreach (Tuple<int, EquipmentGenerationDefinition.ItemGenerationData> item4 in list5) {
					num4 -= item4.Item1;
					if (num4 >= 0) {
						continue;
					}
					string itemsList = item4.Item2.ItemsList;
					int level2 = new LevelProbabilitiesTreeController(playableUnitGenerationDefinition.BaseGenerationLevel, ItemDatabase.ItemGenerationModifierListDefinitions[item4.Item2.ItemLevelModifiersList]).GenerateLevel();
					ItemsListDefinition itemsListDefinition = ItemDatabase.ItemsListDefinitions[itemsList];
					int num5 = 0;
					bool flag3 = false;
					foreach (KeyValuePair<ItemSlotDefinition.E_ItemSlotId, List<EquipmentSlot>> equipmentSlot in PlayableUnit.EquipmentSlots) {
						if (equipmentSlot.Key == ItemSlotDefinition.E_ItemSlotId.LeftHand) {
							flag3 = true;
							break;
						}
					}
					Func<ItemDefinition, bool> func = (ItemDefinition itemDefinition) => itemDefinition.Hands == ItemDefinition.E_Hands.OneHand;
					bool flag4;
					do {
						flag4 = true;
						ItemDefinition itemDefinition2 = ItemManager.TakeRandomItemInList(itemsListDefinition, (key == ItemSlotDefinition.E_ItemSlotId.RightHand && !flag3) ? func : null);
						if (itemDefinition2 != null) {
							if (itemDefinition2.Hands == ItemDefinition.E_Hands.TwoHands && !flag3) {
								flag4 = false;
							}
							if (flag4) {
								ItemManager.ItemGenerationInfo generationInfo = default(ItemManager.ItemGenerationInfo);
								generationInfo.Destination = key;
								generationInfo.ItemDefinition = itemDefinition2;
								generationInfo.Level = level2;
								generationInfo.Rarity = RarityProbabilitiesTreeController.GenerateRarity(ItemDatabase.ItemRaritiesListDefinitions[item4.Item2.ItemRaritiesList]);
								generationInfo.SkipMalusAffixes = true;
								TheLastStand.Model.Item.Item item3 = ItemManager.GenerateItem(generationInfo);
								EquipItem(item3, null, shouldRefreshMetaCondition: false, onLoad: true);
							}
						}
					}
					while (!flag4 && ++num5 < 1000);
					if (num5 == 1000) {
						TPSingleton<PlayableUnitManager>.Instance.LogWarning("The generation of " + PlayableUnit.PlayableUnitName + "'s equipment took way longer than expected and couldn't find a suitable item.");
					}
					break;
				}
			}
			PlayableUnit.UnitStatsController.SnapBaseStatTo(UnitStatDefinition.E_Stat.Health, UnitStatDefinition.E_Stat.HealthTotal);
			PlayableUnit.UnitStatsController.SnapBaseStatTo(UnitStatDefinition.E_Stat.Armor, UnitStatDefinition.E_Stat.ArmorTotal);
			PlayableUnit.UnitStatsController.SnapBaseStatTo(UnitStatDefinition.E_Stat.Mana, UnitStatDefinition.E_Stat.ManaTotal);
			PlayableUnit.UnitStatsController.SnapBaseStatTo(UnitStatDefinition.E_Stat.ActionPoints, UnitStatDefinition.E_Stat.ActionPointsTotal);
			PlayableUnit.UnitStatsController.SnapBaseStatTo(UnitStatDefinition.E_Stat.MovePoints, UnitStatDefinition.E_Stat.MovePointsTotal);
			UpdateInjuryStage();
			if (PlayableUnit.EquippedWeaponSetIndex != 0) {
				SwitchWeaponSet();
			}
			while ((double)level > PlayableUnit.Level) {
				LevelUp();
				PlayableUnit.ExperienceInCurrentLevel = 0f;
			}
			text += "\n--- End of playableUnit generation ---\n";
			TPSingleton<PlayableUnitManager>.Instance.Log(text);
		}
	}
}