using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace Religion
{
    class JobDriver_AttendWorship : JobDriver
    {
        private readonly string report = "";
        public override string GetReport()
        {
            if (report != "")
            {
                return base.ReportStringProcessed(report);
            }
            return base.GetReport();
        }

        protected Building_Lectern Lectern => (Building_Lectern)job.GetTarget(TargetIndex.A).Thing;
        Pawn Preacher
        {
            get
            {
                return Lectern.CompAssignableToPawn.AssignedPawnsForReading[0];
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = this.job.targetA;
            Job job = this.job;
            int num = this.job.def.joyMaxParticipants;
            int num2 = 0;
            if (!pawn.Reserve(target, job, num, num2, null, errorOnFailed))
            {
                return false;
            }
            pawn = this.pawn;
            target = this.job.targetB;
            job = this.job;
            if (!pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
            {
                return false;
            }
            if (TargetC.HasThing)
            {
                if (TargetC.Thing is Building_Bed bed)
                {
                    pawn = this.pawn;
                    LocalTargetInfo targetC = this.job.targetC;
                    job = this.job;
                    num2 = bed.SleepingSlotsCount;
                    num = 0;
                    if (!pawn.Reserve(targetC, job, num2, num, null, errorOnFailed))
                    {
                        return false;
                    }
                }
                else
                {
                    pawn = this.pawn;
                    LocalTargetInfo targetC = this.job.targetC;
                    job = this.job;
                    if (!pawn.Reserve(targetC, job, 1, -1, null, errorOnFailed))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool CanBeginNowWhileLyingDown()
        {
            return TargetC.HasThing && TargetC.Thing is Building_Bed && JobInBedUtility.InBedOrRestSpotNow(pawn, TargetC);
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.EndOnDespawnedOrNull(TargetIndex.A, JobCondition.Incompletable);
            bool hasBed = TargetC.HasThing && TargetC.Thing is Building_Bed;

            Toil goToChurch = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
            yield return goToChurch;

            Toil watch = new Toil();
            watch.AddPreTickAction(() =>
            {
                pawn.rotationTracker.FaceCell(TargetA.Thing.OccupiedRect().ClosestCellTo(pawn.Position));
                pawn.GainComfortFromCellIfPossible();
                pawn.rotationTracker.FaceCell(TargetB.Cell);
                if (Preacher.CurJob.def != ReligionDefOf.HoldWorship)
                {
                    ReadyForNextToil();
                }
            });
            watch.defaultCompleteMode = ToilCompleteMode.Delay;
            watch.defaultDuration = job.def.joyDuration;
            watch.handlingFacing = true;
            yield return watch;

            yield return Toils_Jump.JumpIf(watch, () => Preacher.CurJob.def == ReligionDefOf.HoldWorship);

            AddFinishAction(() =>
            {
            if (pawn.Position == TargetC.Cell)
                {
                    ReligionUtility.TryGainTempleRoomThought(pawn);
                    ReligionUtility.AttendedWorshipThought(pawn, Preacher);
                    ReligionUtility.TryAddAddiction(pawn, Preacher);
                }
            });
            yield break;
        }
    }
}
