# RemoteKeycard
**Let's open doors without a card in hand!**

# Configuration
Key | Value type | Default value | Example |  Description
:-- | :-- | :--: | :--: | :--
is_enabled | boolean | true || Enabling/disabling the plugin's functionality
handle_lockers_access | boolean | true || Enabling/disabling handling of access to lockers
handle_generators_access | boolean | true || Enabling/disabling handling of access to generators
handle_outside_panel_access | boolean | true || Enabling/disabling handling of access to the outsite nuke panel
cards | string[] | empty array | [KeycardJanitor, 3, 11] | Cards only that will allow you to do this, if its empty, then everything possible is used

**Since RK now has a config block in `(server_port)-config.yml`, note it**
