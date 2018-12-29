using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace Religion
{
    class JobDriver_HoldLecture : JobDriver
    {
        protected Building_Lectern lectern
        {
            get
            {
                return (Building_Lectern)base.job.GetTarget(TargetIndex.A).Thing;
            }
        }

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
            this.FailOnDestroyedOrNull(TargetIndex.A);

            Toil goToAltar = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return goToAltar;

            Toil waitingTime = new Toil();
            waitingTime.defaultCompleteMode = ToilCompleteMode.Delay;
            waitingTime.defaultDuration = 740;
            waitingTime.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            waitingTime.initAction = delegate
            {
                report = "Waiting for congregation".Translate();
            };
            yield return waitingTime;

            Toil preachingTime = new Toil();
            preachingTime.initAction = delegate
            {
                report = "Read a prayer".Translate();
                //MoteMaker.MakeInteractionBubble(this.pawn, null, ThingDefOf.Mote_Speech, lectern.def.j);
            };
            preachingTime.defaultCompleteMode = ToilCompleteMode.Delay;
            preachingTime.defaultDuration = 1000;
            preachingTime.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            preachingTime.tickAction = delegate
            {
                Pawn actor = this.pawn;
                actor.skills.Learn(SkillDefOf.Social, 0.25f);
            };
            yield return preachingTime;
            this.AddFinishAction(() =>
            {
                ReligionUtility.HeldWorshipThought(pawn);
            });
            yield break;
        }
    }
}
