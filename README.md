# A bare bones template for Vampire Survivor
### Important scriptable objects
* `Gameplay Configuration`
  * Contains XP to level map
  * The additional delta each of the three boosts would provide
  * Entire game round duration
* `SpawnConfiguration`
  * Contains the list of spawnable creatures
  * Additionally contains the reduction factor per level for each spawn time
  * Make use of `UnitData` such as `Bat`, `Zombie` or `Skeleton` for additional meta data
* `Unit Data`
  * Contains basics such as HP, movement speed, damage per tick etc

### Summary of the project

The project has quite a lot of its parts linked using a [PubSub](https://devkit.games/) _(Also link to the Package used)_ architecture.

TL;DR:Allows for even more decouples systems allowing them to not even care/know about who listens to them regardless. GC friendly as there can be no tight coupling between the listeners as well as the senders through a single object.

Additional use of object pooling for most of the dynamically spawned elements such as the creeps as well as the projects have been done using [LeanPool](https://carloswilkes.com/Documentation/LeanPool)\
__Unfortunate caveat seems like a black box causing issues_

### General folder structure
* Art - (Art, music, media)
* System Name
  * Components (Mono classes)
  * Prefabs (Associated prefabs)
  * Classes (Non mono c# classes or struct)
  * ScriptableObjects (As per namesake)
    * _Res (The Scriptable assets created)
* Jobs (Experimental Job classes and structs)