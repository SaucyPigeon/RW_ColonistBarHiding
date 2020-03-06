using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using HarmonyLib;
using UnityEngine;
using Verse;
using System.Reflection;

namespace ColonistBarHiding.Patches.ColonistBarDrawLocsFinder
{
	using ColonistBarDrawLocsFinder = RimWorld.ColonistBarDrawLocsFinder;
	using ColonistBar = RimWorld.ColonistBar;

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
		/*
			//if (this.ColonistBar.Entries.Count == 0)
			if (ColonistBarUtility.GetVisibleEntries(this.colonistBar).Count == 0)
			{
				outDrawLocs.Clear();
				scale = 1f;
				return;
			}
			this.CalculateColonistsInGroup(); -> patch with GetGroupEntryCounts()
			bool onlyOneRow;
			int maxPerGlobalRow;
			scale = this.FindBestScale(out onlyOneRow, out maxPerGlobalRow); -> patch
			this.CalculateDrawLocs(outDrawLocs, scale, onlyOneRow, maxPerGlobalRow); -> patch
		*/

		/*
		Replace Entries with GetVisibleEntries
		*/
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var entriesGetter = AccessTools.PropertyGetter(typeof(ColonistBar), nameof(ColonistBar.Entries));
			var visibleEntries = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetVisibleEntries), new[] { typeof(ColonistBar) });

			return instructions.MethodReplacer(from: entriesGetter, to: visibleEntries);
		}

		//private static void CalculateDrawLocs(List<Vector2> outDrawLocs, List<int> entriesInGroup, out float scale, ref List<int> horizontalSlotsPerGroup)
		//{
		//	if (!ColonistBarUtility.AnyVisibleEntries())
		//	{
		//		outDrawLocs.Clear();
		//		scale = 1f;
		//	}
		//	else
		//	{
		//		// Patched			entriesInGroup = ColonistBarDrawLocsUtility.GetGroupEntryCounts();
		//		scale = ColonistBarDrawLocsUtility.GetBestScale(
		//			entriesInGroup,
		//			out bool onlyOneRow, out int maxPerGlobalRow, out horizontalSlotsPerGroup);
		//		 ColonistBarDrawLocsUtility.GetDrawLocs(scale, onlyOneRow, maxPerGlobalRow, entriesInGroup, horizontalSlotsPerGroup, outDrawLocs);
		//	}
		//}
	}
}
