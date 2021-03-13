using RimWorld;
using Verse;

namespace Religion
{
    internal class PlaceWorker_AltarPlace : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
            Thing thingToIgnore = null, Thing thing = null)
        {
            var room = loc.GetRoom(map);
            if (room == null)
            {
                return "IndoorOnly".Translate();
            }

            if (room.Role == RoomRoleDefOf.None)
            {
                return "IndoorOnly".Translate();
            }

            var andAdjacentThings = room.ContainedAndAdjacentThings;
            foreach (var adjacentThing in andAdjacentThings)
            {
                if (adjacentThing.def == checkingDef || checkingDef.blueprintDef == adjacentThing.def)
                {
                    return "OneAltarInRoom".Translate();
                }
            }

            return true;
        }
    }
}