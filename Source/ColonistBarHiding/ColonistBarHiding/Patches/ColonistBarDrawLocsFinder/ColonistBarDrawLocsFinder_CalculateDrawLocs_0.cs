using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Harmony;
using UnityEngine;
using Verse;
using System.Reflection;

namespace ColonistBarHiding.Patches.ColonistBarDrawLocsFinder
{
	using ColonistBarDrawLocsFinder = RimWorld.ColonistBarDrawLocsFinder;

	/// <summary>
	/// Patch for ColonistBarDrawLocsFinder.CalculateDrawLocs(), which gets the
	/// drawing locations for colonist entries on the colonist bar. This patch
	/// makes colonist marked as hidden to not be shown on the colonist bar.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("CalculateDrawLocs")]
	[HarmonyPatch(new Type[] { typeof(List<Vector2>), typeof(float), typeof(bool), typeof(int) })]
	internal class ColonistBarDrawLocsFinder_CalculateDrawLocs_0
	{
		[HarmonyPrefix]
		private static bool Prefix(
			List<Vector2> outDrawLocs, float scale, bool onlyOneRow, int maxPerGlobalRow,
			ref List<int> ___horizontalSlotsPerGroup, ref List<int> ___entriesInGroup)
		{
			ColonistBarDrawLocsUtility.GetDrawLocs(scale, onlyOneRow, maxPerGlobalRow, ___entriesInGroup, ___horizontalSlotsPerGroup, outDrawLocs);
			return false;
		}
	}
}
