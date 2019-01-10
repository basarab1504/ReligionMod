using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;


namespace Religion
{
    class CompReligionBook : ThingComp
    {
        public CompProperties_CompReligionBook Props
        {
            get
            {
                return (CompProperties_CompReligionBook)this.props;
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
