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
	public abstract class DocBase {
		protected ArrayList references = new ArrayList();
		protected PBRoot parent;

		public DocBase(PBRoot parent) {
			this.parent = parent;
		}

		public PBRoot ParentObject {
			get { return this.parent; }
		}

		public IEnumerable ReferenceLinks {
			get { return this.references; }
		}

		public int CrossReferenceLinksCount {
			get {
				int count = 0;
				foreach (ReferenceLink link in this.ReferenceLinks) {
					if (link.IsCrossLink) count++;
				}

				return count;
			}
		}

		public int ReferenceLinksCount {
			get {
				return references.Count - this.CrossReferenceLinksCount;
			}
		}

		public virtual bool IsEmpty() {
			if (references.Count > 0) return false;

			return true;
		}

		public void AddReference(ReferenceLink reflink) {
			references.Add(reflink);
		}
	}
}