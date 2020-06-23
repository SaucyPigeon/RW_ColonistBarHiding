using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBar.MapColonistsOrCorpsesInScreenRect(), which
	/// returns colonists or corpses in the given rect. This patch accounts for
	/// hidden pawns.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch("MapColonistsOrCorpsesInScreenRect")]
	internal class ColonistBar_MapColonistsOrCorpsesInScreenRect
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
