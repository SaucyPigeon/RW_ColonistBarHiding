using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBar.ColonistsOrCorpsesInScreenRect(), which
	/// returns colonists or corpses in the given rect. This patch accounts for
	/// hidden pawns.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch("ColonistsOrCorpsesInScreenRect")]
	internal class ColonistBar_ColonistsOrCorpsesInScreenRect
	{
		// Modified public method ColonistBar.ColonistsOrCorpsesInScreenRect()
		private static List<Thing> ColonistsOrCorpsesInScreenRect(Rect rect,
			List<Pair<Thing,Map>> tmpColonistsWithMap, List<Thing> tmpColonists)
		{
			List<Vector2> drawLocs = Find.ColonistBar.DrawLocs;
			var entries = ColonistBarUtility.GetVisibleEntries();
			Vector2 size = Find.ColonistBar.Size;
			tmpColonistsWithMap.Clear();
			for (int i = 0; i < drawLocs.Count; i++)
			{
				if (rect.Overlaps(new Rect(drawLocs[i].x, drawLocs[i].y, size.x, size.y)))
				{
					Pawn pawn = entries[i].pawn;
					if (pawn != null)
					{
						Thing first;
						if (pawn.Dead && pawn.Corpse != null && pawn.Corpse.SpawnedOrAnyParentSpawned)
						{
							first = pawn.Corpse;
						}
						else
						{
							first = pawn;
						}
						tmpColonistsWithMap.Add(new Pair<Thing, Map>(first, entries[i].map));
					}
				}
			}
			if (WorldRendererUtility.WorldRenderedNow)
			{
				if (tmpColonistsWithMap.Any((Pair<Thing, Map> x) => x.Second == null))
				{
					tmpColonistsWithMap.RemoveAll((Pair<Thing, Map> x) => x.Second != null);
					goto IL_1A1;
				}
			}
			if (tmpColonistsWithMap.Any((Pair<Thing, Map> x) => x.Second == Find.CurrentMap))
			{
				tmpColonistsWithMap.RemoveAll((Pair<Thing, Map> x) => x.Second != Find.CurrentMap);
			}
		IL_1A1:
			tmpColonists.Clear();
			for (int j = 0; j < tmpColonistsWithMap.Count; j++)
			{
				tmpColonists.Add(tmpColonistsWithMap[j].First);
			}
			tmpColonistsWithMap.Clear();
			return tmpColonists;

		}

		[HarmonyPrefix]
		private static bool Prefix(ref List<Thing> __result, Rect rect,
			List<Pair<Thing, Map>> ___tmpColonistsWithMap, List<Thing> ___tmpColonists)
		{
			__result = ColonistsOrCorpsesInScreenRect(rect,
				___tmpColonistsWithMap, ___tmpColonists);
			return false;
		}
	}
}
