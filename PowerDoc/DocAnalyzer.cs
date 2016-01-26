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

namespace PowerDoc {
	public abstract class DocAnalyzer {
		protected enum DocSections  { None, Event, Function }

		protected IPowerScriptHolder powerScriptHolder;
		protected DocSections currentSection = DocSections.None;

		protected string returnsText;
		protected string descriptionText;
		protected string noteText;
		protected string historyText;
		protected ArrayList arguments = new ArrayList();
		private string currentArgument;
		protected string exampleText;
		protected ArrayList references = new ArrayList();
		protected string currentRefference;
		protected AbstractReferenceLinkResolver resolver = new ReferenceLinkResolver();

		public void ResolveReferenceLinks(Target target, bool cross) {
			resolver.ResolveRelatedLinks(target, cross);
		}

		public abstract void AnalyzeLine(string line);

		public IPowerScriptHolder ScriptHolder {
			set { powerScriptHolder = value; }
		}

		public virtual void StartEvent() {
			currentSection = DocSections.Event;
		}

		public virtual void EndEvent() {
			currentSection = DocSections.None;
		}

		public virtual void StartFunction() {
			currentSection = DocSections.Function;
		}

		public virtual void EndFunction() {
			currentSection = DocSections.None;
		}

		protected void FinishScript() {
			if (returnsText.Length > 0)
				powerScriptHolder.CreateDocReturns(returnsText);
			if (descriptionText.Length > 0)
				powerScriptHolder.CreateDocDescription(descriptionText);
			if (noteText.Length > 0)
				powerScriptHolder.CreateDocNote(noteText);
			if (historyText.Length > 0)
				powerScriptHolder.CreateDocHistory(historyText);
			if (exampleText.Length > 0)
				powerScriptHolder.CreateDocExample(exampleText);

			AddPotentialArgument();
			if (arguments.Count > 0)
				powerScriptHolder.CreateDocArguments(( string[] ) arguments.ToArray(typeof(string)));

			AddPotentialReference();
			if (references.Count > 0)
				powerScriptHolder.CreateDocReferences(( string[] ) references.ToArray(typeof(string)), resolver);
		}

		protected void ResetSections() {
			returnsText = "";
			descriptionText = "";
			noteText = "";
			historyText = "";
			exampleText = "";
			arguments.Clear();
			currentArgument = "";
			references.Clear();
			currentRefference = "";
		}

		protected void AddPotentialArgument() {
			if (currentArgument.Length > 0) {
				arguments.Add(currentArgument);
				currentArgument = "";
			}
		}

		protected void AppendToCurrentArgument(string str) {
			currentArgument += str;
		}

		protected void AddPotentialReference() {
			if (currentRefference.Length > 0) {
				references.Add(currentRefference);
				currentRefference = "";
			}
		}

		protected void AppendToCurrentReference(string str) {
			currentRefference += str;
		}
	}
}
