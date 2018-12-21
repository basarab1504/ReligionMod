using System.Collections.Generic;
using Verse;
using RimWorld;

namespace Religion
{
    class PlaceWorker_NextToAltar : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            for (int index1 = 0; index1 < 8; ++index1)
            {  
                IntVec3 c = loc + GenAdj.AdjacentCellsAround[index1];
                if (c.InBounds(map))
                {
                    List<Thing> thingList = c.GetThingList(map);
                    for (int index2 = 0; index2 < thingList.Count; ++index2)
                    {
                        ThingDef thingDef = GenConstruct.BuiltDefOf(thingList[index2].def) as ThingDef;
                        if (thingDef != null && thingDef.building != null && thingDef.building.wantsHopperAdjacent)
                            return (AcceptanceReport)true;
                    }
                }
            }
            return (AcceptanceReport)"MustPlaceNextToAltar".Translate();
        }
    }
}
