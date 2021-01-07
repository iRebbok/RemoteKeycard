using Interactables.Interobjects.DoorUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RemoteKeycard
{
    public static class Keycard
    {
        [Flags]
        public enum Permissions
        {
            None = 0x0,
            Checkpoints = 0x1,
            ExitGates = 0x2,
            Intercom = 0x4,
            AlphaWarhead = 0x8,
            ContainmentLevelOne = 0x10,
            ContainmentLevelTwo = 0x20,
            ContainmentLevelThree = 0x40,
            ArmoryLevelOne = 0x80,
            ArmoryLevelTwo = 0x100,
            ArmoryLevelThree = 0x200,
            ScpOverride = 0x400,

            Pedestals = 0x800
        }

        public static readonly ReadOnlyDictionary<string, Permissions> BackwardsCompatibility = new ReadOnlyDictionary<string, Permissions>(new Dictionary<string, Permissions>
        {
            ["CONT_LVL_1"] = Permissions.ContainmentLevelOne,
            ["CONT_LVL_2"] = Permissions.ContainmentLevelTwo,
            ["CONT_LVL_3"] = Permissions.ContainmentLevelThree,

            ["ARMORY_LVL_1"] = Permissions.ArmoryLevelOne,
            ["ARMORY_LVL_2"] = Permissions.ArmoryLevelTwo,
            ["ARMORY_LVL_3"] = Permissions.ArmoryLevelThree,

            ["INCOM_ACC"] = Permissions.Intercom,
            ["CHCKPOINT_ACC"] = Permissions.Checkpoints,
            ["EXIT_ACC"] = Permissions.ExitGates,

            ["PEDESTAL_ACC"] = Permissions.Pedestals,
        });

        public static Permissions ToTruthyPermissions(this KeycardPermissions keycardPermissions) => (Permissions)keycardPermissions;

        public static Permissions ToTruthyPermissions(string permission)
        {
            if (string.IsNullOrEmpty(permission))
                return Permissions.None;

            BackwardsCompatibility.TryGetValue(permission, out var p);
            return p;
        }

        public static Permissions ToTruthyPermissions(string[] permissions)
        {
            var p = Permissions.None;
            for (var z = 0; z < permissions.Length; z++)
                p |= ToTruthyPermissions(permissions[z]);

            return p;
        }

        public static bool HasFlagFast(this Permissions permissions, Permissions flag, bool requireAll) => requireAll ? (permissions & flag) == flag : (permissions & flag) != 0;
    }
}
