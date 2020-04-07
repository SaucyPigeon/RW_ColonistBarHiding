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
			if (GetVisibleEntriesCountFrom(Find.ColonistBar.Entries) == 0 && fromColonistBar)
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
		public static bool IsHidden(this ColonistBar.Entry entry)
		{
			return IsHidden(entry.pawn);
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
		public static List<int> GetVisibleGroups(this ColonistBar colonistBar)
		{
			var entries = GetVisibleEntriesFrom(colonistBar.Entries);
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
		public static int GetVisibleEntriesCountFrom(this List<ColonistBar.Entry> source)
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
		/// Gets visible colonist bar entries from the provided source of entries.
		/// </summary>
		/// <param name="source">The source of entries.</param>
		/// <returns>Visible entries from source.</returns>
		public static List<ColonistBar.Entry> GetVisibleEntriesFrom(this List<ColonistBar.Entry> source)
		{
			Log.Warning("Getting visible entries");
			var r = source.Where(x => !IsHidden(x)).ToList();
			Log.Warning("Got visible entries");

			Log.Warning("Printing visible entries:");
			foreach (var e in r)
			{
				Log.Warning(e.pawn.Name.ToString());
			}
			return r;
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

		public static List<ColonistBar.Entry> GetVisibleEntries(this ColonistBar colonistBar)
		{
			return GetVisibleEntriesFrom(colonistBar.Entries);
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
			var visibleGroups = GetVisibleGroups(Find.ColonistBar);
			return visibleGroups.IndexOf(group);
		}
	}
}
