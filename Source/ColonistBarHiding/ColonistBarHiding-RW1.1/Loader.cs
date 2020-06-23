using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using System.Reflection;
using RimWorld;
using Verse;
using UnityEngine;

namespace ColonistBarHiding
{
	/// <summary>
	/// Main entry point for Colonist Bar Hiding mod.
	/// </summary>
	[StaticConstructorOnStartup]
	class Loader
	{
		static Loader()
		{
#if DEBUG
			Harmony.DEBUG = true;
			Log.Warning("Colonist Bar Hiding is in debug mode. Please contact the mod author if you see this.");
#endif
		
			// Load harmony
			const string id = "uk.saucypigeon.rimworld.mod.colonistbarhiding";
			var harmony = new Harmony(id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			// Load settings
			ColonistBarUtility.HiddenPawnTracker = new HiddenPawnTracker();
		}
	}
}
