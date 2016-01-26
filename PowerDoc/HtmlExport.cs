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
using System.IO;

namespace PowerDoc {
	public class HtmlExport {
		private string outputDirectory;
		private string linkBase;
		private Workspace workspace;
		private Target target = null;

		public void Export(Workspace workspace, string destdir) {
			Export(workspace, destdir, destdir);
		}

		public void Export(Workspace workspace, string destdir, string link_base) {
			this.workspace = workspace;
			this.outputDirectory = destdir;
			this.linkBase = link_base;

			foreach (Target target in workspace.Childs) {
				this.target = target;
				break;
			}

			Index();
			Toc();
			AllObjects();
			DocedObjects();
			InheritanceHierarchy();
			Libraries();
		}

		#region Basic HTML
		protected void Header(StreamWriter fw, string title, string root_path) {
			fw.WriteLine("<html dir=\"LTR\">");
			fw.WriteLine("  <head>");
			fw.WriteLine("    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">");
			fw.WriteLine("    </meta>");
			fw.WriteLine("    <meta name=\"generator\" content=\"PowerDoc\" />");
			fw.WriteLine("    <title>NDoc.Core</title>");
			fw.WriteLine("    <link rel=\"stylesheet\" type=\"text/css\" href=\"" + root_path + "ndoc.css\">");
			fw.WriteLine("    </link>");
			fw.WriteLine("    <script src=\"" + root_path + "ndoc.js\" type=\"text/javascript\">");
			fw.WriteLine("    </script>");

			fw.WriteLine("  </head>");
			fw.WriteLine("  <body id=\"bodyID\" class=\"dtBODY\">");
			fw.WriteLine("    <INPUT class=\"userDataStyle\" id=\"userDataCache\" type=\"hidden\" />");
			fw.WriteLine("    <div id=\"nsbanner\">");
			fw.WriteLine("      <div id=\"bannerrow1\">");
			fw.WriteLine("        <table class=\"bannerparthead\" cellspacing=\"0\">");
			fw.WriteLine("          <tr id=\"hdr\">");
			fw.WriteLine("            <td class=\"runninghead\" nowrap=\"true\">NPIS</td>");
			fw.WriteLine("            <td class=\"product\" nowrap=\"true\">");
			fw.WriteLine("            </td>");
			fw.WriteLine("          </tr>");
			fw.WriteLine("        </table>");
			fw.WriteLine("      </div>");
			fw.WriteLine("      <div id=\"TitleRow\">");
			fw.WriteLine("        <h1 class=\"dtH1\">" + title + "</h1>");
			fw.WriteLine("      </div>");
			fw.WriteLine("    </div>");
			fw.WriteLine("    <div id=\"nstext\">");
		}

		protected void Header(StreamWriter fw, string title, Library from) {
			Header(fw, title, "");
		}

		protected void Header(StreamWriter fw, string title, PowerObject from) {
			if (from.IsNested)
				Header(fw, title, "../../");
			else
				Header(fw, title, "../");
		}

		protected void Header(StreamWriter fw, string title, ScriptBase from) {
			Header(fw, title, "../../");
		}

		protected void Footer(StreamWriter fw) {
			fw.WriteLine("    </div>");
			fw.WriteLine("  </body>");
			fw.WriteLine("</html>");
		}

		protected string Img(string img_name, string path) {
			return "<img src=\"" + path + img_name + "\"></img>";
		}

		protected string Img(string img_name, Library from) {
			return Img(img_name, "");
		}

		protected string Img(string img_name, PowerObject from) {
			if (from.IsNested)
				return Img(img_name, "../../");
			else
				return Img(img_name, "../");
		}

		protected string Img(string img_name, ScriptBase from) {
			return Img(img_name, "../../");
		}
		#endregion

		#region Library
		protected void Libraries() {
			foreach (Library lib in target.Childs) {
				Trace.WriteLine("Dokumentace pro: " + lib.Name);
				DocumentLibrary(lib);
			}
		}

		protected void LibraryReferences(StreamWriter fw, Library lib, string title, bool cross) {
			fw.WriteLine("<h4 class=\"dtH4\">" + title + "</h4>");
			fw.WriteLine("<p>");
			foreach (ReferenceLink link in lib.Documentation.ReferenceLinks) {
				if (link.IsCrossLink == cross) {
					if (link.IsResolved) {
						fw.Write("<a href=\"" + GetRelLink(link, lib) + "\">" + link.LinkText + "</a> ");
					} else {
						fw.Write(link.LinkText);
					}
				}
			}
			fw.WriteLine();
			fw.WriteLine("</p>");
		}

		protected void LibraryReferences(StreamWriter fw, Library lib) {
			if (lib.Documentation.ReferenceLinksCount > 0) {
				LibraryReferences(fw, lib, "Reference:", false);
			}

			if (lib.Documentation.CrossReferenceLinksCount > 0) {
				LibraryReferences(fw, lib, "Køížové reference:", true);
			}
		}

		protected void DocumentLibrary(Library lib) {
			StreamWriter fw = new StreamWriter(outputDirectory + "\\" + lib.Name + ".html");
			Directory.CreateDirectory(outputDirectory + "\\" + lib.Name + "\\");

			Header(fw, lib.Name, lib);

			Applications(fw, lib);
			Windows(fw, lib);
			Menus(fw, lib);
			UserObjects(fw, lib);
			GlobalFunctions(fw, lib);
			Structures(fw, lib);

			LibraryReferences(fw, lib);

			fw.Close();
		}

		protected void LibrarySectionHeader(StreamWriter fw, string title) {
			fw.WriteLine("      <h3 class=\"dtH3\">" + title + "</h3>");
			fw.WriteLine("      <div class=\"tablediv\">");
			fw.WriteLine("        <table class=\"dtTABLE\" cellspacing=\"0\">");
			fw.WriteLine("          <tr valign=\"top\">");
			fw.WriteLine("            <th width=\"30%\">" + title + "</th>");
			fw.WriteLine("            <th width=\"70%\">Popis</th>");
			fw.WriteLine("          </tr>");
		}

		protected void LibrarySectionFooter(StreamWriter fw) {
			fw.WriteLine("        </table>");
			fw.WriteLine("      </div>");
		}

		protected void LibrarySectionObject(StreamWriter fw, Library lib, LibraryObject obj) {
			fw.WriteLine("          <tr valign=\"top\">");
			fw.WriteLine("            <td width=\"30%\">");

			fw.WriteLine("              ");
			if (obj is ObjectWindow) {
				fw.Write(Img("objwin.png", ""));
			} else if (obj is ObjectMenu) {
				fw.Write(Img("objmenu.png", ""));
			} else if (obj is ObjectUserObject) {
				fw.Write(Img("objuser.png", ""));
			} else if (obj is ObjectFunction) {
				fw.Write(Img("objfun.png", ""));
			} else if (obj is ObjectStructure) {
				fw.Write(Img("objstr.png", ""));
			} else if (obj is ObjectApplication) {
				fw.Write(Img("objapp.png", ""));
			}

			fw.WriteLine("<a href=\"" + lib.Name + "/" + obj.Name + ".html\">" + obj.Name + "</a>");
			fw.WriteLine("            </td>");
			fw.WriteLine("            <td width=\"70%\">" + obj.Comments + "</td>");
			fw.WriteLine("          </tr>");
		}
		#endregion

		#region Linky & Adresare
		// TODO: optimalizovat relativni linky, tak aby se neprechazelo o adresar vis, pokud to neni nutne (napr.
		// link z jednoho eventu do jineho eventu, ktere oba patri do jednoho objektu)
		#region Linky - ScriptBase
		protected string GetLink(ScriptBase to) {
			if (to is ScriptEvent) {
				return GetLink(to as ScriptEvent);
			} else {
				Debug.Assert(to is ScriptFunction, "Parametr to neni ani ScriptEvent ani ScriptFunction");
				return GetLink(to as ScriptFunction);
			}
		}

		protected string GetRelLink(ScriptBase to, ScriptBase from) {
			if (to is ScriptEvent) {
				if (from is ScriptEvent) {
					return GetRelLink(to as ScriptEvent, from as ScriptEvent);
				} else {
					Debug.Assert(from is ScriptFunction, "Parametr from neni ani ScriptEvent ani ScriptFunction");
					return GetRelLink(to as ScriptEvent, from as ScriptFunction);
				}
			} else {
				Debug.Assert(to is ScriptFunction, "Parametr to neni ani ScriptEvent ani ScriptFunction");
				if (from is ScriptEvent) {
					return GetRelLink(to as ScriptFunction, from as ScriptEvent);
				} else {
					Debug.Assert(from is ScriptFunction, "Parametr from neni ani ScriptEvent ani ScriptFunction");
					return GetRelLink(to as ScriptFunction, from as ScriptFunction);
				}
			}
		}

		protected string GetRelLink(ScriptBase to, PowerObject from) {
			if (to is ScriptEvent) {
				return GetRelLink(to as ScriptEvent, from);
			} else {
				Debug.Assert(to is ScriptFunction, "Parametr to neni ani ScriptEvent ani ScriptFunction");
				return GetRelLink(to as ScriptFunction, from);
			}
		}

		protected string GetRelLink(ScriptBase to, Library from) {
			if (to is ScriptEvent) {
				return GetRelLink(to as ScriptEvent, from);
			} else {
				Debug.Assert(to is ScriptFunction, "Parametr to neni ani ScriptEvent ani ScriptFunction");
				return GetRelLink(to as ScriptFunction, from);
			}
		}

		protected string GetRelLink(Library to, ScriptBase from) {
			if (from is ScriptEvent) {
				return GetRelLink(to, from as ScriptEvent);
			} else {
				Debug.Assert(from is ScriptFunction, "Parametr from neni ani ScriptEvent ani ScriptFunction");
				return GetRelLink(to, from as ScriptFunction);
			}
		}

		protected string GetRelLink(PowerObject to, ScriptBase from) {
			if (from is ScriptEvent) {
				return GetRelLink(to, from as ScriptEvent);
			} else {
				Debug.Assert(from is ScriptFunction, "Parametr from neni ani ScriptEvent ani ScriptFunction");
				return GetRelLink(to, from as ScriptFunction);
			}
		}
		#endregion

		#region Link - Event
		protected string GetLink(ScriptEvent ev) {
			PowerObject obj = ( PowerObject ) ev.Parent;
			Library lib = ( Library ) obj.Parent;
			string path = ev.Name + ".html";

			while (obj.ParentClass != null) {
				path = obj.Name + "." + path;
				obj = obj.ParentClass;
			}
			path = lib.Name + '/' + obj.Name + '/' + path;

			return path;
		}

		protected string GetAbsLink(ScriptEvent script_to) {
			return linkBase + "/" + GetLink(script_to);
		}

		protected string GetRelLink(ScriptEvent to, ScriptEvent from) {
			return "../../" + GetLink(to);
		}

		protected string GetRelLink(ScriptEvent to, ScriptFunction from) {
			return "../../" + GetLink(to);
		}

		protected string GetRelLink(ScriptEvent to, PowerObject from) {
			if (from.IsNested)
				return "../../" + GetLink(to);
			else
				return "../" + GetLink(to);
		}

		protected string GetRelLink(ScriptEvent to, Library from) {
			return GetLink(to);
		}
		#endregion

		#region Link - Function
		protected string GetLink(ScriptFunction fun) {
			PowerObject obj = ( PowerObject ) fun.Parent;
			Library lib = ( Library ) obj.Parent;
			string path = fun.Name + ".html";

			path = lib.Name + '/' + obj.Name + '/' + path;
			return path;
		}

		protected string GetAbsLink(ScriptFunction fun) {
			return linkBase + '/' + GetLink(fun);
		}

		protected string GetRelLink(ScriptFunction to, ScriptEvent from) {
			return "../../" + GetLink(to);
		}

		protected string GetRelLink(ScriptFunction to, ScriptFunction from) {
			return "../../" + GetLink(to);
		}

		protected string GetRelLink(ScriptFunction to, PowerObject from) {
			if (from.IsNested)
				return "../../" + GetLink(to);
			else
				return "../" + GetLink(to);
		}

		protected string GetRelLink(ScriptFunction to, Library from) {
			return GetLink(to);
		}
		#endregion

		#region Link - PowerObject
		protected string GetLink(PowerObject obj) {
			string res;

			if (!obj.IsHardwired) {
				Library lib = ( Library ) obj.Parent;

				if (obj.ParentClass != null) {
					res = obj.Name + ".html";
					while (obj.ParentClass != null) {
						if (obj.ParentClass.ParentClass != null)
							res = obj.ParentClass.Name + "." + res;
						else
							res = obj.ParentClass.Name + "/" + res;
						obj = obj.ParentClass;
					}
					res = lib.Name + "/" + res;
				} else {
					res = lib.Name + "/" + obj.Name + ".html";
				}
			} else
				res = "/" + "hardwired/" + obj.Name + ".html";

			return res;
		}

		protected string GetAbsLink(PowerObject fun) {
			return linkBase + '/' + GetLink(fun);
		}

		protected string GetRelLink(PowerObject to, ScriptEvent from) {
			return "../../" + GetLink(to);
		}

		protected string GetRelLink(PowerObject to, ScriptFunction from) {
			return "../../" + GetLink(to);
		}

		protected string GetRelLink(PowerObject to, PowerObject from) {
			if (from.IsNested)
				return "../../" + GetLink(to);
			else
				return "../" + GetLink(to);
		}

		protected string GetRelLink(PowerObject to, Library from) {
			return GetLink(to);
		}
		#endregion

		#region Link - Library
		protected string GetLink(Library lib) {
			return lib.Name + ".html";
		}

		protected string GetAbsLink(Library fun) {
			return linkBase + '/' + GetLink(fun);
		}

		protected string GetRelLink(Library to, ScriptEvent from) {
			return "../../" + GetLink(to);
		}

		protected string GetRelLink(Library to, ScriptFunction from) {
			return "../../" + GetLink(to);
		}

		protected string GetRelLink(Library to, PowerObject from) {
			if (from.IsNested)
				return "../../" + GetLink(to);
			else
				return "../" + GetLink(to);
		}

		protected string GetRelLink(Library to, Library from) {
			return GetLink(to);
		}
		#endregion

		#region Link - PBRoot
		protected string GetRelLink(ReferenceLink link, PowerObject obj) {
			if (link.IsResolved) {
				if (link.ObjectLink is Library) return GetRelLink(link.ObjectLink as Library, obj);
				if (link.ObjectLink is PowerObject) return GetRelLink(link.ObjectLink as PowerObject, obj);
				if (link.ObjectLink is ScriptBase) return GetRelLink(link.ObjectLink as ScriptBase, obj);
			} else {
				// TODO: osetrit abstract link
			}
	
			return "";
		}

		protected string GetRelLink(ReferenceLink link, Library lib) {
			if (link.IsResolved) {
				if (link.ObjectLink is Library) return GetRelLink(link.ObjectLink as Library, lib);
				if (link.ObjectLink is PowerObject) return GetRelLink(link.ObjectLink as PowerObject, lib);
				if (link.ObjectLink is ScriptBase) return GetRelLink(link.ObjectLink as ScriptBase, lib);
			} else {
				// TODO: osetrit abstract link
			}
	
			return "";
		}

		protected string GetRelLink(PBRoot root, ScriptBase script) {
			if (root is Library) return GetRelLink(root as Library, script);
			if (root is PowerObject) return GetRelLink(root as PowerObject, script);
			if (root is ScriptBase) return GetRelLink(root as ScriptBase, script);

			return "";
		}
		#endregion

		protected string GetOutputDirectory(PowerObject obj) {
			string path;
			Library lib = ( Library ) obj.Parent;

			path = "\\" + obj.TopParentClass.Name;
			path = outputDirectory + "\\" + lib.Name + "\\" + path;

			return path;
		}

		protected string GetOutputFile(PowerObject obj) {
			string path;
			Library lib = ( Library ) obj.Parent;

			path = obj.Name + ".html";
			while (obj.ParentClass != null) {
				if (obj.ParentClass.ParentClass != null)
					path = obj.ParentClass.Name + "." + path;
				else
					path = obj.ParentClass.Name + "\\" + path;
				obj = obj.ParentClass;
			}

			path = outputDirectory + "\\" + lib.Name + "\\" + path;

			return path;
		}

		protected string GetOutputFile(ScriptEvent ev) {
			PowerObject obj = ( PowerObject ) ev.Parent;
			Library lib = ( Library ) obj.Parent;
			string path = ev.Name + ".html";

			while (obj.ParentClass != null) {
				path = obj.Name + "." + path;
				obj = obj.ParentClass;
			}
			path = obj.Name + "\\" + path;

			return outputDirectory + "\\" + lib.Name + "\\" + path;
		}

		protected string GetOutputFile(ScriptFunction fun) {
			PowerObject obj = ( PowerObject ) fun.Parent;
			Library lib = ( Library ) obj.Parent;
			string path = fun.Name + ".html";

			while (obj.ParentClass != null) {
				path = obj.Name + "." + path;
				obj = obj.ParentClass;
			}
			path = obj.Name + "\\" + path;

			return outputDirectory + "\\" + lib.Name + "\\" + path;
		}
		#endregion

		#region Script
		protected void DocumentScriptBase(ScriptBase script) {
			StreamWriter fw;

			if (script is ScriptFunction)
				fw = new StreamWriter(GetOutputFile(script as ScriptFunction));
			else
				fw = new StreamWriter(GetOutputFile(script as ScriptEvent));

			DocumentScriptBase(fw, script);

			fw.Close();
		}

		protected void ScriptBaseSyntax(StreamWriter fw, ScriptBase script) {
			fw.WriteLine("<PRE class=\"syntax\">");

			fw.Write(script.Access.ToString("g").ToLower() + ' ');

			if (script.ReturnType != null) {
				if (script is ScriptFunction)
					fw.Write("function ");
				else
					fw.Write("event ");

				fw.Write("<a href=\"" + GetRelLink(script.ReturnType, script) + "\">" + script.ReturnType.Name + "</a> ");
			} else {
				if (script is ScriptFunction)
					fw.Write("subroutine ");
				else
					fw.Write("event ");
			}

			fw.Write("<b>" + script.Name + "</b>");

			if (script.ArgumentsCount > 0) {
				fw.Write(" (");
				bool first = true;
				foreach (Argument arg in script.Arguments) {
					if (!first)
						fw.Write(", ");
					else
						first = false;
					fw.WriteLine();

					fw.Write("   ");
					if (arg.ArgPassingType != Argument.PassingType.Value) {
						fw.Write(arg.ArgPassingType.ToString("g").ToLower() + ' ');
					}

					fw.Write("<a href=\"" + GetRelLink(arg.VariableType, script) + "\">" + arg.VariableType.Name + "</a> ");
					fw.Write("<b>" + arg.Name + "</b>");
				}
				fw.WriteLine();
				fw.WriteLine(')');
			} else {
				fw.Write("()");
			}

			fw.WriteLine("</PRE>");
		}

		protected void ScriptBaseArgumentsDocumentation(StreamWriter fw, ScriptBase script) {
			if (script.HasArguments) {
				fw.WriteLine("<h4 class=\"dtH4\">Parametry</h4>");
				fw.WriteLine("<dl>");
				foreach (Argument arg in script.Arguments) {
					fw.WriteLine("<dt><i>" + arg.Name + "</i></dt>");
					fw.WriteLine("<dd>" + arg.Documentation.Description + "</dd>");
				}
				fw.WriteLine("</dl>");
			}
		}

		protected void ScriptBaseDocumentationSections(StreamWriter fw, ScriptBase script) {
			if (script.Documentation.HasDescription) {
				fw.WriteLine("<h4 class=\"dtH4\">Popis</h4>");
				fw.WriteLine("<p>");
				fw.WriteLine(script.Documentation.Description);
				fw.WriteLine("</p>");
			}

			ScriptBaseArgumentsDocumentation(fw, script);

			if (script.Documentation.HasReturns) {
				fw.WriteLine("<h4 class=\"dtH4\">Navratova hodnota</h4>");
				fw.WriteLine("<p>");
				fw.WriteLine(script.Documentation.Returns);
				fw.WriteLine("</p>");
			}

			if (script.Documentation.HasNotes) {
				fw.WriteLine("<h4 class=\"dtH4\">Poznamka</h4>");
				fw.WriteLine("<p>");
				fw.WriteLine(script.Documentation.Notes);
				fw.WriteLine("</p>");
			}

			if (script.Documentation.HasExample) {
				fw.WriteLine("<h4 class=\"dtH4\">Pøíklad</h4>");
				fw.WriteLine("<PRE class=\"syntax\">");
				fw.WriteLine(script.Documentation.Example.TrimEnd());
				fw.WriteLine("</PRE>");
			}

			if (script.Documentation.HasHistory) {
				fw.WriteLine("<h4 class=\"dtH4\">Historie</h4>");
				fw.WriteLine("<p>");
				fw.WriteLine(script.Documentation.History);
				fw.WriteLine("</p>");
			}
		}

		protected void ScriptBaseReferences(StreamWriter fw, ScriptBase script, string title, bool cross) {
			PowerObject obj = script.Parent as PowerObject;
			Library lib = obj.Parent as Library;
		
			fw.WriteLine("<h4 class=\"dtH4\">" + title + "</h4>");
			fw.WriteLine("<p>");

			foreach (ReferenceLink link in script.Documentation.ReferenceLinks) {
				if (link.IsCrossLink == cross) {
					fw.WriteLine("<a href=\"" + GetRelLink(link.ObjectLink, script) + "\">" + link.LinkText + "</a>");
				}
			}
			fw.WriteLine("</p>");
		}

		protected void ScriptBaseReferences(StreamWriter fw, ScriptBase script) {
			if (script.Documentation.ReferenceLinksCount > 0)
				ScriptBaseReferences(fw, script, "Reference:", false);
			if (script.Documentation.CrossReferenceLinksCount > 0)
				ScriptBaseReferences(fw, script, "Køížové reference:", true);

		}

		protected void DocumentScriptBase(StreamWriter fw, ScriptBase script) {
			PowerObject obj = script.Parent as PowerObject;
			Library lib = obj.Parent as Library;

			Header(fw, script.FullName, script);

			fw.WriteLine("<p>Knihovna: <a href=\"" + GetRelLink(lib, script) + "\">" + lib.Name + "</a></p>");
			fw.WriteLine("<p>Objekt: <a href=\"" + GetRelLink(obj, script) + "\">" + obj.FullName + "</a></p>");
			if (script.Throws != null) {
				fw.WriteLine("<p>Vyjímka <a href=\"" + GetRelLink(script.Throws, script) + "\">" + script.Throws.Name + "</a></p>");
			}

			// syntaxe funkce
			ScriptBaseSyntax(fw, script);
			ScriptBaseDocumentationSections(fw, script);
			ScriptBaseReferences(fw, script);

			Footer(fw);
			fw.Close();
		}
		#endregion

		#region Objects
		protected void ObjectFunctions(StreamWriter fw, PowerObject obj, PowerObject.Access access) {
			if (obj.GetFunctionsCount(true, access) > 0) {
				string access_str = access.ToString("g");
				fw.WriteLine("<h4 class=\"dtH4\">" + access_str + " funkce</h4>");
				fw.WriteLine("<div class=\"tablediv\">");
				fw.WriteLine("<table class=\"dtTABLE\" cellspacing=\"0\">");

				obj.SortFunctions();

				foreach (ScriptFunction fun in obj.SortedFunctions) {
					if (fun.Access == access) {
						DocumentScriptBase(fun);
						fw.WriteLine("<tr VALIGN=\"top\">");
						fw.WriteLine("<td width=\"40%\">" + Img("pubmethod.gif", obj) + "<a href=\"" + GetRelLink(fun, obj) + "\">" + fun.Name + "</a></td>");
						fw.WriteLine("<td width=\"60%\">" + fun.Documentation.Description + "</td>");
						fw.WriteLine("</tr>");
					}
				}

				if (access != PowerObject.Access.Private) {
					PowerObject super = obj.SuperClass;
					while (super != null) {
						foreach (ScriptFunction fun in super.SortedFunctions) {
							if (fun.Access == access) {
								fw.WriteLine("<tr VALIGN=\"top\">");
								fw.WriteLine("<td width=\"40%\">" + Img("pubmethod.gif", obj) + "<a href=\""  + GetRelLink(fun, obj) + "\">" + fun.Name + "</a> (<b>" + super.Name + "</b>)</td>");
								fw.WriteLine("<td width=\"60%\">" + fun.Documentation.Description + "</td>");
								fw.WriteLine("</tr>");
							}
						}
						super = super.SuperClass;
					}
				}

				fw.WriteLine("</table></div>");
			}
		}

		protected void ObjectEvents(StreamWriter fw, PowerObject obj) {
			if (obj.GetEventsCount(true) > 0) {
				obj.SortEvents();
				fw.WriteLine("<h4 class=\"dtH4\">Eventy</h4>");
				fw.WriteLine("<div class=\"tablediv\">");
				fw.WriteLine("<table class=\"dtTABLE\" cellspacing=\"0\">");
				foreach (ScriptEvent ev in obj.SortedEvents) {
					fw.WriteLine("<tr VALIGN=\"top\">");
					fw.WriteLine("<td width=\"40%\">" + Img("pubevent.gif", obj) + "<a href=\"" + GetRelLink(ev, obj) + "\">" + ev.Name + "</a></td>");
					fw.WriteLine("<td width=\"60%\">" + ev.Documentation.Description + "</td>");
					fw.WriteLine("</tr>");
					DocumentScriptBase(ev);
				}

				PowerObject super = obj.SuperClass;
				while (super != null) {
					super.SortFunctions();
					foreach (ScriptEvent ev in super.SortedEvents) {
						fw.WriteLine("<tr VALIGN=\"top\">");
						fw.WriteLine("<td width=\"40%\">" + Img("pubevent.gif", obj) + "<a href=\"" + GetRelLink(ev, obj) + "\">" + ev.Name + "</a> (<b>" + super.Name + "</b>)</td>");
						fw.WriteLine("<td width=\"60%\">" + ev.Documentation.Description + "</td>");
						fw.WriteLine("</tr>");
					}
					super = super.SuperClass;
				}
				fw.WriteLine("</table></div>");
			}
		}

		protected void ObjectReferences(StreamWriter fw, PowerObject obj, string title, bool cross) {
			Library lib = obj.ParentLibrary;

			fw.WriteLine("<h4 class=\"dtH4\">" + title + "</h4>");
			fw.WriteLine("<p>");
			foreach (ReferenceLink link in obj.Documentation.ReferenceLinks) {
				if (link.IsCrossLink == cross) {
					if (link.IsResolved) {
						fw.Write("<a href=\"" + GetRelLink(link, obj) + "\">" + link.LinkText + "</a> ");
					} else {
						fw.Write(link.LinkText);
					}
				}
			}
			fw.WriteLine();
			fw.WriteLine("</p>");
		}

		protected void ObjectReferences(StreamWriter fw, PowerObject obj) {
			if (obj.Documentation.ReferenceLinksCount > 0) {
				ObjectReferences(fw, obj, "Reference:", false);
			}

			if (obj.Documentation.CrossReferenceLinksCount > 0) {
				ObjectReferences(fw, obj, "Køížové reference:", true);
			}
		}

		protected void DocumentNestedObject(PowerObject obj) {
			Debug.Assert(obj.ParentClass != null);
			Library lib = ( Library ) obj.Parent;
			StreamWriter fw = new StreamWriter(GetOutputFile(obj));
			Directory.CreateDirectory(GetOutputDirectory(obj));

			Header(fw, obj.FullName, obj);

			fw.WriteLine("<p>Knihovna: <a href=\"" + GetRelLink(lib, obj) + "\">" + lib.Name + "</a></p>");

			// kompozice
			// TODO: predelat na treeview jako je v TOCu
			fw.WriteLine("<p>");
			fw.WriteLine("<a href=\"" + GetRelLink(obj.ParentClass, obj) + "\">" + obj.ParentClass.Name + "</a><br />");
			foreach (PowerObject child in obj.ParentClass.ChildObjects) {
				if (child == obj) {
					fw.WriteLine("&nbsp;&nbsp;&nbsp;<b>" + child.Name + "</b><br />");
					foreach (PowerObject child2 in child.ChildObjects) {
						fw.WriteLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"" + GetRelLink(child2, obj) + "\">" + child2.FullName + "</a><br />");
						DocumentNestedObject(child2);
					}
				}
				else
					fw.WriteLine("&nbsp;&nbsp;&nbsp;<a href=\"" + GetRelLink(child, obj) + "\">" + child.FullName + "</a><br />");
			}
			fw.WriteLine("</p>");

			// dedicnost
			// TODO: mozna pujde predelat na funkci, tak aby to bylo pro global objects a nested objects stejny
			string indent = "";
			ArrayList supers = obj.GetAllSuperClasses();
			if (supers.Count > 0) {
				for (int i = supers.Count - 1; i >= 0; i--) {
					PowerObject super_class = ( PowerObject ) supers[i];
					fw.WriteLine(indent + "<a href=\"" + GetRelLink(super_class, obj) + "\">" + super_class.FullName + "</a>");
					indent += "&nbsp;&nbsp;&nbsp;";
					fw.WriteLine("<br />");
				}
			}
			fw.WriteLine("<b>" + indent + obj.FullName + "</b><br />");
			fw.WriteLine("</p>");

			ObjectEvents(fw, obj);
			ObjectReferences(fw, obj);

			Footer(fw);
			fw.Close();
		}

