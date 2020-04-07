using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;


	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch(nameof(ColonistBar.Entries), MethodType.Getter)]
	static class DEBUG_ColonistBar_get_Entries
	{
		[HarmonyPostfix]
		private static void Postfix()
		{
			Log.Warning("get_Entries postfix");
		}
	}
}
