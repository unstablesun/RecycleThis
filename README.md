# RecycleThis

This prototype runs and was tested last with Unity 2019.1.10f1. 2018 should also work.

Load the scene in Assets/Scenes/MatchProto.unity
Run the Scene and you should see matches being made until it stops, then you can drag a gem to make a match. (no drag anim though)

Main code is in Assets/Scripts/MatchGame

The Main loop is in GridLogic.cs this is a switch statement. ToDo refactor to class state machine

HexManager.cs is a partial class responsible for all scans of the grid, matches, fall, fill, etc...

HexObject.cs is the unit cell for the grid, it's doesn't move, it just handles what gem is attached to itself

GemManager.cs is the module that handles gems that appear in the level.

GemObject.cs this is the piece that appears on the grid

Cell.cs is not used.
