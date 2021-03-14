using Verse;

namespace Religion
{
    internal class PlaceWorker_NextToAltar : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
            Thing thingToIgnore = null, Thing thing = null)
        {
            var altarIn = false;
            var room = loc.GetRoom(map);
            if (room == null)
            {
                return "MustPlaceNextToBuildedAltar".Translate();
            }

            var andAdjacentThings = room.ContainedAndAdjacentThings;
            foreach (var adjacentThing in andAdjacentThings)
            {
                if (adjacentThing.def == checkingDef || checkingDef.blueprintDef == adjacentThing.def)
                {
                    return "OneLecternInRoom".Translate();
                }

                if (adjacentThing is Building_Altar)
                {
                    altarIn = true;
                }
            }

            if (altarIn)
            {
                return true;
            }

            return "MustPlaceNextToBuildedAltar".Translate();
        }
    }
}