		protected void GlobalFunction(ObjectFunction libfun) {
			PowerObject gfun = libfun.MainObject;
			Library lib = ( Library ) libfun.Parent;
			StreamWriter fw = new StreamWriter(GetOutputFile(gfun));
			ScriptFunction gfun_script = gfun.GetFunction(gfun.Name);

			Header(fw, gfun.Name, gfun);
			fw.WriteLine("<p>Knihovna: <a href=\"" + GetRelLink(lib, gfun) + "\">" + lib.Name + "</a></p>");

			ScriptBaseSyntax(fw, gfun_script);
			ScriptBaseDocumentationSections(fw, gfun_script);

			ObjectReferences(fw, gfun);

			Footer(fw);

			fw.Close();
		}

		protected void DocumentObject(ScriptedLibraryObject libobj) {
			PowerObject obj = libobj.MainObject;
			Library lib = ( Library ) libobj.Parent;
			string path = outputDirectory + "\\" + (libobj.Parent as Library).Name + "\\";
			StreamWriter fw = new StreamWriter(path + "\\" + obj.Name + ".html");
			Directory.CreateDirectory(path + "\\" + obj.Name);

			Header(fw, obj.Name, obj);

			fw.WriteLine("<p>Knihovna: <a href=\"" + GetRelLink(lib, obj) + "\">" + lib.Name + "</a></p>");
			fw.WriteLine("<p>Autoinstance: ");
			if (obj.IsAutoInstantiate)
				fw.WriteLine("<b>ano</b>");
			else
				fw.WriteLine("ne");

			fw.WriteLine("</a></p>");

			// kompozice
			string indent;
			if (obj.ChildObjectsCount > 0) {
				fw.WriteLine("<p>");
				fw.WriteLine("<b>" + obj.Name + "</b><br />");
				indent = "&nbsp;&nbsp;&nbsp;";
				foreach (PowerObject child in obj.ChildObjects) {
					fw.WriteLine(indent + "<a href=\"" + GetRelLink(child, obj) + "\">" + child.Name + "</a><br />");
					DocumentNestedObject(child);
				}
				fw.WriteLine("</p>");
			}

			// dedicnost
			indent = "";
			ArrayList supers = obj.GetAllSuperClasses();
			if (supers.Count > 0) {
				for (int i = supers.Count - 1; i >= 0; i--) {
					PowerObject super_class = ( PowerObject ) supers[i];
					fw.WriteLine(indent + "<a href=\"" + GetRelLink(super_class, obj) + "\">" + super_class.FullName + "</a>");

					indent += "&nbsp;&nbsp;&nbsp;";
					fw.WriteLine("<br />");
				}
			}
			fw.WriteLine("<b>" + indent + obj.Name + "</b><br />");
			indent += "&nbsp;&nbsp;&nbsp;";
			foreach (PowerObject inherited in obj.InheritedObjects) {
				if (inherited.ParentClass == null)
					fw.WriteLine(indent + "<a href=\"" + GetRelLink(inherited, obj) + "\">" + inherited.FullName + "</a><br />");
			}
			fw.WriteLine("</p>");

			fw.WriteLine("<h4 class=\"dtH4\">Popis</h4>");
			fw.WriteLine("<p>" + libobj.Comments + "</p>");

			// eventy
			ObjectEvents(fw, obj);

			// funkce
			ObjectFunctions(fw, obj, PowerObject.Access.Public);
			ObjectFunctions(fw, obj, PowerObject.Access.Protected);
			ObjectFunctions(fw, obj, PowerObject.Access.Private);

			ObjectReferences(fw, obj);

			Footer(fw);
			fw.Close();
		}

