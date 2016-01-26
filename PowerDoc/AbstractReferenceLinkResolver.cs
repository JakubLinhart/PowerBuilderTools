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
	public abstract class AbstractReferenceLinkResolver {
		private ArrayList relatedLinks = new ArrayList();
		private Target target;

		public abstract void ResolveLink(ReferenceLink link);

		public void AddLink(ReferenceLink link) {
			relatedLinks.Add(link);
		}

		public void ResolveRelatedLinks(Target target, bool crosslinks) {
			this.target = target;
			foreach (ReferenceLink link in relatedLinks) {
				ResolveLink(link);
				if (crosslinks && link.ObjectLink != null) link.CreateCrossLink();
			}
		}

		protected PBRoot TryStandard(ReferenceLink link) {
			Library lib = TryLibrary(link);
			if (lib != null) return lib;

			PowerObject obj = TryPowerObject(link);
			if (obj != null) return obj;

			obj = TryNestedPowerObject(link);
			if (obj != null) return obj;

			ScriptBase script = TryScriptBase(link);
			if (script != null) return script;

			PBRoot root = TryFullName(link);
			if (root != null) return root;

			return null;
		}

		protected Library TryLibrary(ReferenceLink link) {
			foreach (Library lib in target.Libraries) {
				if (lib.Name == link.LinkText) {
					return lib;
				}
			}

			return null;
		}

		protected PowerObject TryPowerObject(ReferenceLink link) {
			return Namespace.GetObject(link.LinkText);
		}

		protected PowerObject TryNestedPowerObject(ReferenceLink link) {
//			PowerObject obj = link.ScriptBaseParent.ParentPowerObject;
//			return obj.GetChildClass(link.LinkText);
			return null;
		}

		protected ScriptBase TryScriptBase(ReferenceLink link) {
			ScriptBase script = null;

/*			PowerObject obj = link.ScriptBaseParent.ParentPowerObject;
			script = obj.GetFunction(link.LinkText);
			if (script != null) return script;
			script = obj.GetEvent(link.LinkText);*/

			return script;
		}

		protected PBRoot TryFullName(ReferenceLink link) {
			if (link.LinkText.IndexOf(".") > 0) {
			} else {
			}
			return null;
		}
	}
}