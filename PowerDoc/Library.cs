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
	public enum LibraryObjectType { Application = 0, Datawindow, Function, Menu, Query, Structure, UserObject,
												Window, Pipeline, Project, ProxyObject, Binary
											}

	public class Library : PBRoot {
		private string fileName;
		private LibraryObject[] sortedObjects = new LibraryObject[0];

		public Library(Target parent, string name, string file_name) : base(parent, name) {
			this.fileName = file_name;

			int pos = name.LastIndexOf('\\');
			if (pos > 0 && pos < name.Length) {
				this.name = name.Substring(pos + 1);
			}

			this.documentation = new LibraryDoc(this);
		}

		public Target ParentTarget {
			get { return ( Target ) this.Parent; }
		}

		public string FileName {
			get { return fileName; }
		}

		public IEnumerable SortedLibraryObjects {
			get { return sortedObjects; }
		}

		public void AddObject(LibraryObject obj) {
			this.childList.Add(obj);
		}

		public void SortLibraryObjects() {
			if (childList.Count == 0) return;
			sortedObjects = new LibraryObject[childList.Count];
			childList.CopyTo(sortedObjects);
			Array.Sort(sortedObjects);
		}

		public override void Compile() {
			Trace.Indent();
			PblFile file = PblFile.OpenPbl(this.FileName);
			file.LoadDirectory();
			foreach (PblEntry ent in file.Entries) {
				string ext = ent.Name.Substring(ent.Name.Length - 3, 3);
				string name = ent.Name.Substring(0, ent.Name.Length - 4);

				LibraryObject newobj;
				switch (ext) {
					case "srf" :
						newobj = new ObjectFunction(this, name, ent.Size);
						break;
					case "sra" :
						newobj = new ObjectApplication(this, name, ent.Size);
						break;
					case "srm" :
						newobj = new ObjectMenu(this, name, ent.Size);
						break;
					case "sru" :
						newobj = new ObjectUserObject(this, name, ent.Size);
						break;
					case "srw" :
						newobj = new ObjectWindow(this, name, ent.Size);
						break;
					default :
						newobj = null;
						break;
				}

				if (newobj != null) {
					this.AddObject(newobj);
					newobj.Script = file.LoadEntrySyntax(ent.Name);
					newobj.Comments = ent.Comments;
					newobj.Compile();
				}
			}

			Trace.Unindent();
			file.Close();
		}
	}
}