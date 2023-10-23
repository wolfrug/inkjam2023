EXTERNAL EXT_AddToList(x,y)
EXTERNAL EXT_RemoveFromList(x,y)
EXTERNAL EXT_AddToDictionary(x,y,z)
EXTERNAL EXT_RemoveFromDictionary(x,y)
EXTERNAL EXT_HasValue(x,y)
EXTERNAL EXT_GetValue(x,y)

LIST ingredients = Flour, Roadkill, PigsBlood, Medicine
VAR notesTaken = 0

===function EXT_AddToList(x,y)
[Added {x} to the list {y}]
~return y
===function EXT_RemoveFromList(x,y)
[Removed {x} from the list {y}]
~return y
===function EXT_AddToDictionary(x,y,z)
[Adds the Key Value Pair ({x},{y}) to {z}]
~return z

===function EXT_RemoveFromDictionary(x,y)
[Removes the key and value of key {x} from {y}]
~return y
===function EXT_HasValue(x,y)
[Checks if {x} exists in list {y}]
~return true
===function EXT_GetValue(x,y)
[Gets either the index {x} or the key {x} from list {y}]
~return x

===function AddToList(key, ref list)
~list = EXT_AddToList(key, list)
===function RemoveFromList(key, ref list)
~list = EXT_RemoveFromList(key, list)
===function AddToDictionary(key, value, ref list)
~list = EXT_AddToDictionary(key, value, list)
===function RemoveFromDictionary(key, ref list)
~list = EXT_RemoveFromDictionary(key, list)
===function HasValue(key, ref list)
~return EXT_HasValue(key, list)
===function GetValue(key, ref list)
~return EXT_GetValue(key, list)

===function IsInteractable(b)
{b:
INTERACTABLE(true)
-else:
INTERACTABLE(false)
}
