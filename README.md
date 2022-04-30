# ConfigEgocentrism
 
Add configuration options for the item Egocentrism. Default config is vanilla behavior. Run the game with the mod installed at least once to generate the config file.

# Configuration

## Feature Toggle
|Config|Type|Default|Notes|
|------|----|-------|-----|
|`Projectiles Enabled`|true/false|true|Enables the generation of projectiles.|
|`Transform Enabled`|true/false|true|Enables the transformation of other items into itself.|

## Projectiles Interval
Sets the interval at which projectiles are generated. <br>**Formula**: `Base / (Stack * StackMult)^StackExponent`<br>**Warning**: If the divisor part of the formula computes to `0.0` or a negative number, it gets ignored and the formula becomes `Base / 1.0`.

|Config|Type|Default|Notes|
|------|----|-------|-----|
|`Interval Base`|number|3.0|Value is in seconds.|
|`Interval Stack Multiplier`|number|1.0|Only accepts positive values or 0.0. Default value used if input is negative.|
|`Interval Stack Exponent`|number|1.0|Only accepts positive values or 0.0. Default value used if input is negative.|

## Projectiles Damage
Sets the damage multiplier of each generated projectile. <br>**Formula**: `Base + (Stack * StackMult)^StackExponent`

|Config|Type|Default|
|------|----|-------|
|`Damage Base`|number|3.6|
|`Damage Stack Multiplier`|number|0.0|
|`Damage Stack Exponent`|number|1.0|

## Projectiles Max Amount
Sets the max amount of projectiles surrounding the player. <br>**Formula**: `Base + (Stack * StackMult)^StackExponent`

|Config|Type|Default|Notes|
|------|----|-------|-----|
|`Max Amount Base`|number|2.0||
|`Max Amount Stack Multiplier`|number|1.0||
|`Max Amount Stack Exponent`|number|1.0||
|`Max Amount Rounding Mode`|string|AlwaysDown|Sets the rounding mode when calculating projectiles max amount.<br>Valid Values: `AlwaysDown`, `AlwaysUp`, `Closest`|

## Projectiles Range
Sets the targeting range of projectiles. <br>**Formula**: `Base + (Stack * StackMult)^StackExponent`

|Config|Type|Default|
|------|----|-------|
|`Range Base`|number|15.0|
|`Range Stack Mutliplier`|number|0.0|
|`Range Stack Exponent`|number|1.0|

## Transform Interval
Sets the interval between each item transform. <br>**Formula**: `Base / (Stack * StackMult)^StackExponent`<br>**Warning**: If the divisor part of the formula computes to `0.0` or a negative number, it gets ignored and the formula becomes `Base / 1.0`.

|Config|Type|Default|Notes|
|------|----|-------|-----|
|`Interval Base`|number|60.0|Value is in seconds.|
|`Interval Stack Multiplier`|number|0.0|Only accepts positive values or 0.0. Default value used if input is negative.|
|`Interval Stack Exponent`|number|0.0|Only accepts positive values or 0.0. Default value used if input is negative.|

## Transform Item Amount
Sets the number of items transformed at each iteration. <br>**Formula**: `Base + (Stack * StackMult)^StackExponent`

|Config|Type|Default|Notes|
|------|----|-------|-----|
|`Transform Amount Base`|number|1.0||
|`Transform Amount Stack Multiplier`|number|0.0||
|`Transform Amount Stack Exponent`|number|1.0||
|`Transform Amount Rounding Mode`|string|AlwaysDown|Sets the rounding mode when calculating item transform amount.<br>Valid Values: `AlwaysDown`, `AlwaysUp`, `Closest`|

## Transform Item Filter
Filters Item Tiers to NOT transform. Avoid removing `untiered` from that list to not lose character passives, artifacts and other hidden stuff.

|Config|Type|Default|Notes|
|------|----|-------|-----|
|`Filter`|string|untiered|Format : `tier1,tier2,tier3`<br>Valid Tiers: untiered, white, green, red, blue, yellow, voidwhite, voidgreen, voidred, voidyellow|

## zMiscellaneous
Internal settings to ensure updating version works properly. Do not edit manually.

|Config|Type|Default|Notes|
|------|----|-------|-----|
|`Plugin Version`|string|0.0.0|Last Plugin Version loaded. Used for cleaning pre-rework config entries.|

# Changelog 

- 1.2.0
    - Complete rework to allow very fine tuning of Egocentrism.
    - Wipe of previous versions config. Was necessary to make the config file actually usable.

- 1.1.0
	- Add Configurable item filter to Transform mechanism.
	- Add Configurable range to projectiles.
	
- 1.0.0 
	- Initial Release.