		protected void Window(ObjectWindow window) {
			DocumentObject(window);
		}

		protected void Windows(StreamWriter fw, Library lib) {
			ArrayList windows = new ArrayList();

			foreach (LibraryObject obj in lib.Childs) {
				if (obj is ObjectWindow) {
					windows.Add(obj);
				}
			}
			if (windows.Count <= 0) return;

			windows.Sort();
			LibrarySectionHeader(fw, "Window");
			foreach (ObjectWindow win in windows) {
				LibrarySectionObject(fw, lib, win);
				Window(win);
			}
			LibrarySectionFooter(fw);
		}

		protected void Menu(ObjectMenu menu) {
			DocumentObject(menu);
		}

		protected void Menus(StreamWriter fw, Library lib) {
			ArrayList menus = new ArrayList();

			foreach (LibraryObject obj in lib.Childs) {
				if (obj is ObjectMenu) {
					menus.Add(obj);
				}
			}
			if (menus.Count <= 0) return;

			LibrarySectionHeader(fw, "Menu");
			menus.Sort();
			foreach (ObjectMenu menu in menus) {
				LibrarySectionObject(fw, lib, menu);
				Menu(menu);
			}
			LibrarySectionFooter(fw);
		}

		protected void UserObject(ObjectUserObject obj) {
			DocumentObject(obj);
		}

