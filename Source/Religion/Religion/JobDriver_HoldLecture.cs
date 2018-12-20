using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace Religion
{
    class JobDriver_HoldLecture : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, this.job.def.joyMaxParticipants, 0, (ReservationLayerDef)null, true);
        }

        private string report = "";
        public override string GetReport()
        {
            if (report != "")
            {
                return base.ReportStringProcessed(report);
            }
            return base.GetReport();
        }

        //public override bool TryMakePreToilReservations(bool errorOnFailed)
        //{
        //    return true;
        //}

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            //this.FailOnDestroyedOrNull(TargetIndex.A);

            Toil goToAltar = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return goToAltar;

            Toil waitingTime = new Toil();
            waitingTime.defaultCompleteMode = ToilCompleteMode.Delay;
            waitingTime.defaultDuration = 740;
            waitingTime.initAction = delegate
            {
                report = "Waiting for congregation".Translate();
            };
            yield return waitingTime;

            Toil preachingTime = new Toil();
            preachingTime.initAction = delegate
            {
                report = "Read a prayer".Translate();
            };
            preachingTime.defaultCompleteMode = ToilCompleteMode.Delay;
            preachingTime.defaultDuration = 600;
            preachingTime.tickAction = delegate
            {
                Pawn actor = this.pawn;
                actor.skills.Learn(SkillDefOf.Social, 0.25f);
            };
            yield return preachingTime;

            //this.AddFinishAction(() =>
            //{
            //    //When the ritual is finished -- then let's give the thoughts
            //    if (Altar.currentWorshipState == Building_SacrificialAltar.WorshipState.finishing ||
            //        Altar.currentWorshipState == Building_SacrificialAltar.WorshipState.finished)
            //    {
            //        CultUtility.AttendWorshipTickCheckEnd(PreacherPawn, this.pawn);
            //        Cthulhu.Utility.DebugReport("Called end tick check");
            //    }
            //    pawn.ClearAllReservations();
            //});
            yield break;
        }
    }
}
