using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;

namespace ColonistBarHiding.Patches.ColonistBarColonistDrawer
{
	using ColonistBarColonistDrawer = RimWorld.ColonistBarColonistDrawer;
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBarColonistDrawer.GroupFrameRect() which gets the group frame for a given group. This patch accounts for hidden colonists.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarColonistDrawer))]
	[HarmonyPatch("GroupFrameRect")]
	internal class ColonistBarColonistDrawer_GroupFrameRect
	{
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
