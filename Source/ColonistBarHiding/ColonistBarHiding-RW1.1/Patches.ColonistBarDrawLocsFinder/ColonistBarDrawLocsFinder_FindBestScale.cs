using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;
using System.Reflection.Emit;

namespace ColonistBarHiding.Patches.ColonistBarDrawLocsFinder
{
	using ColonistBarDrawLocsFinder = RimWorld.ColonistBarDrawLocsFinder;
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBarDrawLocsFinder.FindBestScale(), which finds the
	/// best scale for the colonist bar depending on the amount of colonists.
	/// This patch accounts for hidden colonists.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("FindBestScale")]
	internal class ColonistBarDrawLocsFinder_FindBestScale
	{
		/*
		Replace Entries with GetVisibleEntries
		When accessing horizontalSlotsPerGroup, change index by GetGroupRelativeToVisible
		*/
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var entriesGetter = AccessTools.PropertyGetter(typeof(ColonistBar), nameof(ColonistBar.Entries));
			var visibleEntries = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetVisibleEntries), new[] { typeof(ColonistBar) });

			var horizontalSlotsPerGroupField = AccessTools.Field(typeof(ColonistBarDrawLocsFinder), "horizontalSlotsPerGroup");
			var entryGroupField = AccessTools.Field(typeof(ColonistBar.Entry), nameof(ColonistBar.Entry.group));

			var getGroupRelativeToVisible = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetGroupRelativeToVisible));

			bool horizontalSlotsPerGroup = false;

			foreach (var instruction in instructions)
			{
				if (instruction.Calls(entriesGetter))
				{
					instruction.operand = visibleEntries;
				}
				else if (instruction.LoadsField(horizontalSlotsPerGroupField))
				{
					horizontalSlotsPerGroup = true;
				}
				yield return instruction;
				if (horizontalSlotsPerGroup && instruction.LoadsField(entryGroupField))
				{
					yield return new CodeInstruction(OpCodes.Call, getGroupRelativeToVisible);
				}
			}
		}
	}
}
