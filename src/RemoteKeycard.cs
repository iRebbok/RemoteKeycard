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

        // Just make an empty space as []
        public ItemType[] Cards { get; set; } = Array.Empty<ItemType>();
    }

    public sealed class RemoteKeycard : Plugin<RKConfig>
    {
        internal static RemoteKeycard instance;

        private readonly LogicHandler _logicHandler = new LogicHandler();
        private readonly Harmony _harmony = new Harmony("dev.rebb");
        private readonly MethodInfo _prefixToPatch = AccessTools.Method("Exiled.Events.Patches.Events.Player.ActivatingWarheadPanel:Prefix");
        private readonly HarmonyMethod _transpiler = new HarmonyMethod(typeof(RemoteKeycard), nameof(ExiledPrefixPatch));

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
#if DEBUG
            Log.Debug($"Allowed items for processing: {(Config.Cards?.Length > 0 ? string.Join(", ", Config.Cards) : "(null)")}");

            var lastDebug = Harmony.DEBUG;
            Harmony.DEBUG = true;
#endif
            _harmony.Patch(_prefixToPatch, transpiler: _transpiler);
#if DEBUG
            Harmony.DEBUG = lastDebug;
#endif
        }

        public override void OnDisabled()
        {
            PlayerHandlers.InteractingDoor -= _logicHandler.OnDoorAccess;
            PlayerHandlers.InteractingLocker -= _logicHandler.OnLockerAccess;
            PlayerHandlers.UnlockingGenerator -= _logicHandler.OnGeneratorAccess;
            PlayerHandlers.ActivatingWarheadPanel -= _logicHandler.OnOutsitePanelAccess;
            _harmony.Unpatch(_prefixToPatch, _transpiler.method);
        }

        private static IEnumerable<CodeInstruction> ExiledPrefixPatch(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Predicate<CodeInstruction> searchPredicate = i => i.opcode == OpCodes.Ldloc_1;

            var index = newInstructions.FindIndex(searchPredicate);
            var label = newInstructions[index + 1].operand;

            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PlayerInteract), nameof(PlayerInteract._inv))),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Inventory), nameof(Inventory.items))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(SyncList<Inventory.SyncItemInfo>), nameof(SyncList<Inventory.SyncItemInfo>.Count))),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse, label)
            });

            index = newInstructions.FindIndex(index, searchPredicate);
            label = newInstructions[newInstructions.FindIndex(index, i => i.opcode == OpCodes.Br_S || i.opcode == OpCodes.Br)].operand;

            var notNullLabel = generator.DefineLabel();
            newInstructions[index].WithLabels(notNullLabel);
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Brtrue, notNullLabel),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Br, label)
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
