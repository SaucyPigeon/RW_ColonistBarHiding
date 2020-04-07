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
			Log.Message("CalculateDrawLocs", true);
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
			throw new Exception("POSTFIX");
		}

		private class LoopReplacer
		{
			public bool InLoop = false;
			public bool Finished = false;
			public int BranchCounter = 0;

			public Predicate<CodeInstruction> StartCondition;
			public Predicate<CodeInstruction> EndCondition;

			public IEnumerable<CodeInstruction> Replacement;

			public void Start()
			{
				InLoop = true;
			}

			public IEnumerable<CodeInstruction> End()
			{
				InLoop = false;
				Finished = true;
				foreach (var replacementInstruction in Replacement)
				{
					yield return replacementInstruction;
				}
			}

			public bool CanStart(CodeInstruction x)
			{
				return !Finished && StartCondition.Invoke(x);
			}

			public bool CanEnd(CodeInstruction x)
			{
				return InLoop && EndCondition.Invoke(x);
			}
		}


		/*
		Replace first loop with call to FlattenHorizontalSlots(entriesInGroup, horizontalSlotsPerGroup)
		Replace first Entries.Count with GetVisibleEntriesCountFrom(Entries)
		Add condition for second loop: !ColonistBarUtility.IsHidden(entries[i])
		*/
		// Need to change this entirely
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
		{
			var flattenHorizontalSlots = AccessTools.Method(typeof(ColonistBarDrawLocsUtility), nameof(ColonistBarDrawLocsUtility.FlattenHorizontalSlots));
			var entriesInGroupField = AccessTools.Field(typeof(ColonistBarDrawLocsFinder), "entriesInGroup");
			var horizontalSlotsPerGroupField = AccessTools.Field(typeof(ColonistBarDrawLocsFinder), "horizontalSlotsPerGroup");
			var entriesGetter = AccessTools.PropertyGetter(typeof(ColonistBar), nameof(ColonistBar.Entries));
			var getVisibleEntriesFrom = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.GetVisibleEntriesFrom), new[] { typeof(List<ColonistBar.Entry>) });


			var loopReplacer0 = new LoopReplacer()
			{
				InLoop = false,
				Finished = false,
				BranchCounter = 0,
				StartCondition = (CodeInstruction x) => { return x.LoadsConstant(0); },
				EndCondition = (CodeInstruction x) => { return x.Branches(out var _); },
				Replacement = new[]
				{
					// Load entriesInGroup
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldfld, entriesInGroupField),
					// Load horizontalSlotsPerGroup
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldfld, horizontalSlotsPerGroupField),
					// Call FlattenHorizontalSlots(entriesInGroup, horizontalSlotsPerGroup)
					new CodeInstruction(OpCodes.Call, flattenHorizontalSlots)
				}
			};

			bool gotEntriesAlready = false;
			bool addedLoopCondition = false;

			Label loopHead = ilGenerator.DefineLabel();

			foreach (var instruction in instructions)
			{
				if (loopReplacer0.CanStart(instruction))
				{
					loopReplacer0.Start();
				}
				else if (loopReplacer0.CanEnd(instruction))
				{
					loopReplacer0.BranchCounter++;
					// Loop end
					if (loopReplacer0.BranchCounter >= 2)
					{
						foreach (var replacement in loopReplacer0.End())
						{
							yield return replacement;
						}
						continue;
					}
				}

				// Ignore when in first loop
				if (loopReplacer0.InLoop)
				{
					//yield return new CodeInstruction(OpCodes.Nop);
					continue;
				}

				// Only after first loop finished
				if (!addedLoopCondition && loopReplacer0.Finished)
				{
					if (instruction.opcode == OpCodes.Ldloc_S)
					{
						addedLoopCondition = true;
						var labels = instruction.labels;
						instruction.labels = new List<Label>();

						// Load entries[i]
						yield return new CodeInstruction(OpCodes.Ldloc_S, 4)
						{
							labels = labels
						};

						yield return new CodeInstruction(OpCodes.Ldloc_S, 9);
						var get_Item = AccessTools.Method(typeof(List<ColonistBar.Entry>), "get_Item", new[] { typeof(int) });
						yield return new CodeInstruction(OpCodes.Callvirt, get_Item);

						// Call IsHidden(entry)
						var isHidden = AccessTools.Method(typeof(ColonistBarUtility), nameof(ColonistBarUtility.IsHidden), new[] { typeof(ColonistBar.Entry) });
						yield return new CodeInstruction(OpCodes.Call, isHidden);

						// Branch to beginning of loop if true
						// We can use the saved label because it branches to the beginning
						yield return new CodeInstruction(OpCodes.Brtrue, loopHead);
					}
				}

				if (loopReplacer0.Finished && instruction.opcode == OpCodes.Br_S)
				{
					ilGenerator.MarkLabel(loopHead);
					instruction.labels.Add(loopHead);
				}
				yield return instruction;

				if (!gotEntriesAlready && instruction.Calls(entriesGetter))
				{
					gotEntriesAlready = true;
					yield return new CodeInstruction(OpCodes.Call, getVisibleEntriesFrom);
				}
			}
		}
	}
}