		protected void UserObjects(StreamWriter fw, Library lib) {
			ArrayList userobjects = new ArrayList();

			foreach (LibraryObject obj in lib.Childs) {
				if (obj is ObjectUserObject) {
					userobjects.Add(obj);
				}
			}
			if (userobjects.Count <= 0) return;

			LibrarySectionHeader(fw, "UserObject");
			userobjects.Sort();
			foreach (ObjectUserObject obj in userobjects) {
				LibrarySectionObject(fw, lib, obj);
				UserObject(obj);
			}
			LibrarySectionFooter(fw);
		}

		protected void GlobalFunctions(StreamWriter fw, Library lib) {
			ArrayList functions = new ArrayList();

			foreach (LibraryObject obj in lib.Childs) {
				if (obj is ObjectFunction) {
					functions.Add(obj);
				}
			}
			if (functions.Count <= 0) return;

			LibrarySectionHeader(fw, "Function");
			functions.Sort();
			foreach (ObjectFunction fun in functions) {
				LibrarySectionObject(fw, lib, fun);
				GlobalFunction(fun);
			}
			LibrarySectionFooter(fw);
		}

		protected void Application(ObjectApplication app) {
			DocumentObject(app);
		}

		protected void Applications(StreamWriter fw, Library lib) {
			ArrayList applications = new ArrayList();

			foreach (LibraryObject obj in lib.Childs) {
				if (obj is ObjectApplication) {
					applications.Add(obj);
				}
			}
			if (applications.Count <= 0) return;

			LibrarySectionHeader(fw, "Application");
			applications.Sort();
			foreach (ObjectApplication app in applications) {
				LibrarySectionObject(fw, lib, app);
				Application(app);
			}
			LibrarySectionFooter(fw);
		}

