using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ColonistBarHiding
{
	public static class ColonistBarUtility
	{
		public static HiddenPawnTracker HiddenPawnTracker;

		public static List<Pawn> GetColonistBarPawns()
		{
			var list = new List<Pawn>();
			foreach (var entry in Find.ColonistBar.Entries)
			{
				list.Add(entry.pawn);
			}
			return list;
		}

		public static void RemoveColonist(Pawn pawn)
		{
			HiddenPawnTracker.Hide(pawn);
		}

		public static void RestoreColonist(Pawn pawn)
		{
			HiddenPawnTracker.Show(pawn);
		}

		public static bool IsHidden(Pawn pawn)
		{
			// Fix bug where HiddenPawnTracker==null if item within is detroyed between saves without mod.
			if (HiddenPawnTracker == null)
			{
				HiddenPawnTracker = new HiddenPawnTracker();
			}
			return HiddenPawnTracker.IsHidden(pawn);
		}
	}
}
