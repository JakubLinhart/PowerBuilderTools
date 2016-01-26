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

namespace PowerDoc {
	public interface IPowerScriptHolder {
		void CreateUnresolvedGlobalType(string name, string super);
		void CreateUnresolvedGlobalType(string name, string super, bool autoinstant);
		void CreateUnresolvedLocalType(string name, string super, string parent);
		void CreateUnresolvedLocalType(string name, string super);
		void SetThrows(string throws);
		void CreateGlobalType(string name, string super);
		void CreateEvent(string name);
		void CreateEvent(string name, string returns);
		void CreateEventInParent(string name, string parent);
		void CreateEventInParent(string name, string returns, string parent);
		void CreateFunction(string name, PowerObject.Access access);
		void CreateFunction(string name, PowerObject.Access access, string returns);
		void EventFinished();
		void FunctionFinished();
		void CreateArgument(string name, string type);
		void CreateArgument(string name, string type, Argument.PassingType pass);
		void CreateArgument(string name, string type, Argument.PassingType pass, bool is_array);

		void CreateDocDescription(string doc);
		void CreateDocNote(string doc);
		void CreateDocReturns(string doc);
		void CreateDocHistory(string doc);
		void CreateDocArguments(string[] doc);
		void CreateDocExample(string doc);
		void CreateDocReferences(string[] reference, AbstractReferenceLinkResolver resolver);
	}
}
