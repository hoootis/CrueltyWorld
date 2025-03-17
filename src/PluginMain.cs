using System;
using System.Globalization;
using System.Security.Permissions;
using BepInEx;
using UnityEngine;
using System.Security.Permissions;
using On.HUD;
using RWCustom;
using Random = UnityEngine.Random;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace CrueltyWorld
{
    [BepInPlugin(id, name, version)]
    public class PluginMain : BaseUnityPlugin
    {
        public const string id = "mod.hootis.crueltyworld";
        public const string name = "Cruelty World";
        public const string version = "1.0";

        public static TriangleMesh border;
        public static FSprite redman;
        public static Vector2 redmanDirection = new Vector2(1f, 1f);

        public static FSprite ammoBoy;
        public static FLabel ammoBoyCountdown;
        public static float cycleTimerOffset;

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

        public void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld rainWorld)
        {
            orig(rainWorld);
            
            // load textures
            for (int i = 0; i < 10; i++)
            {
                Helpers.LoadFromImage(i + ".png", "crueltykarma-" + i);
            }
            
            Helpers.LoadFromImage("flesh_automaton.png", "crueltyworld-" + "flesh_automaton");
            Helpers.LoadFromImage("power_in_misery.png", "crueltyworld-" + "power_in_misery");
            Helpers.LoadFromImage("divine_light.png", "crueltyworld-" + "divine_light");
            Helpers.LoadFromImage("hope_eradicated.png", "crueltyworld-" + "hope_eradicated");
            
            Helpers.LoadFromImage("redman.png", "crueltyworld-" + "redman");
            
            Helpers.LoadFromImage("ammoboy.png", "ammoboy");
            
            // redman and border
            border = TriangleMesh.MakeLongMesh(1, false, true, "crueltyworld-divine_light");
            Vector2[] screenCorners =
            [
                new(0, 0),
                new(Screen.width, 0),
                new(0, Screen.height),
                new(Screen.width, Screen.height),
            ];
            for (int i = 0; i < 4; i++)
            {
                border.vertices[i] = screenCorners[i];
            }
            redman = new FSprite("crueltyworld-redman");
            redman.SetPosition(Screen.height / 2f, Screen.width / 2f);
            On.PlayerGraphics.AddToContainer += PlayerGraphicsOnAddToContainer;
            On.PlayerGraphics.DrawSprites += PlayerGraphicsOnDrawSprites;
            
            // karmas
            On.HUD.KarmaMeter.KarmaSymbolSprite += KarmaMeterOnKarmaSymbolSprite;
            
            // rain meter to ammo meter
            On.HUD.RainMeter.Draw += RainMeterOnDraw;
            On.HUD.RainMeter.ctor += RainMeterOnctor;
        }

        private void RainMeterOnctor(RainMeter.orig_ctor orig, HUD.RainMeter self, HUD.HUD hud, FContainer fContainer)
        {
            orig(self, hud, fContainer);

            ammoBoy = new FSprite("ammoboy");
            ammoBoyCountdown = new FLabel(Custom.GetFont(), "");
            ammoBoyCountdown.color = Color.green;
            cycleTimerOffset = Random.Range(-0.2f, 1.3f);
            fContainer.AddChild(ammoBoy);
            fContainer.AddChild(ammoBoyCountdown);
        }

        private void RainMeterOnDraw(RainMeter.orig_Draw orig, HUD.RainMeter self, float timeStacker)
        {
            ammoBoy.SetPosition(100, Futile.screen.pixelHeight / 2f);
            ammoBoy.rotation = Mathf.Sin(Time.realtimeSinceStartup * ((1 - self.fRain) * 40 + 1)) * 45f;
            ammoBoyCountdown.text = (self.fRain + cycleTimerOffset).ToString(CultureInfo.InvariantCulture);
            ammoBoyCountdown.SetPosition(ammoBoy.GetPosition());
        }

        private string KarmaMeterOnKarmaSymbolSprite(KarmaMeter.orig_KarmaSymbolSprite orig, bool small, IntVector2 k)
        {
            return "crueltykarma-" + k.x;
        }

        private void PlayerGraphicsOnDrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            
            border.SetElementByName("crueltyworld-" + self.player.Karma switch
            {  
                5 => "divine_light",
                0 => "power_in_misery",
                > 5 => "hope_eradicated",
                > 0 => "flesh_automaton",
                _ => "divine_light"
            });
            
            redman.SetPosition(redman.GetPosition() + redmanDirection);
            if (redman.y - redman.element.sourcePixelSize.y / 2 < 0)
            {
                redmanDirection.y = -redmanDirection.y;
            }
            if (redman.x - redman.element.sourcePixelSize.x / 2 < 0)
            {
                redmanDirection.x = -redmanDirection.x;
            }
            if (redman.y + redman.element.sourcePixelSize.y / 2 > Futile.screen.pixelHeight)
            {
                redmanDirection.y = -redmanDirection.y;
            }
            if (redman.x + redman.element.sourcePixelSize.x / 2 > Futile.screen.pixelWidth)
            {
                redmanDirection.x = -redmanDirection.x;
            }

            redman.rotation += 1f;
        }

        private void PlayerGraphicsOnAddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            orig(self, sLeaser, rCam, newContatiner);
            
            border.RemoveFromContainer();
            redman.RemoveFromContainer();
            rCam.ReturnFContainer("HUD").AddChild(border);
            rCam.ReturnFContainer("HUD").AddChild(redman);
        }
    }
}
