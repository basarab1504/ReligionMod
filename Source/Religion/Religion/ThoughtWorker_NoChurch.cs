﻿using System;
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
            IEnumerable<Building> altars = p.Map.listerBuildings.AllBuildingsColonistOfDef(ReligionDefOf.Altar);
            if (!altars.Any())
                return (ThoughtState)true;
            foreach (Building_Altar a in altars)
                if (a.religion.Count != 0 && a.religion[0] != def.requiredTraits[0])
                    return (ThoughtState)true;
            return ThoughtState.Inactive;
        }
    }
}
