using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace Religion
{
    class InteractionWorker_ReligionTalks : InteractionWorker
    {
        private const float BaseChance = 0.02f;
        private const float SpouseRelationChanceFactor = 0.4f;

        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            if (recipient.story.traits.HasTrait(ReligionDefOf.Atheist))
                return 0.0f;
            if (!initiator.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
                return 0.0f;
            if (recipient.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
                return 0.0f;
            if (recipient.relations.OpinionOf(initiator) > 5)
                return BaseChance;
            return 0.0f;
        }

        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef)
        {
            Trait t = initiator.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait);
            if(t != null)
            {
                if (Rand.Value < Mathf.InverseLerp(0f, 100f, recipient.relations.OpinionOf(initiator)))
                {
                    recipient.story.traits.GainTrait(t);
                    letterText = recipient.ToString() + " in faith".Translate();
                    letterLabel = "Is now religious".Translate();
                    letterDef = LetterDefOf.PositiveEvent;
                }
            }
            base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef);
        }
    }
}