		protected void Structure(ObjectStructure structure) {
			DocumentObject(structure);
		}

		protected void Structures(StreamWriter fw, Library lib) {
			ArrayList structures = new ArrayList();

			foreach (LibraryObject obj in lib.Childs) {
				if (obj is ObjectStructure) {
					structures.Add(obj);
				}
			}
			if (structures.Count <= 0) return;

			LibrarySectionHeader(fw, "Structure");
			structures.Sort();
			foreach (ObjectStructure str in structures) {
				LibrarySectionObject(fw, lib, str);
				Structure(str);
			}
			LibrarySectionFooter(fw);
		}
		#endregion

		#region Basic files
		protected void Index() {
			StreamWriter fw = new StreamWriter(outputDirectory + "\\index.html");
			fw.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Frameset//EN\"> ");
			fw.WriteLine("<html>");
			fw.WriteLine("\t<head>");
			fw.WriteLine("");
			fw.WriteLine("\t\t<title>PowerDoc - dokumentace projektu NPIS</title>");
			fw.WriteLine("\t\t<script language=\"JavaScript\">");
			fw.WriteLine("\t\tif (top.location != self.location) {");
			fw.WriteLine("\t\t\ttop.location = self.location;");
			fw.WriteLine("\t\t}");
			fw.WriteLine("\t\t</script>");
			fw.WriteLine("\t</head>");
			fw.WriteLine("\t<frameset cols=\"250,*\" framespacing=\"6\" bordercolor=\"#6699CC\">");
			fw.WriteLine("\t\t<frame name=\"contents\" src=\"toc.html\" frameborder=\"0\" scrolling=\"no\">");
			fw.WriteLine("\t\t<frame name=\"main\" src=\"all.html\" frameborder=\"1\">");
			fw.WriteLine("\t<noframes>");
			fw.WriteLine("\t\t<p>This page requires frames, but your browser does not support them.</p>");
			fw.WriteLine("\t</noframes>");
			fw.WriteLine("\t</frameset>");
			fw.WriteLine("</html>");
			fw.Close();
		}

