using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld;
using UnityEngine;

namespace ColonistBarHiding.Patches
{
	/// <summary>
	/// Patch for ColonistBarDrawLocsFinder.FindBestScale(), which finds the
	/// best scale for the colonist bar depending on the amount of colonists.
	/// This patch accounts for hidden colonists.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarDrawLocsFinder))]
	[HarmonyPatch("FindBestScale")]
	internal class ColonistBarDrawLocsFinder_FindBestScale
	{
		// Modified private method ColonistBarDrawLocsFinder.FindBestScale().
		private static float FindBestScale(out bool onlyOneRow, out int maxPerGlobalRow, ref List<int> entriesInGroup, ref List<int> horizontalSlotsPerGroup)
		{
			float bestScale = 1f;
			List<ColonistBar.Entry> entries = Find.ColonistBar.Entries;
			int groupsCount = ColonistBarUtility.CalculateGroupsCount();
			while (true)
			{
				float num3 = (ColonistBar.BaseSize.x + 24f) * bestScale;
				float num4 = ColonistBarDrawLocsUtility.MaxColonistBarWidth - (float)(groupsCount - 1) * 25f * bestScale;
				maxPerGlobalRow = Mathf.FloorToInt(num4 / num3);
				onlyOneRow = true;
				bool result = ColonistBarDrawLocsUtility.TryDistributeHorizontalSlotsBetweenGroups(
					maxPerGlobalRow, entriesInGroup, out horizontalSlotsPerGroup);
				if (result)
				{
					int allowedRowsCountForScale = ColonistBarDrawLocsUtility.GetAllowedRowsCountForScale(bestScale);
					bool flag = true;
					int num5 = -1;

					foreach (var entry in entries)
					{
						if (!ColonistBarUtility.IsHidden(entry))
						{
							if (num5 != entry.group)
							{
								num5 = entry.group;
								int num6 = Mathf.CeilToInt((float)entriesInGroup[entry.group] / (float)horizontalSlotsPerGroup[entry.group]);
								if (num6 > 1)
								{
									onlyOneRow = false;
								}
								if (num6 > allowedRowsCountForScale)
								{
									flag = false;
									break;
								}
							}
						}
					}

					if (flag)
					{
						break;
					}
				}
				bestScale *= 0.95f;
			}
			return bestScale;
		}

		[HarmonyPrefix]
		private static bool Prefix(float __result, out bool onlyOneRow, out int maxPerGlobalRow, ref List<int> ___entriesInGroup, ref List<int> ___horizontalSlotsPerGroup)
		{
			//__result = FindBestScale(out onlyOneRow, out maxPerGlobalRow, ref ___entriesInGroup, ref ___horizontalSlotsPerGroup);
			__result = ColonistBarDrawLocsUtility.GetBestScale(___entriesInGroup, out onlyOneRow, out maxPerGlobalRow, out ___horizontalSlotsPerGroup);
			return false;
		}
	}
}
