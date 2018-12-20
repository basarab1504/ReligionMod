using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace Religion
{
    class JobDriver_AttendLecture : JobDriver
    {
        Pawn Preacher
        {
            get
            {
                foreach (Pawn pawn in this.pawn.Map.mapPawns.FreeColonistsSpawned) //Ужасно
                {
                    if (pawn.CurJob.def == ReligionDefOf.HoldLecture)
                        return pawn;
                }
                return null;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, this.job.def.joyMaxParticipants, 0, (ReservationLayerDef)null, true);
        }

        //public override bool TryMakePreToilReservations(bool errorOnFailed)
        //{
        //    Pawn pawn = this.pawn;
        //    LocalTargetInfo target = this.job.targetA;
        //    Job job = this.job;
        //    int num = this.job.def.joyMaxParticipants;
        //    int num2 = 0;
        //    if (!pawn.Reserve(target, job, num, num2, null, errorOnFailed))
        //    {
        //        return false;
        //    }
        //    pawn = this.pawn;
        //    target = this.job.targetB;
        //    job = this.job;
        //    if (!pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
        //    {
        //        return false;
        //    }
        //    if (base.TargetC.HasThing)
        //    {
        //        if (base.TargetC.Thing is Building_Bed)
        //        {
        //            pawn = this.pawn;
        //            LocalTargetInfo targetC = this.job.targetC;
        //            job = this.job;
        //            num2 = ((Building_Bed)base.TargetC.Thing).SleepingSlotsCount;
        //            num = 0;
        //            if (!pawn.Reserve(targetC, job, num2, num, null, errorOnFailed))
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            pawn = this.pawn;
        //            LocalTargetInfo targetC = this.job.targetC;
        //            job = this.job;
        //            if (!pawn.Reserve(targetC, job, 1, -1, null, errorOnFailed))
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        //public override bool CanBeginNowWhileLyingDown()
        //{
        //    return base.TargetC.HasThing && base.TargetC.Thing is Building_Bed && JobInBedUtility.InBedOrRestSpotNow(this.pawn, base.TargetC);
        //}

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.EndOnDespawnedOrNull(TargetIndex.A, JobCondition.Incompletable);
            bool hasBed = base.TargetC.HasThing && base.TargetC.Thing is Building_Bed;
            Toil watch;
            //watch.handlingFacing = true;
            if (hasBed)
            {
                this.KeepLyingDown(TargetIndex.C);
                yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.C, TargetIndex.None);
                yield return Toils_Bed.GotoBed(TargetIndex.C);
                watch = Toils_LayDown.LayDown(TargetIndex.C, true, false, true, true);
                watch.AddFailCondition(() => !watch.actor.Awake());
            }
            else
            {
                yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
                watch = new Toil();
                watch.defaultCompleteMode = ToilCompleteMode.Delay;
                watch.defaultDuration = 600;
            }
            watch.AddPreTickAction(delegate
            {
                this.WatchTickAction();
            });
            //watch.AddFinishAction(delegate
            //{
            //    JoyUtility.TryGainRecRoomThought(this.pawn);
            //});
            yield return watch;
            yield return Toils_Jump.JumpIf(watch, () => Preacher.CurJob.def == ReligionDefOf.HoldLecture);
            pawn.ClearAllReservations();
        }

        protected virtual void WatchTickAction()
        {
            this.pawn.rotationTracker.FaceCell(base.TargetA.Cell);
            this.pawn.GainComfortFromCellIfPossible();
            if (Preacher.CurJob.def != ReligionDefOf.HoldLecture)
            {
                this.ReadyForNextToil();
            }
        }

        public override object[] TaleParameters()
        {
            return new object[]
            {
                this.pawn,
                base.TargetA.Thing.def
            };
        }
    }
}
