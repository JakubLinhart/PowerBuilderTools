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
	public class ArgumentDoc : DocBase {
		private Argument parentArgument;
		private string description;

		public ArgumentDoc(Argument arg) : base (arg){
			this.parentArgument = arg;
		}

		public bool HasDescription {
			get { return (this.description.Length > 0); }
		}

		public string Description {
			get { return this.description; }
			set { this.description = value; }
		}

		public override bool IsEmpty() {
			return (description.Length <= 0);
		}
	}
}
