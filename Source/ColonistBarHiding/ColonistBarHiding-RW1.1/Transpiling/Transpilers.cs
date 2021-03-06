﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace ColonistBarHiding.Transpiling
{
	public static class Transpilers
	{
		public static IEnumerable<CodeInstruction> MethodReplacer(this IEnumerable<CodeInstruction> instructions, MethodBase from, MethodBase to, int count)
		{
			if (from == null)
				throw new ArgumentNullException(nameof(from));
			if (to == null)
				throw new ArgumentNullException(nameof(to));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));

			foreach (var instruction in instructions)
			{
				var method = instruction.operand as MethodBase;
				if (count > 0 && method == from)
				{
					instruction.opcode = to.IsConstructor ? OpCodes.Newobj : OpCodes.Call;
					instruction.operand = to;
					count--;
				}
				yield return instruction;
			}
		}

		public static IEnumerable<CodeInstruction> FieldReplacer(this IEnumerable<CodeInstruction> instructions, FieldInfo from, MethodBase to)
		{
			if (from == null)
				throw new ArgumentNullException(nameof(from));
			if (to == null)
				throw new ArgumentNullException(nameof(to));
			
			foreach (var instruction in instructions)
			{
				if (instruction.LoadsField(from))
				{
					instruction.opcode = to.IsConstructor ? OpCodes.Newobj : OpCodes.Call;
					instruction.operand = to;
				}
				yield return instruction;
			}
		}

		/// <summary>
		/// Add a call to a method after a target field when another field has already been loaded.
		/// </summary>
		/// <param name="instructions">The code instructions to transpile.</param>
		/// <param name="before">The field that must be loaded before adding the method.</param>
		/// <param name="target">The field after which to add the method.</param>
		/// <param name="method">The method to add.</param>
		/// <returns>The transpiled code instructions.</returns>
		public static IEnumerable<CodeInstruction> MethodAdder(this IEnumerable<CodeInstruction> instructions, FieldInfo before, FieldInfo target, MethodBase method)
		{
			if (before == null)
				throw new ArgumentNullException(nameof(before));
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			bool beforeFieldLoaded = false;

			foreach (var instruction in instructions)
			{
				if (instruction.LoadsField(before))
				{
					beforeFieldLoaded = true;
				}
				yield return instruction;
				if (beforeFieldLoaded && instruction.LoadsField(target))
				{
					var opcode = method.IsConstructor ? OpCodes.Newobj : OpCodes.Call;
					yield return new CodeInstruction(opcode, operand: method);
					beforeFieldLoaded = false;
				}
			}
		}

		public static IEnumerable<CodeInstruction> MethodAdder(this IEnumerable<CodeInstruction> instructions, FieldInfo before, int? arg_n, MethodBase method)
		{
			if (before == null)
				throw new ArgumentNullException(nameof(before));
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			bool beforeFieldLoaded = false;

			foreach (var instruction in instructions)
			{
				if (instruction.LoadsField(before))
				{
					beforeFieldLoaded = true;
				}
				yield return instruction;
				if (beforeFieldLoaded && instruction.IsLdarg(arg_n))
				{
					var opcode = method.IsConstructor ? OpCodes.Newobj : OpCodes.Call;
					yield return new CodeInstruction(opcode, operand: method);
					beforeFieldLoaded = false;
				}
			}
		}

		public static IEnumerable<CodeInstruction> MethodAdder(this IEnumerable<CodeInstruction> instructions, MethodBase before, MethodBase value, int count)
		{
			if (before == null)
				throw new ArgumentNullException(nameof(before));
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));

			foreach (var instruction in instructions)
			{
				yield return instruction;

				var method = instruction.operand as MethodBase;

				if (count > 0 && method == before)
				{
					var opcode = value.IsConstructor ? OpCodes.Newobj : OpCodes.Call;
					yield return new CodeInstruction(opcode, operand: value);
					count--;
				}
			}
		}

		private static IEnumerable<CodeInstruction> LoopReplacer(this IEnumerable<CodeInstruction> instructions, Predicate<CodeInstruction> start, Predicate<CodeInstruction> end, IEnumerable<CodeInstruction> replacement, int count)
		{
			if (start == null)
				throw new ArgumentNullException(nameof(start));
			if (end == null)
				throw new ArgumentNullException(nameof(end));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));

			bool inLoop = false;
			int counter = 0;

			foreach (var instruction in instructions)
			{
				if (!inLoop && counter < count && start.Invoke(instruction))
				{
					inLoop = true;
				}

				if (inLoop)
				{
					if (end.Invoke(instruction))
					{
						foreach (var r in replacement)
						{
							yield return r;
						}
						inLoop = false;
						counter++;
					}
					continue;
				}

				yield return instruction;
			}
		}

		public static IEnumerable<CodeInstruction> LoopReplacer(this IEnumerable<CodeInstruction> instructions, LoopData loopData)
		{
			if (loopData == null)
				throw new ArgumentNullException(nameof(loopData));

			return LoopReplacer(instructions, loopData.Start, loopData.End, loopData.Replacement, count: 1);
		}
	}
}
