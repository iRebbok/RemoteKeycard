# RemoteKeycard
**Let's open doors without a card in hand!**

# Configuration
Key | Value type | Default value | Example |  Description
:-- | :-- | :--: | :--: | :--
is_enabled | boolean | true || Enabling/disabling the plugin's functionality
handle_lockers_access | boolean | true || Enabling/disabling handling of access to lockers
handle_generators_access | boolean | true || Enabling/disabling handling of access to generators
handle_outside_panel_access | boolean | true || Enabling/disabling handling of access to the outsite nuke panel
cards | string[] | empty array | [KeycardJanitor, 3, 11] | Specific cards that are allowed to be handled by the plugin; empty means all cards are allowed
treat_tutorials_as_humans | boolean | false || Treats tutorials as humans and allows them to open use enabled features

**Since RK now has a config block in `(server_port)-config.yml`, note it**
