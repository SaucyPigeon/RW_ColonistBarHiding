using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Verse;
using RimWorld;

namespace ColonistBarHiding.Patches.ColonistBarDrawLocsFinder
{
	using ColonistBarDrawLocsFinder = RimWorld.ColonistBarDrawLocsFinder;
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonstBarDrawLocsFinder.CalculateGroupsCount(). This patch
	/// takes hidden entries into account.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("CalculateGroupsCount")]
	internal class ColonistBarDrawLocsFinder_CalculateGroupsCount
	{
		[HarmonyPostfix]
		private static void Postfix()
		{
			Log.Message("CalculateGroupsCount postfix");
		}

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
	}
}
