using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld;
using UnityEngine;

namespace ColonistBarHiding.Patches
{
	/// <summary>
	/// Patch for ColonistBarDrawLocsFinder.FindBestScale(), which finds the
	/// best scale for the colonist bar depending on the amount of colonists.
	/// This patch accounts for hidden colonists.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("FindBestScale")]
	internal class ColonistBarDrawLocsFinder_FindBestScale
	{
		[HarmonyPrefix]
		private static bool Prefix(float __result, out bool onlyOneRow, out int maxPerGlobalRow, ref List<int> ___entriesInGroup, ref List<int> ___horizontalSlotsPerGroup)
		{
			//__result = FindBestScale(out onlyOneRow, out maxPerGlobalRow, ref ___entriesInGroup, ref ___horizontalSlotsPerGroup);
			__result = ColonistBarDrawLocsUtility.GetBestScale(___entriesInGroup, out onlyOneRow, out maxPerGlobalRow, out ___horizontalSlotsPerGroup);
			return false;
		}
	}
}
