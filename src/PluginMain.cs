using System;
using BepInEx;

namespace TemplateRWMod
{
    [BepInPlugin(id, name, version)]
    public class PluginMain : BaseUnityPlugin
    {
        public const string id = "hootis.templatemod"; // reflect these in modinfo.json
        public const string name = "template mod";
        public const string version = "1.0";

        private void OnEnable()
        {
            try
            {
                Logger.LogInfo("loading plugin " + name);
                On.RainWorld.OnModsInit += RainWorld_OnModsInit;
                Logger.LogInfo("plugin " + name + " is loaded!");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld rainWorld)
        {
            orig(rainWorld);
        }
    }
}
