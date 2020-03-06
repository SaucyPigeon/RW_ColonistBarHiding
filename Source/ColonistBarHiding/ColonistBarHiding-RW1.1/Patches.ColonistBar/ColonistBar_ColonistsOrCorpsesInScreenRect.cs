using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBar.ColonistsOrCorpsesInScreenRect(), which
	/// returns colonists or corpses in the given rect. This patch accounts for
	/// hidden pawns.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch("ColonistsOrCorpsesInScreenRect")]
	internal class ColonistBar_ColonistsOrCorpsesInScreenRect
	{
		/*
		Replace Entries with GetVisibleEntries
		*/
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var entriesGetter = AccessTools.PropertyGetter(typeof(ColonistBar), nameof(ColonistBar.Entries));
			var getVisibleEntries = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetVisibleEntries), new[] { typeof(ColonistBar) });

			return Transpilers.MethodReplacer(instructions, entriesGetter, getVisibleEntries);
		}
	}
}
