﻿using HarmonyLib;
using System;
using UnityEngine;

namespace NetworkPainter
{
    #region BepInEx
    [BepInEx.BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class NetworkPainter : BepInEx.BaseUnityPlugin
    {
        public const string pluginGuid = "net.elmo.stationeers.NetworkPainter";
        public const string pluginName = "NetworkPainter";
        public const string pluginVersion = "1.4";
        public static void Log(string line)
        {
            Debug.Log("[" + pluginName + "]: " + line);
        }
        void Awake()
        {
            if (Harmony.HasAnyPatches(pluginGuid))
            {
                return;
            }
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
