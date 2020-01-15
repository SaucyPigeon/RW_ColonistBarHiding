using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ColonistBarHiding
{
	/// <summary>
	/// Tracks which pawns on the colonist bar are hidden.
	/// </summary>
	public class HiddenPawnTracker : IExposable
	{
		private List<Pawn> pawns;

		public List<Pawn> Pawns
		{
			get { return pawns; }
		}

		public bool IsHidden(Pawn pawn)
		{
			return Pawns.Contains(pawn);
		}

		public void Hide(Pawn pawn)
		{
			Pawns.Add(pawn);
		}

		public void Show(Pawn pawn)
		{
			if (!IsHidden(pawn))
			{
				throw new ArgumentException($"Cannot remove {pawn.Label} as they are not already hidden.");
			}
			Pawns.Remove(pawn);
		}

		public void ExposeData()
		{
			Scribe_Collections.Look(ref pawns, "hiddenPawns", LookMode.Reference);
		}

		public HiddenPawnTracker(List<Pawn> pawns)
		{
			this.pawns = pawns;
		}

		public HiddenPawnTracker()
		{
			this.pawns = new List<Pawn>();
		}
	}
}
