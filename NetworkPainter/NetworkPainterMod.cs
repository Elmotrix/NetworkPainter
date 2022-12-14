using Assets.Scripts;
using Assets.Scripts.Networking;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace NetworkPainter
{
    [HarmonyPatch(typeof(OnServer), nameof(OnServer.SetCustomColor))]
    public class NetworkPainterMod
    {
        [UsedImplicitly]
        public static void Prefix(Thing thing, int colorIndex)
        {
            if (KeyManager.GetButton(KeyCode.LeftShift))
            {
                return;
            }
            HydroponicTray tray = thing as HydroponicTray;
            if (tray != null)
            {
                foreach (Pipe item in tray.PipeNetwork.StructureList)
                {
                    HydroponicTray trayT = item as HydroponicTray;
                    if (trayT != null)
                    {
                        item.SetCustomColor(colorIndex);
                        if (NetworkManager.IsClient)
                        {
                            NetworkClient.SendToServer(new ThingColorMessage
                            {
                                ThingId = item.netId,
                                ColorIndex = item.CustomColor.Index
                            });
                        }
                    }
                }
                return;
            }
            PassiveVent pv = thing as PassiveVent;
            if (pv != null)
            {
                foreach (Pipe item in pv.PipeNetwork.StructureList)
                {
                    PassiveVent pvt = item as PassiveVent;
                    if (pvt != null)
                    {
                        item.SetCustomColor(colorIndex);
                        if (NetworkManager.IsClient)
                        {
                            NetworkClient.SendToServer(new ThingColorMessage
                            {
                                ThingId = item.netId,
                                ColorIndex = item.CustomColor.Index
                            });
                        }
                    }
                }
                return;
            }
            Pipe pipe = thing as Pipe;
            if (pipe != null)
            {
                foreach (Pipe item in pipe.PipeNetwork.StructureList)
                {
                    PassiveVent pvt = item as PassiveVent;
                    HydroponicTray trayT = item as HydroponicTray;
                    if (pvt == null && trayT == null)
                    {
                        item.SetCustomColor(colorIndex);
                        if (NetworkManager.IsClient)
                        {
                            NetworkClient.SendToServer(new ThingColorMessage
                            {
                                ThingId = item.netId,
                                ColorIndex = item.CustomColor.Index
                            });
                        }
                    }
                }
                return;
            }
            Cable cable = thing as Cable;
            if (cable != null)
            {
                foreach (Cable item in cable.CableNetwork.CableList)
                {
                    item.SetCustomColor(colorIndex);
                    if (NetworkManager.IsClient)
                    {
                        NetworkClient.SendToServer(new ThingColorMessage
                        {
                            ThingId = item.netId,
                            ColorIndex = item.CustomColor.Index
                        });
                    }
                }
                return;
            }
            Chute chute = thing as Chute;
            if (chute != null)
            {
                foreach (Chute item in chute.ChuteNetwork.ChuteList)
                {
                    item.SetCustomColor(colorIndex);
                    if (NetworkManager.IsClient)
                    {
                        NetworkClient.SendToServer(new ThingColorMessage
                        {
                            ThingId = item.netId,
                            ColorIndex = item.CustomColor.Index
                        });
                    }
                }
                return;
            }
            return;
        }
    }
}
