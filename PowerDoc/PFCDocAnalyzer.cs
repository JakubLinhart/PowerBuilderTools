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
using System.Collections;

namespace PowerDoc {
	public class PFCDocAnalyzer : DocAnalyzer {
		private enum PFCSection {Skip, Arguments, Returns, Description, History, Note, Finished}

		private PFCSection pfcSection = PFCSection.Skip;

		public override void AnalyzeLine(string line) {
			if (pfcSection == PFCSection.Finished) return;
			string trimed = line.TrimStart();

			if (trimed.StartsWith("////")) {
				switch (pfcSection) {
					case PFCSection.History :
						pfcSection = PFCSection.Finished;
						break;
					default :
						pfcSection = PFCSection.Skip;
						break;
				}
			} else if (trimed.StartsWith("//\tPublic Function:")) {
				pfcSection = PFCSection.Skip;
			} else if (trimed.StartsWith("//\tArguments:")) {
				Argument(line);
			} else if (trimed.StartsWith("//\tReturns:")) {
				Returns(line);
			} else if (trimed.StartsWith("//\tDescription:")) {
				Description(line);
			} else if (IsNote(line)) {
				Note(line);
			} else if (IsHistory(line)) {
				History(line);
			} else if (trimed.StartsWith("// PowerBuilder Foundation Classes (PFC)")) {
				pfcSection = PFCSection.Finished;
			} else if (trimed.StartsWith("//")) {
				switch (pfcSection) {
					case PFCSection.Returns :
						Returns(line);
						break;
					case PFCSection.Description :
						Description(line);
						break;
					case PFCSection.History :
						History(line);
						break;
					case PFCSection.Note :
						Note(line);
						break;
					case PFCSection.Arguments :
						Argument(line);
						break;
				}
			}
		}

		public override void StartEvent() {
			base.StartEvent ();
			pfcSection = PFCSection.Skip;
			ResetSections();
		}

		public override void EndEvent() {
			base.EndEvent ();
			FinishScript();
		}

		public override void StartFunction() {
			base.StartFunction ();
			pfcSection = PFCSection.Skip;
			ResetSections();
		}

		public override void EndFunction() {
			base.EndFunction ();
			FinishScript();
		}

		protected void Returns(string line) {
			if (pfcSection != PFCSection.Returns) {
				pfcSection = PFCSection.Returns;
				return;
			}

			line = StripComments(line);
			if (line.Length > 0)
				returnsText += line + '\n';
		}

		protected void Description(string line) {
			if (pfcSection != PFCSection.Description) {
				pfcSection = PFCSection.Description;
				line = line.Substring(15).Trim();
			} else {
				line = StripComments(line);
			}

			if (line.Length > 0)
				descriptionText += line + ' ';
		}

		protected bool IsNote(string line) {
			if (line.IndexOf("Note:") > 0) return true;

			return false;
		}

		protected void Note(string line) {
			if (pfcSection != PFCSection.Note) {
				int pos = line.IndexOf("Note:");
				if (pos >= 0) {
					line = line.Substring(pos + 5).Trim();
				} else {
					// TODO: ohlasit chybu
				}
				pfcSection = PFCSection.Note;
			} else {
				line = StripComments(line);
			}

			line = StripComments(line);
			if (line.Length > 0)
				noteText += line + ' ';
		}

		protected void History(string line) {
			if (pfcSection != PFCSection.History) {
				pfcSection = PFCSection.History;
				return;
			}

			line = StripComments(line);
			if (line.Length > 0) {
				if (char.IsNumber(line, 0)) {
					if (historyText.Length > 0)
						historyText += "<br />" + line;
					else
						historyText = line;
				} else {
					historyText += line;
				}
			}
		}

		protected bool IsHistory(string line) {
			if (line.IndexOf("Rev. History") > 0) return true;
			if (line.IndexOf("//\tVersion") >= 0) return true;

			return false;
		}

		protected void Argument(string line) {
			if (pfcSection != PFCSection.Arguments) {
				pfcSection = PFCSection.Arguments;
				return;
			}

			if (line.StartsWith("//\t")) {
				int i;
				bool newarg = false;
				for (i = 3; i < line.Length; i++) {
					if (char.IsLetter(line, i)) {
						newarg = true;
						break;
					}
				}

				int pos = 0;
				if (newarg) {
					pos = line.IndexOf('\t', i);
					if (pos <= 0) newarg = false;
				}
				
				string striped = StripComments(line);
				if (newarg) {
					striped = line.Substring(pos).Trim();
					AddPotentialArgument();
					AppendToCurrentArgument(striped + ' ');
				} else {
					AppendToCurrentArgument(striped + ' ');
				}

/*				
				if (new_arg) {
					AddPotentialArgument();
				}
				AppendToCurrentArgument(striped);*/
			}
		}

		protected string StripComments(string line) {
			int pos = line.IndexOf("//");
			if (pos >= 0) {
				line = line.Substring(pos + 2, line.Length - pos - 2);
			}
			
			return line.Trim();
		}
	}
}