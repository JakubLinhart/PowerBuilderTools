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
	public abstract class ScriptedLibraryObject : LibraryObject, IPBContainer, IPowerScriptHolder {
		// promenne ucastnici se kompilace
		private bool compileInProgress = false;
		private PowerObject currentObject = null;
		private ScriptBase currentScriptBase = null;
		private PowerObject mainObject = null;

		public ScriptedLibraryObject(Library lib, string name, LibraryObjectType type, int size) : base(lib, name, type, size) {
		}

		public PowerObject MainObject {
			get { return mainObject; }
		}

		public PBRoot GetParent() {
			return this.parent;
		}

		public override void Compile() {
			Trace.WriteLine(this.Name);
			Trace.Indent();
			PowerScriptCompiler compiler = new PowerScriptCompiler(this);
			compileInProgress = true;
			compiler.CompileSource(this.Script);
			compileInProgress = false;
			Trace.Unindent();
		}

		// IPowerScriptHolder
		public void CreateGlobalType(string name, string super) {
			Debug.Assert(this.compileInProgress == true);

			// TODO: vytvoreni globalniho typu, jmeno predka musi byt v dobe vytvareni znamo
		}

		public void CreateUnresolvedGlobalType(string name, string super) {
			Debug.Assert(this.compileInProgress == true);
			PowerObject new_object;

			new_object = new PowerObject(this, name, super);
			currentObject = new_object;
			mainObject = currentObject;
		}

		public void CreateUnresolvedGlobalType(string name, string super, bool autoinstant) {
			CreateUnresolvedGlobalType(name, super);
			currentObject.SetAutoInstantiate(autoinstant);
		}

		public void CreateUnresolvedLocalType(string name, string super) {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentObject != null);
			PowerObject old_obj = this.currentObject;

			CreateUnresolvedLocalType(name, super, currentObject.Name);
			this.currentObject = old_obj;
		}

		public void CreateUnresolvedLocalType(string name, string super, string parent) {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentObject != null);
			PowerObject new_object;
			PowerObject parent_class;

			if (parent != currentObject.Name) {
				PowerObject obj = currentObject;
				do {
					parent_class = obj.GetChildClass(parent);
					obj = obj.ParentClass;
				} while (parent_class == null && obj != null);
				if (parent_class == null) {
					parent_class = Namespace.GetObject(parent);
				}
			} else {
				parent_class = currentObject;
			}

			// TODO: osetrit vyjimkou pripad, kdy je parent_class null; znamenalo by to, ze neni znam
			// rodic (trida, v kterem je vytvarena trida umistena) - ten musi byt znam vzdy
			new_object = new PowerObject(currentObject, name, super, parent_class);
			currentObject = new_object;
		}

		public void CreateEventInParent(string name, string parent) {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentObject != null);

			PowerObject new_obj = currentObject.TopParentClass.GetChildClass(parent);
			if (new_obj != null) {
				PowerObject old_obj = currentObject;
				currentObject = new_obj;
				CreateEvent(name);
				currentObject = old_obj;
			} else
				Trace.WriteLine("Neni mozne nalezt tridu " + parent + " pro vytvoreni eventu " + name + " (" + currentObject.FullName + ")");
		}

		public void CreateEventInParent(string name, string returns, string parent) {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentObject != null);

			PowerObject new_obj = currentObject.TopParentClass.GetChildClass(parent);
			if (new_obj != null) {
				PowerObject old_obj = currentObject;
				currentObject = new_obj;
				CreateEvent(name, returns);
				currentObject = old_obj;
			} else
				Trace.WriteLine("Neni mozne nalezt tridu " + parent + " pro vytvoreni eventu " + name + " (" + currentObject.FullName + ")");
			}

		public void CreateEvent(string name) {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentObject != null);

			ScriptEvent ev = new ScriptEvent(currentObject, name);
			currentScriptBase = ev;
		}

		public void CreateEvent(string name, string returns) {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentObject != null);

			ScriptEvent ev = new ScriptEvent(currentObject, name, returns);
			currentScriptBase = ev;
		}

		public void CreateFunction(string name, PowerObject.Access access) {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentObject != null);

			ScriptFunction fun = new ScriptFunction(currentObject, name, access);
			currentScriptBase = fun;
		}

		public void CreateFunction(string name, PowerObject.Access access, string returns) {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentObject != null);

			ScriptFunction fun = new ScriptFunction(currentObject, name, access, returns);
			currentScriptBase = fun;
		}

		public void FunctionFinished() {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentScriptBase != null);

			currentScriptBase = null;
		}

		public void EventFinished() {
			Debug.Assert(this.compileInProgress == true);
			//Debug.Assert(this.currentScriptBase != null);

			currentScriptBase = null;
		}

		public void CreateArgument(string name, string type) {
			CreateArgument(name, type, Argument.PassingType.Value, false);
		}

		public void CreateArgument(string name, string type, Argument.PassingType pass) {
			CreateArgument(name, type, pass, false);
		}

		public void CreateArgument(string name, string type, Argument.PassingType pass, bool is_array) {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentScriptBase != null);

			Argument arg = new Argument(currentScriptBase, name, type, pass, is_array);
		}

		public void SetThrows(string throws) {
			Debug.Assert(this.compileInProgress == true);
			Debug.Assert(this.currentScriptBase != null);

			currentScriptBase.UnresolvedThrows = throws;
		}

		public void CreateDocDescription(string doc) {
			if (currentScriptBase != null) {
				currentScriptBase.Documentation.Description = doc;
			} else {
				// TODO: tohle by se stavat nemelo, vyhodit exception nebo se sem program nesmi dostat
			}
		}

		public void AddException(string except) {
		}

		public void CreateDocNote(string doc) {
			if (currentScriptBase != null) {
				currentScriptBase.Documentation.Notes = doc;
			} else {
				// TODO: tohle by se stavat nemelo, vyhodit exception nebo se sem program nesmi dostat
			}
		}

		public void CreateDocReturns(string doc) {
			if (currentScriptBase != null) {
				currentScriptBase.Documentation.Returns = doc;
			} else {
				// TODO: tohle by se stavat nemelo, vyhodit exception nebo se sem program nesmi dostat
			}
		}

		public void CreateDocHistory(string doc) {
			if (currentScriptBase != null) {
				currentScriptBase.Documentation.History = doc;
			} else {
				// TODO: tohle by se stavat nemelo, vyhodit exception nebo se sem program nesmi dostat
			}
		}

		public void CreateDocExample(string doc) {
			if (currentScriptBase != null) {
				currentScriptBase.Documentation.Example = doc;
			} else {
				// TODO: tohle by se stavat nemelo, vyhodit exception nebo se sem program nesmi dostat
			}
		}

		public void CreateDocArguments(string[] doc) {
			if (currentScriptBase == null) {
				return;
			}

			int i = 0;
			foreach (Argument arg in currentScriptBase.Arguments) {
				if (doc.Length <= i) // TODO: todle asi bude chyba v dokumentaci, dodano mene prvku pole nez je argumentu
					break;
            arg.Documentation.Description = doc[i];
				i++;
			}
		}

		public void CreateDocReferences(string[] doc, AbstractReferenceLinkResolver resolver) {
			if (currentScriptBase == null) {
				return;
			}

			foreach (string text in doc) {
				ReferenceLink link = new ReferenceLink(currentScriptBase.Documentation, text, resolver);
			}
		}
	}
}
