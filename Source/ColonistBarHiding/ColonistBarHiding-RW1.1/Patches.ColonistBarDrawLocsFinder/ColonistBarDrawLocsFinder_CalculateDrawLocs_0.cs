using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using HarmonyLib;
using UnityEngine;
using Verse;
using System.Reflection;
using System.Reflection.Emit;
using ColonistBarHiding.Transpiling;

namespace ColonistBarHiding.Patches.ColonistBarDrawLocsFinder
{
	using ColonistBarDrawLocsFinder = RimWorld.ColonistBarDrawLocsFinder;
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBarDrawLocsFinder.CalculateDrawLocs(), which gets the
	/// drawing locations for colonist entries on the colonist bar. This patch
	/// makes colonist marked as hidden to not be shown on the colonist bar.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("CalculateDrawLocs")]
	[HarmonyPatch(new Type[] { typeof(List<Vector2>), typeof(float), typeof(bool), typeof(int) })]
	internal class ColonistBarDrawLocsFinder_CalculateDrawLocs_0
	{
		[HarmonyPrefix]
		private static void Prefix()
		{
			Log.Message("CalculateDrawLocs PREFIX", true);
			Log.Message($"Entries count: {Find.ColonistBar.Entries.Count}", true);
			Log.Message($"Visible count: {Find.ColonistBar.Entries.GetVisibleEntriesFrom().Count}", true);

			var field = AccessTools.Field(typeof(ColonistBar), "cachedDrawLocs");
			var fieldValue = field.GetValue(Find.ColonistBar);
			var cachedDrawLocs = (List<Vector2>)fieldValue;

			Log.Message($"Cached locs count: {cachedDrawLocs.Count}", true);
		}

		[HarmonyPostfix]
		private static void Postfix()
		{
			Log.Message("CalculateDrawLocs.Postfix", true);
		}

		/*
		Replace first loop with call to FlattenHorizontalSlots(entriesInGroup, horizontalSlotsPerGroup)
		Replace first Entries.Count with GetVisibleEntriesFrom(Entries).Count
		Add condition for second loop: !ColonistBarUtility.IsHidden(entries[i])
		*/
		// Need to change this entirely
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
		{

			var entriesGetter = AccessTools.PropertyGetter(typeof(ColonistBar), nameof(ColonistBar.Entries));
			var getVisibleEntriesFrom = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetVisibleEntriesFrom), new[] { typeof(List<ColonistBar.Entry>) });

			instructions = instructions.MethodAdder(before: entriesGetter, value: getVisibleEntriesFrom, count: 1);

			var flattenHorizontalSlots = AccessTools.Method(typeof(ColonistBarDrawLocsUtility), nameof(ColonistBarDrawLocsUtility.FlattenHorizontalSlots));
			var entriesInGroupField = AccessTools.Field(typeof(ColonistBarDrawLocsFinder), "entriesInGroup");
			var horizontalSlotsPerGroupField = AccessTools.Field(typeof(ColonistBarDrawLocsFinder), "horizontalSlotsPerGroup");

			var loopData = new LoopDataBuilder()
				.Start((CodeInstruction x) => { return x.LoadsConstant(0); })
				.End((CodeInstruction x) => { return x.opcode == OpCodes.Blt_S; })
				.Replacement(new[] {
					// Load entriesInGroup
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldfld, entriesInGroupField),
					// Load horizontalSlotsPerGroup
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldfld, horizontalSlotsPerGroupField),
					// Call FlattenHorizontalSlots(entriesInGroup, horizontalSlotsPerGroup)
					new CodeInstruction(OpCodes.Call, flattenHorizontalSlots)
				}).Build();


			instructions = instructions.LoopReplacer(loopData);



			// Second-to-last ldloc.s 9
			CodeInstruction loopStart = instructions
				.Where(x => x.opcode == OpCodes.Ldloc_S && ((LocalBuilder)x.operand).LocalIndex == 9)
				.Reverse()
				.Skip(1)
				.First();

			bool alreadyAdded = false;
			Label jumpToEnd = ilGenerator.DefineLabel();

			var get_Item = AccessTools.Method(typeof(List<ColonistBar.Entry>), "get_Item", new[] { typeof(int) });
			var isHidden = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.IsHidden), new[] { typeof(ColonistBar.Entry) });

			foreach (var instruction in instructions)
			{
				if (!alreadyAdded)
				{
					if (instruction.opcode == OpCodes.Ldloc_S)
					{
						alreadyAdded = true;

						
						//if (ColonistBarUtility.IsHidden(entries[j])
						//{
						//	continue;
						//}
						
						// Make this first instruction the new label destination
						yield return new CodeInstruction(OpCodes.Ldloc_S, 4) { labels = instruction.labels.ListFullCopy() };
						instruction.labels.Clear();
						yield return new CodeInstruction(OpCodes.Ldloc_S, 9);
						yield return new CodeInstruction(OpCodes.Callvirt, get_Item);
						yield return new CodeInstruction(OpCodes.Call, isHidden);
						yield return new CodeInstruction(OpCodes.Brtrue_S, operand: jumpToEnd);
					}
				}

				if (instruction == loopStart)
				{
					instruction.labels.Add(jumpToEnd);
				}

				yield return instruction;
			}
		}
	}
}
