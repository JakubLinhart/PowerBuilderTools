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
using System.Collections;
using System.Diagnostics;

namespace PowerDoc {
	public abstract class ScriptBase : PBRoot {
		private string unresolvedReturnType = null;
		private string unresolvedThrows = null;

		private PowerObject returnType = null;
		private PowerObject throws = null;
		private Hashtable argumentsTable = new Hashtable();
		private ArrayList argumentsList = new ArrayList();
		private PowerObject.Access access;

		public ScriptBase(PowerObject parent, string name, PowerObject.Access access) : base(parent, name) {
			this.access = access;
		}

		public ScriptBase(PowerObject parent, string name, PowerObject.Access access, string returns) : base(parent, name) {
			this.access = access;
			this.unresolvedReturnType = returns;
		}

		public PowerObject ParentPowerObject {
			get { return ( PowerObject ) this.Parent; }
		}

		public Library ParentLibrary {
			get { return this.ParentPowerObject.ParentLibrary; }
		}

		public Target ParentTarget {
			get { return this.ParentPowerObject.ParentTarget; }
		}

		public new ScriptBaseDoc Documentation {
			get { return ( ScriptBaseDoc ) documentation; }
		}

		public string UnresolvedThrows {
			get { return unresolvedThrows; }
			set {
				// TODO: nesmi jit nastavit v dobe, kdy je this.throws naplneno (melo by byt naplneno pri resolve)
				this.unresolvedThrows = value;
			}
		}

		public override string FullName {
			get {
				PowerObject obj = ( PowerObject ) this.Parent;
				return obj.FullName + '.' + this.Name;
			}
		}

		public bool IsUnresolved {
			get { return (returnType == null && unresolvedReturnType != null); }
		}

		public bool HasArguments {
			get { return (argumentsList.Count > 0); }
		}

		public IEnumerable Arguments {
			get { return argumentsList; }
		}

		public int ArgumentsCount {
			get { return argumentsList.Count; }
		}

		public PowerObject.Access Access {
			get { return this.access; }
		}

		public PowerObject ReturnType {
			get { return this.returnType; }
		}

		public PowerObject Throws {
			get { return this.throws; }
		}

		public void AddArgument(Argument arg) {
			argumentsTable.Add(arg.Name, arg);
			argumentsList.Add(arg);
		}

		public override void Compile() {
		}

		public virtual void Resolve() {
			if (this.IsUnresolved) {
				returnType = GetObject(unresolvedReturnType);
			}
			ResolveArguments();

			if (unresolvedThrows != null) {
				throws = GetObject(unresolvedThrows);
				// TODO: otestovat zda typ existuje
				unresolvedThrows = null;
			}
		}

		protected PowerObject GetObject(string name) {
			PowerObject obj = Namespace.GetObject(name);

			if (obj == null) {
				PowerObject parent_obj = ( PowerObject ) this.Parent;
				obj = parent_obj.GetLocalObject(name);
			}

			// TODO: nahradit exception
			if (obj == null) {
				Trace.WriteLine("Neznamy datvoy typ " + name + " bude formalne nahrazen typem structure");
				obj = Namespace.GetObject("structure");
			}

			return obj;
		}

		private void ResolveArguments() {
			foreach (Argument arg in argumentsList) {
				arg.Resolve();
			}
		}
	}
}
