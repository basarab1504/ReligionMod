using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Religion
{
    // Token: 0x02000D69 RID: 3433
    public class CompAssignableToPawn_Lectern : CompAssignableToPawn
    {
        // Token: 0x17000EA7 RID: 3751
        // (get) Token: 0x060053FD RID: 21501 RVA: 0x001C5C63 File Offset: 0x001C3E63
        public override IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                if (!parent.Spawned || ((Building_Lectern) parent).religion.NullOrEmpty())
                {
                    return Enumerable.Empty<Pawn>();
                }

                return parent.Map.mapPawns.FreeColonists.Where
                (x => x.story.traits.HasTrait(((Building_Lectern) parent).religion[0])
                      && !x.skills.GetSkill(SkillDefOf.Social).TotallyDisabled
                      && !parent.Map.listerBuildings.allBuildingsColonist.Any(u =>
                          u is Building_Lectern lectern &&
                          lectern.CompAssignableToPawn.AssignedPawnsForReading.Contains(x)));
            }
        }

        // Token: 0x060053FC RID: 21500 RVA: 0x001C5C52 File Offset: 0x001C3E52
        protected override string GetAssignmentGizmoDesc()
        {
            return "WorshipDesc".Translate();
        }

        // Token: 0x060053FE RID: 21502 RVA: 0x001C5CA0 File Offset: 0x001C3EA0
        public override string CompInspectStringExtra()
        {
            if (AssignedPawnsForReading.Count == 0)
            {
                return "Owner".Translate() + ": " + "Nobody".Translate();
            }

            if (AssignedPawnsForReading.Count == 1)
            {
                return "Owner".Translate() + ": " + AssignedPawnsForReading[0].Label;
            }

            return "";
        }

        // Token: 0x060053FF RID: 21503 RVA: 0x001C5D26 File Offset: 0x001C3F26
        public override bool AssignedAnything(Pawn pawn)
        {
            return false;
        }

        // Token: 0x06005400 RID: 21504 RVA: 0x001C5D36 File Offset: 0x001C3F36
        public override void TryAssignPawn(Pawn pawn)
        {
            assignedPawns.Clear();
            if (assignedPawns.Contains(pawn))
            {
                return;
            }

            if (pawn.skills.GetSkill(SkillDefOf.Social).levelInt < 8)
            {
                Messages.Message("LowSkillPreacher".Translate() + " " + "MinSkillPreacher".Translate(8.ToString()),
                    MessageTypeDefOf.NegativeEvent);
            }

            assignedPawns.Add(pawn);
        }


        // Token: 0x06005401 RID: 21505 RVA: 0x001C5D4F File Offset: 0x001C3F4F
        public override void TryUnassignPawn(Pawn pawn, bool sort = true)
        {
            if (assignedPawns.Contains(pawn))
            {
                assignedPawns.Remove(pawn);
            }
        }

        public void UnassignAllPawns()
        {
            assignedPawns.Clear();
        }
    }
}