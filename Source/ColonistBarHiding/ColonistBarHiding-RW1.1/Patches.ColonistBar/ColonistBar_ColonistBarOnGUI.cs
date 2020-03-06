using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;
using System.Reflection.Emit;

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
		/*
		Replace Visible to ShouldBeVisible
		Replace first instance of Entries to GetVisibleEntries
		*/
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var visibleGetter = AccessTools.PropertyGetter(typeof(ColonistBar), "Visible");
			var shouldBeVisible = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.ShouldBeVisible), new[] { typeof(ColonistBar) });

			var entriesGetter = AccessTools.PropertyGetter(typeof(ColonistBar), nameof(ColonistBar.Entries));
			var GetVisibleEntries = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetVisibleEntries), new[] { typeof(ColonistBar)});

			bool entriesReplaced = false;

			foreach (var instruction in instructions)
			{
				if (instruction.Calls(visibleGetter))
				{
					instruction.operand = shouldBeVisible;
				}
				else if (!entriesReplaced && instruction.Calls(entriesGetter))
				{
					instruction.operand = GetVisibleEntries;
					entriesReplaced = true;
				}
				yield return instruction;
			}
		}
	}
}
