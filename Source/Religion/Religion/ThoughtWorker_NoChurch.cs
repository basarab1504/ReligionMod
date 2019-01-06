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
            if (p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait) != null)
            {
                TraitDef pawnReligion = p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait).def;
                IEnumerable<Building> altars = p.Map.listerBuildings.AllBuildingsColonistOfDef(ReligionDefOf.Altar);
                foreach (Building_Altar a in altars)
                    if (a.religion.Contains(pawnReligion))
                        return (ThoughtState)false;
                return (ThoughtState)true;
            }
            return (ThoughtState)false;
        }
    }
}
