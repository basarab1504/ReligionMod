using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Religion
{
    internal class IncidentWorker_Providence : IncidentWorker
    {
        private readonly Trait Religion = new(DefDatabase<TraitDef>.AllDefsListForReading
            .FindAll(x => x is TraitDef_ReligionTrait).RandomElement());

        private IEnumerable<Pawn> PotentialVictimCandidates(IIncidentTarget target)
        {
            if (target is Map map)
            {
                return map.mapPawns.FreeColonistsAndPrisoners.Where(x =>
                {
                    if (x.ParentHolder is Building_CryptosleepCasket)
                    {
                        return false;
                    }

                    if (!x.IsFreeColonist)
                    {
                        return x.IsPrisonerOfColony;
                    }

                    if (x.story.traits.allTraits.Any(a =>
                        a.def is TraitDef_ReligionTrait || a.def is TraitDef_NonReligion))
                    {
                        return false;
                    }

                    if (!ReligionUtility.CanBeReligious(x, Religion))
                    {
                        return false;
                    }

                    return x.RaceProps.IsFlesh;
                });
            }

            return ((Caravan) target).PawnsListForReading.Where(x =>
            {
                if (x.ParentHolder is Building_CryptosleepCasket)
                {
                    return false;
                }

                if (!x.IsFreeColonist)
                {
                    return x.IsPrisonerOfColony;
                }

                if (x.story.traits.allTraits.Any(a => a.def is TraitDef_ReligionTrait || a.def is TraitDef_NonReligion))
                {
                    return false;
                }

                if (!ReligionUtility.CanBeReligious(x, Religion))
                {
                    return false;
                }

                return x.RaceProps.IsFlesh;
            });
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var pawn = PotentialVictimCandidates(parms.target).RandomElement();
            if (pawn == null)
            {
                return false;
            }

            pawn.story.traits.GainTrait(Religion);
            Find.LetterStack.ReceiveLetter(def.letterLabel,
                pawn.LabelCap + " " + def.letterText + " " + Religion.LabelCap, def.letterDef, pawn);
            return true;
        }
    }
}