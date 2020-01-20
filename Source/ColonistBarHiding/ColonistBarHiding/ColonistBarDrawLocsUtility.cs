using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace ColonistBarHiding
{
	/// <summary>
	/// Utility class for methods related to colonist bar drawing locations.
	/// </summary>
	public static class ColonistBarDrawLocsUtility
	{
		// Original static private method ColonistBarDrawLocsFinder.GetAllowedRowsCountForScale()
		/// <summary>
		/// Gets the amount of allowed rows for a given scale of the colonist bar.
		/// </summary>
		/// <param name="scale">The scale of the colonist bar.</param>
		/// <returns>The amount of allowed rows.</returns>
		public static int GetAllowedRowsCountForScale(float scale)
		{
			if (scale > 0.58f)
			{
				return 1;
			}
			if (scale > 0.42f)
			{
				return 2;
			}
			return 3;
		}

		// Original private property ColonistBarDrawLocsFinder.MaxColonistBarWidth
		/// <summary>
		/// Returns the maximum colonist bar width.
		/// </summary>
		public static float MaxColonistBarWidth
		{
			get
			{
				return Verse.UI.screenWidth - 520f;
			}
		}

		// Modified private method ColonistBarDrawLocsFinder.CalculateColonistsInGroup()
		/// <summary>
		/// Returns a list where list[i] is the amount of entries belonging to group[i].
		/// </summary>
		/// <returns>A list indexed by group, containing the amount of entries for that group.</returns>
		public static List<int> GetGroupEntryCounts()
		{
			var list = new List<int>();
			List<ColonistBar.Entry> entries = Find.ColonistBar.Entries;
			int groupsCount = ColonistBarUtility.CalculateGroupsCount();
			for (int i = 0; i < groupsCount; i++)
			{
				list.Add(0);
			}

			foreach (var entry in entries)
			{
				if (!ColonistBarUtility.IsHidden(entry))
				{
					//List<int> list;
					//int group;
					// Not my fault...
					//(list = entriesInGroup)[group = entry.group] = list[group] + 1;

					int group = entry.group;
					list[group]++;
				}
			}
			return list;
		}

		// Modified private method ColonistBarDrawLocsFinder.TryDistributeHorizontalSlotsBetweenGroups()
		/// <summary>
		/// Trys to distribute horizontal slots between groups based on a per
		/// row maximum.
		/// </summary>
		/// <param name="maxPerGlobalRow">The maximum amount of slots per row.</param>
		/// <param name="entriesInGroup">The amount of entries per group.</param>
		/// <param name="horizontalSlotsPerGroup">The result of the distribution.</param>
		/// <returns>Returns true if successful, otherwise false.</returns>
		public static bool TryDistributeHorizontalSlotsBetweenGroups(int maxPerGlobalRow, List<int> entriesInGroup, out List<int> horizontalSlotsPerGroup)
		{
			horizontalSlotsPerGroup = new List<int>();

			int groupsCount = ColonistBarUtility.CalculateGroupsCount();
			for (int i = 0; i < groupsCount; i++)
			{
				horizontalSlotsPerGroup.Add(0);
			}
			GenMath.DHondtDistribution(horizontalSlotsPerGroup, (int i) => (float)entriesInGroup[i], maxPerGlobalRow);
			for (int i = 0; i < horizontalSlotsPerGroup.Count; i++)
			{
				if (horizontalSlotsPerGroup[i] == 0)
				{
					int num2 = horizontalSlotsPerGroup.Max();
					if (num2 <= 1)
					{
						return false;
					}
					int num3 = horizontalSlotsPerGroup.IndexOf(num2);
					List<int> list;
					// Not my fault
					int index;
					(list = horizontalSlotsPerGroup)[index = num3] = list[index] - 1;
					int index2;
					(list = horizontalSlotsPerGroup)[index2 = i] = list[index2] + 1;
				}
			}
			return true;
		}

		// Modified private method ColonistBarDrawLocsFinder.FindBestScale().
		/// <summary>
		/// Returns the best scale for the colonist bar.
		/// </summary>
		/// <param name="entriesInGroup">The amount of entries for a given group.</param>
		/// <param name="onlyOneRow">Whether the colonist bar should use only one row.</param>
		/// <param name="maxPerGlobalRow">The maximum amounts of entries per row.</param>
		/// <param name="horizontalSlotsPerGroup">The amount of horizontal slots to use for a given group.</param>
		/// <returns>The best scale based on the amount of entries per group.</returns>
		public static float GetBestScale(List<int> entriesInGroup, out bool onlyOneRow, out int maxPerGlobalRow, out List<int> horizontalSlotsPerGroup)
		{
			float bestScale = 1f;
			List<ColonistBar.Entry> entries = Find.ColonistBar.Entries;
			int groupsCount = ColonistBarUtility.CalculateGroupsCount();
			while (true)
			{
				float num3 = (ColonistBar.BaseSize.x + 24f) * bestScale;
				float num4 = MaxColonistBarWidth - (float)(groupsCount - 1) * 25f * bestScale;
				maxPerGlobalRow = Mathf.FloorToInt(num4 / num3);
				onlyOneRow = true;
				bool result = TryDistributeHorizontalSlotsBetweenGroups(
					maxPerGlobalRow, entriesInGroup, out horizontalSlotsPerGroup);
				if (result)
				{
					int allowedRowsCountForScale = GetAllowedRowsCountForScale(bestScale);
					bool flag = true;
					int currentGroup = -1;

					foreach (var entry in entries)
					{
						if (!ColonistBarUtility.IsHidden(entry))
						{
							if (currentGroup != entry.group)
							{
								currentGroup = entry.group;
								int num6 = Mathf.CeilToInt(
									(float)entriesInGroup[entry.group] / (float)horizontalSlotsPerGroup[entry.group]
									);
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

		// Modified private method ColonistBarDrawLocsFinder.GetDrawLoc()
		/// <summary>
		/// Gets the drawing location for a given entry.
		/// </summary>
		/// <param name="groupStartX">The group's starting x position.</param>
		/// <param name="groupStartY">The group's starting y position.</param>
		/// <param name="group">The group of the entry.</param>
		/// <param name="positionInGroup">The position of the entry in the group.</param>
		/// <param name="scale">The scale of the colonist bar.</param>
		/// <param name="entriesInGroup">The amount of entries for a given group.</param>
		/// <param name="horizontalSlotsPerGroup">The amount of horizontal slots for a given group.</param>
		/// <returns>The drawing location for a given entry.</returns>
		private static Vector2 GetDrawLoc(float groupStartX, float groupStartY, int group, int positionInGroup, float scale, List<int> entriesInGroup, List<int> horizontalSlotsPerGroup)
		{
			int horizonalSlot = horizontalSlotsPerGroup[group];
			int entryCountInGroup = entriesInGroup[group];

			float x = groupStartX + (float)(positionInGroup % horizonalSlot) * scale * (ColonistBar.BaseSize.x + 24f);
			float y = groupStartY + (float)(positionInGroup / horizonalSlot) * scale * (ColonistBar.BaseSize.y + 32f);

			if (positionInGroup >= entryCountInGroup - (entryCountInGroup % horizonalSlot))
			{
				int num2 = horizonalSlot - entryCountInGroup % horizonalSlot;
				x += (float)num2 * scale * (ColonistBar.BaseSize.x + 24f) * 0.5f;
			}
			return new Vector2(x, y);
		}

		// Modified private method ColonistBarDrawLocsFinder.CalculateDrawLocs()
		/// <summary>
		/// Gets the drawing locations for the provided entries on the colonist bar.
		/// </summary>
		/// <param name="scale">The scale of the colonist bar.</param>
		/// <param name="onlyOneRow">Whether to use only one row for the colonist bar.</param>
		/// <param name="maxPerGlobalRow">The maximum amount of entries per row.</param>
		/// <param name="entriesInGroup">The amount of entries in a given group.</param>
		/// <param name="horizontalSlotsPerGroup">The amount of horizontal slots per group.</param>
		/// <param name="drawLocs">The drawling locations for the given entries.</param>
		public static void GetDrawLocs(float scale, bool onlyOneRow, int maxPerGlobalRow, List<int> entriesInGroup, List<int> horizontalSlotsPerGroup, List<Vector2> drawLocs)
		{
			if (scale <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(scale));
			}
			drawLocs.Clear();

			int entriesCount = maxPerGlobalRow;
			if (onlyOneRow)
			{
				for (int i = 0; i < horizontalSlotsPerGroup.Count; i++)
				{
					horizontalSlotsPerGroup[i] = Mathf.Min(horizontalSlotsPerGroup[i], entriesInGroup[i]);
				}
				entriesCount = ColonistBarUtility.GetVisibleEntriesCount();
			}

			int groupsCount = ColonistBarUtility.CalculateGroupsCount();
			float num3 = (ColonistBar.BaseSize.x + 24f) * scale;
			float num4 = (float)entriesCount * num3 + (float)(groupsCount - 1) * 25f * scale;

			List<ColonistBar.Entry> entries = Find.ColonistBar.Entries;
			int currentGroup = -1;
			int numInGroup = -1;
			float groupStartX = ((float)Verse.UI.screenWidth - num4) / 2f;

			foreach (var entry in entries)
			{
				if (!ColonistBarUtility.IsHidden(entry))
				{
					if (currentGroup != entry.group)
					{
						if (currentGroup >= 0)
						{
							groupStartX += 25f * scale;
							groupStartX += (float)horizontalSlotsPerGroup[currentGroup] * scale * (ColonistBar.BaseSize.x + 24f);
						}
						numInGroup = 0;
						currentGroup = entry.group;
					}
					else
					{
						numInGroup++;
					}
					Vector2 drawLoc = GetDrawLoc(groupStartX, 21f, entry.group, numInGroup, scale, entriesInGroup, horizontalSlotsPerGroup);
					drawLocs.Add(drawLoc);
				}
			}
		}
	}
}
