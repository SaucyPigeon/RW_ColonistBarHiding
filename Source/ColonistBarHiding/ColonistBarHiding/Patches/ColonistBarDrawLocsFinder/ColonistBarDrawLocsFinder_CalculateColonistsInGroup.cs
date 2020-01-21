using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld;

namespace ColonistBarHiding.Patches.ColonistBarDrawLocsFinder
{
	using ColonistBarDrawLocsFinder = RimWorld.ColonistBarDrawLocsFinder;

	/// <summary>
	/// Patch for ColonistBarDrawLocsFinder.CalculateColonistsInGroup(), which
	/// gets the amount of entries for each group. This patch accounts for
	/// hidden entries.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("CalculateColonistsInGroup")]
	internal class ColonistBarDrawLocsFinder_CalculateColonistsInGroup
	{
		[HarmonyPrefix]
		private static bool Prefix(ref List<int> ___entriesInGroup)
		{
			___entriesInGroup = ColonistBarDrawLocsUtility.GetGroupEntryCounts();
			return false;
		}
	}
}
