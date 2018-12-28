using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace Religion
{
    public static class ReligionCardUtility
    {
        private static readonly Vector2 WinSize = new Vector2(420f, 480f);
        private static float xOffset = 20f;
        private static float y = 20f;
        private static float width = 140f;
        private static float height = 30f;
        //[TweakValue("Interface", 0.0f, 128f)]
        //private static float PasteX = 400f;
        //[TweakValue("Interface", 0.0f, 128f)]
        //private static float PasteY = 20f;
        //[TweakValue("Interface", 0.0f, 32f)]
        //private static float PasteSizeY = 24f;
        //private static float PasteSizeX = 200f;
        //private static float SizeY = 42f;

        public static float DrawCard(Building_Lectern SelLectern)
        {

            Rect rel = new Rect(xOffset, y, width, height);
            Rect owner = new Rect(xOffset, y + height, width, height);
            //Verse.Text.Anchor = TextAnchor.MiddleLeft;
            //Widgets.Label(rect0, "Religion".Translate());
            //GenUI.ResetLabelAlign();
            //if (Widgets.ButtonText(rel, "SetReligion".Translate(), true, false, true))
            //{
            //    Find.WindowStack.Add((Window)new Dialog_AssignTrait(SelLectern));
            //}

            if(!SelLectern.religion.NullOrEmpty())
            {
                if (Widgets.ButtonText(owner, "SetOwner".Translate(), true, false, true))
                {
                    Find.WindowStack.Add((Window)new Dialog_AssignBuildingOwner(SelLectern));
                }

                if(!SelLectern.owners.NullOrEmpty())
                {
                    Rect timeOf = new Rect(xOffset, y + (height * 2), width, height);
                    string label = "TimeOfLecture".Translate();
                    Widgets.Label(timeOf, label);

                    Rect numer = new Rect(xOffset + width, y + (height * 2), 24f, 24f);
                    Widgets.TextFieldNumeric<int>(numer, ref SelLectern.timeOfLecture, ref SelLectern.timeOfbuffer, 1f, 23f);

                    float forDaysY = y + height * 3;
                    for (int i = 0; i < 15; ++i)
                    {
                        Rect rect4 = new Rect(xOffset, forDaysY + (i * 22), width, 20f);
                        ReligionUtility.CheckboxLabeled(rect4, SelLectern, i, ("Day " + (i + 1)).Translate(), SelLectern.daysOfLectures[i], false, null, null, false);
                    }
                }
            }
            else
                Widgets.Label(new Rect(xOffset, y, width*2, height), "Religion must be assigned through altar".Translate());
            return 0f;
        }
    }
}