		protected void AllObjects() {
			StreamWriter fw = new StreamWriter(outputDirectory + "\\all.html");
			Header(fw, "Index", "");
			Trace.WriteLine("all.html");

			ArrayList all_objects = new ArrayList();
			ArrayList scripts = new ArrayList();
			foreach (Library lib in this.target.Libraries) {
				foreach (LibraryObject libobj in lib.Childs) {
					if (libobj is ScriptedLibraryObject)
						all_objects.Add((libobj as ScriptedLibraryObject).MainObject);
				}
			}
			all_objects.Sort();

			// TODO: zajistit aby se pro objekt globalnich funkce nezobrazovala jeji funkce (f_funkce.f_funkce),
			// pze je to dementni

			foreach (PowerObject obj in all_objects) {
				fw.WriteLine("<a href=\"" + GetLink(obj) + "\">" + obj.Name + "</a><br />");
				scripts.Clear();
				foreach (ScriptBase script in obj.Events) {
					scripts.Add(script);
				}
				foreach (ScriptBase script in obj.Functions) {
					scripts.Add(script);
				}
				scripts.Sort();
				foreach (ScriptBase script in scripts) {
					fw.WriteLine("<a href=\"" + GetLink(script) + "\">" + script.FullName + "</a><br />");
				}
			}

			Footer(fw);
			fw.Close();
		}

