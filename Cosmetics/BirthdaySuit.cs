using KitchenData;
using KitchenLib.Customs;
using UnityEngine;

namespace KitchenSayCheeese.Cosmetics
{
    public class BirthdaySuit : CustomPlayerCosmetic
    {
        public override string UniqueNameID => "birthdaysuit";
        public override CosmeticType CosmeticType => CosmeticType.Outfit;
        public override GameObject Visual => new GameObject();

        public override void OnRegister(GameDataObject gameDataObject)
        {
        }
    }
}