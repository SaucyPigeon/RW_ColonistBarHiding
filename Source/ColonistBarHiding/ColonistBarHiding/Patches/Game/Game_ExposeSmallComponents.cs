using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Harmony;

namespace ColonistBarHiding.Patches.Game
{
	using Game = Verse.Game;

	/// <summary>
	/// Patch for Game.ExposeSmallComponents(), which deals with saving and
	/// loading of game-specific data. This patch adds support for tracking
	/// which colonists are hidden between saves.
	/// </summary>
	[HarmonyPatch(typeof(Game))]
	[HarmonyPatch("ExposeSmallComponents")]
	internal class Game_ExposeSmallComponents
	{
		[HarmonyPrefix]
		private static void Prefix()
		{
			const bool saveDestroyedThings = false;
			Scribe_Deep.Look<HiddenPawnTracker>(ref ColonistBarUtility.HiddenPawnTracker, saveDestroyedThings, "hiddenPawnTracker", new object[0]);
		}
	}
}
