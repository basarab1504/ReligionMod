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
                return (CompProperties_CompRelic)this.props;
            }
        }

        public TraitDef religion
        {
            get
            {
                return Props.religionTrait;
            }
        }
    }
}
