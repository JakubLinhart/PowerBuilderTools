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
	public abstract class LibraryObject : PBRoot {
		protected string comments;
		protected LibraryObjectType type;
		protected int size;
		protected string script = null;

		public new Library Parent {
			get { return ( Library ) parent; }
		}

		public string Comments {
			get { return this.comments; }
			set { comments = value; }
		}

		public string Script {
			get { return this.script; }
			set { this.script = value; }
		}

		public LibraryObjectType Type {
			get { return this.type; }
		}

		public int Size {
			get { return this.size; }
		}

		public LibraryObject(Library lib, string name, LibraryObjectType type, int size) : base(lib, name) {
			this.type = type;
			this.size = size;
		}
	}
}