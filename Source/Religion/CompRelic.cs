using RimWorld;
using Verse;

namespace Religion
{
    internal class CompRelic : ThingComp
    {
        private CompProperties_CompRelic Props => (CompProperties_CompRelic) props;

        public TraitDef Religion => Props.religionTrait;
    }
}