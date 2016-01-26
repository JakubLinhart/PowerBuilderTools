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

namespace PowerDoc {
	public abstract class ConfCompiler : CompilerBase {

		public void Compile(string file_name) {
			LoadTextFile(file_name);
			ResetCurrentPosition();
			Compile();
		}

		protected void BuildDate() {
			string year = GetNumber(4);
			string month = GetNumber(2);
			string day = GetNumber(2);
		}

		protected void SaveFormat() {
			Match("Save Format");
			SkipWhite();
			Version();
			SkipWhite();
			Match('(');
			SkipWhite();
			BuildDate();
			SkipWhite();
			Match(')');
		}

		protected void Version() {
			Match('v');
			string ver = GetVersionNumber();
		}

		protected string GetVersionNumber() {
			bool done;
			int start = CurrentPosition;

			do {
				if (!IsNumber()) {
					throw new PbwException("Cislo verze (podverze) musi zacinat cislici.");
				}

			while (IsNumber()) {
				Skip();
			}

				if (Peek('.')) {
					done = false;
					Skip();
				} else {
					done = true;
				}
			} while(!done);
			
			return SubString(start, CurrentPosition);
		}

		protected string[] GetStringList() {
			bool done = false;
			ArrayList list = new ArrayList();
			Match('"');

			while (!done) {
				int start = CurrentPosition;
				while (!Peek(';')) {
					if (Peek('"')) {
						done = true;
						break;
					}
					Skip();
				}
				string element = SubString(start, CurrentPosition);
				list.Add(element);
				if (!done) {
					Skip();
				}
			}
			Match('"');

			return (string[]) list.ToArray(System.Type.GetType("System.String"));
		}

		protected string GetString() {
			Match('\"');
			int start = CurrentPosition;
			do {
				Skip();
			} while(!Peek('\"'));
			Skip();
			return SubString(start, CurrentPosition - 1);
		}
	}
}