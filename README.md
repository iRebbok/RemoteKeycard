# Archived project
This project is archived and will never get my attention anymore.<br>
If you're looking for this plugin, check out the [**Beryl's repo**](https://github.com/SebasCapo/RemoteKeycard).<br>
Make sure to switch to the [**Beryl's version**](https://github.com/SebasCapo/RemoteKeycard) of this plugin when game version 11.0.0 is out.
I actually don't know if this version will work with future game/EXILED updates.<br>
Archived since 12/07/21.
---
# RemoteKeycard
**Let's open doors without a card in the hand!**

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
