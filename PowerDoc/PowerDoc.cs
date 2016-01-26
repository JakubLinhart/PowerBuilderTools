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
	class PowerDoc {

		static void PrintObjectHierarchy(PowerObject root) {
			Trace.WriteLine(root.Name);
			Trace.Indent();
			foreach (PowerObject obj in root.InheritedObjects) {
				PrintObjectHierarchy(obj);
			}
			Trace.Unindent();
		}

		static void PrintUserDefinedObjectsHierarchy() {
			Trace.WriteLine("");
			PrintUserDefinedObjectsHierarchy(Namespace.GetObject("PowerObject"));
		}

		static void PrintUserDefinedObjectsHierarchy(PowerObject root) {
			if (!root.IsHardwired) {
				// potomci uzivatelsky definovaneho objektu nemohou byt hardwired
				Trace.WriteLine(root.Name);
				Trace.Indent();
				foreach (PowerObject obj in root.InheritedObjects) {
					PrintObjectHierarchy(obj);
				}
				Trace.Unindent();
			} else {
				foreach (PowerObject obj in root.InheritedObjects) {
					PrintUserDefinedObjectsHierarchy(obj);
				}
			}
		}

		static void PrintObjectStructure(PowerObject root) {
			Trace.WriteLine(root.Name);
			Trace.Indent();
			foreach (PowerObject obj in root.ChildObjects) {
				PrintObjectStructure(obj);
			}
			Trace.Unindent();
		}

		static void PrintUserDefinedObjectsStructure() {
			Trace.WriteLine("");
			PrintUserDefinedObjectsStructure(Namespace.GetObject("PowerObject"));
		}

		static void PrintArguments(ScriptBase script) {
			if (script.HasArguments) {
				Trace.Write("(");
				foreach (Argument arg in script.Arguments) {
					Trace.Write(arg.VariableType.Name + " " + arg.Name + ", ");
				}
				Trace.Write(")");
			}
		}

		static void PrintEvents(PowerObject obj) {
			foreach (ScriptEvent ev in obj.Events) {
				Trace.Write(ev.Name);
				PrintArguments(ev);
				Trace.WriteLine("");
			}
		}

		static void PrintFunctions(PowerObject obj) {
			foreach (ScriptFunction fun in obj.Functions) {
				Trace.Write(fun.Name);
				PrintArguments(fun);
				Trace.WriteLine("");
			}
		}

		static void PrintUserDefinedObjectsStructure(PowerObject root) {
			if (!root.IsHardwired) {
				// potomci uzivatelsky definovaneho objektu nemohou byt hardwired
				if (root.ParentClass == null) {
					// vynechani vsech trid, ktere jsou obsazeny v jinem objektu
					Trace.WriteLine(root.Name);
					Trace.Indent();
					foreach (PowerObject obj in root.ChildObjects) {
						PrintObjectStructure(obj);
					}
					PrintEvents(root);
					PrintFunctions(root);
					Trace.Unindent();

					// vypis vsech oddedenych objektu
					foreach (PowerObject obj in root.InheritedObjects) {
						PrintUserDefinedObjectsStructure(obj);
					}
				}
			} else {
				foreach (PowerObject obj in root.InheritedObjects) {
					PrintUserDefinedObjectsStructure(obj);
				}
			}
		}

		static void PrintFunctionsAndEvents() {
			Trace.WriteLine("");
			foreach (PowerObject obj in Namespace.Objects) {
				if (!obj.IsHardwired) {
					Trace.WriteLine(obj.Name);
					Trace.Indent();
					PrintEvents(obj);
					PrintFunctions(obj);
					Trace.Unindent();
				}
			}
		}

		static void PrintUnresolvedObjects() {
			Trace.WriteLine("");
			Trace.WriteLine("Unresolved objects:");
			foreach (PowerObject obj in Namespace.Objects) {
				if (obj.IsUnresolved) {
					Trace.WriteLine(obj.Name);
				}
			}
		}

		static void Main(string[] args) {
			TextWriterTraceListener myWriter = new TextWriterTraceListener(System.Console.Out);
			Trace.Listeners.Add(myWriter);
			PFCDocAnalyzer pfcDoc = new PFCDocAnalyzer();
			KTKDocAnalyzer ktkDoc = new KTKDocAnalyzer();

			PowerScriptCompiler.AddAnalyzer(pfcDoc);
			PowerScriptCompiler.AddAnalyzer(ktkDoc);
			Workspace workspace = Workspace.Load(args[0], args[1]);
			workspace.Compile();
			ktkDoc.ResolveReferenceLinks(workspace.MainTarget, true);

			HtmlExport export = new HtmlExport();
			export.Export(workspace, args[2]);

			workspace.Close();
		}
	}
}
