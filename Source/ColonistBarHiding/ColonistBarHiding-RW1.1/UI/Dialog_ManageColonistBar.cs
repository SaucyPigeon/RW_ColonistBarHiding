using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace ColonistBarHiding.UI
{
	/// <summary>
	/// A UI dialog for managing the colonist bar, specifically for marking
	/// and unmarking which colonists are hidden.
	/// </summary>
	public class Dialog_ManageColonistBar : Window
	{
		private readonly bool fromColonistBar;

		private Vector2 scrollPosition;
		private float viewHeight;

		new const float Margin = 10f;

		public override void DoWindowContents(Rect inRect)
		{
			var rect = inRect.ContractedBy(Margin);
			// Avoid overlapping with close button
			const float distFromBottom = 40f;
			rect.height -= distFromBottom;
			var rect4 = new Rect(0, 0, rect.width - 16f, this.viewHeight);

			Widgets.BeginScrollView(rect, ref scrollPosition, rect4, true);
			var rect5 = rect4;
			rect5.width -= 16f;
			rect5.height = 9999f;

			Listing_Standard list = new Listing_Standard()
			{
				ColumnWidth = rect5.width,
				maxOneColumn = true,
				verticalSpacing = 6f
			};
			list.Begin(rect5);

			var pawns = ColonistBarUtility.GetColonistBarPawns();
			foreach (var pawn in pawns)
			{
				var pawnRect = list.GetRect(6f);
				AddButton(pawnRect, list, pawn, fromColonistBar);
			}

			if (Event.current.type == EventType.Layout)
			{
				this.viewHeight = list.CurHeight;
			}
			list.End();
			Widgets.EndScrollView();

			// Bottom buttons
			DoBottomButtons(rect, distFromBottom);
		}

		// TODO: Add pawn icon in menu, next to name
		private static void AddButton(Rect rect, Listing_Standard list, Pawn pawn, bool fromColonistBar)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}
			if (pawn == null)
			{
				throw new ArgumentNullException(nameof(pawn));
			}
			float truncWidth = rect.width / 2f - 4f;
			string pawnLabel = pawn.Label.Truncate(truncWidth);

			if (ColonistBarUtility.IsHidden(pawn))
			{
				string buttonLabel = "ColonistBarHiding.Restore".Translate();
				buttonLabel = buttonLabel.Truncate(truncWidth);
				if (list.ButtonTextLabeled(pawnLabel, buttonLabel))
				{
					ColonistBarUtility.RestoreColonist(pawn);
				}
			}
			else
			{
				string buttonLabel = "ColonistBarHiding.Remove".Translate();
				buttonLabel = buttonLabel.Truncate(truncWidth);
				if (list.ButtonTextLabeled(pawnLabel, buttonLabel))
				{
					ColonistBarUtility.RemoveColonist(pawn, fromColonistBar);
				}
			}
		}

		private void DoBottomButtons(Rect rect, float distFromBottom)
		{
			// Show all, Hide all, Close

			float heightPer = distFromBottom - 5f;
			float totalWidth = rect.width;
			float xGap = 10f;
			float widthPer = (totalWidth - (3 * xGap)) / 3f;

			Rect buttonRect = new Rect(rect.x, rect.yMax + 10f, widthPer, heightPer);

			if (Widgets.ButtonText(buttonRect, "ColonistBarHiding.ShowAllColonists".Translate()))
			{
				ColonistBarUtility.RestoreAllColonists();
			}
			buttonRect.x += widthPer + xGap;
			if (Widgets.ButtonText(buttonRect, "ColonistBarHiding.HideAllColonists".Translate()))
			{
				ColonistBarUtility.RemoveAllColonists(fromColonistBar);
			}
			buttonRect.x += widthPer + xGap;
			if (Widgets.ButtonText(buttonRect, "ColonistBarHiding.Close".Translate()))
			{
				this.Close();
			}
		}

		public Dialog_ManageColonistBar(bool fromColonistBar)
		{
			this.forcePause = true;
			this.doCloseX = true;
			this.doCloseButton = false;
			this.closeOnClickedOutside = true;
			this.absorbInputAroundWindow = true;
			this.fromColonistBar = fromColonistBar;
		}
	}
}
