using HarmonyLib;
using Kitchen;

namespace KitchenSayCheeese.Patches
{
    [HarmonyPatch]
    internal static class ApplianceView_Patch
    {
        [HarmonyPatch(typeof(ApplianceView), "UpdateData")]
        [HarmonyPrefix]
        static void UpdateData_Prefix(ApplianceView __instance, ref ApplianceView.ViewData view_data)
        {
            //if (view_data.InteractTarget && PlayerInteractionController.RenderInteractedAppliancesOnly)
            //{
            //    __instance.gameObject.SetLayer(Main.INTERACTION_LAYER);
            //}
            //else
            //{
            //    __instance.gameObject.SetLayer(LayerMask.NameToLayer("Default"));
            //}
            view_data.InteractTarget = !PlayerInteractionController.HighlightInteraction? false : view_data.InteractTarget;
        }
    }
}
