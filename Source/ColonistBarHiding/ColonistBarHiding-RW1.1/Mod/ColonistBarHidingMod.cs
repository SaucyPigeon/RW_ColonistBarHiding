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
			var listingStandard = new Listing_Standard();
			listingStandard.Begin(inRect);

			if (Current.Game != null)
			{
				if (listingStandard.ButtonText("ColonistBarHiding.GetSettings".Translate()))
				{
					Find.WindowStack.Add(new Dialog_ManageColonistBar(false));
				}
			}
			else
			{
				listingStandard.Label("ColonistBarHiding.NoCurrentGame".Translate());
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
