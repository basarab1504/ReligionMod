using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace Religion
{
    class PlaceWorker_AltarPlace : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            Room room = loc.GetRoom(map, RegionType.Set_Passable);
            if(room != null)
            if (room.Role != RoomRoleDefOf.None)
            {
                List<Thing> andAdjacentThings = room.ContainedAndAdjacentThings;
                    for (int index = 0; index < andAdjacentThings.Count; ++index)
                    {
                        if (andAdjacentThings[index].def == checkingDef || checkingDef.blueprintDef == andAdjacentThings[index].def)
                        {
                            return "OneAltarInRoom".Translate();
                        }
                    }                 
                    return true;
            }
            return "IndoorOnly".Translate();
        }
    }
}
