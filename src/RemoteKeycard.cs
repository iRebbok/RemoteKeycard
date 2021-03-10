using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using HarmonyLib;
using Mirror;
using NorthwoodLib.Pools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

using PlayerHandlers = Exiled.Events.Handlers.Player;

namespace RemoteKeycard
{
    public sealed class RKConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool HandleLockersAccess { get; set; } = true;
        public bool HandleGeneratorsAccess { get; set; } = true;
        public bool HandleOutsidePanelAccess { get; set; } = true;
        public bool TreatTutorialsAsHumans { get; set; }

        // Just make an empty space as []
        public ItemType[] Cards { get; set; } = Array.Empty<ItemType>();
    }

    public sealed class RemoteKeycard : Plugin<RKConfig>
    {
        internal static RemoteKeycard instance;

        public override System.Version RequiredExiledVersion { get; } = new System.Version(2, 9, 0);

        private readonly LogicHandler _logicHandler = new LogicHandler();
        private readonly Harmony _harmony = new Harmony("dev.rebb");
        private readonly MethodInfo _methodToPatch = AccessTools.Method("PlayerInteract:CallCmdSwitchAWButton");
        private readonly HarmonyMethod _transpiler = new HarmonyMethod(typeof(RemoteKeycard), nameof(ActivatingOutsitePanelPatch));

        public bool PatchedSuccessfully { get; private set; }
        public override PluginPriority Priority => PluginPriority.Higher;

        public RemoteKeycard()
        {
            instance = this;
        }

        public override void OnEnabled()
        {
            PlayerHandlers.InteractingDoor += _logicHandler.OnDoorAccess;
            PlayerHandlers.InteractingLocker += _logicHandler.OnLockerAccess;
            PlayerHandlers.UnlockingGenerator += _logicHandler.OnGeneratorAccess;
            PlayerHandlers.ActivatingWarheadPanel += _logicHandler.OnOutsitePanelAccess;

            Debug($"Allowed items for processing: {(Config.Cards?.Length > 0 ? string.Join(", ", Config.Cards) : "(null)")}");

            PatchSafely();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            PlayerHandlers.InteractingDoor -= _logicHandler.OnDoorAccess;
            PlayerHandlers.InteractingLocker -= _logicHandler.OnLockerAccess;
            PlayerHandlers.UnlockingGenerator -= _logicHandler.OnGeneratorAccess;
            PlayerHandlers.ActivatingWarheadPanel -= _logicHandler.OnOutsitePanelAccess;

            UnpatchSafely();

            base.OnDisabled();
        }

        private void PatchSafely()
        {
            try
            {
#if DEBUG
                var lastDebug = Harmony.DEBUG;
                Harmony.DEBUG = true;
#endif
                _harmony.Patch(_methodToPatch, transpiler: _transpiler);
#if DEBUG
                Harmony.DEBUG = lastDebug;
#endif

                PatchedSuccessfully = true;
            }
            catch (Exception e)
            {
                Log.Error("An exception was thrown during patching, it won't interfere with the overall work of the plugin but affects the activating outsite panel feature.");
                Log.Error(e);
            }
        }

        private void UnpatchSafely()
        {
            try
            {
                if (PatchedSuccessfully)
                {
                    _harmony.Unpatch(_methodToPatch, _transpiler.method);
                }

                PatchedSuccessfully = false;
            }
            catch (Exception e)
            {
                Log.Error("An exception was thrown during unpatching");
                Log.Error(e);
            }
        }

        private static IEnumerable<CodeInstruction> ActivatingOutsitePanelPatch(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // Ok, so I need to make it invoke the event even if
            // the item is null (the one in the player's hands)
            // ---> there's no need to invoke the event if the player
            //      has no items in his inventory

            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Predicate<CodeInstruction> searchPredicate = i => i.opcode == OpCodes.Ldloc_1;

            var index = newInstructions.FindIndex(searchPredicate);

            // Get the label of br below ldloc.1
            var label = newInstructions[index + 1].operand;

            // Remove ldloc.1 & br
            newInstructions.RemoveRange(index, 2);
            newInstructions.InsertRange(index, new[]
            {
                // if (this._inv.items.Count == 0)
                //      return;

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PlayerInteract), nameof(PlayerInteract._inv))),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Inventory), nameof(Inventory.items))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(SyncList<Inventory.SyncItemInfo>), nameof(SyncList<Inventory.SyncItemInfo>.Count))),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse, label)
            });

            index = newInstructions.FindIndex(index, searchPredicate);

            // Get the label of newobj of the event
            label = newInstructions[newInstructions.FindIndex(index, i => i.opcode == OpCodes.Br_S || i.opcode == OpCodes.Br)].operand;

            var notNullLabel = generator.DefineLabel();
            newInstructions[index].WithLabels(notNullLabel);

            // if (item != null)
            //      push item.permissions.Contains("CONT_LVL_3");    <--- in the IL
            // else
            //      push false
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Brtrue, notNullLabel),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Br_S, label)
            });

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        [Conditional("DEBUG")]
        public static void Debug(string message) => Log.Debug(message, true);

        // I don't love reflection
        public override void OnRegisteringCommands() { }
        public override void OnUnregisteringCommands() { }
    }
}
