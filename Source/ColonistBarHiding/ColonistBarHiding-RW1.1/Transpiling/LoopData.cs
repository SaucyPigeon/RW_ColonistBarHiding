using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace ColonistBarHiding.Transpiling
{
	public class LoopData
	{
		public Predicate<CodeInstruction> Start { get; }
		public Predicate<CodeInstruction> End { get; }
		public IEnumerable<CodeInstruction> Replacement { get; }

		public LoopData(Predicate<CodeInstruction> start, Predicate<CodeInstruction> end, IEnumerable<CodeInstruction> replacement)
		{
			Start = start ?? throw new ArgumentNullException(nameof(start));
			End = end ?? throw new ArgumentNullException(nameof(end));
			Replacement = replacement ?? throw new ArgumentNullException(nameof(replacement));
		}
	}
}
