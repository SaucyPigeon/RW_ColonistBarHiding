﻿using System;
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

		public override void DoWindowContents(Rect inRect)
		{
			var listingStandard = new Listing_Standard
			{
				ColumnWidth = inRect.width
			};
			listingStandard.Begin(inRect);

			var pawns = ColonistBarUtility.GetColonistBarPawns();
			foreach (var pawn in pawns)
			{
				Rect rect = listingStandard.GetRect(24f);
				DoPawnRow(rect, pawn, fromColonistBar);
				listingStandard.Gap(6f);
			}
			listingStandard.End();
		}

		// TODO: Add pawn icon in menu, next to name
		private static void DoPawnRow(Rect rect, Pawn pawn, bool fromColonistBar)
		{
			GUI.BeginGroup(rect);
			WidgetRow widgetRow = new WidgetRow(0f, 0f, UIDirection.RightThenUp, 99999f, 4f);
			widgetRow.Gap(4f);

			// TODO fix this...
			float width = rect.width - widgetRow.FinalX - 4f - Text.CalcSize("ColonistBarHiding.Restore".Translate()).x - 16f - 4f - Text.CalcSize("ColonistBarHiding.Restore".Translate()).x - 16f - 4f - 24f;
			widgetRow.Label(pawn.Label, width);

			if (ColonistBarUtility.IsHidden(pawn))
			{
				if (widgetRow.ButtonText("ColonistBarHiding.Restore".Translate(), null, true, false))
				{
					ColonistBarUtility.RestoreColonist(pawn);
				}
			}
			else
			{
				if (widgetRow.ButtonText("ColonistBarHiding.Remove".Translate(), null, true, false))
				{
					ColonistBarUtility.RemoveColonist(pawn, fromColonistBar);
				}
			}
			GUI.EndGroup();
		}

		public Dialog_ManageColonistBar(bool fromColonistBar)
		{
			this.forcePause = true;
			this.doCloseX = true;
			this.doCloseButton = true;
			this.closeOnClickedOutside = true;
			this.absorbInputAroundWindow = true;
			this.fromColonistBar = fromColonistBar;
		}
	}
}
