using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;

namespace Religion
{
    class TraitDef_ReligionTrait : TraitDef
    {
        public List<TraitDef> foreignReligions = new List<TraitDef>();
        public bool isAgressive;

        //public new bool ConflictsWith(Trait other)
        //{
        //    if (other.def.conflictingTraits != null)
        //    {
        //        for (int index = 0; index < other.def.conflictingTraits.Count; ++index)
        //        {
        //            if (other.def.conflictingTraits[index] == this || other.def is TraitDef_ReligionTrait)
        //            {
        //                this.conflictingTraits.Add(other.def);
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
    }
}
