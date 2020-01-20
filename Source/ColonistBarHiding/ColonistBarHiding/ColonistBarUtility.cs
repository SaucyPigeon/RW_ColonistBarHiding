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
	/// A general utility class for methods related to the colonist bar.
	/// </summary>
	public static class ColonistBarUtility
	{
		public static HiddenPawnTracker HiddenPawnTracker;

		public static List<Pawn> GetColonistBarPawns()
		{
			var list = new List<Pawn>();
			foreach (var entry in Find.ColonistBar.Entries)
			{
				list.Add(entry.pawn);
			}
			return list;
		}

		/// <summary>
		/// Marks the pawn as hidden for the colonist bar.
		/// </summary>
		/// <param name="pawn">The pawn to hide.</param>
		public static void RemoveColonist(Pawn pawn)
		{
			HiddenPawnTracker.Hide(pawn);
			Find.ColonistBar.MarkColonistsDirty();
		}

		/// <summary>
		/// Marks the pawn as shown for the colonist bar.
		/// </summary>
		/// <param name="pawn">The pawn to show.</param>
		public static void RestoreColonist(Pawn pawn)
		{
			HiddenPawnTracker.Show(pawn);
			Find.ColonistBar.MarkColonistsDirty();
		}

		/// <summary>
		/// Checks if the given pawn is hidden for the colonist bar.
		/// </summary>
		/// <param name="pawn">The pawn to check.</param>
		/// <returns>True if the pawn is marked as hidden, false otherwise.</returns>
		public static bool IsHidden(Pawn pawn)
		{
			// Fix bug where HiddenPawnTracker==null if item within is detroyed between saves without mod.
			if (HiddenPawnTracker == null)
			{
				HiddenPawnTracker = new HiddenPawnTracker();
			}
			return HiddenPawnTracker.IsHidden(pawn);
		}

		/// <summary>
		/// Checks if the given colonist bar entry is hidden.
		/// </summary>
		/// <param name="entry">The colonist bar entry to check.</param>
		/// <returns>True if the entry is marked as hidden, false otherwise.</returns>
		public static bool IsHidden(ColonistBar.Entry entry)
		{
			return IsHidden(entry.pawn);
		}

		/// <summary>
		/// Calculates the amount of groups for the colonist bar.
		/// </summary>
		/// <returns>The amount of groups in the colonist bar.</returns>
		public static int CalculateGroupsCount()
		{
			List<ColonistBar.Entry> entries = Find.ColonistBar.Entries;
			int currentGroup = -1;
			int groupsCount = 0;

			foreach (var entry in entries)
			{
				//if (!IsHidden(entry))
				//{
					if (currentGroup != entry.group)
					{
						groupsCount++;
						currentGroup = entry.group;
					}
				//}
			}
			return groupsCount;
		}

		private static int GetVisibleEntriesCountFrom(List<ColonistBar.Entry> source)
		{
			var sourceCount = source.Count;
			var hiddenCount = HiddenPawnTracker.HiddenCount;
			return sourceCount - hiddenCount;
		}

		/// <summary>
		/// Gets the amount of visible colonist bar entries.
		/// </summary>
		/// <returns>The amount of visible colonist bar entries.</returns>
		public static int GetVisibleEntriesCount()
		{
			return GetVisibleEntriesCountFrom(Find.ColonistBar.Entries);
		}

		// Modified private property ColonistBar.Visible
		/// <summary>
		/// Returns whether the colonist bar should be visible.
		/// </summary>
		//public static bool ShouldBeVisible
		//{
		//	get
		//	{
		//		return Verse.UI.screenWidth >= 800 && Verse.UI.screenHeight >= 500 && GetVisibleEntriesCount() != 0;
		//	}
		//}

		private static List<ColonistBar.Entry> GetVisibleEntriesFrom(List<ColonistBar.Entry> source)
		{
			return source.Where(x => !IsHidden(x)).ToList();
		}

		public static bool ShouldBeVisible()
		{
			return ShouldBeVisible(Find.ColonistBar.Entries);
		}

		public static bool ShouldBeVisible(List<ColonistBar.Entry> cachedEntries)
		{
			return Verse.UI.screenWidth >= 800 && Verse.UI.screenHeight >= 500 && GetVisibleEntriesCountFrom(cachedEntries) != 0;
		}

		/// <summary>
		/// Gets visible entries on the colonist bar.
		/// </summary>
		/// <returns>Entries that are marked as visible on the colonist bar.</returns>
		public static List<ColonistBar.Entry> GetVisibleEntries()
		{
			return GetVisibleEntriesFrom(Find.ColonistBar.Entries);
		}

		// Modified private property ColonistBar.ShowGroupFrames
		/// <summary>
		/// Gets whether the colonist bar should display group frames.
		/// </summary>
		public static bool ShowGroupFrames
		{
			get
			{
				List<ColonistBar.Entry> entries = GetVisibleEntries();
				int num = -1;
				for (int i = 0; i < entries.Count; i++)
				{
					num = Mathf.Max(num, entries[i].group);
				}
				return num >= 1;
			}
		}

		// Modified private method ColonstBar.Reorder()
		/// <summary>
		/// Reorders the colonist bar.
		/// </summary>
		/// <param name="from">The index to reorder from.</param>
		/// <param name="to">The index to reorder to.</param>
		/// <param name="entryGroup">The original group of the entry.</param>
		/// <param name="cachedEntries">The cached entries of the colonist bar.</param>
		public static void Reorder(int from, int to, int entryGroup, List<ColonistBar.Entry> cachedEntries)
		{
			int num = 0;
			Pawn pawn = null;
			Pawn pawn2 = null;
			Pawn pawn3 = null;

			foreach (var entry in cachedEntries)
			{
				if (entry.group == entryGroup && entry.pawn != null)
				{
					if (num == from)
					{
						pawn = entry.pawn;
					}
					if (num == to)
					{
						pawn2 = entry.pawn;
					}
					pawn3 = entry.pawn;
					num++;
				}
			}
			if (pawn == null)
			{
				return;
			}
			int num2 = (pawn2 == null) ? (pawn3.playerSettings.displayOrder + 1) : pawn2.playerSettings.displayOrder;
			foreach (var entry in cachedEntries)
			{
				Pawn pawn4 = entry.pawn;
				if (pawn4 != null)
				{
					if (pawn4.playerSettings.displayOrder == num2)
					{
						if (pawn2 != null && entry.group == entryGroup)
						{
							if (pawn4.thingIDNumber < pawn2.thingIDNumber)
							{
								pawn4.playerSettings.displayOrder--;
							}
							else
							{
								pawn4.playerSettings.displayOrder++;
							}
						}
					}
					else if (pawn4.playerSettings.displayOrder > num2)
					{
						pawn4.playerSettings.displayOrder++;
					}
					else
					{
						pawn4.playerSettings.displayOrder--;
					}
				}
			}
			pawn.playerSettings.displayOrder = num2;
			Find.ColonistBar.MarkColonistsDirty();
			MainTabWindowUtility.NotifyAllPawnTables_PawnsChanged();
		}

		// Modified private method ColonistBar.DrawColonistMouseAttachment()
		/// <summary>
		/// Draws the mouse attachment for the colonist at the given index.
		/// </summary>
		/// <param name="index">The index of the colonist.</param>
		/// <param name="dragStartPos">The starting drag position.</param>
		/// <param name="entryGroup">The group of the entry.</param>
		/// <param name="cachedEntries">The cached entries of the colonist bar.</param>
		/// <param name="drawer">The drawer of the colonist bar.</param>
		public static void DrawColonistMouseAttachment(int index, Vector2 dragStartPos, int entryGroup, List<ColonistBar.Entry> cachedEntries, ColonistBarColonistDrawer drawer)
		{
			Pawn pawn = null;
			Vector2 vector = default(Vector2);
			int num = 0;
			for (int i = 0; i < cachedEntries.Count; i++)
			{
				if (cachedEntries[i].group == entryGroup && cachedEntries[i].pawn != null)
				{
					if (num == index)
					{
						pawn = cachedEntries[i].pawn;
						vector = Find.ColonistBar.DrawLocs[i];
						break;
					}
					num++;
				}
			}
			if (pawn != null)
			{
				RenderTexture renderTexture = PortraitsCache.Get(pawn, ColonistBarColonistDrawer.PawnTextureSize, ColonistBarColonistDrawer.PawnTextureCameraOffset, 1.28205f);
				var size = Find.ColonistBar.Size;
				Rect rect = new Rect(vector.x, vector.y, size.x, size.y);
				Rect pawnTextureRect = drawer.GetPawnTextureRect(rect.position);
				pawnTextureRect.position += Event.current.mousePosition - dragStartPos;
				RenderTexture iconTex = renderTexture;
				Rect? customRect = new Rect?(pawnTextureRect);
				GenUI.DrawMouseAttachment(iconTex, string.Empty, 0f, default(Vector2), customRect);
			}
		}
	}
}
