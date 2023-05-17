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
            bool checkered = KeyManager.GetButton(KeyCode.LeftControl);
            if (thing is HydroponicTray tray)
            {
                foreach (Pipe item in tray.PipeNetwork.StructureList)
                {
                    if (item is HydroponicTray && (!checkered || NPutility.CheckeredPaintCheck(thing, item)))
                    {
                        NPutility.Paint(item, colorIndex);
                    }
                }
                return;
            }
            if (thing is PassiveVent pv)
            {
                foreach (Pipe item in pv.PipeNetwork.StructureList)
                {
                    if (item is PassiveVent && (!checkered || NPutility.CheckeredPaintCheck(thing, item)))
                    {
                        NPutility.Paint(item, colorIndex);
                    }
                }
                return;
            }
            if (thing is Pipe pipe)
            {
                foreach (Pipe item in pipe.PipeNetwork.StructureList)
                {
                    if (!(item is PassiveVent) && !(item is HydroponicTray) && (!checkered || NPutility.CheckeredPaintCheck(thing, item)))
                    {
                        NPutility.Paint(item, colorIndex);
                    }
                }
                return;
            }
            if (thing is Cable cable)
            {
                foreach (Cable item in cable.CableNetwork.CableList)
                {
                    if (!checkered || NPutility.CheckeredPaintCheck(thing, item))
                    {
                        NPutility.Paint(item, colorIndex);
                    }
                }
                return;
            }
            if (thing is Chute chute)
            {
                foreach (Chute item in chute.ChuteNetwork.StructureList)
                {
                    if (!checkered || NPutility.CheckeredPaintCheck(thing, item))
                    {
                        NPutility.Paint(item, colorIndex);
                    }
                }
                return;
            }
            return;
        }
    }

    public class NPutility
    {
        public static void Paint(Thing thing, int colorIndex)
        {
            thing.SetCustomColor(colorIndex);
            if (NetworkManager.IsClient)
            {
                NetworkClient.SendToServer(new ThingColorMessage
                {
                    ThingId = thing.netId,
                    ColorIndex = thing.CustomColor.Index
                });
            }
        }

        public static bool CheckeredPaintCheck(Thing original, Thing thing)
        {
            float one = (Mathf.Round(Mathf.Abs(original.Position.x) * 2) % 2) == (Mathf.Round(Mathf.Abs(thing.Position.x) * 2) % 2) ? 1 : 0;
            float two = (Mathf.Round(Mathf.Abs(original.Position.y) * 2) % 2) == (Mathf.Round(Mathf.Abs(thing.Position.y) * 2) % 2) ? 1 : 0;
            float three = (Mathf.Round(Mathf.Abs(original.Position.z) * 2) % 2) == (Mathf.Round(Mathf.Abs(thing.Position.z) * 2) % 2) ? 1 : 0;

            return (one + two + three) % 2 != 0;
        }
    }
}
