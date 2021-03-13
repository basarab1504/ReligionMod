using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Religion
{
    internal class InteractionWorker_ReligionTalks : InteractionWorker
    {
        private const float BaseChance = 0.10f;

        private const float SpouseRelationChanceFactor = 0.4f;

        //    private static readonly SimpleCurve CompatibilityFactorCurve = new SimpleCurve()
        //{
        //  {
        //    new CurvePoint(2.5f, 4f),
        //    true
        //  },
        //  {
        //    new CurvePoint(1.5f, 3f),
        //    true
        //  },
        //  {
        //    new CurvePoint(0.5f, 2f),
        //    true
        //  },
        //  {
        //    new CurvePoint(-0.5f, 1f),
        //    true
        //  },
        //  {
        //    new CurvePoint(-1f, 0.75f),
        //    true
        //  },
        //  {
        //    new CurvePoint(-2f, 0.5f),
        //    true
        //  },
        //  {
        //    new CurvePoint(-3f, 0.4f),
        //    true
        //  }
        //};
        private static readonly SimpleCurve OpinionFactorCurve = new()
        {
            new CurvePoint(100f, 6f),
            new CurvePoint(50f, 4f),
            new CurvePoint(25f, 2f),
            new CurvePoint(0.0f, 1f),
            new CurvePoint(-50f, 0.1f),
            new CurvePoint(-100f, 0.0f)
        };

        private static readonly SimpleCurve MoodFactorCurve = new()
        {
            new CurvePoint(0.0f, 2f),
            new CurvePoint(0.5f, 0.2f),
            new CurvePoint(1f, 0.1f)
        };

        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            return ChanceToConvert(initiator, recipient);
        }

        private float ChanceToConvert(Pawn initiator, Pawn recipient)
        {
            if (recipient.story.traits.allTraits.Any(x => x.def is TraitDef_NonReligion))
            {
                return 0.0f;
            }

            if (!initiator.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
            {
                return 0.0f;
            }

            if (recipient.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
            {
                return 0.0f;
            }

            var t = initiator.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait);
            if (!ReligionUtility.CanBeReligious(recipient, t))
            {
                return 0.0f;
            }

            var num = 1f * OpinionFactorCurve.Evaluate(initiator.relations.OpinionOf(recipient));
            var curLevel = recipient.needs.mood.CurLevel;
            num *= MoodFactorCurve.Evaluate(curLevel);
            return 0.003f * num;
        }

        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks,
            out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
        {
            var t = initiator.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait);
            base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef,
                out lookTargets);
            if (t == null)
            {
                return;
            }

            //if (Rand.Value < Mathf.InverseLerp(0f, 100f, recipient.relations.OpinionOf(initiator)))
            //{
            recipient.story.traits.GainTrait(t);
            letterText = recipient + " " + "NowBelieveIn".Translate() + " " + t.LabelCap;
            letterLabel = "IsNowReligious".Translate();
            letterDef = LetterDefOf.PositiveEvent;
            //}
        }
    }
}