using System.Collections.Generic;
using Verse;
using RimWorld;

namespace Religion
{
    class PlaceWorker_NextToAltar : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            bool altarIn = false;
            Room room = loc.GetRoom(map, RegionType.Set_Passable);
            if (room != null)
            {
                List<Thing> andAdjacentThings = room.ContainedAndAdjacentThings;
                for (int index = 0; index < andAdjacentThings.Count; ++index)
                {
                    if (andAdjacentThings[index].def == checkingDef || checkingDef.blueprintDef == andAdjacentThings[index].def)
                        return "OneLecternInRoom".Translate();

                    if (andAdjacentThings[index] is Building_Altar)
                    {
                        altarIn = true;                       
                    }                    
                }
                if(altarIn)
                    return true;
            }
            return "MustPlaceNextToBuildedAltar".Translate();
        }
    }
}
