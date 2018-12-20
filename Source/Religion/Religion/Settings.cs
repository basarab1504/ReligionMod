using UnityEngine;
using Verse;

namespace Religion
{
    class Settings : ModSettings
    {
        //int lectureDuration;
        //int pawnLectureWaiting;
        //int lectureTime;

        //public Settings()
        //{
        //    this.lectureDuration = 600;
        //    this.pawnLectureWaiting = 750;
        //    this.lectureTime = 6;
        //}

        //public void DoWindowContents(Rect canvas)
        //{
        //    Listing_Standard listingStandard = new Listing_Standard();
        //    listingStandard.Begin(canvas);
        //    if (listingStandard.ButtonText("Default".Translate(), (string)null))
        //    {
        //        this.lectureDuration = 600;
        //        this.pawnLectureWaiting = 750;
        //        this.lectureTime = 6;
        //    }
        //    listingStandard.Label(MizuStrings.OptionGrowthRateFactorInNotWatering.Translate() + " (" + 0.0f.ToString("F2") + " - " + 1f.ToString("F2") + ")", -1f, (string)null);
        //    listingStandard.TextFieldNumeric<float>(ref this.fertilityFactorInNotWatering, ref this.fertilityFactorInNotWateringBuffer, 0.0f, 1f);
        //    listingStandard.Label(MizuStrings.OptionGrowthRateFactorInWatering.Translate() + " (" + 0.1f.ToString("F2") + " - " + 5f.ToString("F2") + ")", -1f, (string)null);
        //    listingStandard.TextFieldNumeric<float>(ref this.fertilityFactorInWatering, ref this.fertilityFactorInWateringBuffer, 0.1f, 5f);
        //    listingStandard.End();
        //}

        //public override void ExposeData()
        //{
        //    base.ExposeData();
        //    Scribe_Values.Look<int>(ref this.lectureDuration, "fertilityFactorInNotWatering", 600, false);
        //    Scribe_Values.Look<int>(ref this.pawnLectureWaiting, "fertilityFactorInWatering", 750, false);
        //    Scribe_Values.Look<int>(ref this.lectureTime, "fertilityFactorInWatering", 6, false);
        //}
    }
}
