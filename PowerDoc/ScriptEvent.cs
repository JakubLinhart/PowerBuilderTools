// Copyright (C) 2007  Jakub Linhart
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;

namespace PowerDoc {
	public class ScriptEvent : ScriptBase {
		public ScriptEvent(PowerObject parent, string name) : base(parent, name, PowerObject.Access.Public) {
			parent.AddEvent(this);

			documentation = new ScriptEventDoc(this);
		}

		public ScriptEvent(PowerObject parent, string name, string returns) : base(parent, name, PowerObject.Access.Public, returns) {
			parent.AddEvent(this);

			documentation = new ScriptEventDoc(this);
		}
	}
}
