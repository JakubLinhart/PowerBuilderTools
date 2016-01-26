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
	public class KTKDocAnalyzer : DocAnalyzer {
		private enum KTKSections { None, Description, Note, Arguments, Returns, History, Ref, Example };

		private const string DescriptionTag = "popis:";
		private const string NameTag = "nazev:";
		private const string ArgumentsTag = "parametry:";
		private const string ReturnsTag = "navratova hodnota:";
		private const string ExampleTag = "priklad:";
		private const string HistoryTag = "v:";
		private const string RefTag = "ref:";
		private const string NoteTag = "poznamka:";

		private KTKSections section = KTKSections.None;

		public KTKDocAnalyzer() {
			this.resolver = new KTKReferenceLinkResolver();
		}

		public override void AnalyzeLine(string line) {
			int pos = line.IndexOf("///");
			if (pos < 0 || pos + 3 == line.Length) return;
			if (line[pos + 3] == '/') return;

			string striped = StripDocComment(line).Trim();
			if (striped.StartsWith(DescriptionTag)) {
				Description(striped);
			} else if (striped.StartsWith(NameTag)) {
				Name(striped);
			} else if (striped.StartsWith(ArgumentsTag)) {
				Arguments(striped);
			} else if (striped.StartsWith(ReturnsTag)) {
				Returns(striped);
			} else if (striped.StartsWith(NoteTag)) {
				Note(striped);
			} else if (striped.StartsWith(ExampleTag)) {
				Example(striped, line);
			} else if (striped.StartsWith(HistoryTag)) {
				History(striped, true);
			} else if (striped.StartsWith(RefTag)) {
				Ref(striped);
			} else {
				// TODO: otestovat, zda radek nezacina oznacenim nejake sekce, ktera neexistuje
				switch (section) {
					case KTKSections.Description :
						Description(striped);
						break;
					case KTKSections.Note :
						Note(striped);
						break;
					case KTKSections.Arguments :
						Arguments(striped);
						break;
					case KTKSections.Returns :
						Returns(striped);
						break;
					case KTKSections.History :
						History(striped, false);
						break;
					case KTKSections.Ref :
						Ref(striped);
						break;
					case KTKSections.Example :
						Example(striped, line);
						break;
				}
			}
		}

		private string StripDocComment(string line) {
			int pos = line.IndexOf("///");
			if (pos < 0) return line;
			return line.Substring(pos + 3);
		}

		private void Description(string line) {
			if (section != KTKSections.Description) {
				line = line.Substring(DescriptionTag.Length).Trim();
				section = KTKSections.Description;
			}

			if (line.Length > 0)
				descriptionText += line + ' ';
		}

		private void Name(string line) {
			// TODO: otestovat, zda se shoduje s dokumentovanym eventem/funkci
		}

		private void Arguments(string line) {
			if (section != KTKSections.Arguments) {
				line = line.Substring(ArgumentsTag.Length).Trim();
				section = KTKSections.Arguments;
			}

			int pos = line.IndexOf("--");
			if (pos > 0) {
				AddPotentialArgument();
				line = line.Substring(pos + 2).Trim();
			}

			if (line.Length <= 0) return;

			AppendToCurrentArgument(line + ' ');
		}

		private void Returns(string line) {
			if (section != KTKSections.Returns) {
				line = line.Substring(ReturnsTag.Length).Trim();
				section = KTKSections.Returns;
			}

			if (line.Length <= 0) return;

			returnsText += line;
		}

		private void Note(string line) {
			if (section != KTKSections.Note) {
				line = line.Substring(NoteTag.Length).Trim();
				section = KTKSections.Note;
			}

			if (line.Length <= 0) return;

			noteText += line + ' ';
		}

		private void Example(string striped, string line) {
			if (section != KTKSections.Example) {
				line = striped.Substring(ExampleTag.Length).Trim();
				section = KTKSections.Example;
			} else {
				line = StripDocComment(line);
			}

			if (line.Length <= 0) return;

			exampleText += line + '\n';
		}

		private void Ref(string line) {
			if (section != KTKSections.Ref) {
				line = line.Substring(RefTag.Length).Trim();
				section = KTKSections.Ref;
			} else {
				line = StripDocComment(line);
			}

			int start, i;

			start = 0;
			i = 0;
			bool seek_start = false;
			string link;

			while (i < line.Length) {
				if (!char.IsLetterOrDigit(line[i]) && line[i] != '.' && line[i] != '_') {
					if (!seek_start) {
						seek_start = true;
						link = line.Substring(start, i - start).Trim();
						AppendToCurrentReference(link);
						AddPotentialReference();
					}
				} else {
					if (seek_start) {
						seek_start = false;
						start = i;
					}
				}
				i++;
			}

			if (!seek_start) {
				link = line.Substring(start, i - start).Trim();
				AppendToCurrentReference(link);
				AddPotentialReference();
			}
		}

		private void History(string line, bool new_version) {
			bool break_line = false;

			if (section != KTKSections.History) {
				line = line.Substring(HistoryTag.Length).Trim();
				section = KTKSections.History;
			} else
				break_line = new_version;

			if (line.Length <= 0) return;

			if (break_line)
				historyText += "<br />";
			historyText += line;
			if (new_version)
				historyText += "<br />";
		}

		public override void StartEvent() {
			base.StartEvent ();
			ResetSections();
		}

		public override void EndEvent() {
			base.EndEvent ();
			FinishScript();
		}

		public override void StartFunction() {
			base.StartFunction ();
			ResetSections();
		}

		public override void EndFunction() {
			base.EndFunction ();
			FinishScript();
		}

	}
}