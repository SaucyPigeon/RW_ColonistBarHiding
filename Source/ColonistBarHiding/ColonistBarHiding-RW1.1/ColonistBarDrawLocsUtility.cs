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
		[Obsolete("This method is identical to the original.")]
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
		public static Vector2 GetDrawLoc(float groupStartX, float groupStartY, int group, int positionInGroup, float scale, List<int> entriesInGroup, List<int> horizontalSlotsPerGroup)
		{
			// When accessing horizontalSlotsPerGroup, call GetGroupRelativeToVisible on group

			int horizontalSlot = horizontalSlotsPerGroup[ColonistBarUtility.GetGroupRelativeToVisible(group)];

			if (horizontalSlot == 0)
			{
				throw new ArgumentException("Horizontal slot cannot be equal to zero.");
			}
			int entryCountInGroup = entriesInGroup[group];

			float x = groupStartX + (float)(positionInGroup % horizontalSlot) * scale * (ColonistBar.BaseSize.x + 24f);
			float y = groupStartY + (float)(positionInGroup / horizontalSlot) * scale * (ColonistBar.BaseSize.y + 32f);

			if (positionInGroup >= entryCountInGroup - (entryCountInGroup % horizontalSlot))
			{
				int num2 = horizontalSlot - entryCountInGroup % horizontalSlot;
				x += (float)num2 * scale * (ColonistBar.BaseSize.x + 24f) * 0.5f;
			}
			return new Vector2(x, y);
		}

		/// <summary>
		/// Flattens the horizontal slots to the lowest between itself and
		/// relevant counts of entries for a group.
		/// </summary>
		/// <param name="entriesInGroup">The amount of entries per group.</param>
		/// <param name="horizontalSlotsPerGroup">The amount of horizontal slots per group.</param>
		public static void FlattenHorizontalSlots(List<int> entriesInGroup, List<int> horizontalSlotsPerGroup)
		{
			var visibleGroups = ColonistBarUtility.GetVisibleGroups();

			foreach (var visibleGroup in visibleGroups)
			{
				int index = ColonistBarUtility.GetGroupRelativeToVisible(visibleGroup);
				horizontalSlotsPerGroup[index] = Mathf.Min(horizontalSlotsPerGroup[index], entriesInGroup[visibleGroup]);
			}
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
				FlattenHorizontalSlots(entriesInGroup, horizontalSlotsPerGroup);
				entriesCount = ColonistBarUtility.GetVisibleEntriesCount();
			}

			int groupsCount = ColonistBarUtility.GetVisibleGroupsCount();
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
