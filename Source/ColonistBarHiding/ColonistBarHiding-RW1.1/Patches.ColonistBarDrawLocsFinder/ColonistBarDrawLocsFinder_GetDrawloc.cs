using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;
using System.Reflection.Emit;

namespace ColonistBarHiding.Patches.ColonistBarDrawLocsFinder
{
	using ColonistBarDrawLocsFinder = RimWorld.ColonistBarDrawLocsFinder;
	using ColonistBar = RimWorld.ColonistBar;

	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("GetDrawLoc")]
	public static class ColonistBarDrawLocsFinder_GetDrawLoc
	{
		/*
		Replace horizontalSlotsPerGroup[group] with horizontalSlotsPerGroup[GetGroupRelativeToVisible(group)]
		*/
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var horizontalSlotsPerGroupField = AccessTools.Field(typeof(ColonistBarDrawLocsFinder), "horizontalSlotsPerGroup");
			var getGroupRelativeToVisible = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetGroupRelativeToVisible));

			bool horizontalSlotsPerGroup = false;

			foreach (var instruction in instructions)
			{
				if (instruction.LoadsField(horizontalSlotsPerGroupField))
				{
					horizontalSlotsPerGroup = true;
				}
				yield return instruction;
				if (horizontalSlotsPerGroup && instruction.IsLdarg(3))
				{
					horizontalSlotsPerGroup = false;
					yield return new CodeInstruction(OpCodes.Call, getGroupRelativeToVisible);
				}
			}
		}
	}
}
