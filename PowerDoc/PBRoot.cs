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

namespace PowerDoc {
	public abstract class PBRoot : IComparable {
		protected PBRoot parent;
		protected string name;
		protected ArrayList childList = new ArrayList(); // TODO: predelat na private
		protected DocBase documentation;

		public PBRoot(PBRoot parent, string name) {
			this.name = name.ToLower();
			this.parent = parent;
		}

		public PBRoot Parent {
			get { return parent; }
		}

		public virtual string Name { 
			get { return name; }
		}

		public virtual string FullName {
			get { return name; } 
		}

		public IEnumerable Childs {
			get { return childList; }
		}

		public DocBase Documentation {
			get { return this.documentation; }
		}

		public virtual bool HasDocumentation {
			get {
				return !documentation.IsEmpty();
			}
		}

		public abstract void Compile();

		#region IComparable Members
		public int CompareTo(object obj) {
			PBRoot root = obj as PBRoot;
			if (root != null) {
				return this.Name.CompareTo(root.Name);
			} else {
				// TODO: vyhodit exception
			}

			return 0;
		}
		#endregion
	}
}