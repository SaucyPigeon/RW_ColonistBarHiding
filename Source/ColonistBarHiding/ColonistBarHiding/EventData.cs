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
		public EventType Type
		{
			get;
		}

		public int Button
		{
			get;
		}

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
