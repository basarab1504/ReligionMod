using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Religion
{
    class CompProperties_RelProp : CompProperties
    {
        public float rangeRadius = 5.9f;
        public SoundDef hitSound = SoundDefOf.TinyBell;

        public CompProperties_RelProp()
        {
            this.compClass = typeof(ThingComp_ReligionComp);
        }
    }
}
