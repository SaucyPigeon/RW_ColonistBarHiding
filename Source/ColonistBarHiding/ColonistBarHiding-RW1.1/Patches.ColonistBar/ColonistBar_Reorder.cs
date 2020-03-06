using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection.Emit;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;

	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch(nameof(ColonistBar.Reorder))]
	public static class ColonistBar_Reorder
	{
		/*
		Replace cachedEntries with GetVisibleEntries
		*/
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var cachedEntriesField = AccessTools.Field(typeof(ColonistBar), "cachedEntries");
			var getVisibleEntries = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetVisibleEntries), new[] { typeof(ColonistBar) });

			foreach (var instruction in instructions)
			{
				if (instruction.LoadsField(cachedEntriesField))
				{
					instruction.opcode = OpCodes.Call;
					instruction.operand = getVisibleEntries;
				}
				yield return instruction;
			}
		}
	}
}
