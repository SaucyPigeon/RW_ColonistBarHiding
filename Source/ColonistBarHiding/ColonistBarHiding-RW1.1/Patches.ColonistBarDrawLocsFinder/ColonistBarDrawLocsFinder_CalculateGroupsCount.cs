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
	/// Patch for ColonstBarDrawLocsFinder.CalculateGroupsCount(). This patch
	/// takes hidden entries into account.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("CalculateGroupsCount")]
	internal class ColonistBarDrawLocsFinder_CalculateGroupsCount
	{
		[HarmonyPrefix]
		private static bool Prefix(int __result)
		{
			__result = ColonistBarUtility.GetVisibleGroupsCount();
			return false;
		}
	}
}
