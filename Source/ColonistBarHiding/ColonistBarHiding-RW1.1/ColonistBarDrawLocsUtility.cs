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
	}
}
