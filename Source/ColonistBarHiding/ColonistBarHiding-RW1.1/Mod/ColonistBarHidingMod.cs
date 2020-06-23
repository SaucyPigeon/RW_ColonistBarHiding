using System;
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
		private static bool sortColonistBarIsLoaded;

		public static bool SortColonistBarIsLoaded
		{
			get
			{
				return sortColonistBarIsLoaded;
			}
		}


		public ColonistBarHidingMod(ModContentPack content) : base(content)
		{
			string packageId = "Azelion.SortColonistBar";
			if (ModLister.GetActiveModWithIdentifier(packageId) != null)
			{
				sortColonistBarIsLoaded = true;
			}
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			var rect = inRect.ContractedBy(6f);

			var buttonRect = new Rect(rect.x, rect.y, 200f, 32f);
			if (Current.Game != null)
			{
				if (Widgets.ButtonText(buttonRect, "ColonistBarHiding.GetSettings".Translate()))
				{
					Find.WindowStack.Add(new Dialog_ManageColonistBar(false));
				}
			}
			else
			{
				Widgets.Label(rect, "ColonistBarHiding.NoCurrentGame".Translate());
			}
			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return "ColonistBarHiding".Translate();
		}
	}
}
