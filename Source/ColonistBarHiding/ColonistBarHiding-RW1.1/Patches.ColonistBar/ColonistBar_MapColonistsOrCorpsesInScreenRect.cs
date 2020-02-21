using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBar.MapColonistsOrCorpsesInScreenRect(), which
	/// returns colonists or corpses in the given rect. This patch accounts for
	/// hidden pawns.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch("MapColonistsOrCorpsesInScreenRect")]
	internal class ColonistBar_MapColonistsOrCorpsesInScreenRect
	{
		// Modified public method ColonistBar.MapColonistsOrCorpsesInScreenRect()
		private static List<Thing> MapColonistsOrCorpsesInScreenRect(Rect rect,
			List<Thing> tmpMapColonistsOrCorpsesInScreenRect)
		{
			tmpMapColonistsOrCorpsesInScreenRect.Clear();
			if (!ColonistBarUtility.ShouldBeVisible())
			{
				return tmpMapColonistsOrCorpsesInScreenRect;
			}
			List<Thing> list = Find.ColonistBar.ColonistsOrCorpsesInScreenRect(rect);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Spawned)
				{
					tmpMapColonistsOrCorpsesInScreenRect.Add(list[i]);
				}
			}
			return tmpMapColonistsOrCorpsesInScreenRect;
		}

		[HarmonyPrefix]
		private static bool Prefix(ref List<Thing> __result, Rect rect,
			List<Thing> ___tmpMapColonistsOrCorpsesInScreenRect)
		{
			__result = MapColonistsOrCorpsesInScreenRect(rect,
				___tmpMapColonistsOrCorpsesInScreenRect);
			return false;
		}
	}
}
