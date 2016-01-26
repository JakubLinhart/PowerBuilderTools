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
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace PowerDoc {
	public class Target : PBRoot {
		private string applicationObject;
		private string applicationLibrary;
		private string workingDirectory;
		private string[] libraryList;
		private Library[] sortedLibraries = new Library[0];

		public static Target Load(Workspace workspace, string name, string pbt_file) {
			Target target = new Target(workspace, name, pbt_file);
			target.Open(pbt_file);

			return target;
		}

		public Target(Workspace parent, string name, string pbt_file) : base(parent, name) {
			parent.AddTarget(this);
			workingDirectory = Path.GetDirectoryName(pbt_file);
		}

		public IEnumerable Libraries {
			get { return this.childList; }
		}

		public Workspace Workspace {
			get { return ( Workspace ) this.Parent; }
		}

		public string[] LibraryList {
			get {
				// TODO: zkontrolovat zda libraryList existuje a je platny. Pokud ne, vyrobit ho.
				return libraryList;
			}
		}

		public IEnumerable SortedLibraries {
			get { return sortedLibraries; }
		}

		public void AddLibrary(Library lib) {
			this.childList.Add(lib);
		}

		public void SortLibraries() {
			if (childList.Count == 0) return;

			sortedLibraries = new Library[childList.Count];
			childList.CopyTo(sortedLibraries);
			Array.Sort(sortedLibraries);
		}

		public void Open(string pbtFile) {
			// TODO: aserce v pripade, ze je target jiz otevren
			this.applicationObject = null;
			this.applicationLibrary = null;
			PbtCompiler comp = new PbtCompiler(this);
			comp.Compile(pbtFile);
		}

		internal void SetLibraryList(string[] libs) {
			foreach (string libfile in libs) {
				Library lib = new Library(this, libfile, this.GetFullFileName(libfile));
				this.AddLibrary(lib);
			}

			BuildLibraryList(libs);
		}

		public void Close() {
		}

		public override void Compile() {
			Trace.Indent();
			foreach (Library lib in this.childList) {
				Trace.WriteLine("Kumpiluji knihovnu: " + lib.Name);
				lib.Compile();
			}
			Trace.Unindent();
			ResolveObjects();
		}

		protected void ResolveObjects() {
			foreach (PowerObject obj in Namespace.Objects) {
				obj.Resolve();
			}
		}

		internal void SetApplicationLibrary(string library_name) {
			this.applicationLibrary = library_name;
		}

		internal void SetApplicationObject(string object_name) {
			this.applicationObject = object_name;
		}

		protected void BuildLibraryList(string[] lib_list) {
			int i;
			
			libraryList = new string[lib_list.Length];
			for (i = 0; i < lib_list.Length; i++) {
				libraryList[i] = this.GetFullFileName(lib_list[i]);
			}
		}

		protected void BuildLibraryList() {
			// TODO: Implementace vytvoreni seznamu souboru knihoven z Library objektu, ktere jsou tomuto targetu
			// prirazeny.
		}

		protected string GetFullFileName(string name) {
			return Path.Combine(this.workingDirectory, name.Replace("\\\\", "\\"));
		}
	}
}
