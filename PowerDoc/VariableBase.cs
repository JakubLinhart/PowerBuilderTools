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
using System.Diagnostics;

namespace PowerDoc {
	public class VariableBase : PBRoot {
		private bool isArray = false;
		private int arrayRange = -1;
		private string unresolvedType = null;
		private PowerObject variableType = null;

		public VariableBase(ScriptBase parent, string name, string type) : base(parent, name) {
			isArray = false;
			this.unresolvedType = type;
		}

		public VariableBase(ScriptBase parent, string name, string type, bool is_array) : base(parent, name) {
			this.isArray = is_array;
			this.unresolvedType = type;
		}

		public bool IsUnresolved {
			get { return (variableType == null && unresolvedType != null); }
		}

		public PowerObject VariableType {
			get { return variableType; }
		}

		public void Resolve() {
			if (this.IsUnresolved) {
				variableType = this.GetObject(unresolvedType);
				unresolvedType = null;
			}
		}

		public override void Compile() {
		}

		protected PowerObject GetObject(string name) {
			PowerObject obj = Namespace.GetObject(name);

			if (obj == null) {
				ScriptBase script = ( ScriptBase ) this.Parent;
				PowerObject parent_obj = ( PowerObject ) script.Parent;
				obj = parent_obj.GetLocalObject(name);
			}

			// TODO: nahradit exception
			if (obj == null) {
				Trace.WriteLine("Neznamy datvoy typ " + name + " bude formalne nahrazen typem structure");
				obj =  Namespace.GetObject("structure");
			}

			return obj;
		}
	}
}
