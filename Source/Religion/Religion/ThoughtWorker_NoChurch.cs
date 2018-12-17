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
            ThoughtDef_AltarThought relDef = this.def as ThoughtDef_AltarThought;
            if (!p.Map.listerBuildings.ColonistsHaveBuilding(relDef.altar))
                return (ThoughtState)true;
            return ThoughtState.Inactive;
        }
    }
}
