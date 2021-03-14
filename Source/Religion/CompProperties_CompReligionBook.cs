using RimWorld;
using Verse;

namespace Religion
{
    internal class CompProperties_CompReligionBook : CompProperties
    {
        public TraitDef religionTrait;

        public CompProperties_CompReligionBook()
        {
            compClass = typeof(CompReligionBook);
        }
    }
}