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
	public class ReferenceLink {
		private string linkText;
		private PBRoot objectLink;
		private object abstractLink;
		private DocBase parent;
		private bool crossLink;

		protected ReferenceLink(DocBase parent, PBRoot link, bool cross) {
			this.linkText = link.FullName;
			this.abstractLink = null;
			this.objectLink = link;
			this.parent = parent;
			this.parent.AddReference(this);
			this.crossLink = cross;
		}

		public ReferenceLink(DocBase parent, string link_text, AbstractReferenceLinkResolver resolver) {
			this.linkText = link_text;
			this.objectLink = null;
			this.abstractLink = null;
			this.parent = parent;
			resolver.AddLink(this);
			this.parent.AddReference(this);
			this.crossLink = false;
		}

		public bool IsResolved {
			get { return this.objectLink != null; }
		}

		public string LinkText {
			get { return linkText; }
		}

		public PBRoot ObjectLink {
			get { return objectLink; }
			set { objectLink = value; }
		}

		public object AbstractLink {
			get { return abstractLink; }
		}

		public DocBase Parent {
			get { return this.parent; }
		}

		public bool IsCrossLink {
			get { return this.crossLink; }
		}

		public void CreateCrossLink() {
			// pro vytvoreni crosslinku musi byt link ukazovat na nejaky objekt a zaroven nesmi byt sam crosslinkem
			if (this.ObjectLink == null || this.IsCrossLink) return;

			// vytvoreni reference na objekt tohoto linku v objektu, na ktery tento link ukazuje
			// vytvoreni krizove reference, tzn. vytvoreni reference zpet z ciloveho objektu
			ReferenceLink cross = new ReferenceLink(this.ObjectLink.Documentation, this.Parent.ParentObject, true);
		}
	}
}
