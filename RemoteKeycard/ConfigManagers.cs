using Smod2.API;
using System.Collections.Generic;
using System.Linq;

namespace RemoteKeycard
{
    public class ConfigManagers
    {
        public bool RPCInfo { get; private set; }
        public bool RPCRemote { get; private set; }
        public bool RPCDefaultIfNone { get; private set; }
        public int RPCMode = 1;

        private static ConfigManagers singleton;
        public static ConfigManagers Manager
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new ConfigManagers();
                }
                return singleton;
            }
        }

        // Readonly  
        private readonly List<string> DoorPerms = new List<string>()
        {
            "CONT_LVL_1",
            "CONT_LVL_2",
            "CONT_LVL_3",
            "ARMORY_LVL_1",
            "ARMORY_LVL_2",
            "ARMORY_LVL_3",
            "CHCKPOINT_ACC",
            "EXIT_ACC",
            "INCOM_ACC"
        };
        private readonly List<string> NameDoors = new List<string>()
        {
            "HCZ_ARMORY",
            "914",
            "012_BOTTOM",
            "106_BOTTOM",
            "LCZ_ARMORY",
            "GATE_A",
            "106_SECONDARY",
            "GATE_B",
            "012",
            "079_SECOND",
            "106_PRIMARY",
            "049_ARMORY",
            "NUKE_SURFACE",
            "NUKE_ARMORY",
            "CHECKPOINT_ENT",
            "CHECKPOINT_LCZ_B",
            "HID_RIGHT",
            "173_ARMORY",
            "CHECKPOINT_LCZ_A",
            "173",
            "ESCAPE",
            "HID_LEFT",
            "HID",
            "096",
            "372",
            "ESCAPE_INNER",
            "SURFACE_GATE",
            "INTERCOM",
            "079_FIRST"
        };
        private readonly List<string> RPCModes = new List<string>()
        {
            { "Remote Door Controls" },
            { "Custom Access Card" },
            { "Door List" },
            { "Door Access" },
            { "Door List + Door Access" },
            { "Custom Access Card + Door Access" }
        };

        internal readonly Dictionary<int, List<string>> DefaultCardAccess = new Dictionary<int, List<string>>()
        {
            { 0,    new List<string>(new string[] { "CONT_LVL_1", "CONT_LVL_2", "CONT_LVL_3", "ARMORY_LVL_1", "ARMORY_LVL_2", "ARMORY_LVL_3", "CHCKPOINT_ACC", "EXIT_ACC", "INCOM_ACC" })       },
            { 1,    new List<string>(new string[] { "CONT_LVL_1", "CONT_LVL_2", "ARMORY_LVL_1", "ARMORY_LVL_2", "ARMORY_LVL_3", "CHCKPOINT_ACC", "EXIT_ACC", "INCOM_ACC" })                     },
            { 2,    new List<string>(new string[] { "CONT_LVL_1", "CONT_LVL_2", "CONT_LVL_3", "CHCKPOINT_ACC", "EXIT_ACC", "INCOM_ACC" })                                                       },
            { 3,    new List<string>(new string[] { "CONT_LVL_1", "CONT_LVL_2", "ARMORY_LVL_1", "ARMORY_LVL_2", "ARMORY_LVL_3", "CHCKPOINT_ACC", "EXIT_ACC", "INCOM_ACC" })                     },
            { 4,    new List<string>(new string[] { "CONT_LVL_1", "CONT_LVL_2", "ARMORY_LVL_1", "ARMORY_LVL_2", "CHCKPOINT_ACC", "EXIT_ACC" })                                                  },
            { 5,    new List<string>(new string[] { "CONT_LVL_1", "CONT_LVL_2", "CONT_LVL_3", "CHCKPOINT_ACC", "INCOM_ACC" })                                                                   },
            { 6,    new List<string>(new string[] { "CONT_LVL_1", "CONT_LVL_2", "ARMORY_LVL_1", "ARMORY_LVL_2", "CHCKPOINT_ACC" })                                                              },
            { 7,    new List<string>(new string[] { "CONT_LVL_1", "ARMORY_LVL_1", "CHCKPOINT_ACC" })                                                                                            },
            { 8,    new List<string>(new string[] { "CONT_LVL_1", "CHCKPOINT_ACC" })                                                                                                            },
            { 9,    new List<string>(new string[] { "CONT_LVL_1", "CONT_LVL_2", "CHCKPOINT_ACC" })                                                                                              },
            { 10,   new List<string>(new string[] { "CONT_LVL_1", "CONT_LVL_2" })                                                                                                               },
            { 11,   new List<string>(new string[] { "CONT_LVL_1" })                                                                                                                             },
        };
        internal readonly Dictionary<string, string> DefaultDoorAccess = new Dictionary<string, string>()
        {
            { "HCZ_ARMORY",                     "ARMORY_LVL_1"  },
            { "106_SECONDARY",                  "CONT_LVL_3"    },
            { "914",                            "CONT_LVL_1"    },
            { "LCZ_ARMORY",                     "ARMORY_LVL_1"  },
            { "079_SECOND",                     "CONT_LVL_3"    },
            { "GATE_A",                         "EXIT_ACC"      },
            { "GATE_B",                         "EXIT_ACC"      },
            { "106_BOTTOM",                     "CONT_LVL_3"    },
            { "106_PRIMARY",                    "CONT_LVL_3"    },
            { "NUKE_ARMORY",                    "ARMORY_LVL_2"  },
            { "012",                            "CONT_LVL_2"    },
            { "049_ARMORY",                     "ARMORY_LVL_2"  },
            { "CHECKPOINT_ENT",                 "CHCKPOINT_ACC" },
            { "NUKE_SURFACE",                   "CONT_LVL_3"    },
            { "CHECKPOINT_LCZ_A",               "CHCKPOINT_ACC" },
            { "CHECKPOINT_LCZ_B",               "CHCKPOINT_ACC" },
            { "HID",                            "ARMORY_LVL_3"  },
            { "079_FIRST",                      "CONT_LVL_3"    },
            { "096",                            "CONT_LVL_2"    },
            { "INTERCOM",                       "INCOM_ACC"     }
        };
        internal readonly Dictionary<string, List<int>> DefaultDoorList = new Dictionary<string, List<int>>()
        {
            { "HCZ_ARMORY",                     new List<int>(new int[] { 0,1,3,4,6,7 })                    },
            { "106_SECONDARY",                  new List<int>(new int[] { 0,2,5 })                          },
            { "914",                            new List<int>(new int[] { 0,1,2,3,4,5,6,7,8,9,10,11 })      },
            { "LCZ_ARMORY",                     new List<int>(new int[] { 0,1,3,4,6,7 })                    },
            { "079_SECOND",                     new List<int>(new int[] { 0,2,5 })                          },
            { "GATE_A",                         new List<int>(new int[] { 0,1,2,3,4 })                      },
            { "GATE_B",                         new List<int>(new int[] { 0,1,2,3,4 })                      },
            { "106_BOTTOM",                     new List<int>(new int[] { 0,2,5 })                          },
            { "106_PRIMARY",                    new List<int>(new int[] { 0,2,5 })                          },
            { "NUKE_ARMORY",                    new List<int>(new int[] { 0,1,3,4,6 })                      },
            { "012",                            new List<int>(new int[] { 0,1,2,3,4,5,6,9,10 })             },
            { "049_ARMORY",                     new List<int>(new int[] { 0,1,3,4,6 })                      },
            { "CHECKPOINT_ENT",                 new List<int>(new int[] { 0,1,2,3,4,5,6,7,8,9 })            },
            { "NUKE_SURFACE",                   new List<int>(new int[] { 0,2,5 })                          },
            { "CHECKPOINT_LCZ_A",               new List<int>(new int[] { 0,1,2,3,4,5,6,7,8,9 })            },
            { "CHECKPOINT_LCZ_B",               new List<int>(new int[] { 0,1,2,3,4,5,6,7,8,9 })            },
            { "HID",                            new List<int>(new int[] { 0,1,3 })                          },
            { "079_FIRST",                      new List<int>(new int[] { 0,2 })                            },
            { "096",                            new List<int>(new int[] { 0,1,2,3,4,5,6,9,10 })             },
            { "INTERCOM",                       new List<int>(new int[] { 0,1,2,3,5 })                      }
        };
        internal readonly List<ItemType> DCard = new List<ItemType>()
        {
            {  ItemType.O5_LEVEL_KEYCARD                },
            {  ItemType.CHAOS_INSURGENCY_DEVICE         },
            {  ItemType.FACILITY_MANAGER_KEYCARD        },
            {  ItemType.MTF_COMMANDER_KEYCARD           },
            {  ItemType.MTF_LIEUTENANT_KEYCARD          },
            {  ItemType.CONTAINMENT_ENGINEER_KEYCARD    },
            {  ItemType.SENIOR_GUARD_KEYCARD            },
            {  ItemType.GUARD_KEYCARD                   },
            {  ItemType.ZONE_MANAGER_KEYCARD            },
            {  ItemType.MAJOR_SCIENTIST_KEYCARD         },
            {  ItemType.SCIENTIST_KEYCARD               },
            {  ItemType.JANITOR_KEYCARD                 }
        };

        // Customs
        internal readonly List<int> CardsList = new List<int>();
        internal readonly Dictionary<int, List<string>> CustomCardAccess = new Dictionary<int, List<string>>();
        internal readonly Dictionary<string, string> CustomDoorAccess = new Dictionary<string, string>();
        internal readonly Dictionary<string, List<int>> CustomDoorList = new Dictionary<string, List<int>>();

        internal void ReloadConfig()
        {
            int CONFIG_MODE = ConfigFile.GetInt("rpc_mode", 1);

            RPCInfo = ConfigFile.GetBool("rpc_info", true);
            RPCRemote = ConfigFile.GetBool("rpc_remote", true);
            RPCDefaultIfNone = ConfigFile.GetBool("rpc_default_if_none", false);

            ClearingData();

            if (CONFIG_MODE > 0 && CONFIG_MODE < 7)
            {
                RPCMode = CONFIG_MODE;
                RemoteKeycard.plugin.Info($"Successfully loaded mode {RPCModes[RPCMode]}");
            }
            else RemoteKeycard.plugin.Warn("MODE1: The current mode is not recognized, the default value is '1'");

            if (RPCMode == 1) LoadRemote();
            else if (RPCMode == 2) LoadConfigModeOne();
            else if (RPCMode == 3) LoadConfigModeTwo();
            else if (RPCMode == 4) LoadConfigModeTree();
            else if (RPCMode == 5) LoadConfigModeFour();
            else if (RPCMode == 6) LoadConfigModeFive();
            else RemoteKeycard.plugin.Warn("MODE2: Error recognizing the current work mode, contact the developer");
        }

        internal void LoadRemote()
        {
            if (RPCRemote)
            {
                string CardList = ConfigFile.GetString("rpc_card_list", "0,1,2,3,4,5,6,7,8,9,10,11");
                string[] Cards = CardList.Split(',');

                if (Cards[0].Trim() != string.Empty)
                {
                    for (int x = 0; x < Cards.Length; x++)
                    {
                        int CardID = int.TryParse(Cards[x], out int z) ? z : -1;

                        if (CardID >= 0 && CardID <= 11)
                        {
                            if (!CardsList.Contains(CardID)) CardsList.Add(CardID);
                            else RemoteKeycard.plugin.Warn($"REMOTE4: Duplicate value '{CardID}' in CardsList");
                        }
                        else RemoteKeycard.plugin.Warn($"REMOTE3: Incorrect value '{Cards[x]}'");
                    }
                }
                else RemoteKeycard.plugin.Warn("REMOTE2: Incorrect format");
            }
            else if (RPCMode == 1) RemoteKeycard.plugin.Warn("REMOTE1: Value 'rpc_remote' installed on 'false'. I don't know why he's working now ¯\\_(ツ)_/¯");
        }

        // Custom Access Card
        internal void LoadConfigModeOne()
        {
            LoadRemote();
            string ConfigCCA = ConfigFile.GetString("rpc_card_access", string.Empty);

            if (ConfigCCA != string.Empty)
            {
                string[] CardsAndPerms = ConfigCCA.Split(',');

                foreach (string CardAndPerm in CardsAndPerms)
                {
                    if (CardAndPerm.Contains(':'))
                    {
                        string[] CardAndPermDict = CardAndPerm.Split(':');
                        string Perm = CardAndPermDict[0].Trim().ToUpper();

                        if (Perm != string.Empty)
                        {
                            if (DoorPerms.Contains(Perm))
                            {
                                if (CardAndPermDict[1].Trim() != string.Empty)
                                {
                                    string[] Cards = CardAndPermDict[1].Split('&');

                                    foreach (string Card in Cards)
                                    {
                                        int CardID = int.TryParse(Card, out int z) ? z : -1;

                                        if (CardID >= 0 && CardID <= 11)
                                        {
                                            if (!CustomCardAccess.ContainsKey(CardID)) CustomCardAccess.Add(CardID, new List<string>(new string[] { Perm }));
                                            else
                                            {
                                                if (!CustomCardAccess[CardID].Contains(Perm)) CustomCardAccess[CardID].Add(Perm);
                                                else RemoteKeycard.plugin.Warn($"CA7: Duplicate permission '{Perm}' in CardID '{CardID}'");
                                            }
                                        }
                                        else RemoteKeycard.plugin.Warn($"CA6: Incorrect value '{Card.Trim()}'");
                                    }
                                }
                                else RemoteKeycard.plugin.Warn($"CA5: CList value not set in permission '{Perm}'");
                            }
                            else RemoteKeycard.plugin.Warn($"CA4: Wrong permission '{Perm}'");
                        }
                        else RemoteKeycard.plugin.Warn("CA3: Permission value not set");

                    }
                    else RemoteKeycard.plugin.Warn($"CA2: Incorrect format in the line '{CardAndPerm}'");
                }
            }
            else RemoteKeycard.plugin.Warn("CA1: Incorrect format");
        }

        // Door List
        internal void LoadConfigModeTwo()
        {
            LoadRemote();
            string ConfigDL = ConfigFile.GetString("rpc_door_list", string.Empty);

            if (ConfigDL.Trim() != string.Empty)
            {
                string[] DoorsAndCards = ConfigDL.Split(',');

                if (DoorsAndCards[0].Trim() != string.Empty)
                {
                    foreach (string DoorAndCards in DoorsAndCards)
                    {
                        if (DoorAndCards.Contains(':'))
                        {
                            string[] DoorAndCardsDict = DoorAndCards.Split(':');
                            string DoorName = DoorAndCardsDict[0].Trim().ToUpper();

                            if (DoorName != string.Empty)
                            {
                                if (NameDoors.Contains(DoorName))
                                {
                                    string[] DoorCards = DoorAndCardsDict[1].Split('&');

                                    if (DoorCards[0].Trim() != string.Empty)
                                    {
                                        for (int z = 0; z < DoorCards.Length; z++)
                                        {
                                            int CardID = int.TryParse(DoorCards[z], out int t) ? t : -1;

                                            if (CardID >= 0 && CardID <= 11)
                                            {
                                                if (!CustomDoorList.ContainsKey(DoorName)) CustomDoorList.Add(DoorName, new List<int>(new int[] { CardID }));
                                                else
                                                {
                                                    if (!CustomDoorList[DoorName].Contains(CardID)) CustomDoorList[DoorName].Add(CardID);
                                                    else RemoteKeycard.plugin.Warn($"DL7: Duplicate CardID '{CardID}' in DoorName '{DoorName}'");
                                                }
                                            }
                                        }
                                    }
                                    else RemoteKeycard.plugin.Warn($"DL6: Incorrect format CList in DoorName '{DoorName}'");
                                }
                                else RemoteKeycard.plugin.Warn($"DL5: Incorrect name of door '{DoorName}'");
                            }
                            else RemoteKeycard.plugin.Warn("DL4: DoorName value not set");
                        }
                        else RemoteKeycard.plugin.Warn($"DL3: Incorrect format in the line '{DoorAndCards}'");
                    }
                }
                else RemoteKeycard.plugin.Warn("DL2: Incorrect format");
            }
            else RemoteKeycard.plugin.Warn("DL1: Incorrect format");
        }

        // Door Access
        internal void LoadConfigModeTree()
        {
            LoadRemote();
            string ConfigDA = ConfigFile.GetString("rpc_door_access", string.Empty);

            if (ConfigDA.Trim() != string.Empty)
            {
                string[] DoorsAndPerms = ConfigDA.Split(',');

                if (DoorsAndPerms[0].Trim() != string.Empty)
                {
                    foreach (string DoorAndPerms in DoorsAndPerms)
                    {
                        if (DoorAndPerms.Contains(':'))
                        {
                            string[] DoorAndPermsDict = DoorAndPerms.Split(':');
                            string DoorName = DoorAndPermsDict[0].Trim().ToUpper();

                            if (DoorName != string.Empty)
                            {
                                if (NameDoors.Contains(DoorName))
                                {
                                    string Perm = DoorAndPermsDict[1].Trim().ToUpper();

                                    if (Perm != string.Empty)
                                    {
                                        if (DoorPerms.Contains(Perm))
                                        {
                                            if (!CustomDoorAccess.ContainsKey(DoorName)) CustomDoorAccess.Add(DoorName, Perm);
                                            else RemoteKeycard.plugin.Warn($"DA8: Duplicate permission '{Perm}' in DoorName '{DoorName}'");
                                        }
                                        else RemoteKeycard.plugin.Warn($"DA7: Wrong permission '{Perm}'");
                                    }
                                    else RemoteKeycard.plugin.Warn($"DA6: Permission value not set in door '{DoorName}'");
                                }
                                else RemoteKeycard.plugin.Warn($"DA5: Incorrect name of door '{DoorName}'");
                            }
                            else RemoteKeycard.plugin.Warn("DA4: DoorName value not set");
                        }
                        else RemoteKeycard.plugin.Warn($"DA3: Incorrect format in the line '{DoorAndPerms}'");
                    }
                }
                else RemoteKeycard.plugin.Warn("DA2: Incorrect format");
            }
            else RemoteKeycard.plugin.Warn("DA1: Incorrect format");
        }

        // Door List + Door Access
        internal void LoadConfigModeFour()
        {
            LoadRemote();
            LoadConfigModeTwo();
            LoadConfigModeFour();
            // ¯\_(ツ)_/¯
        }

        // Custom Access Card + Door Access
        internal void LoadConfigModeFive()
        {
            LoadRemote();
            LoadConfigModeOne();
            LoadConfigModeTree();
            // ¯\_(ツ)_/¯ x2
        }

        internal void ClearingData()
        {
            CardsList.Clear();
            CustomCardAccess.Clear();
            CustomDoorAccess.Clear();
            CustomDoorList.Clear();
        }
    }
}
