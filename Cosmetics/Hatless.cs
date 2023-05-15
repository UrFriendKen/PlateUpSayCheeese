using KitchenData;
using KitchenLib.Customs;
using UnityEngine;

namespace KitchenSayCheeese.Cosmetics
{
    public class Hatless : CustomPlayerCosmetic
    {
        public override string UniqueNameID => "hatless";
        public override CosmeticType CosmeticType => CosmeticType.Hat;
        public override GameObject Visual => new GameObject();
        public override bool BlockHats => true;

        public override void OnRegister(GameDataObject gameDataObject)
        {
        }
    }
}