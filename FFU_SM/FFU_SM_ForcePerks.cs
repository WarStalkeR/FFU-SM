using MonoMod;
using System;
using System.Collections.Generic;
using TheLastStand.Definition.Unit.Perk;
using TheLastStand.Manager;
using TheLastStand.Manager.Unit;
using TheLastStand.Model.Unit;
using TheLastStand.View.Unit.Perk;
using TPLib;
using TPLib.Log;

namespace TheLastStand.Controller.Unit.Perk {
    public class patch_UnitPerkTreeController : UnitPerkTreeController {
		[MonoModIgnore] public patch_UnitPerkTreeController(UnitPerkTreeView view, PlayableUnit playableUnit) : base(view, playableUnit) { }
		[MonoModIgnore] private bool HasPerkAlready(PerkDefinition perkDefinition) { return false; }
		private PerkDefinition PickRandomPerkDefinition(List<UnitPerkCollectionDefinition> collections, int perkTierIndex, int perkIndex) {
			List<Tuple<PerkDefinition, int>> list = new List<Tuple<PerkDefinition, int>>();
			int num = 0;
			if (collections[perkIndex] != null) {
				foreach (Tuple<PerkDefinition, int> item in collections[perkIndex].PerksFromTier[perkTierIndex + 1]) {
					//FFU_Shattered_Magic.ModLog.Message($"Collection #{perkIndex}, Tier #{perkTierIndex}, Perk: {item.Item1.Id}");
					switch (perkIndex) {
						case 0: switch (item.Item1.Id) {
							case "Sprint": return item.Item1;
							case "Bodybuilder": return item.Item1;
							case "Vampire": return item.Item1;
							case "HeadOn": return item.Item1;
							case "Boom": return item.Item1;
						} break;
						case 1: switch (item.Item1.Id) {
							case "Harvester": return item.Item1;
							case "MagicFuel": return item.Item1;
							case "Energized": return item.Item1;
							case "Blink": return item.Item1;
							case "SheerPower": return item.Item1;
						} break;
						case 2: switch (item.Item1.Id) {
							case "SteadyAim": return item.Item1;
							case "Spotter": return item.Item1;
							case "LongerWeapons": return item.Item1;
							case "ProximityShot": return item.Item1;
							case "SurgicalStrike": return item.Item1;
						} break;
						case 5: switch (item.Item1.Id) {
							case "Coagulation": return item.Item1;
							case "MartialPunch": return item.Item1;
							case "LoneWolf": return item.Item1;
							case "ManaShield": return item.Item1;
							case "NightOwl": return item.Item1;
						} break;
						case 6: switch (item.Item1.Id) {
							case "PotionThrow": return item.Item1;
							case "Overload": return item.Item1;
							case "RunicGift": return item.Item1;
							case "VolatileReaction": return item.Item1;
							case "DontPanic": return item.Item1;
						} break;
						case 7: switch (item.Item1.Id) {
							case "QuickReload": return item.Item1;
							case "Nimble": return item.Item1;
							case "Initiator": return item.Item1;
							case "Flexibility": return item.Item1;
							case "BigGameHunter": return item.Item1;
						} break;
                    }
					if (!HasPerkAlready(item.Item1)) {
						list.Add(item);
						num += item.Item2;
					}
				}
			}
			if (list.Count == 0) {
				TPSingleton<PlayableUnitManager>.Instance.LogError("Something went wrong on the perk tree generation :\n" + $"There are no potential perks for slot : {perkIndex + 1} ; tier : {perkTierIndex + 1} ; Collection : {collections[perkIndex]?.Id}. Use default instead.");
				return null;
			}
			int randomRange = RandomManager.GetRandomRange(TPSingleton<PlayableUnitManager>.Instance, 0, num);
			foreach (Tuple<PerkDefinition, int> item2 in list) {
				num -= item2.Item2;
				if (randomRange >= num) {
					return item2.Item1;
				}
			}
			TPSingleton<PlayableUnitManager>.Instance.LogError("Something went wrong with the weight algorithm.", CLogLevel.MAJOR);
			return null;
		}
	}
}
