using Harmony;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using ColonistBarHiding.UI;

namespace ColonistBarHiding.Patches
{
	[HarmonyPatch(typeof(ColonistBarColonistDrawer))]
	[HarmonyPatch("HandleClicks")]
	internal class ColonistBarColonistDrawer_HandleClicks
	{
		/// <summary>
		/// Save state of event, as the original event from Event.current is used
		/// once original method is called.
		/// </summary>
		private static EventData eventData;

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
