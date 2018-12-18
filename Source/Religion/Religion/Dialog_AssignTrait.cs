using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;

namespace Religion
{
    class Dialog_AssignTrait : Window
    {
        private IAssignableTrait assignable;
        private Vector2 scrollPosition;
        private const float EntryHeight = 35f;

        public Dialog_AssignTrait(IAssignableTrait assignable)
        {
            this.assignable = assignable;
            this.doCloseButton = true;
            this.doCloseX = true;
            this.closeOnClickedOutside = true;
            this.absorbInputAroundWindow = true;
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
            Rect viewRect = new Rect(0.0f, 0.0f, outRect.width - 16f, (float)((double)this.assignable.AssigningTrait.Count<TraitDef>() * 35.0 + 100.0));
            Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect, true);
            try
            {
                float y = 0.0f;
                bool flag = false;
                foreach (TraitDef assignedTraitDef in this.assignable.AssignedTraits)
                {
                    flag = true;
                    Rect rect = new Rect(0.0f, y, viewRect.width * 0.6f, 32f);
                    Widgets.Label(rect, assignedTraitDef.defName);
                    rect.x = rect.xMax;
                    rect.width = viewRect.width * 0.4f;
                    if (Widgets.ButtonText(rect, "BuildingUnassign".Translate(), true, false, true))
                    {
                        this.assignable.TryUnassignTrait(assignedTraitDef);
                        SoundDefOf.Click.PlayOneShotOnCamera((Map)null);
                        return;
                    }
                    y += 35f;
                }
                if (flag)
                    y += 15f;
                foreach (TraitDef assigningCandidate in this.assignable.AssigningTrait)
                {
                    if (!this.assignable.AssignedTraits.Contains<TraitDef>(assigningCandidate))
                    {
                        Rect rect = new Rect(0.0f, y, viewRect.width * 0.6f, 32f);
                        Widgets.Label(rect, assigningCandidate.defName);
                        rect.x = rect.xMax;
                        rect.width = viewRect.width * 0.4f;
                        string label = !this.assignable.AssignedAnything(assigningCandidate) ? "BuildingAssign".Translate() : "BuildingReassign".Translate();
                        if (Widgets.ButtonText(rect, label, true, false, true))
                        {
                            this.assignable.TryAssignTrait(assigningCandidate);
                            if (this.assignable.MaxAssignedTraitsCount == 1)
                            {
                                this.Close(true);
                                break;
                            }
                            SoundDefOf.Click.PlayOneShotOnCamera((Map)null);
                            break;
                        }
                        y += 35f;
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
