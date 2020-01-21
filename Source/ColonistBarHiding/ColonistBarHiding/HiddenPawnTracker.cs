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

		/// <summary>
		/// The pawns marked as hidden.
		/// </summary>
		public List<Pawn> Pawns
		{
			get { return pawns; }
		}

		/// <summary>
		/// Checks if the given pawn is hidden.
		/// </summary>
		/// <param name="pawn">The pawn to check.</param>
		/// <returns>True if the pawn is hidden, false otherwise.</returns>
		public bool IsHidden(Pawn pawn)
		{
			return Pawns.Contains(pawn);
		}

		/// <summary>
		/// Hides the pawn.
		/// </summary>
		/// <param name="pawn">The pawn to hide.</param>
		public void Hide(Pawn pawn)
		{
			Pawns.Add(pawn);
		}

		/// <summary>
		/// Shows the pawn.
		/// </summary>
		/// <param name="pawn">The pawn to show.</param>
		public void Show(Pawn pawn)
		{
			if (!IsHidden(pawn))
			{
				throw new ArgumentException($"Cannot remove {pawn.Label} as they are not already hidden.");
			}
			Pawns.Remove(pawn);
		}

		/// <summary>
		/// Gets the amount of hidden pawns.
		/// </summary>
		public int HiddenCount
		{
			get
			{
				return Pawns.Count;
			}
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
