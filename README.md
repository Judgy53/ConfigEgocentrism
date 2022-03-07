# ConfigEgocentrism
 
Add configuration options for the item Egocentrism. Run the game with the mod installed at least once to generate the config file.

# Configuration

- `ProjectilesEnabled` : Enables the generation of projectiles. (Default: `true`)
- `ProjectilesInterval` : Sets the interval between each generation of projectiles (in seconds). (Default: `3.0`)
- `ProjectilesDamage` : Sets the damage multiplier of projectiles. (Default: `3.6`)
- `ProjectilesMaxAmountBase` : Sets the base max amount of projectiles. (Default: `2`)
- `ProjectilesMaxAmountStack` : Sets the max amount of projectiles per item in the stack. (Default: `1`)
- `ProjectilesRangeBase` : Sets the base targeting range of projectiles. (Default: `15.0`)
- `ProjectilesRangeStack` : Sets the additional targetting range per item in the stack. (Default: `0.0`)
- `TransformEnabled` : Enables the transformation of other items. (Default: `true`)
- `TransformInterval` : Sets the interval between each item transform (in seconds). (Default: `60.0`)
- `TransformCount` : Sets the max number of items transformed at each iteration. (Default: `1`)
- `TransformFilter` : 
	- Filters Item Tiers to NOT transform. Avoid removing `untiered` from that list to not lose character passives, artifacts and other hidden stuff.
	- Valid Tiers: `untiered,white,green,red,blue,yellow,voidwhite,voidgreen,voidred,voidyellow`
	- Format : `tier1,tier2,tier3`
	- Default: `untiered`

# Changelog 

- 1.1.0
	- Add Configurable item filter to Transform mechanism.
	- Add Configurable range to projectiles.
	
- 1.0.0 
	- Initial Release.