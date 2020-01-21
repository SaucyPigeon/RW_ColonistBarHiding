//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using RimWorld;
//using Verse;
//using Harmony;

//namespace ColonistBarHiding.Patches
//{
//	/// <summary>
//	/// Patch for ColonistBarColonistDrawer.DrawColonist(), which deals with
//	/// drawing colonists on the colonist bar. This patch makes colonists that
//	/// are marked as hidden to not be shown on the colonist bar.
//	/// </summary>
//	[HarmonyPatch(typeof(ColonistBarColonistDrawer))]
//	[HarmonyPatch("DrawColonist")]
//	class ColonistBarColonistDrawer_DrawColonist
//	{
//		[HarmonyPrefix]
//		static bool Prefix(Pawn colonist)
//		{
//			if (ColonistBarUtility.IsHidden(colonist))
//			{
//				return false;
//			}
//			return true;
//		}
//	}
//}
