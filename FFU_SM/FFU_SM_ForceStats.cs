#pragma warning disable CS0108

using MonoMod;
using System.Collections.Generic;
using System.Linq;
using TheLastStand.Database.Unit;
using TheLastStand.Definition.Unit;
using TheLastStand.Framework.Extensions;
using TheLastStand.Manager;
using TheLastStand.Manager.Unit;
using TheLastStand.Model.Unit;
using TheLastStand.Serialization.Unit;
using TPLib;
using UnityEngine;

namespace TheLastStand.Controller.Unit {
    public class patch_UnitLevelUpController : UnitLevelUpController {
        [MonoModIgnore] public patch_UnitLevelUpController(UnitLevelUpDefinition definition, SerializedLevelUpBonuses container) : base (definition, container) { }
		public void DrawAvailableStats(bool isDrawingMainStat = true) {
			UnitLevelUp.CommonNbReroll = 9;
			List<UnitLevelUp.SelectedStatToLevelUp> selectedStatToLevelUps;
			Dictionary<UnitStatDefinition.E_Stat, UnitLevelUpStatDefinition> dictionary;
			int a;
			List<int> list;
			if (isDrawingMainStat) {
				selectedStatToLevelUps = UnitLevelUp.AvailableMainStats;
				dictionary = PlayableUnitDatabase.UnitLevelUpMainStatDefinitions;
				a = UnitLevelUp.MainNbReroll;
				list = UnitLevelUp.UnitLevelUpDefinition.MainStatDraws;
			} else {
				selectedStatToLevelUps = UnitLevelUp.AvailableSecondaryStats;
				dictionary = PlayableUnitDatabase.UnitLevelUpSecondaryStatDefinitions;
				a = UnitLevelUp.SecondaryNbReroll;
				list = UnitLevelUp.UnitLevelUpDefinition.SecondaryStatDraws;
			}
			selectedStatToLevelUps.Clear();
			DeselectStat();
			int num = 5;
			while (num != selectedStatToLevelUps.Count) {
				List<UnitLevelUpStatDefinition> list2 = new List<UnitLevelUpStatDefinition>(dictionary.Values);
				list2.RemoveAll((UnitLevelUpStatDefinition stat) => selectedStatToLevelUps.Any((UnitLevelUp.SelectedStatToLevelUp y) => y.Definition.Stat == stat.Stat) || UnitLevelUp.PlayableUnit.UnitStatsController.GetStat(stat.Stat).Base >= UnitLevelUp.PlayableUnit.UnitStatsController.GetStat(stat.Stat).Boundaries.y);
				float num2 = 0f;
				int count = list2.Count;
				if (count == 0) {
					TPSingleton<PlayableUnitManager>.Instance.LogWarning("No available stat found on unit to level up -> Returning a random one. This can be perfectly fine if you used debug commands to buff a hero.");
					UnitLevelUp.SelectedStatToLevelUp item = new UnitLevelUp.SelectedStatToLevelUp(dictionary.Values.PickRandom(), UnitLevelUp.E_StatLevelUpRarity.BigRarity);
					selectedStatToLevelUps.Add(item);
					break;
				}
				for (int i = 0; i < count; i++) {
					num2 += list2[i].Weight;
				}
				float randomRange = RandomManager.GetRandomRange(this, 0f, num2);
				float num3 = 0f;
				for (int j = 0; j < count; j++) {
					num3 += list2[j].Weight;
					if (!(randomRange <= num3)) {
						continue;
					}
					HashSet<int> hashSet = new HashSet<int>();
					foreach (KeyValuePair<UnitLevelUp.E_StatLevelUpRarity, int> bonuse in list2[j].Bonuses) {
						hashSet.Add((int)bonuse.Key);
					}
					int num4 = (int)(RarityProbabilitiesTreeController.GenerateRarity(UnitLevelUp.UnitLevelUpDefinition.RaritiesList) - 1);
					if (hashSet.Contains(num4)) {
						UnitLevelUp.SelectedStatToLevelUp item2 = new UnitLevelUp.SelectedStatToLevelUp(list2[j], UnitLevelUp.E_StatLevelUpRarity.BigRarity);
						selectedStatToLevelUps.Add(item2);
						break;
					}
				}
			}
		}
	}
}