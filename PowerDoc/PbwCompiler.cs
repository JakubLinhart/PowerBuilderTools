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
	internal class PbwCompiler : ConfCompiler {
		private Workspace workspace;

		public PbwCompiler(Workspace workspace) {
			this.workspace = workspace;
		}

		protected override void Compile() {
			SaveFormat();
			SkipWhite();
			do {
				Section();
				SkipWhite();
			} while (!Eof());
		}

		protected void Section() {
			string ident;

			if (MatchMayBe("@begin")) {
				SkipWhite();
				ident = GetIdent();
				SkipWhite();
				switch (ident) {
					case "Unchecked" :
						Unchecked();
						break;
					case "Targets" :
						Targets();
						break;
					default :
						throw new PbwException("Neznama konfiguracni sekce.");
				}
				SkipWhite();
				Match("@end;");
			} else {
				ident = GetIdent();
				SkipWhite();
				switch (ident) {
					case "DefaultTarget" :
						GetString();
						break;
					case "DefaultRemoteTarget" :
						GetString();
						break;
					default :
						throw new CompilerException("Neocekavany parametr: " + ident);
				}
				SkipWhite();
				Match(';');
				SkipWhite();
			}
		}

		protected void Unchecked() {
			// zatim mam tutu sekci v pbw souboru prazdnou
		}

		protected void Targets() {
			string num;
			string target;

			do {
				num = GetNumber();
				SkipWhite();
				target = GetString();
				workspace.AddTarget(target);
				SkipWhite();
				Match(';');
				SkipWhite();
			} while (!Peek("@end;"));
		}
	}
}