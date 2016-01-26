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

namespace PowerDoc {
	internal class PbtCompiler : ConfCompiler {
		private Target target;

		public PbtCompiler(Target target) {
			this.target = target;
		}

		protected override void Compile() {
			SaveFormat();
			SkipWhite();
			Projects();
			AppName();
			AppLib();
			LibList();
			Type();
		}

		protected void Projects() {
			if (MatchMayBe("@begin Projects")) {
				SkipWhite();
				SkipLines("@end;");
				SkipWhite();
			}
		}

		protected void AppName() {
			Match("appname");
			SkipWhite();
			string appname = GetString();
			Match(';');
			SkipWhite();

			target.SetApplicationObject(appname);
		}

		protected void AppLib() {
			Match("applib");
			SkipWhite();
			string applib = GetString();
			Match(';');
			SkipWhite();

			target.SetApplicationLibrary(applib);
		}

		protected void LibList() {
			Match("LibList");
			SkipWhite();
			string[] liblist = GetStringList();
			Match(';');
			SkipWhite();

			target.SetLibraryList(liblist);
		}

		protected void Type() {
			Match("type");
			SkipWhite();
			string type = GetString();
			Match(';');
			SkipWhite();
		}
	}
}