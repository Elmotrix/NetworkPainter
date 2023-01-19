using HarmonyLib;
using System;
using UnityEngine;

namespace NetworkPainter
{
    #region BepInEx
    [BepInEx.BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class AtmosphericRealismOverhaul : BepInEx.BaseUnityPlugin
    {
        public const string pluginGuid = "net.elmo.stationeers.AtmosphericRealismOverhaul";
        public const string pluginName = "AtmosphericRealismOverhaul";
        public const string pluginVersion = "1.0";
        public static void Log(string line)
        {
            Debug.Log("[" + pluginName + "]: " + line);
        }
        void Awake()
        {
            try
            {
                var harmony = new Harmony(pluginGuid);
                harmony.PatchAll();
                Log("Patch succeeded");
            }
            catch (Exception e)
            {
                Log("Patch Failed");
                Log(e.ToString());
            }
        }
    }
    #endregion
}
