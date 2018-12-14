using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;

namespace Religion
{
    class RoomRoleWorker_Church : RoomRoleWorker
    {
        public override float GetScore(Room room)
        {
            int num = 0;
            List<Thing> andAdjacentThings = room.ContainedAndAdjacentThings;
            for (int index = 0; index < andAdjacentThings.Count; ++index)
            {
                Thing thing = andAdjacentThings[index];
                if (thing.def.category == ThingCategory.Building && thing.def.defName == "Altar")
                    ++num;
            }
            return (float)num * 7.6f;
        }
    }
}