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
	public class Namespace {
		private static ArrayList workspaces = new ArrayList();
		private static Hashtable objects = new Hashtable();
		private static char[] nameSeparator = {'.'};

		public static IEnumerable Objects {
			get { return objects.Values; }
		}

		public static string RemoveAfter(string str, char ch) {
			int pos = str.IndexOf(ch);

			if (pos <= 0) return str;
			return str.Substring(0, pos);
		}

		public static string RemoveBefore(string str, char ch) {
			int pos = str.IndexOf(ch);

			if (pos <= 0) return str;
			return str.Substring(pos + 1, str.Length - pos - 1);
		}

		public static void AddWorkspace(Workspace workspace) {
			Namespace.workspaces.Add(workspace);
		}

		public static PowerObject GetObject(string name) {
			PowerObject obj = ( PowerObject ) objects[name.ToLower()];

			return obj;
		}

		public static void AddObject(PowerObject obj) {
			objects.Add(obj.Name, obj);
		}
	}
}