using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;
using System.Reflection.Emit;
using ColonistBarHiding.Transpiling;

namespace ColonistBarHiding.Patches.ColonistBarDrawLocsFinder
{
	using ColonistBarDrawLocsFinder = RimWorld.ColonistBarDrawLocsFinder;
	using ColonistBar = RimWorld.ColonistBar;

	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("GetDrawLoc")]
	public static class ColonistBarDrawLocsFinder_GetDrawLoc
	{
#if DEBUG
		[HarmonyPrefix]
		private static void Prefix()
		{
		}

		[HarmonyPostfix]
		private static void Postfix()
		{

		}
#endif

		/*
		Replace horizontalSlotsPerGroup[group] with horizontalSlotsPerGroup[GetGroupRelativeToVisible(group)]
		*/
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var horizontalSlotsPerGroupField = AccessTools.Field(typeof(ColonistBarDrawLocsFinder), "horizontalSlotsPerGroup");
			var getGroupRelativeToVisible = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetGroupRelativeToVisible));

			int arg_n = 3;

			return instructions.MethodAdder(before: horizontalSlotsPerGroupField, arg_n, method: getGroupRelativeToVisible);
		}
	}
}
