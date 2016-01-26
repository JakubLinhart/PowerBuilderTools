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
	internal class ScriptFunctionsEnumerator : IEnumerator {
		private IEnumerator hashEnumerator;
		private IEnumerator listEnumerator;
		private Hashtable functions;

		internal ScriptFunctionsEnumerator(Hashtable functions) {
			this.functions = functions;

			hashEnumerator = functions.Values.GetEnumerator();
		}

		public void Reset() {
			hashEnumerator.Reset();
			listEnumerator = null;
		}

		public object Current {
			get {
				if (listEnumerator == null) return null;

				return listEnumerator.Current;
			}
		}

		public bool MoveNext() {
			if (listEnumerator == null || !listEnumerator.MoveNext()) {
				if (!hashEnumerator.MoveNext()) return false;
				listEnumerator = (( ArrayList ) hashEnumerator.Current).GetEnumerator();
				listEnumerator.MoveNext();
			}

			return true;
		}
	}


	internal class ScriptFunctionsEnumerable : IEnumerable {
		private Hashtable functions;

		internal ScriptFunctionsEnumerable(Hashtable functions) {
			this.functions = functions;
		}

		public IEnumerator GetEnumerator() {
			return new ScriptFunctionsEnumerator(this.functions);
		}
	}

}