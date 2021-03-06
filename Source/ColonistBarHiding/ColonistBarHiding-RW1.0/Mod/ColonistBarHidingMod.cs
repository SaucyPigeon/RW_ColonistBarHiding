﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using ColonistBarHiding.UI;

namespace ColonistBarHiding.Mod
{
	using Mod = Verse.Mod;

	/// <summary>
	/// The <see cref="Verse.Mod"/> class for <see cref="ColonistBarHiding"/>.
	/// </summary>
	public class ColonistBarHidingMod : Mod
	{
		public ColonistBarHidingMod(ModContentPack content) : base(content)
		{
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			var listingStandard = new Listing_Standard();
			listingStandard.Begin(inRect);
			WidgetRow widgetRow = new WidgetRow(0f, 0f, UIDirection.RightThenUp, 99999f, 4f);
			widgetRow.Gap(4f);
			if (Current.Game != null)
			{
				if (widgetRow.ButtonText("ColonistBarHiding.GetSettings".Translate(), null, true, false))
				{
					Find.WindowStack.Add(new Dialog_ManageColonistBar(false));
				}
			}
			else
			{
				widgetRow.Label("ColonistBarHiding.NoCurrentGame".Translate());
			}
			listingStandard.End();
			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return "ColonistBarHiding".Translate();
		}
	}
}
