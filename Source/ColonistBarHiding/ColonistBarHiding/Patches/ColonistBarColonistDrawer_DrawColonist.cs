using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;

namespace ColonistBarHiding.Patches
{
	[HarmonyPatch(typeof(ColonistBarColonistDrawer))]
	[HarmonyPatch("DrawColonist")]
	class ColonistBarColonistDrawer_DrawColonist
	{
		[HarmonyPrefix]
		static bool Prefix(Pawn colonist)
		{
			if (ColonistBarUtility.IsHidden(colonist))
			{
				Log.Message($"Colonist {colonist.Label} was hidden.");
				return false;
			}
			return true;
		}
	}
}
