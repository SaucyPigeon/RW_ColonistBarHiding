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
		/// <summary>
		/// Tracks pawns marked as hidden on the colonist bar. 
		/// </summary>
		public static HiddenPawnTracker HiddenPawnTracker;

		/// <summary>
		/// Gets pawns saved on the colonist bar.
		/// </summary>
		/// <returns>Pawns that are saved as entries on the colonist bar.</returns>
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
		/// Show the message for when all colonist bar entries are hidden.
		/// </summary>
		public static void ShowAllHiddenMessage()
		{
			Messages.Message("ColonistBarHiding.MessageAllEntriesHidden".Translate(), MessageTypeDefOf.CautionInput, false);
		}

		/// <summary>
		/// Marks the pawn as hidden for the colonist bar.
		/// </summary>
		/// <param name="pawn">The pawn to hide.</param>
		/// <param name="fromColonistBar">Whether the function was called from the colonist bar floating menu option.</param>
		public static void RemoveColonist(Pawn pawn, bool fromColonistBar)
		{
			HiddenPawnTracker.Hide(pawn);
			Find.ColonistBar.MarkColonistsDirty();
			if (!AnyVisibleEntries() && fromColonistBar)
			{
				ShowAllHiddenMessage();
			}
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
			// Fixed bug where HiddenPawnTracker==null if item within is detroyed between saves without mod.
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
		/// Calculates the amount of visible groups for the colonist bar.
		/// </summary>
		/// <returns>The amount of visible groups in the colonist bar.</returns>
		public static int GetVisibleGroupsCount()
		{
			var entries = GetVisibleEntries();
			int currentGroup = -1;
			int groupsCount = 0;

			foreach (var entry in entries)
			{
				if (currentGroup != entry.group)
				{
					groupsCount++;
					currentGroup = entry.group;
				}
			}
		
			return groupsCount;
		}

		/// <summary>
		/// Calculates the total amount of groups for the colonist bar.
		/// </summary>
		/// <returns>The total amount of groups in the colonist bar.</returns>
		public static int GetTotalGroupsCount()
		{
			var entries = Find.ColonistBar.Entries;
			int currentGroup = -1;
			int groupsCount = 0;

			foreach (var entry in entries)
			{
				if (currentGroup != entry.group)
				{
					groupsCount++;
					currentGroup = entry.group;
				}
			}

			return groupsCount;
		}

		/// <summary>
		/// Gets groups that are visible on the colonist bar.
		/// </summary>
		/// <returns>Groups that are visible on the colonist bar.</returns>
		public static List<int> GetVisibleGroups()
		{
			var entries = GetVisibleEntries();
			var list = new List<int>();
			int currentGroup = -1;

			foreach (var entry in entries)
			{
				if (currentGroup != entry.group)
				{
					list.Add(entry.group);
					currentGroup = entry.group;
				}
			}
			return list;
		}

		/// <summary>
		/// Returns the amount of colonist entries hidden from the colonist bar.
		/// </summary>
		/// <returns>The amount of colonist entries hidden from the colonist bar.</returns>
		public static int GetHiddenCount()
		{
			if (HiddenPawnTracker == null)
			{
				HiddenPawnTracker = new HiddenPawnTracker();
			}
			return HiddenPawnTracker.HiddenCount;
		}

		/// <summary>
		/// Returns the count of visible entries from a given list of entries.
		/// </summary>
		/// <param name="source">The source list.</param>
		/// <returns>Count of visible entries in source.</returns>
		public static int GetVisibleEntriesCountFrom(List<ColonistBar.Entry> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var sourceCount = source.Count;
			var hiddenCount = GetHiddenCount();
			var result = sourceCount - hiddenCount;
			if (result < 0)
			{
				result = 0;
			}
			return result;
		}

		/// <summary>
		/// Returns whether there are any visible entries for the colonist bar.
		/// </summary>
		/// <returns>Whether there are any visible entries for the colonist bar.</returns>
		public static bool AnyVisibleEntries()
		{
			var count = GetVisibleEntriesCount();
			return count > 0;
		}

		/// <summary>
		/// Gets the amount of visible colonist bar entries.
		/// </summary>
		/// <returns>The amount of visible colonist bar entries.</returns>
		public static int GetVisibleEntriesCount()
		{
			return GetVisibleEntriesCountFrom(Find.ColonistBar.Entries);
		}
		
		/// <summary>
		/// Gets visible colonist bar entries from the provided source of entries.
		/// </summary>
		/// <param name="source">The source of entries.</param>
		/// <returns>Visible entries from source.</returns>
		public static List<ColonistBar.Entry> GetVisibleEntriesFrom(List<ColonistBar.Entry> source)
		{
			return source.Where(x => !IsHidden(x)).ToList();
		}

		/// <summary>
		/// Returns whether the colonist bar should be visible.
		/// </summary>
		/// <returns>Whether the colonist bar should be visible.</returns>
		[Obsolete("Use extension style method instead.")]
		public static bool ShouldBeVisible()
		{
			return ShouldBeVisible(Find.ColonistBar.Entries);
		}

		public static bool ShouldBeVisible(this ColonistBar colonistBar)
		{
			return ShouldBeVisible(colonistBar.Entries);
		}

		/// <summary>
		/// Returns whether the colonist bar should be visible based on the provided entries. Original private property: <see cref="ColonistBar.Visible"/>.
		/// </summary>
		/// <param name="cachedEntries">The entries to test.</param>
		/// <returns>Whether the colonist bar should be visible based on the given entries.</returns>
		public static bool ShouldBeVisible(List<ColonistBar.Entry> cachedEntries)
		{
			return Verse.UI.screenWidth >= 800 && Verse.UI.screenHeight >= 500 && GetVisibleEntriesCountFrom(cachedEntries) != 0;
		}

		/// <summary>
		/// Gets visible entries on the colonist bar.
		/// </summary>
		/// <returns>Entries that are marked as visible on the colonist bar.</returns>
		[Obsolete("Use extension style method.")]
		public static List<ColonistBar.Entry> GetVisibleEntries()
		{
			return GetVisibleEntriesFrom(Find.ColonistBar.Entries);
		}

		public static List<ColonistBar.Entry> GetVisibleEntries(this ColonistBar colonistBar)
		{
			return GetVisibleEntriesFrom(colonistBar.Entries);
		}

		/// <summary>
		/// Tries to get the colonist bar entry at the given position. Original public method: <see cref="ColonistBar.TryGetEntryAt(Vector2, out ColonistBar.Entry)"/>.
		/// </summary>
		/// <param name="pos">The target position.</param>
		/// <param name="entry">The result of the search.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public static bool TryGetEntryAt(Vector2 pos, out ColonistBar.Entry entry)
		{
			List<Vector2> drawLocs = Find.ColonistBar.DrawLocs;
			var entries = ColonistBarUtility.GetVisibleEntries();
			Vector2 size = Find.ColonistBar.Size;
			for (int i = 0; i < drawLocs.Count; i++)
			{
				Rect rect = new Rect(drawLocs[i].x, drawLocs[i].y, size.x, size.y);
				if (rect.Contains(pos))
				{
					entry = entries[i];
					return true;
				}
			}
			entry = default(ColonistBar.Entry);
			return false;
		}

		/// <summary>
		/// Gets the rect for the given index of the colonist bar.
		/// </summary>
		/// <param name="index">The index for which to get the rect.</param>
		/// <returns>The rect of the index.</returns>
		[Obsolete("Inline this method.")]
		public static Rect GetRect(int index)
		{
			var drawLoc = Find.ColonistBar.DrawLocs[index];
			var size = Find.ColonistBar.Size;
			return new Rect(drawLoc, size);
		}

		/// <summary>
		/// Gets the given group as an index relative to the groups visible on
		/// the colonist bar.
		/// </summary>
		/// <param name="group">The group to convert.</param>
		/// <returns>The given group as an index relative to the groups visible
		/// on the colonist bar.</returns>
		public static int GetGroupRelativeToVisible(int group)
		{
			var visibleGroups = GetVisibleGroups();
			return visibleGroups.IndexOf(group);
		}
	}
}
