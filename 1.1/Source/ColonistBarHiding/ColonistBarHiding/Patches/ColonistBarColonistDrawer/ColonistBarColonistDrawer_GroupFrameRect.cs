using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using RimWorld;
using Verse;

namespace ColonistBarHiding.Patches.ColonistBarColonistDrawer
{
	using ColonistBarColonistDrawer = RimWorld.ColonistBarColonistDrawer;
	using ColonistBar = RimWorld.ColonistBar;

	/// <summary>
	/// Patch for ColonistBarColonistDrawer.GroupFrameRect() which gets the group frame for a given group. This patch accounts for hidden colonists.
	/// </summary>
	[HarmonyPatch(typeof(ColonistBarColonistDrawer))]
	[HarmonyPatch("GroupFrameRect")]
	internal class ColonistBarColonistDrawer_GroupFrameRect
	{
		// Modified private method ColonistBarColonistDrawer.GroupFrameRect()
		private static Rect GroupFrameRect(int group)
		{
			float num = 99999f;
			float num2 = 0f;
			float num3 = 0f;
			var entries = ColonistBarUtility.GetVisibleEntries();
			List<Vector2> drawLocs = Find.ColonistBar.DrawLocs;
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].group == group)
				{
					num = Mathf.Min(num, drawLocs[i].x);
					num2 = Mathf.Max(num2, drawLocs[i].x + Find.ColonistBar.Size.x);
					num3 = Mathf.Max(num3, drawLocs[i].y + Find.ColonistBar.Size.y);
				}
			}
			return new Rect(num, 0f, num2 - num, num3).ContractedBy(-12f * Find.ColonistBar.Scale);
		}

		[HarmonyPrefix]
		private static bool Prefix(ref Rect __result, int group)
		{
			__result = GroupFrameRect(group);
			return false;
		}
	}
}
