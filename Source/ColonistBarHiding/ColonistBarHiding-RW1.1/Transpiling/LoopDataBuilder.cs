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
	public class LoopDataBuilder
	{
		private Predicate<CodeInstruction> start;
		private Predicate<CodeInstruction> end;
		private IEnumerable<CodeInstruction> replacement;

		private void Validate()
		{
			if (start == null)
				throw new ArgumentNullException(nameof(start));
			if (end == null)
				throw new ArgumentNullException(nameof(end));
			if (replacement == null)
				throw new ArgumentNullException(nameof(replacement));
		}

		public LoopDataBuilder Start(Predicate<CodeInstruction> value)
		{
			start = value;
			return this;
		}

		public LoopDataBuilder End(Predicate<CodeInstruction> value)
		{
			end = value;
			return this;
		}

		public LoopDataBuilder Replacement(IEnumerable<CodeInstruction> value)
		{
			replacement = value;
			return this;
		}

		public LoopData Build()
		{
			this.Validate();
			return new LoopData(start, end, replacement);
		}

		public LoopDataBuilder()
		{

		}
	}
}
