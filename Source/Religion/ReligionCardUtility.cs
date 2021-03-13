using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Religion
{
    public static class ReligionCardUtility
    {
        private static readonly Vector2 WinSize = new(420f, 480f);
        private static readonly float xOffset = 20f;
        private static readonly float y = 20f;
        private static readonly float width = 140f;

        private static readonly float height = 30f;
        //[TweakValue("Interface", 0.0f, 128f)]
        //private static float PasteX = 400f;
        //[TweakValue("Interface", 0.0f, 128f)]
        //private static float PasteY = 20f;
        //[TweakValue("Interface", 0.0f, 32f)]
        //private static float PasteSizeY = 24f;
        //private static float PasteSizeX = 200f;
        //private static float SizeY = 42f;

        public static void DrawCard(Building_Lectern SelLectern)
        {
            var rel = new Rect(xOffset, y, width, height);
            var owner = new Rect(xOffset, y + height, width, height);
            //Verse.Text.Anchor = TextAnchor.MiddleLeft;
            //Widgets.Label(rect0, "Religion".Translate());
            //GenUI.ResetLabelAlign();
            //if (Widgets.ButtonText(rel, "SetReligion".Translate(), true, false, true))
            //{
            //    Find.WindowStack.Add((Window)new Dialog_AssignTrait(SelLectern));
            //}

            if (!SelLectern.religion.NullOrEmpty())
            {
                if (Prefs.DevMode)
                {
                    Widgets.CheckboxLabeled(rel, "didWorshipToday", ref SelLectern.didWorship);
                }

                if (Widgets.ButtonText(owner, "SetOwner".Translate(), true, false))
                {
                    if (SelLectern.Map.mapPawns.FreeColonists.Any
                    (x => x.story.traits.HasTrait(SelLectern.religion[0])
                          && !x.skills.GetSkill(SkillDefOf.Social).TotallyDisabled))
                    {
                        Find.WindowStack.Add(new Dialog_AssignBuildingOwner(SelLectern.CompAssignableToPawn));
                    }
                    else
                    {
                        Messages.Message("NoAvaliableCandidates".Translate(), MessageTypeDefOf.NegativeEvent);
                    }
                }

                if (!SelLectern.CompAssignableToPawn.AssignedPawns.Any())
                {
                    return;
                }

                var timeOf = new Rect(xOffset, y + (height * 2), width, height);
                string label = "TimeOfWorship".Translate();
                string dayOf = "DayOf".Translate();
                Widgets.Label(timeOf, label);

                var numer = new Rect(xOffset + width, y + (height * 2), 24f, 24f);
                Widgets.TextFieldNumeric(numer, ref SelLectern.timeOfWorship, ref SelLectern.timeOfbuffer, 1f, 23f);

                var forDaysY = y + (height * 3);
                for (var i = 0; i < 15; ++i)
                {
                    var rect4 = new Rect(xOffset, forDaysY + (i * 22), width, 20f);
                    ReligionUtility.CheckboxLabeled(rect4, SelLectern, i, dayOf + " " + (i + 1),
                        SelLectern.daysOfWorships[i]);
                }
            }
            else
            {
                Widgets.Label(new Rect(xOffset, y, width * 2, height),
                    "ReligionMustBeAssignedThroughAltar".Translate());
            }
        }
    }
}