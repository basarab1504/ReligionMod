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
        private static readonly SimpleCurve OpinionFactorCurve = new SimpleCurve()
    {
      {
        new CurvePoint(100f, 6f),
        true
      },
      {
        new CurvePoint(50f, 4f),
        true
      },
      {
        new CurvePoint(25f, 2f),
        true
      },
      {
        new CurvePoint(0.0f, 1f),
        true
      },
      {
        new CurvePoint(-50f, 0.1f),
        true
      },
      {
        new CurvePoint(-100f, 0.0f),
        true
      }
    };
        private static readonly SimpleCurve MoodFactorCurve = new SimpleCurve()
    {
      {
        new CurvePoint(0.0f, 2f),
        true
      },
      {
        new CurvePoint(0.5f, 0.2f),
        true
      },
      {
        new CurvePoint(1f, 0.1f),
        true
      }
    };

        private const float BaseChance = 0.10f;
        private const float SpouseRelationChanceFactor = 0.4f;

        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            return ChanceToConvert(initiator, recipient);
        }

        public float ChanceToConvert(Pawn initiator, Pawn recipient)
        {
            if (recipient.story.traits.allTraits.Any(x => x.def is TraitDef_NonReligion))
                return 0.0f;
            if (!initiator.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
                return 0.0f;
            if (recipient.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
                return 0.0f;
            Trait t = initiator.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait);
            if (!ReligionUtility.CanBeReligious(recipient, t))
                return 0.0f;
                float num;
                num = 1f * OpinionFactorCurve.Evaluate((float)initiator.relations.OpinionOf(recipient))/* * CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient))*/;
                float curLevel = recipient.needs.mood.CurLevel;
                num *= MoodFactorCurve.Evaluate(curLevel);
                return 0.003f * num;
        }

        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef)
        {
            Trait t = initiator.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait);
            base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef);
            if (t != null)
            {
                //if (Rand.Value < Mathf.InverseLerp(0f, 100f, recipient.relations.OpinionOf(initiator)))
                //{
                    recipient.story.traits.GainTrait(t);
                    letterText = recipient.ToString() + " " + "NowBelieveIn".Translate() + " " + t.LabelCap;
                    letterLabel = "IsNowReligious".Translate();
                    letterDef = LetterDefOf.PositiveEvent;
                //}
            }           
        }
    }
}
