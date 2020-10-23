using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Religion
{
    class CompRelic : ThingComp
    {
        public CompProperties_CompRelic Props
        {
            get
            {
                return (CompProperties_CompRelic)props;
            }
        }

        public TraitDef Religion
        {
            get
            {
                return Props.religionTrait;
            }
        }
    }
}