		private int ObjDocCount(PowerObject obj) {
			int count = 0;

			foreach (PowerObject nested in obj.ChildObjects) {
				count += ObjDocCount(nested);
			}

			foreach (ScriptFunction fun in obj.Functions) {
				if (fun.HasDocumentation) count++;
			}

			foreach (ScriptEvent ev in obj.Events) {
				if (ev.HasDocumentation) count++;
			}

			return count;
		}

		private void DocedObjects() {
			StreamWriter fw = new StreamWriter(outputDirectory + "\\doced.html");
			Header(fw, "Seznam dokumentovaných objektù", "");
			Trace.WriteLine("doced.html");

			ArrayList doced_objects = new ArrayList();
			foreach (PowerObject obj in Namespace.Objects) {
				int count = ObjDocCount(obj);
				if (count > 0) {
					fw.WriteLine("<a href=\"" + GetLink(obj) + "\">" + obj.Name + "</a> (" + count + ") ");
				}
			}

			Footer(fw);
			fw.Close();
		}

		private void InheritanceHierarchy(StreamWriter fw, PowerObject super, int indent) {
			for (int i = 0; i < indent; i++)
				fw.WriteLine("&nbsp;&nbsp;&nbsp;");
			fw.WriteLine("<a href=\"" + GetLink(super) + "\">" + super.Name + "</a><br />");
			super.SortInheritedClasses();
			foreach (PowerObject obj in super.SortedInheritedObjects) {
				if (!obj.IsNested)
					InheritanceHierarchy(fw, obj, indent + 1);
			}
		}

