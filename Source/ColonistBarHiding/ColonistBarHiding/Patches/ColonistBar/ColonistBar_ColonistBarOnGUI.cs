using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using RimWorld;
using Verse;

namespace ColonistBarHiding.Patches.ColonistBar
{
	using ColonistBar = RimWorld.ColonistBar;
	
	/// <summary>
	/// Harmony patch for <see cref="ColonistBar.ColonistBarOnGUI"/> which
	/// handles the colonist bar's display on a GUI call. This patch accounts
	/// for hidden colonists. 
	/// </summary>
	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch("ColonistBarOnGUI")]
	internal class ColonistBar_ColonistBarOnGUI
	{
		/// <summary>
		/// Handles clicks on the <see cref="ColonistBar"/> group frame.
		/// </summary>
		/// <param name="currentGroup">The current colonist bar group to handle.</param>
		private static void HandleGroupFrameClicks(int currentGroup)
		{
			if (ColonistBarUtility.ShowGroupFrames)
			{
				var colonistBar = Find.ColonistBar;
				var entries = colonistBar.Entries;

				for (int i = 0; i < colonistBar.DrawLocs.Count; i++)
				{
					var entry = entries[i];
					bool flag = currentGroup != entry.group;
					currentGroup = entry.group;
					if (flag)
					{
						colonistBar.drawer.HandleGroupFrameClicks(entry.group);
					}
				}
			}
		}

		/// <summary>
		/// Handles the repainting of the <see cref="ColonistBar"/> for a given <see cref="ColonistBar.Entry"/>.
		/// </summary>
		/// <param name="entry">The <see cref="ColonistBar"/> entry to repaint.</param>
		/// <param name="rect">The <see cref="Rect"/> to draw when drawing the entry pawn.</param>
		/// <param name="isDifferentGroup">Whether the <see cref="ColonistBar.Entry"/> belongs to a different group compared to the previous entry.</param>
		/// <param name="reordering">Whether the <see cref="ColonistBar"/> entry is part of a reordering.</param>
		/// <param name="colonistsToHighlight">Pawns that have to be highlighted on the <see cref="ColonistBar"/>.</param>
		private static void HandleRepaint(ColonistBar.Entry entry, Rect rect, bool isDifferentGroup, bool reordering, List<Pawn> colonistsToHighlight)
		{ 
			if (Event.current.type == EventType.Repaint)
			{
				if (isDifferentGroup && ColonistBarUtility.ShowGroupFrames)
				{
					Find.ColonistBar.drawer.DrawGroupFrame(entry.group);
				}
				if (entry.pawn != null)
				{
					bool highlight = colonistsToHighlight.Contains(entry.pawn);
					Find.ColonistBar.drawer.DrawColonist(
						rect, entry.pawn, entry.map, highlight, reordering);
				}
			}
		}

		/// <summary>
		/// Gets the reorderable group for the <see cref="ColonistBar"/>.
		/// </summary>
		/// <param name="entry">The entry to reorder.</param>
		/// <param name="entries">The colonist bar entries.</param>
		/// <returns>The new group to reorder to.</returns>
		private static int GetReorderableGroup(ColonistBar.Entry entry, List<ColonistBar.Entry> entries)
		{
			int group = entry.group;
			return ReorderableWidget.NewGroup(delegate (int from, int to)
			{
				ColonistBarUtility.Reorder(from, to, group, entries);
			}, 
			ReorderableDirection.Horizontal,
			Find.ColonistBar.SpaceBetweenColonistsHorizontal,
			delegate (int index, Vector2 dragStartPos)
			{
				ColonistBarUtility.DrawColonistMouseAttachment(
					index, dragStartPos, group, entries);
			});
		}

		/// <summary>
		/// Handles clicks and repaints to the colonist bar.
		/// </summary>
		/// <param name="colonistsToHighlight">The colonists to highlight on the colonist bar.</param>
		private static void HandleOnGUI(List<Pawn> colonistsToHighlight)
		{
			var colonistBar = Find.ColonistBar;
			var entries = ColonistBarUtility.GetVisibleEntries();

			int currentGroup = -1;
			int reorderableGroup = -1;
			for (int i = 0; i < colonistBar.DrawLocs.Count; i++)
			{
				var rect = ColonistBarUtility.GetRect(i);
				var entry = entries[i];

				bool isDifferentGroup = currentGroup != entry.group;
				currentGroup = entry.group;
				if (isDifferentGroup)
				{
					reorderableGroup = GetReorderableGroup(entry, entries);
				}
				bool reordering;
				if (entry.pawn != null)
				{
					colonistBar.drawer.HandleClicks(
						rect, entry.pawn, reorderableGroup, out reordering);
				}
				else
				{
					reordering = false;
				}
				HandleRepaint(entry, rect, isDifferentGroup, reordering, colonistsToHighlight);
			}
		}

		/// <summary>
		/// Handles the colonist bar's display on a GUI call, accounting for
		/// hidden colonists. Original method: <see cref="ColonistBar.ColonistBarOnGUI"/>.
		/// </summary>
		/// <param name="colonistsToHighlight">The colonists to highlight on the colonist bar.</param>
		private static void ColonistBarOnGUI(List<Pawn> colonistsToHighlight)
		{
			if (ColonistBarUtility.ShouldBeVisible())
			{
				if (Event.current.type != EventType.Layout)
				{
					HandleOnGUI(colonistsToHighlight);
					const int currentGroup = -1;
					HandleGroupFrameClicks(currentGroup);
				}
				if (Event.current.type == EventType.Repaint)
				{
					colonistsToHighlight.Clear();
				}
			}
		}

		[HarmonyPrefix]
		private static bool Prefix(List<Pawn> ___colonistsToHighlight)
		{
			ColonistBarOnGUI(___colonistsToHighlight);
			return false;
		}
	}
}
