using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;
using System.Reflection.Emit;
using ColonistBarHiding.Transpiling;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;
	
	/// <summary>
	/// Harmony patch for <see cref="ColonistBar.ColonistBarOnGUI"/> which
	/// handles the colonist bar's display on a GUI call. This patch accounts
	/// for hidden colonists. 
	/// </summary>
	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch("ColonistBarOnGUI")]
	internal class ColonistBar_ColonistBarOnGUI
	{
#if DEBUG
		[HarmonyPrefix]
		private static void Prefix()
		{
			var field = AccessTools.Field(typeof(ColonistBar), "cachedDrawLocs");
			var fieldValue = field.GetValue(Find.ColonistBar);
			var cachedDrawLocs = (List<Vector2>)fieldValue;

			Log.Message($"CachedDrawLocs.count: {cachedDrawLocs.Count}", true);
			Log.Message($"Visible entries count: {Find.ColonistBar.Entries.GetVisibleEntriesFrom().Count}", true);

			Log.Message("Iterating through cachedDrawLocs...", true);

			for (int i = 0; i < cachedDrawLocs.Count; i++)
			{
				Log.Message($"Entry for cachedDrawLocs i={i}: {Find.ColonistBar.Entries.GetVisibleEntriesFrom()[i].pawn.Name}", true);
			}
			Log.Message(" ", true);
		}
#endif

		/*
		Replace Visible to ShouldBeVisible
		Replace first instance of Entries to GetVisibleEntries
		
		Bug: have two maps, one empty of pawns, the other full of pawns. When
		the map with pawns has all colonists hidden, the colonist bar only shows
		a rect for the map without pawns. Expected result: both maps are shown.
		*/
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{			
			var visibleGetter = AccessTools.PropertyGetter(typeof(ColonistBar), "Visible");
			var shouldBeVisible = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.ShouldBeVisible), new[] { typeof(ColonistBar) });
			
			var entriesGetter = AccessTools.PropertyGetter(typeof(ColonistBar), nameof(ColonistBar.Entries));
			var GetVisibleEntries = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetVisibleEntries), new[] { typeof(ColonistBar)});

			return instructions
				.MethodReplacer(from: visibleGetter, to: shouldBeVisible)
				.MethodReplacer(from: entriesGetter, to: GetVisibleEntries, count: 1);
		}
	}
}
