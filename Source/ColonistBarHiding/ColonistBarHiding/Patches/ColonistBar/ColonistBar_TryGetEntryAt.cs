using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld;
using UnityEngine;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBar.TryGetEntryAt(), which tries to get a colonist
	/// bar entry at a given position. This patch accounts for hidden
	/// colonists.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch("TryGetEntryAt")]
	internal class ColonistBar_TryGetEntryAt
	{
		[HarmonyPrefix]
		private static bool Prefix(bool __result, Vector2 pos, ref ColonistBar.Entry entry)
		{
			__result = ColonistBarUtility.TryGetEntryAt(pos, out entry);
			return false;
		}
	}
}
