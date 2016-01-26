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
	public class ScriptFunction : ScriptBase {
		private bool overloaded = false;

		public ScriptFunction(PowerObject parent, string name, PowerObject.Access access) : base(parent, name, access) {
			parent.AddFunction(this);

			documentation = new ScriptFunctionDoc(this);
		}

		public ScriptFunction(PowerObject parent, string name, PowerObject.Access access, string returns) : base(parent, name, access, returns) {
			parent.AddFunction(this);

			documentation = new ScriptFunctionDoc(this);
		}

		public bool IsOverloaded {
			get { return overloaded; }
		}

		public override void Resolve() {
			base.Resolve();

			PowerObject obj = ( PowerObject ) this.Parent;
			if (obj.GetFunctionOverloadCount(this.Name) > 1)
				overloaded = true;
			else
				overloaded = false;
		}
	}
}
