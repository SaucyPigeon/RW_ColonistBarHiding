using Harmony;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using ColonistBarHiding.UI;

namespace ColonistBarHiding.Patches.ColonistBarColonistDrawer
{
	using ColonistBarColonistDrawer = RimWorld.ColonistBarColonistDrawer;

	/// <summary>
	/// Patch for ColonistBarColonistDrawer.HandleClicks(), which handles
	/// clicks on the colonist bar UI. This patch adds a right-click floating
	/// menu, where the user can access a menu to manage which colonists are
	/// hidden.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarColonistDrawer))]
	[HarmonyPatch("HandleClicks")]
	internal class ColonistBarColonistDrawer_HandleClicks
	{
		/// <summary>
		/// Save state of event, as the original event from Event.current is used
		/// once original method is called.
		/// </summary>
		private static EventData eventData;

		/// <summary>
		/// Handle a right-click on the colonist bar UI, and show a
		/// floating-menu option for managing hidden colonists.
		/// </summary>
		/// <param name="rect">The rect of the colonist on the colonist bar.</param>
		/// <param name="colonist">The colonist on the colonist bar.</param>
		private static void HandleRightClick(Rect rect, Pawn colonist)
		{
			if (eventData.RightMouseButton() && Mouse.IsOver(rect))
			{
				var settings = new FloatMenuOption(
					"ColonistBarHiding.GetSettings".Translate(),
					delegate
					{
						Find.WindowStack.Add(new Dialog_ManageColonistBar());
					});

				var optionList = new List<FloatMenuOption>()
					{
						settings
					};

				Find.WindowStack.Add(new FloatMenu(optionList));
			}
		}

		[HarmonyPrefix]
		private static void Prefix()
		{
			eventData = new EventData(Event.current);
		}

		[HarmonyPostfix]
		private static void Postfix(Rect rect, Pawn colonist)
		{
			HandleRightClick(rect, colonist);
			eventData = null;
		}
	}
}
