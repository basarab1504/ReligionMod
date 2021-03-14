using RimWorld;
using Verse;

namespace Religion
{
    internal class CompReligionBook : ThingComp
    {
        private CompProperties_CompReligionBook Props => (CompProperties_CompReligionBook) props;

        public TraitDef Religion => Props.religionTrait;
    }
}