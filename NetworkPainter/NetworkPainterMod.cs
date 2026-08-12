using Assets.Scripts;
using Assets.Scripts.Networking;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using JetBrains.Annotations;
using Objects.RoboticArm;
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

            switch (thing)
            {
                case HydroponicTray tray when tray.PipeNetwork != null:
                    foreach (var item in tray.PipeNetwork.StructureList)
                        if (item is HydroponicTray) NPutility.TryPaint(thing, item as Thing, colorIndex, checkered);
                    break;
                case PassiveVent pv when pv.PipeNetwork != null:
                    foreach (var item in pv.PipeNetwork.StructureList)
                        if (item is PassiveVent) NPutility.TryPaint(thing, item as Thing, colorIndex, checkered);
                    break;
                case Pipe pipe when pipe.PipeNetwork != null:
                    foreach (var item in pipe.PipeNetwork.StructureList)
                        if (!(item is PassiveVent) && !(item is HydroponicTray))
                            NPutility.TryPaint(thing, item as Thing, colorIndex, checkered);
                    break;
                case Cable cable when cable.CableNetwork != null:
                    foreach (var item in cable.CableNetwork.CableList)
                        NPutility.TryPaint(thing, item, colorIndex, checkered);
                    break;
                case Chute chute when chute.ChuteNetwork != null:
                    foreach (var item in chute.ChuteNetwork.StructureList)
                        NPutility.TryPaint(thing, item as Thing, colorIndex, checkered);
                    break;
                case RoboticArmRailBase rail when rail.RoboticArmNetwork != null:
                    foreach (var item in rail.RoboticArmNetwork.StructureList)
                        if (!(item is RoboticArmDock)) NPutility.TryPaint(thing, item as Thing, colorIndex, checkered);
                    break;
            }
            return;
        }
    }

    public class NPutility
    {
        public static void TryPaint(Thing original, Thing item, int colorIndex, bool checkered)
        {
            if (item == null) return;
            if (checkered && !CheckeredPaintCheck(original, item)) return;

            Paint(item, colorIndex);
        }
        private static void Paint(Thing thing, int colorIndex)
        {
            thing.SetCustomColor(colorIndex);
            if (NetworkManager.IsClient)
            {
                NetworkClient.SendToServer(new ThingColorMessage
                {
                    ThingId = thing.ReferenceId,
                    ColorIndex = colorIndex
                }, NetworkChannel.GeneralTraffic);
            }
        }

        private static bool CheckeredPaintCheck(Thing original, Thing thing)
        {
            float one = (Mathf.Round(Mathf.Abs(original.Position.x) * 2) % 2) == (Mathf.Round(Mathf.Abs(thing.Position.x) * 2) % 2) ? 1 : 0;
            float two = (Mathf.Round(Mathf.Abs(original.Position.y) * 2) % 2) == (Mathf.Round(Mathf.Abs(thing.Position.y) * 2) % 2) ? 1 : 0;
            float three = (Mathf.Round(Mathf.Abs(original.Position.z) * 2) % 2) == (Mathf.Round(Mathf.Abs(thing.Position.z) * 2) % 2) ? 1 : 0;

            return (one + two + three) % 2 != 0;
        }
    }
}
