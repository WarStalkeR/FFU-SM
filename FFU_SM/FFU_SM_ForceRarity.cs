#pragma warning disable CS0108
#pragma warning disable CS0626

using MonoMod;
using System.Collections;
using TheLastStand.Definition.Item;
using TheLastStand.Manager;

namespace TheLastStand.View.ProductionReport {
	public class patch_ChooseRewardPanel : ChooseRewardPanel {
		private extern IEnumerator orig_ReloadRerollRewardShelfs();
		private IEnumerator ReloadRerollRewardShelfs() {
			PanicManager.Panic.PanicReward.RemainingNbRerollReward = 9;
			return orig_ReloadRerollRewardShelfs();
		}
	}
}

namespace TheLastStand.Manager.Item {
	public class patch_ItemManager : ItemManager {
		public static Model.Item.Item GenerateItem(ItemSlotDefinition.E_ItemSlotId itemDestination, CreateItemDefinition createItemDefinition, int level) {
			ItemDefinition itemDefinition = TakeRandomItemInList(createItemDefinition.ItemsListDefinition);
			int higherExistingLevelFromInitValue = itemDefinition.GetHigherExistingLevelFromInitValue(level);
			int num = 1000;
			while (higherExistingLevelFromInitValue == -1 && --num > 0) {
				itemDefinition = TakeRandomItemInList(createItemDefinition.ItemsListDefinition);
				higherExistingLevelFromInitValue = itemDefinition.GetHigherExistingLevelFromInitValue(level);
			}
			ItemGenerationInfo generationInfo = default(ItemGenerationInfo);
			generationInfo.Destination = itemDestination;
			generationInfo.ItemDefinition = itemDefinition;
			generationInfo.Level = higherExistingLevelFromInitValue;
			generationInfo.Rarity = ItemDefinition.E_Rarity.Epic;
			return GenerateItem(generationInfo);
		}
	}
}