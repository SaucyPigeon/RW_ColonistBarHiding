using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Harmony;
using UnityEngine;
using Verse;
using System.Reflection;

namespace ColonistBarHiding.Patches
{
	/// <summary>
	/// Patch for ColonistBarDrawLocsFinder.CalculateDrawLocs(), which gets the
	/// drawing locations for colonist entries on the colonist bar. This patch
	/// makes colonist marked as hidden to not be shown on the colonist bar.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("CalculateDrawLocs")]
	[HarmonyPatch(new Type[] { typeof(List<Vector2>), typeof(float)}, new[] { ArgumentType.Normal, ArgumentType.Out})]
	internal class ColonistBarDrawLocsFinder_CalculateDrawLocs_1
	{
		private static void CalculateDrawLocs(List<Vector2> outDrawLocs, List<int> entriesInGroup, out float scale, ref List<int> horizontalSlotsPerGroup)
		{
			if (ColonistBarUtility.GetVisibleEntriesCount() == 0)
			{
				outDrawLocs.Clear();
				scale = 1f;
			}
			else
			{
				entriesInGroup = ColonistBarDrawLocsUtility.GetGroupEntryCounts();
				scale = ColonistBarDrawLocsUtility.GetBestScale(
					entriesInGroup,
					out bool onlyOneRow, out int maxPerGlobalRow, out horizontalSlotsPerGroup);
				 ColonistBarDrawLocsUtility.GetDrawLocs(scale, onlyOneRow, maxPerGlobalRow, entriesInGroup, horizontalSlotsPerGroup, outDrawLocs);
			}
		}

		[HarmonyPrefix]
		private static bool Prefix(
			List<Vector2> outDrawLocs, out float scale, ref List<int> ___entriesInGroup, ref List<int> ___horizontalSlotsPerGroup)
		{
			CalculateDrawLocs(outDrawLocs, ___entriesInGroup, out scale, ref ___horizontalSlotsPerGroup);
			return false;
		}

		//[HarmonyPostfix]
		//private static void Postfix(ref float scale)
		//{
		//}
	}
}
