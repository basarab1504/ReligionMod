using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Religion
{
    internal class Dialog_AssignTrait : Window
    {
        private const float EntryHeight = 35f;
        private readonly IAssignableTrait assignable;
        private Vector2 scrollPosition;

        public Dialog_AssignTrait(IAssignableTrait assignable)
        {
            this.assignable = assignable;
            doCloseButton = true;
            doCloseX = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
        }

        public override Vector2 InitialSize => new(620f, 500f);

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            var outRect = new Rect(inRect);
            outRect.yMin += 20f;
            outRect.yMax -= 40f;
            outRect.width -= 16f;
            var viewRect = new Rect(0.0f, 0.0f, outRect.width - 16f,
                (float) ((assignable.AssigningTrait.Count() * 35.0) + 100.0));
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            try
            {
                var y = 0.0f;
                var flag = false;
                foreach (var assignedTraitDef in assignable.AssignedTraits)
                {
                    flag = true;
                    var rect = new Rect(0.0f, y, viewRect.width * 0.6f, 32f);
                    Widgets.Label(rect, assignedTraitDef.degreeDatas[0].label);
                    rect.x = rect.xMax;
                    rect.width = viewRect.width * 0.4f;
                    if (Widgets.ButtonText(rect, "BuildingUnassign".Translate(), true, false))
                    {
                        assignable.TryUnassignTrait(assignedTraitDef);
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        return;
                    }

                    y += EntryHeight;
                }

                if (flag)
                {
                    y += 15f;
                }

                foreach (var assigningCandidate in assignable.AssigningTrait)
                {
                    if (assignable.AssignedTraits.Contains(assigningCandidate))
                    {
                        continue;
                    }

                    var rect = new Rect(0.0f, y, viewRect.width * 0.6f, 32f);
                    Widgets.Label(rect, assigningCandidate.degreeDatas[0].label);
                    rect.x = rect.xMax;
                    rect.width = viewRect.width * 0.4f;
                    string label = !assignable.AssignedAnything(assigningCandidate)
                        ? "BuildingAssign".Translate()
                        : "BuildingReassign".Translate();
                    if (Widgets.ButtonText(rect, label, true, false))
                    {
                        assignable.TryAssignTrait(assigningCandidate);
                        if (assignable.MaxAssignedTraitsCount == 1)
                        {
                            Close();
                            break;
                        }

                        SoundDefOf.Click.PlayOneShotOnCamera();
                        break;
                    }

                    y += EntryHeight;
                }
            }
            finally
            {
                Widgets.EndScrollView();
            }
        }
    }
}