		private void InheritanceHierarchy() {
			StreamWriter fw = new StreamWriter(outputDirectory + "\\inheritance.html");
			Header(fw, "Dìdiènost", "");
			Trace.WriteLine("inheritance.html");

			ArrayList user_classes = new ArrayList();
			foreach (PowerObject obj in Namespace.Objects) {
				if (!obj.IsNested && !obj.IsHardwired && (obj.SuperClass == null || obj.SuperClass.IsHardwired))
					user_classes.Add(obj);
			}
			user_classes.Sort();

			foreach (PowerObject obj in user_classes)
				InheritanceHierarchy(fw, obj, 0);

			Footer(fw);
			fw.Close();
		}
		#endregion

		#region TOC
		protected void Toc() {
			StreamWriter fw = new StreamWriter(outputDirectory + "\\toc.html");
			fw.WriteLine("<html>");
			fw.WriteLine("<head>");
			fw.WriteLine("  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">");
			fw.WriteLine("  <title>Obsah</title>");
			fw.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"tree.css\" />");
			fw.WriteLine("  <script src=\"tree.js\" language=\"javascript\" type=\"text/javascript\">");
			fw.WriteLine("  </script>");
			fw.WriteLine("</head>");
			fw.WriteLine("<body id=\"docBody\" style=\"background-color: #6699CC; color: White; margin: 0px 0px 0px 0px;\" onload=\"resizeTree()\" onresize=\"resizeTree()\" onselectstart=\"return false;\">");
			fw.WriteLine("  <div style=\"font-family: verdana; font-size: 8pt; cursor: pointer; margin: 6 4 8 2; text-align: right\" onmouseover=\"this.style.textDecoration='underline'\" onmouseout=\"this.style.textDecoration='none'\" onclick=\"syncTree(window.parent.frames[1].document.URL)\">sync toc</div>");
			fw.WriteLine("    <div id=\"tree\" style=\"top: 35px; left: 0px;\" class=\"treeDiv\">");
			fw.WriteLine("      <div id=\"treeRoot\" onselectstart=\"return false\" ondragstart=\"return false\">");

			fw.WriteLine("        <div class=\"treeNode\">");
			fw.WriteLine("          <img src=\"treenodedot.gif\" class=\"treeNoLinkImage\"/>");
			fw.WriteLine("          <a href=\"all.html\" target=\"main\" class=\"treeUnselected\" onclick=\"clickAnchor(this)\">Index</a>");
			fw.WriteLine("        </div>");

			fw.WriteLine("        <div class=\"treeNode\">");
			fw.WriteLine("          <img src=\"treenodedot.gif\" class=\"treeNoLinkImage\"/>");
			fw.WriteLine("          <a href=\"doced.html\" target=\"main\" class=\"treeUnselected\" onclick=\"clickAnchor(this)\">Dokumentováno</a>");
			fw.WriteLine("        </div>");

			fw.WriteLine("        <div class=\"treeNode\">");
			fw.WriteLine("          <img src=\"treenodedot.gif\" class=\"treeNoLinkImage\"/>");
			fw.WriteLine("          <a href=\"inheritance.html\" target=\"main\" class=\"treeUnselected\" onclick=\"clickAnchor(this)\">Dìdiènost</a>");
			fw.WriteLine("        </div>");


			TocLibraries(fw);

			fw.WriteLine("      </div>");
			fw.WriteLine("    </div>");
			fw.WriteLine("  </div>");
			fw.WriteLine("</body>");
			fw.WriteLine("</html>");
			fw.Close();
		}

		protected void TocLibraries(StreamWriter fw) {
			target.SortLibraries();
			foreach (Library lib in this.target.SortedLibraries) {
				fw.WriteLine("        <div class=\"treeNode\">");
				fw.WriteLine("          <img src=\"treenodeplus.gif\" class=\"treeLinkImage\" onclick=\"expandCollapse(this.parentNode)\"/>");
                fw.WriteLine("          <a href=\"" + lib.Name + ".html\" target=\"main\" class=\"treeUnselected\" onclick=\"clickAnchor(this)\">" + lib.Name + "</a>");
				fw.WriteLine("          <div class=\"treeSubnodesHidden\">");
				TocLibraryObjects(fw, lib);
				fw.WriteLine("          </div>");
				fw.WriteLine("        </div>");
			}
		}

		protected void TocLibraryObjects(StreamWriter fw, Library lib) {
			lib.SortLibraryObjects();
			foreach (LibraryObject obj in lib.SortedLibraryObjects) {
				fw.WriteLine("          <div class=\"treeNode\">");
				fw.WriteLine("            <img src=\"treenodedot.gif\" class=\"treeNoLinkImage\"/>");
				fw.WriteLine("            <a href=\"" + lib.Name + "/" + obj.Name + ".html\" target=\"main\" class=\"treeUnselected\" onclick=\"clickAnchor(this)\">" + obj.Name + "</a>");
				fw.WriteLine("          </div>");
			}
		}
		#endregion
	}
}
