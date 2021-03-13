using RimWorld;
using Verse;

namespace Religion
{
    internal class CompProperties_CompRelic : CompProperties
    {
        public TraitDef religionTrait;

        public CompProperties_CompRelic()
        {
            compClass = typeof(CompRelic);
        }
    }
}