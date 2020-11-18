using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;

namespace Religion
{
    class Dialog_AssignTrait : Window
    {
        private readonly IAssignableTrait assignable;
        private Vector2 scrollPosition;
        private const float EntryHeight = 35f;

        public Dialog_AssignTrait(IAssignableTrait assignable)
        {
            this.assignable = assignable;
            doCloseButton = true;
            doCloseX = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
        }

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(620f, 500f);
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Rect outRect = new Rect(inRect);
            outRect.yMin += 20f;
            outRect.yMax -= 40f;
            outRect.width -= 16f;
            Rect viewRect = new Rect(0.0f, 0.0f, outRect.width - 16f, (float)(assignable.AssigningTrait.Count<TraitDef>() * 35.0 + 100.0));
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect, true);
            try
            {
                float y = 0.0f;
                bool flag = false;
                foreach (TraitDef assignedTraitDef in assignable.AssignedTraits)
                {
                    flag = true;
                    Rect rect = new Rect(0.0f, y, viewRect.width * 0.6f, 32f);
                    Widgets.Label(rect, assignedTraitDef.degreeDatas[0].label);
                    rect.x = rect.xMax;
                    rect.width = viewRect.width * 0.4f;
                    if (Widgets.ButtonText(rect, "BuildingUnassign".Translate(), true, false, true))
                    {
                        assignable.TryUnassignTrait(assignedTraitDef);
                        SoundDefOf.Click.PlayOneShotOnCamera(null);
                        return;
                    }
                    y += EntryHeight;
                }
                if (flag)
                    y += 15f;
                foreach (TraitDef assigningCandidate in assignable.AssigningTrait)
                {
                    if (!assignable.AssignedTraits.Contains<TraitDef>(assigningCandidate))
                    {
                        Rect rect = new Rect(0.0f, y, viewRect.width * 0.6f, 32f);
                        Widgets.Label(rect, assigningCandidate.degreeDatas[0].label);
                        rect.x = rect.xMax;
                        rect.width = viewRect.width * 0.4f;
                        string label = !assignable.AssignedAnything(assigningCandidate) ? "BuildingAssign".Translate() : "BuildingReassign".Translate();
                        if (Widgets.ButtonText(rect, label, true, false, true))
                        {
                            assignable.TryAssignTrait(assigningCandidate);
                            if (assignable.MaxAssignedTraitsCount == 1)
                            {
                                Close(true);
                                break;
                            }
                            SoundDefOf.Click.PlayOneShotOnCamera(null);
                            break;
                        }
                        y += EntryHeight;
                    }
                }
            }
            finally
            {
                Widgets.EndScrollView();
            }
        }
    }
}
