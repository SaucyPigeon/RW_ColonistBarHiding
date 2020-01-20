using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ColonistBarHiding
{
	/// <summary>
	/// Stores state from the Event class.
	/// </summary>
	class EventData
	{
		/// <summary>
		/// The type of the event.
		/// </summary>
		public EventType Type
		{
			get;
		}

		/// <summary>
		/// The button of the event.
		/// </summary>
		public int Button
		{
			get;
		}

		/// <summary>
		/// Checks if the right mouse button is pressed during the event.
		/// </summary>
		/// <returns>True if the right mouse button is pressed, false otherwise.</returns>
		public bool RightMouseButton()
		{
			return Type == EventType.MouseDown && Button == 1;
		}

		public EventData(Event e)
		{
			Type = e.type;
			Button = e.button;
		}
	}
}
