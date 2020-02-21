using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBar.ColonistOrCorpseAt, which returns the colonist
	/// or corpse at the given position. This patch accounts for hidden
	/// entries.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch("ColonistOrCorpseAt")]
	class ColonistBar_ColonistOrCorpseAt
	{
		// Original public method ColonistBar.ColonistOrCorpseAt()
		private static Thing ColonistOrCorpseAt(Vector2 pos)
		{
			if (!ColonistBarUtility.ShouldBeVisible())
			{
				return null;
			}
			ColonistBar.Entry entry;
			if (!ColonistBarUtility.TryGetEntryAt(pos, out entry))
			{
				return null;
			}
			Pawn pawn = entry.pawn;
			Thing result;
			if (pawn != null && pawn.Dead && pawn.Corpse != null && pawn.Corpse.SpawnedOrAnyParentSpawned)
			{
				result = pawn.Corpse;
			}
			else
			{
				result = pawn;
			}
			return result;
		}

		[HarmonyPrefix]
		private static bool Prefix(ref Thing __result, Vector2 pos)
		{
			__result = ColonistOrCorpseAt(pos);
			return false;
		}
	}
}
