using Smod2.API;
using Smod2.Events;
using System;

namespace RemoteKeycard
{
    public enum ItemInt
    {
        NULL = -1,
        O5_LEVEL_KEYCARD = 0,
        CHAOS_INSURGENCY_DEVICE = 1,
        FACILITY_MANAGER_KEYCARD = 2,
        MTF_COMMANDER_KEYCARD = 3,
        MTF_LIEUTENANT_KEYCARD = 4,
        CONTAINMENT_ENGINEER_KEYCARD = 5,
        SENIOR_GUARD_KEYCARD = 6,
        GUARD_KEYCARD = 7,
        ZONE_MANAGER_KEYCARD = 8,
        MAJOR_SCIENTIST_KEYCARD = 9,
        SCIENTIST_KEYCARD = 10,
        JANITOR_KEYCARD = 11
    }

    public class LogicManager
    {
        // Remote
        public static void LogicOneRemote(PlayerDoorAccessEvent ev)
        {
            if (ev.Door.Permission != string.Empty && ev.Allow == false && ev.Destroy == false && ev.Player.GetInventory().Count > 0 && ev.Door.Locked == false)
            {
                for (int z = 0; z < 12; z++)
                {
                    if (ConfigManagers.Manager.CardsList.Contains(z))
                    {
                        ItemType item = ConfigManagers.Manager.DCard[z];
                        if (ev.Player.HasItem(item))
                        {
                            if (ConfigManagers.Manager.DefaultCardAccess[z].Contains(ev.Door.Permission))
                            {
                                ev.Allow = true;
                                if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE1: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{item}' thanks to permission '{ev.Door.Permission}'.");
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Custom Card Access +- Remote and Default
        public static void LogicTwo(PlayerDoorAccessEvent ev)
        {
            if (ev.Door.Permission != string.Empty && ev.Door.Destroyed == false && ev.Door.Locked == false)
            {
                ItemInt CurrentCard = Enum.TryParse(ev.Player.GetCurrentItem().ItemType.ToString(), out ItemInt z) ? z : ItemInt.NULL;

                int CardID = (int)CurrentCard;
                if (ConfigManagers.Manager.CustomCardAccess.ContainsKey(CardID))
                {
                    if (ConfigManagers.Manager.CustomCardAccess[CardID].Contains(ev.Door.Permission))
                    {
                        ev.Allow = true;
                    }
                    else
                    {
                        if (ConfigManagers.Manager.RPCRemote)
                        {
                            bool Really = false;
                            for (int x = 0; x < 12; z++)
                            {
                                if (ConfigManagers.Manager.CardsList.Contains(x))
                                {
                                    ItemInt CurrentItem = (ItemInt)x;
                                    ItemType ResultItem = (ItemType)Enum.Parse(typeof(ItemType), CurrentItem.ToString());

                                    if (ev.Player.HasItem(ResultItem))
                                    {
                                        if (ConfigManagers.Manager.CustomCardAccess.ContainsKey(x))
                                        {
                                            if (ConfigManagers.Manager.CustomCardAccess[x].Contains(ev.Door.Permission))
                                            {
                                                if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE2: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{ResultItem}' thanks to permission '{ev.Door.Permission}'.");
                                                Really = true;
                                                ev.Allow = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (!Really) ev.Allow = false;
                        }
                        else ev.Allow = false;
                    }
                }
                else if (ConfigManagers.Manager.RPCDefaultIfNone)
                {
                    if (ConfigManagers.Manager.DefaultCardAccess.ContainsKey(CardID))
                    {
                        if (ConfigManagers.Manager.DefaultCardAccess[CardID].Contains(ev.Door.Permission)) ev.Allow = true;
                        else
                        {
                            if (ConfigManagers.Manager.RPCRemote)
                            {
                                bool Really = false;
                                for (int n = 0; n < 12; n++)
                                {
                                    if (ConfigManagers.Manager.CardsList.Contains(n))
                                    {
                                        ItemInt CurrentItem = (ItemInt)n;
                                        ItemType ResultItem = (ItemType)Enum.Parse(typeof(ItemType), CurrentItem.ToString());

                                        if (ev.Player.HasItem(ResultItem))
                                        {
                                            if (ConfigManagers.Manager.DefaultCardAccess[n].Contains(ev.Door.Permission))
                                            {
                                                if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE2: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{ResultItem}' thanks to permission '{ev.Door.Permission}'.");
                                                Really = true;
                                                ev.Allow = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (!Really) ev.Allow = false;
                            }
                            else ev.Allow = false;
                        }
                    }
                    else ev.Allow = false;
                }
                else ev.Allow = false;
            }
        }

        // Door List +- Remote and Default
        public static void LogicTree(PlayerDoorAccessEvent ev)
        {
            if (ev.Door.Name != string.Empty && ev.Door.Destroyed == false && ev.Door.Locked == false)
            {
                ItemInt CurrentCard = Enum.TryParse(ev.Player.GetCurrentItem().ItemType.ToString(), out ItemInt z) ? z : ItemInt.NULL;

                int CardID = (int)CurrentCard;
                if (ConfigManagers.Manager.CustomDoorList.ContainsKey(ev.Door.Name))
                {
                    if (ConfigManagers.Manager.CustomDoorList[ev.Door.Name].Contains(CardID)) ev.Allow = true;
                    else
                    {
                        if (ConfigManagers.Manager.RPCRemote)
                        {
                            bool Really = false;
                            for (int v = 0; v < 12; v++)
                            {
                                if (ConfigManagers.Manager.CardsList.Contains(v))
                                {
                                    ItemInt CurrentItem = (ItemInt)v;
                                    ItemType ResultItem = (ItemType)Enum.Parse(typeof(ItemType), CurrentItem.ToString());

                                    if (ev.Player.HasItem(ResultItem))
                                    {
                                        if (ConfigManagers.Manager.CustomDoorList[ev.Door.Name].Contains(v))
                                        {
                                            if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE3: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{ResultItem}' thanks to permission '{ev.Door.Permission}'.");
                                            Really = true;
                                            ev.Allow = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!Really) ev.Allow = false;
                        }
                        else ev.Allow = false;
                    }
                }
                else if (ConfigManagers.Manager.RPCDefaultIfNone)
                {
                    if (ConfigManagers.Manager.DefaultDoorList.ContainsKey(ev.Door.Name))
                    {
                        if (ConfigManagers.Manager.DefaultDoorList[ev.Door.Name].Contains(CardID)) ev.Allow = true;
                        else
                        {
                            if (ConfigManagers.Manager.RPCRemote)
                            {
                                bool Really = false;
                                for (int k = 0; k < 12; k++)
                                {
                                    if (ConfigManagers.Manager.CardsList.Contains(k))
                                    {
                                        ItemInt CurrentItem = (ItemInt)k;
                                        ItemType ResultItem = (ItemType)Enum.Parse(typeof(ItemType), CurrentItem.ToString());

                                        if (ev.Player.HasItem(ResultItem))
                                        {
                                            if (ConfigManagers.Manager.DefaultDoorList[ev.Door.Name].Contains(k))
                                            {
                                                if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE3: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{ResultItem}' thanks to permission '{ev.Door.Permission}'.");
                                                Really = true;
                                                ev.Allow = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (!Really) ev.Allow = false;
                            }
                            else ev.Allow = false;
                        }
                    }
                    else ev.Allow = true;
                }
                else ev.Allow = false;
            }
        }

        // Door Access +- Remote and Default
        public static void LogicFour(PlayerDoorAccessEvent ev)
        {
            if (ev.Door.Name != string.Empty && ev.Door.Locked == false && ev.Door.Destroyed == false)
            {
                ItemInt CurrentCard = Enum.TryParse(ev.Player.GetCurrentItem().ItemType.ToString(), out ItemInt z) ? z : ItemInt.NULL;

                int CardID = (int)CurrentCard;
                if (ConfigManagers.Manager.CustomDoorAccess.ContainsKey(ev.Door.Name))
                {
                    if (ConfigManagers.Manager.DefaultCardAccess.ContainsKey(CardID))
                    {
                        if (ConfigManagers.Manager.DefaultCardAccess[CardID].Contains(ConfigManagers.Manager.CustomDoorAccess[ev.Door.Name])) ev.Allow = true;
                    }
                    else
                    {
                        if (ConfigManagers.Manager.RPCRemote)
                        {
                            bool Really = false;
                            for (int b = 0; b < 12; b++)
                            {
                                if (ConfigManagers.Manager.CardsList.Contains(b))
                                {
                                    ItemInt CurrentItem = (ItemInt)b;
                                    ItemType ResultItem = (ItemType)Enum.Parse(typeof(ItemType), CurrentItem.ToString());

                                    if (ev.Player.HasItem(ResultItem))
                                    {
                                        if (ConfigManagers.Manager.DefaultCardAccess[b].Contains(ConfigManagers.Manager.CustomDoorAccess[ev.Door.Name]))
                                        {
                                            if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE4: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{ResultItem}' thanks to permission '{ev.Door.Permission}'.");
                                            Really = true;
                                            ev.Allow = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!Really) ev.Allow = false;
                        }
                        else ev.Allow = false;
                    }
                }
                else if (ConfigManagers.Manager.RPCDefaultIfNone)
                {
                    if (ConfigManagers.Manager.DefaultDoorAccess.ContainsKey(ev.Door.Name))
                    {
                        if (ConfigManagers.Manager.DefaultCardAccess.ContainsKey(CardID))
                        {
                            if (ConfigManagers.Manager.DefaultCardAccess[CardID].Contains(ConfigManagers.Manager.DefaultDoorAccess[ev.Door.Name])) ev.Allow = true;
                        }
                        else
                        {
                            if (ConfigManagers.Manager.RPCRemote)
                            {
                                bool Really = false;
                                for (int f = 0; f < 12; f++)
                                {
                                    if (ConfigManagers.Manager.CardsList.Contains(f))
                                    {
                                        ItemInt CurrentItem = (ItemInt)f;
                                        ItemType ResultItem = (ItemType)Enum.Parse(typeof(ItemType), CurrentItem.ToString());

                                        if (ev.Player.HasItem(ResultItem))
                                        {
                                            if (ConfigManagers.Manager.DefaultCardAccess[f].Contains(ConfigManagers.Manager.DefaultDoorAccess[ev.Door.Name]))
                                            {
                                                if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE4: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{ResultItem}' thanks to permission '{ev.Door.Permission}'.");
                                                Really = true;
                                                ev.Allow = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (!Really) ev.Allow = false;
                            }
                            else ev.Allow = false;
                        }
                    }
                    else ev.Allow = true;
                }
                else ev.Allow = false;
            }
        }

        // Door List + Door Access +- Remote and Default
        public static void LogicFive(PlayerDoorAccessEvent ev)
        {
            if (ev.Door.Name != string.Empty && ev.Door.Locked == false && ev.Door.Destroyed == false)
            {
                ItemInt CurrentCard = Enum.TryParse(ev.Player.GetCurrentItem().ItemType.ToString(), out ItemInt z) ? z : ItemInt.NULL;

                int CardID = (int)CurrentCard;
                if (ConfigManagers.Manager.CustomDoorList.ContainsKey(ev.Door.Name) && ConfigManagers.Manager.CustomDoorAccess.ContainsKey(ev.Door.Name))
                {
                    if (ConfigManagers.Manager.CustomDoorList[ev.Door.Name].Contains(CardID) && ConfigManagers.Manager.DefaultCardAccess[CardID].Contains(ConfigManagers.Manager.CustomDoorAccess[ev.Door.Name])) ev.Allow = true;
                    else
                    {
                        if (ConfigManagers.Manager.RPCRemote)
                        {
                            bool Really = false;
                            for (int g = 0; g < 12; g++)
                            {
                                ItemInt CurrentItem = (ItemInt)g;
                                ItemType ResultItem = (ItemType)Enum.Parse(typeof(ItemType), CurrentItem.ToString());
                                if (ev.Player.HasItem(ResultItem))
                                {
                                    if (ConfigManagers.Manager.CustomDoorList[ev.Door.Name].Contains(g) && ConfigManagers.Manager.DefaultCardAccess[g].Contains(ConfigManagers.Manager.CustomDoorAccess[ev.Door.Name]))
                                    {
                                        if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE5: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{ResultItem}' thanks to permission '{ev.Door.Permission}'.");
                                        Really = true;
                                        ev.Allow = true;
                                        break;
                                    }
                                }
                            }
                            if (!Really) ev.Allow = false;
                        }
                        else ev.Allow = false;
                    }
                }
                else if (ConfigManagers.Manager.RPCDefaultIfNone)
                {
                    if (ConfigManagers.Manager.DefaultDoorList.ContainsKey(ev.Door.Name) && ConfigManagers.Manager.DefaultDoorAccess.ContainsKey(ev.Door.Name))
                    {
                        if (ConfigManagers.Manager.DefaultDoorList[ev.Door.Name].Contains(CardID) && ConfigManagers.Manager.DefaultCardAccess[CardID].Contains(ConfigManagers.Manager.DefaultDoorAccess[ev.Door.Name])) ev.Allow = true;
                        else
                        {
                            if (ConfigManagers.Manager.RPCRemote)
                            {
                                bool Really = false;
                                for (int h = 0; h < 12; h++)
                                {
                                    if (ConfigManagers.Manager.DefaultDoorList[ev.Door.Name].Contains(h) && ConfigManagers.Manager.DefaultCardAccess[h].Contains(ConfigManagers.Manager.DefaultDoorAccess[ev.Door.Name]))
                                    {
                                        if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE5: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{ConfigManagers.Manager.DCard[h]}' thanks to permission '{ev.Door.Permission}'.");
                                        Really = true;
                                        ev.Allow = true;
                                        break;
                                    }
                                }
                                if (!Really) ev.Allow = false;
                            }
                            else ev.Allow = false;
                        }
                    }
                    else ev.Allow = true;
                }
                else ev.Allow = false;
            }
        }

        // Custom Card Access + Door Access +- Remote and Default
        public static void LogicSix(PlayerDoorAccessEvent ev)
        {
            if (ev.Door.Name != string.Empty && ev.Door.Locked == false && ev.Door.Destroyed == false)
            {
                ItemInt CurrentCard = Enum.TryParse(ev.Player.GetCurrentItem().ItemType.ToString(), out ItemInt z) ? z : ItemInt.NULL;

                int CardID = (int)CurrentCard;
                if (ConfigManagers.Manager.CustomCardAccess.ContainsKey(CardID) && ConfigManagers.Manager.CustomDoorAccess.ContainsKey(ev.Door.Name))
                {
                    if (ConfigManagers.Manager.CustomCardAccess[CardID].Contains(ConfigManagers.Manager.CustomDoorAccess[ev.Door.Name])) ev.Allow = true;
                    else
                    {
                        if (ConfigManagers.Manager.RPCRemote)
                        {
                            bool Really = false;
                            for (int j = 0; j < 12; j++)
                            {
                                ItemInt CurrentItem = (ItemInt)j;
                                ItemType ResultItem = (ItemType)Enum.Parse(typeof(ItemType), CurrentItem.ToString());

                                if (ev.Player.HasItem(ResultItem))
                                {
                                    if (ConfigManagers.Manager.CustomCardAccess.ContainsKey(j))
                                    {
                                        if (ConfigManagers.Manager.CustomCardAccess[j].Contains(ConfigManagers.Manager.CustomDoorAccess[ev.Door.Name]))
                                        {
                                            if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE5: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{ResultItem}' thanks to permission '{ev.Door.Permission}'.");
                                            Really = true;
                                            ev.Allow = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!Really) ev.Allow = false;
                        }
                        else ev.Allow = false;
                    }
                }
                else if (ConfigManagers.Manager.RPCDefaultIfNone)
                {
                    if (ConfigManagers.Manager.DefaultDoorAccess.ContainsKey(ev.Door.Name))
                    {
                        if (ConfigManagers.Manager.DefaultCardAccess.ContainsKey(CardID))
                        {
                            if (ConfigManagers.Manager.DefaultCardAccess[CardID].Contains(ConfigManagers.Manager.DefaultDoorAccess[ev.Door.Name])) ev.Allow = true;
                            else
                            {
                                if (ConfigManagers.Manager.RPCRemote)
                                {
                                    bool Really = false;
                                    for (int t = 0; t < 12; t++)
                                    {
                                        ItemInt CurrentItem = (ItemInt)t;
                                        ItemType ResultItem = (ItemType)Enum.Parse(typeof(ItemType), CurrentItem.ToString());

                                        if (ev.Player.HasItem(ResultItem))
                                        {
                                            if (ConfigManagers.Manager.DefaultCardAccess[t].Contains(ConfigManagers.Manager.DefaultDoorAccess[ev.Door.Name]))
                                            {
                                                if (ConfigManagers.Manager.RPCInfo) RemoteKeycard.plugin.Info($"INFO_MODE5: Player '{ev.Player.Name}' open the door '{ev.Door.Name}' with the help '{ResultItem}' thanks to permission '{ev.Door.Permission}'.");
                                                Really = true;
                                                ev.Allow = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!Really) ev.Allow = false;
                                }
                                else ev.Allow = false;
                            }
                        }
                        else ev.Allow = false;
                    }
                    else ev.Allow = true;
                }
                else ev.Allow = false;
            }
        }
    }
}
