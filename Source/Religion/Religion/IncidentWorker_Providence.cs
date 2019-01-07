using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace Religion
{
    class IncidentWorker_Providence : IncidentWorker
    {
        public IEnumerable<Pawn> PotentialVictimCandidates(IIncidentTarget target)
        {
            Map map = target as Map;
            if (map != null)
            return map.mapPawns.FreeColonistsAndPrisoners.Where<Pawn>((Func<Pawn, bool>)(x =>
            {
                if (x.ParentHolder is Building_CryptosleepCasket)
                    return false;
                if (!x.IsFreeColonist)
                    return x.IsPrisonerOfColony;
                if (x.story.traits.allTraits.Any(a => a.def is TraitDef_ReligionTrait || a.def == ReligionDefOf.Atheist))
                    return false;
                return x.RaceProps.IsFlesh;
            }));
            return ((Caravan)target).PawnsListForReading.Where<Pawn>((Func<Pawn, bool>)(x =>
            {
                if (x.ParentHolder is Building_CryptosleepCasket)
                    return false;
                if (!x.IsFreeColonist)
                    return x.IsPrisonerOfColony;
                if (x.story.traits.allTraits.Any(a => a.def is TraitDef_ReligionTrait || a.def == ReligionDefOf.Atheist))
                    return false;
                return x.RaceProps.IsFlesh;
            }));
        }

        Trait Religion()
        {
            return new Trait(DefDatabase<TraitDef>.AllDefsListForReading.FindAll(x => x is TraitDef_ReligionTrait).RandomElement());
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Pawn pawn = PotentialVictimCandidates(parms.target).RandomElement();
            if (pawn == null)
                return false;
            pawn.story.traits.GainTrait(Religion());
            Find.LetterStack.ReceiveLetter(this.def.letterLabel, pawn.LabelCap + " " + def.letterText, this.def.letterDef, (LookTargets)pawn, (Faction)null, (string)null);
            return true;
        }
    }
}
