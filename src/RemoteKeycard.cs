using EXILED;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RemoteKeycard
{
    public sealed class RemoteKeycard : Plugin
    {
        internal const string VERSION = "1.4.2";

        private readonly LogicHandler _logicHandler = new LogicHandler();

        public override string getName { get; } = nameof(RemoteKeycard);

        public override void OnReload() { }

        public override void OnDisable()
        {
            Events.DoorInteractEvent -= _logicHandler.OnDoorAccess;
        }

        public override void OnEnable()
        {
            if (!Config.GetBool("rk_disable"))
            {
                var arrayItems = Config.GetStringList("rk_cards");
                if (arrayItems.Count != 0)
                {
                    var allowedItems = new List<ItemType>();
                    ItemType allowedItem = ItemType.None;
                    foreach (var item in arrayItems)
                    {
                        if (Enum.TryParse<ItemType>(item, true, out var enumedItem))
                        {
                            allowedItem = enumedItem;
                        }
                        else if (int.TryParse(item, NumberStyles.Number, CultureInfo.InvariantCulture, out var numericItem)
                            && Enum.IsDefined(typeof(ItemType), numericItem))
                        {
                            allowedItem = (ItemType)numericItem;
                        }

                        if (allowedItem == ItemType.None)
                            continue;

                        allowedItems.Add(allowedItem);
                    }
                    _logicHandler._allowedTypes = allowedItems;
                }

                Events.DoorInteractEvent += _logicHandler.OnDoorAccess;
            }
        }
    }
}
