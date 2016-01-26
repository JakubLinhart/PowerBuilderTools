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
	public class PowerScriptCompiler : CompilerBase {
		protected enum GlobalKeyWords {None, On, Forward, Type, End, Global, Variables, ForwardPrototypes, Public,
		Private, Protected, Readonly, Ref, Event, Function, Subroutine, TypePrototypes};

		static private ArrayList docAnalyzers = new ArrayList();

		private IPowerScriptHolder powerScriptHolder;
		
		public PowerScriptCompiler(IPowerScriptHolder holder) {
			this.powerScriptHolder = holder;
		}

		static public void AddAnalyzer(DocAnalyzer analyzer) {
			docAnalyzers.Add(analyzer);
		}

		protected override void Compile() {
			foreach (DocAnalyzer analyzer in docAnalyzers) {
				analyzer.ScriptHolder = powerScriptHolder;
			}

			GlobalTypeDefinition();
		}

		protected void GlobalTypeDefinition() {
			GlobalTypeDeclaration();

			while(!Eof()) {
				GlobalKeyWords word;
				SkipWhite();
				word = LookupKeyWords();
				
				switch (word) {
					case GlobalKeyWords.On :
						OnEvent();
						break;
					case GlobalKeyWords.Type :
						Type();
						break;
					case GlobalKeyWords.Global :
						Global();
						break;
					case GlobalKeyWords.ForwardPrototypes :
						ForwardPrototypes();
						break;
					case GlobalKeyWords.Public :
						AccessModifier(PowerObject.Access.Public);
						break;
					case GlobalKeyWords.Protected :
						AccessModifier(PowerObject.Access.Protected);
						break;
					case GlobalKeyWords.Private :
						AccessModifier(PowerObject.Access.Private);
						break;
					case GlobalKeyWords.Event :
						Event();
						break;
					default :
						throw new CompilerException("Nepovolene klicove slovo: " + word.ToString());
				}
				SkipWhite();
			}
		}

		protected void GlobalTypeDeclaration() {
			bool forwarded = false;
			string name;
			string super_class;

			if (MatchMayBe("forward")) {
				SkipWhite();
				Match("global type");
				SkipWhite();
				name = GetIdent();
				SkipWhite();
				Match("from");
				SkipWhite();
				super_class = GetIdent();
				SkipWhite();
				SkipLines("end type");
				SkipWhite();
				powerScriptHolder.CreateUnresolvedGlobalType(name, super_class);
				forwarded = true;
				SkipLines("end forward");
			}
			SkipWhite();

			if (MatchMayBe("global variables")) {
				// TODO: dodelat globalni promenne
				SkipLines("end variables");
				SkipWhite();
			}

			while (MatchMayBe("type ")) {
				SkipWhite();
				if (!forwarded) {
					// TODO: chyba, nezname rodicovskej deklarovanej ve forwarded!
				}
				Type();
				SkipWhite();
			}

			if (MatchMayBe("shared variables")) {
				SkipWhite();
				// TODO: zpracovani shared variable
				SkipLines("end variables");
				SkipWhite();
			}

			Match("global type");
			SkipWhite();
			name = GetIdent();
			SkipWhite();

			Match("from");
			SkipWhite();

			super_class = GetIdent();
			SkipWhite();
			bool autoinstant = false;
			if (MatchMayBe("autoinstantiate")) {
				autoinstant = true;
				SkipWhite();
			}

			SkipLines("end type");

			if (!forwarded) {
				powerScriptHolder.CreateUnresolvedGlobalType(name, super_class, autoinstant);
			} else {
			}

			SkipWhite();
		}

		protected void OnEvent() {
			string obj;
			string name;

			obj = GetIdent();
			if (MatchMayBe('.')) {
				name = GetIdent();
			} else {
				name = obj;
				Match(';');
			}
			SkipWhite();
			SkipLines("end on");
		}

		protected void Type() {
			GlobalKeyWords word = LookupKeyWords();
			switch (word) {
				case GlobalKeyWords.None :
					// local type
					string nestedParent;
					string obj = GetIdent();
					SkipWhite();

					Match("from");
					SkipWhite();

					string baseobj = GetIdent();
					if (MatchMayBe('`')) {
						nestedParent = GetIdent();
						baseobj += "." + nestedParent;
					}

					SkipWhite();

					if (MatchMayBe("within ")) {
						SkipWhite();
						string parent = GetIdent();
						SkipWhite();
						powerScriptHolder.CreateUnresolvedLocalType(obj, baseobj, parent);
					} else {
						powerScriptHolder.CreateUnresolvedLocalType(obj, baseobj);
					}

					SkipLines("end type");
					break;

				case GlobalKeyWords.Variables :
					SkipWhite();
					SkipLines("end variables");
					break;

				case GlobalKeyWords.TypePrototypes :
					SkipWhite();
					SkipLines("end prototypes");
					break;
			}
		}

		protected void Global() {
			GlobalKeyWords word = LookupKeyWords();
			switch (word) {
				case GlobalKeyWords.None :
					// jedna se o globalni promennou
					string type = GetIdent();
					SkipWhite();
					string name = GetIdent();
					SkipWhite();
					break;

				case GlobalKeyWords.Function :
					Function(PowerObject.Access.Global);
					break;

				case GlobalKeyWords.Subroutine :
					Subroutine(PowerObject.Access.Global);
					break;

				default :
					throw new CompilerException("neocekavane slovo: " + word.ToString());
			}
		}

		protected void ForwardPrototypes() {
			// TODO: provest kompilaci hlavicek funkci
			SkipLines("end prototypes");
			SkipWhite();
		}

		protected void AccessModifier(PowerObject.Access access) {
			SkipWhite();
			GlobalKeyWords word = LookupKeyWords();
			SkipWhite();
			switch (word) {
				case GlobalKeyWords.Function :
					Function(access);
					break;
				case GlobalKeyWords.Subroutine :
					Subroutine(access);
					break;
			}
		}

		protected void Function(PowerObject.Access access) {
			string type = GetIdent();
			SkipWhite();
			string name = GetIdent();
			SkipWhite();

			powerScriptHolder.CreateFunction(name, access, type);

			Match("("); SkipWhite();
			if (!Peek(")"))
				ParameterList();
			Match(")"); SkipWhite();

			if (MatchMayBe("throws")) {
				SkipWhite();
				string except = GetIdent();
				powerScriptHolder.SetThrows(except);
				SkipWhite();
			}

			Match(";");
			StartFunction();
			Script("end function");
			EndFunction();
			powerScriptHolder.FunctionFinished();
		}

		protected void Subroutine(PowerObject.Access access) {
			SkipWhite();
			string name = GetIdent();
			SkipWhite();
			powerScriptHolder.CreateFunction(name, access);
			Match('('); SkipWhite();
			if (!Peek(')')) ParameterList();
			Match(')'); SkipWhite();

			if (MatchMayBe("throws")) {
				SkipWhite();
				string except = GetIdent();
				powerScriptHolder.SetThrows(except);
				SkipWhite();
			}

			Match(';');
			StartFunction();
			Script("end subroutine");
			EndFunction();
			powerScriptHolder.FunctionFinished();
		}

		protected void ParameterList() {
			string type;
			string name;
			Argument.PassingType pass;

			do {
				SkipWhite();
				GlobalKeyWords word = LookupKeyWords();
				switch (word) {
					case GlobalKeyWords.None :
						pass = Argument.PassingType.Value;
						break;
					case GlobalKeyWords.Readonly :
						pass = Argument.PassingType.Readonly;
						SkipWhite();
						break;
					case GlobalKeyWords.Ref :
						pass = Argument.PassingType.Ref;
						SkipWhite();
						break;
					default :
						throw new CompilerException("Nepovolene klicove slovo: " + word.ToString());
				}
				SkipWhite();
				type = GetIdent();
				SkipWhite();
				name = GetIdent();
				if (MatchMayBe('[')) {
					// jedna se o pole
					string range;
					if (!Peek(']')) {
						range = GetNumber();
					}
					Match(']');
					name = name;
				}
				SkipWhite();
				powerScriptHolder.CreateArgument(name, type, pass);
			} while(MatchMayBe(','));
		}

		protected void Event() {
			string name;
			string type = null;

			SkipWhite();
			if (MatchMayBe("type ")) {
				type = GetIdent();
				SkipWhite();
			}
			name = GetIdent();

			string parent;
			if (MatchMayBe("::")) {
				parent = name;
				name = GetIdent();
				if (type != null)
					powerScriptHolder.CreateEventInParent(name, type, parent);
				else
					powerScriptHolder.CreateEventInParent(name, parent);
			} else {
				if (type != null)
					powerScriptHolder.CreateEvent(name, type);
				else
					powerScriptHolder.CreateEvent(name);
			}
			SkipWhite();

			if (MatchMayBe('(')) {
				SkipWhite();
				if (!Peek(')'))
					ParameterList();
				Match(')');
				SkipWhite();
			}
			MatchMayBe(';');
			StartEvent();
			Script("end event");
			EndEvent();
			SkipWhite();
			powerScriptHolder.EventFinished();
		}

		protected void Script(string terminator) {
			string line;

			SkipWhite();
			while (!MatchMayBe(terminator)) {
				line = GetLine();
				foreach (DocAnalyzer doc in docAnalyzers) {
					doc.AnalyzeLine(line);
				}
				Skip(line.Length);
				SkipWhite();
			}
		}

		protected void StartEvent() {
			foreach (DocAnalyzer doc in docAnalyzers)
				doc.StartEvent();
		}

		protected void EndEvent() {
			foreach (DocAnalyzer doc in docAnalyzers)
				doc.EndEvent();
		}

		protected void StartFunction() {
			foreach (DocAnalyzer doc in docAnalyzers)
				doc.StartFunction();
		}

		protected void EndFunction() {
			foreach (DocAnalyzer doc in docAnalyzers)
				doc.EndFunction();
		}

		protected GlobalKeyWords LookupKeyWords() {
			if (MatchMayBe("on ")) {
				return GlobalKeyWords.On;
			}

			if (MatchMayBe("type ")) {
				return GlobalKeyWords.Type;
			}

			if (MatchMayBe("event ")) {
				return GlobalKeyWords.Event;
			}

			if (MatchMayBe("global ")) {
				return GlobalKeyWords.Global;
			}

			if (MatchMayBe("variables")) {
				return GlobalKeyWords.Variables;
			}

			if (MatchMayBe("forward prototypes")) {
				return GlobalKeyWords.ForwardPrototypes;
			}

			if (MatchMayBe("public ")) {
				return GlobalKeyWords.Public;
			}

			if (MatchMayBe("private ")) {
				return GlobalKeyWords.Private;
			}

			if (MatchMayBe("protected ")) {
				return GlobalKeyWords.Protected;
			}

			if (MatchMayBe("ref ")) {
				return GlobalKeyWords.Ref;
			}

			if (MatchMayBe("readonly ")) {
				return GlobalKeyWords.Readonly;
			}

			if (MatchMayBe("function ")) {
				return GlobalKeyWords.Function;
			}

			if (MatchMayBe("subroutine ")) {
				return GlobalKeyWords.Subroutine;
			}

			if (MatchMayBe("prototypes")) {
				return GlobalKeyWords.TypePrototypes;
			}

			return GlobalKeyWords.None;
		}
	}
}