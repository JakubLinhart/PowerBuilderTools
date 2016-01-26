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
using System.Collections;

namespace PowerDoc {
	public class ScriptBaseDoc : DocBase {
		private string description;
		private string notes;
		private string returns;
		private string history;
		private string example;

		public ScriptBaseDoc(ScriptBase parent) : base(parent) {
			this.parent = parent;
		}

		public ScriptBase ScriptBaseParent {
			get { return ( ScriptBase ) this.parent; }
		}

		public string Description {
			get { return description; }
			set { description = value; }
		}

		public bool HasDescription {
			get { return (description != null && description.Length > 0); }
		}

		public string Notes {
			get { return notes; }
			set { notes = value; }
		}

		public bool HasNotes {
			get { return (notes != null && notes.Length > 0); }
		}

		public string Returns {
			get { return returns; }
			set { returns = value; }
		}

		public bool HasReturns {
			get { return (returns != null && returns.Length > 0); }
		}
		
		public string History {
			get { return history; }
			set { history = value; }
		}

		public bool HasExample {
			get { return (example != null && example.Length > 0); }
		}

		public string Example {
			get { return example; }
			set { example = value; }
		}

		public bool HasHistory {
			get { return (history != null && history.Length > 0); }
		}

		// TODO: mozna by bylo dobre predelat na getter
		public override bool IsEmpty() {
			if (this.HasReturns) return false;
			if (this.HasExample) return false;
			if (this.HasHistory) return false;
			if (this.HasNotes) return false;
			if (this.HasDescription) return false;

			return base.IsEmpty();
		}
	}
}