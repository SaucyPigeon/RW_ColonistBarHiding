using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Harmony;

namespace ColonistBarHiding.Patches
{
	[HarmonyPatch(typeof(Game))]
	[HarmonyPatch("ExposeSmallComponents")]
	internal class Game_ExposeSmallComponents
	{
		[HarmonyPrefix]
		private static void Prefix()
		{
			Scribe_Deep.Look<HiddenPawnTracker>(ref ColonistBarUtility.HiddenPawnTracker, "hiddenPawnTracker", new object[0]);
		}
	}
}
