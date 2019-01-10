using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
namespace Religion
{
    class CompProperties_CompReligionBook : CompProperties
    {
        public TraitDef religionTrait;

    public CompProperties_CompReligionBook()
    {
        this.compClass = typeof(CompReligionBook);
    }
}
}
