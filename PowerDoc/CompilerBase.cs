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

namespace  PowerDoc {
	public abstract class CompilerBase {
		private string content;
		private int currentPos = 0;

		protected int CurrentPosition {
			get { return currentPos; }
		}

		protected char CurrentChar {
			get { return content[currentPos]; }
		}

		public void CompileSource(string source) {
			content = source;
			Compile();
		}

		protected abstract void Compile();

		protected void LoadTextFile(string file_name) {
			StreamReader reader = new StreamReader(file_name);
			char[] content = new char[reader.BaseStream.Length];
			reader.Read(content, 0, ( int ) reader.BaseStream.Length);

			this.content = new string(content);
		}

		protected string GetIdent() {
			int start = CurrentPosition;

			if (!IsAlpha()) {
				throw new PbwException("Identifikator musi zacinat pismenem.");
			}
			Skip();

			while (IsAlphaNum() || Peek('_') || Peek('-')) {
				Skip();
			}

			return SubString(start, CurrentPosition);
		}
	
		protected string GetNumber() {
			if (!IsNumber()) {
				throw new PbwException("Je ocekavano cislo.");
			}

			int start = CurrentPosition;
			do {
				Skip();
			} while (IsNumber());

			return SubString(start, CurrentPosition);
		}

		protected string GetNumber(int count) {
			int start = CurrentPosition;

			for (int i = 0; i < count; i++) {
				if (!IsNumber()) {
					throw new PbwException("Je ocekavano " + count.ToString() + " cislic, ale nalezeno bylo pouze " + i.ToString());
				}
				Skip();
			}

			return SubString(start, start + count);
		}
		protected bool Peek(string str) {
			return (content.Substring(CurrentPosition)).StartsWith(str);
		}

		protected bool Peek(char ch) {
			return (content[CurrentPosition] == ch);
		}

		protected void Match(char ch) {
			if (char.ToLower(content[CurrentPosition]) != char.ToLower(ch)) {
				throw new PbwException("Je ocekavan znak '" + ch + "'");
			}

			Skip();
		}

		protected void Match(string str) {
			if (!content.Substring(CurrentPosition).ToLower().StartsWith(str.ToLower())) {
				throw new PbwException("Je ocekavan retezec '" + str + "'");
			}

			Skip(str.Length);
		}

		protected bool MatchMayBe(string str) {
			if (str.Length + CurrentPosition > content.Length)
				return false;
			if (content.Substring(CurrentPosition, str.Length).ToLower() != str.ToLower())
				return false;
			Skip(str.Length);

			return true;
		}

		protected bool MatchMayBe(char ch) {
			if (this.CurrentChar != ch) return false;
			Skip();

			return true;
		}

		protected void SkipWhite() {
			while (!Eof() && char.IsWhiteSpace(CurrentChar)) {
				Skip();
			}
		}

		protected bool Eol() {
			if (CurrentChar == '\n')
				return true;

			return false;
		}

		protected bool Eof() {
			if (currentPos < content.Length && CurrentChar != '\0') return false;

			return true;
		}

		protected bool IsAlpha() {
			return char.IsLetter(content, CurrentPosition);
		}

		protected bool IsAlphaNum() {
			return char.IsLetterOrDigit(content, CurrentPosition);
		}

		protected bool IsNumber() {
			return char.IsNumber(content, CurrentPosition);
		}

		protected void Skip() {
			currentPos++;
		}

		protected void Skip(int count) {
			currentPos += count;
		}

		protected void SkipLine() {
			SkipWhite();
			do {
				Skip();
			} while(!Eol());
			SkipWhite();
		}

		protected void SkipLines(string terminator) {
			while(!MatchMayBe(terminator)) {
				SkipLine();
			} 
		}

		protected string GetLine() {
			int pos;

			pos = content.IndexOf('\n', currentPos);
			if (pos > 0) return SubString(currentPos, pos);

			return content.Substring(currentPos);
		}

		protected void ResetCurrentPosition() {
			currentPos = 0;
		}

		protected string SubString(int start, int end) {
			return content.Substring(start, end - start);
		}
	}
}