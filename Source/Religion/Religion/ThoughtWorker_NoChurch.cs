using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;

namespace Religion
{
    public class ThoughtWorker_NoChurch : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.Map.listerBuildings.ColonistsHaveBuilding(ReligionDefOf.Altar))
                return (ThoughtState)true;
            IEnumerable<Building> altars = p.Map.listerBuildings.AllBuildingsColonistOfDef(ReligionDefOf.Altar);
            foreach (Building_Altar a in altars)
                    if (a.religion.Contains(def.requiredTraits[0]))
                        return (ThoughtState)false;
            return (ThoughtState)true;
        }
    }
}
