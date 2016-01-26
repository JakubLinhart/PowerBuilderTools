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
	public class Workspace : PBRoot {
		private string workspaceFileName;
		private string workingDirectory;
		private ArrayList targetNames = new ArrayList();

		public static Workspace Load(string pbw_name, string pbw_dir) {
			string workspace_name = Namespace.RemoveAfter(pbw_name, '.');
			Workspace workspace = new Workspace(workspace_name);
			workspace.workingDirectory = pbw_dir;
			workspace.Open(Path.Combine(pbw_dir, pbw_name));
			return workspace;
		}

		public Workspace(string name) : base(null, name) {
			Namespace.AddWorkspace(this);
		}

		public new PBRoot Parent {
			get { throw new Exception("Workspace nema zadnou nadrizenou polozku."); }
		}

		// TODO: todle je fakt strasny, workspace nesmi byt nadrizenym objektem Target, target muze byt ale nemusi
		// byt obsazen ve workspace
		public Target MainTarget {
			get { return ( Target ) childList[0]; }
		}

		public void AddTarget(Target target) {
			this.childList.Add(target);
		}

		internal void AddTarget(string name) {
			string target_name;
			int startpos, endpos;

			startpos = name.LastIndexOf('\\');
			endpos = name.LastIndexOf('.');
			if (startpos > 0 && endpos > 0 && startpos < endpos) {
				target_name = name.Substring(startpos + 1, name.Length - endpos + 1);
			} else {
				if (endpos > 0) {
					target_name = name.Substring(0, endpos);
				} else {
					target_name = name;
				}
			}

			Target target = Target.Load(this, target_name, Path.Combine(this.workingDirectory, name));
		}

		public override void Compile() {
			foreach (Target target in childList) {
				Trace.WriteLine("Kompiluji target: " + target.Name);
				target.Compile();
			}
		}

		public void Open(string file_name) {
			PbwCompiler comp = new PbwCompiler(this);
			workspaceFileName = file_name;
			comp.Compile(file_name);
		}

		public void Close() {
		}

		public string GetFullFileName(string name) {
			return Path.Combine(this.workingDirectory, name);
		}
	}
}
