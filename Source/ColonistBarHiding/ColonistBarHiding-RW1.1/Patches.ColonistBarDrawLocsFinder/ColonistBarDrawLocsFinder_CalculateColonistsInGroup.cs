using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Verse;
using RimWorld;

namespace ColonistBarHiding.Patches.ColonistBarDrawLocsFinder
{
	using ColonistBarDrawLocsFinder = RimWorld.ColonistBarDrawLocsFinder;

	/// <summary>
	/// Patch for ColonistBarDrawLocsFinder.CalculateColonistsInGroup(), which
	/// gets the amount of entries for each group. This patch accounts for
	/// hidden entries.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("CalculateColonistsInGroup")]
	internal class ColonistBarDrawLocsFinder_CalculateColonistsInGroup
	{
		[HarmonyPrefix]
		private static bool Prefix(ref List<int> ___entriesInGroup)
		{
			var entries = ColonistBarUtility.GetVisibleEntries(Find.ColonistBar);
			var totalGroupsCount = ColonistBarUtility.GetTotalGroupsCount();

			var list = new List<int>(totalGroupsCount);
			for (int i = 0; i < totalGroupsCount; i++)
			{
				list.Add(0);
			}
			foreach (var entry in entries)
			{
				int group = entry.group;
				list[group]++;
			}
			___entriesInGroup = list;
			return false;
		}
	}
}
