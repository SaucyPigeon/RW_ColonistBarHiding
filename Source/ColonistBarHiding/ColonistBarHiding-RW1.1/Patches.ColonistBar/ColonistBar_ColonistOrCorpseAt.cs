using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBar.ColonistOrCorpseAt, which returns the colonist
	/// or corpse at the given position. This patch accounts for hidden
	/// entries.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch("ColonistOrCorpseAt")]
	class ColonistBar_ColonistOrCorpseAt
	{
		/*
		Replace Visible to ShouldBeVisible
		*/
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var visibleGetter = AccessTools.PropertyGetter(typeof(ColonistBar), "Visible");
			var shouldBeVisible = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.ShouldBeVisible), new[] { typeof(ColonistBar) });

			return instructions.MethodReplacer(from: visibleGetter, to: shouldBeVisible);
		}
	}
}
