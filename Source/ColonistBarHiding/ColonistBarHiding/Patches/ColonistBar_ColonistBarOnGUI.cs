using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using RimWorld;
using Verse;

namespace ColonistBarHiding.Patches
{
	[HarmonyPatch(typeof(ColonistBar))]
	[HarmonyPatch("ColonistBarOnGUI")]
	internal class ColonistBar_ColonistBarOnGUI
	{
		private static void HandleGroupFrameClicks(ColonistBar colonistBar, int currentGroup)
		{
			if (ColonistBarUtility.ShowGroupFrames)
			{
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

		private static void HandleRepaint(ColonistBar colonistBar, ColonistBar.Entry entry, Rect rect, bool isDifferentGroup, bool reordering, List<Pawn> colonistsToHighlight)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (isDifferentGroup && ColonistBarUtility.ShowGroupFrames)
				{
					colonistBar.drawer.DrawGroupFrame(entry.group);
				}
				if (entry.pawn != null)
				{
					colonistBar.drawer.DrawColonist(rect, entry.pawn, entry.map, colonistsToHighlight.Contains(entry.pawn), reordering);
				}
			}
		}

		private static int GetReorderableGroup(ColonistBar colonistBar, ColonistBar.Entry entry, List<ColonistBar.Entry> cachedEntries)
		{
			return ReorderableWidget.NewGroup(delegate (int from, int to)
			{
				ColonistBarUtility.Reorder(from, to, entry.group, cachedEntries);
			}, ReorderableDirection.Horizontal, colonistBar.SpaceBetweenColonistsHorizontal, delegate (int index, Vector2 dragStartPos)
			{
				ColonistBarUtility.DrawColonistMouseAttachment(index, dragStartPos, entry.group, cachedEntries);
			});
		}

		// Modified private method ColonistBar.ColonstBarOnGUI()
		private static void ColonistBarOnGUI(ColonistBar colonistBar, List<ColonistBar.Entry> cachedEntries, List<Pawn> colonistsToHighlight)
		{
			if (!ColonistBarUtility.ShouldBeVisible())
			{
				return;
			}
			if (Event.current.type != EventType.Layout)
			{
				//List<ColonistBar.Entry> entries = colonistBar.Entries;
				var entries = ColonistBarUtility.GetVisibleEntries();
				int currentGroup = -1;
				bool showGroupFrames = ColonistBarUtility.ShowGroupFrames;
				int reorderableGroup = -1;
				for (int i = 0; i < colonistBar.DrawLocs.Count; i++)
				{
					Rect rect = new Rect(colonistBar.DrawLocs[i].x, colonistBar.DrawLocs[i].y, colonistBar.Size.x, colonistBar.Size.y);
					ColonistBar.Entry entry = entries[i];


					bool isDifferentGroup = currentGroup != entry.group;
					currentGroup = entry.group;
					if (isDifferentGroup)
					{
						reorderableGroup = GetReorderableGroup(colonistBar, entry, cachedEntries);
					}
					bool reordering;
					if (entry.pawn != null)
					{
						colonistBar.drawer.HandleClicks(rect, entry.pawn, reorderableGroup, out reordering);
					}
					else
					{
						reordering = false;
					}
					HandleRepaint(colonistBar, entry, rect, isDifferentGroup, reordering, colonistsToHighlight);
				}
				currentGroup = -1;
				HandleGroupFrameClicks(colonistBar, currentGroup);
			}
			if (Event.current.type == EventType.Repaint)
			{
				colonistsToHighlight.Clear();
			}
		}

		[HarmonyPrefix]
		private static bool Prefix(ColonistBar __instance, List<ColonistBar.Entry> ___cachedEntries, List<Pawn> ___colonistsToHighlight)
		{
			ColonistBarOnGUI(__instance, ___cachedEntries, ___colonistsToHighlight);
			return false;
		}
	}
}
