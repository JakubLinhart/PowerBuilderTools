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
using System.IO;
using System.Text;

namespace PowerDoc {
	public class PblReader : System.IO.BinaryReader {
		public PblReader(FileStream fs) : base(fs) {
		}

		public PblReader(string file) : base(new FileStream(file, FileMode.Open)) {
		}

		public long Position {
			get { return this.BaseStream.Position; }
		}

		public string ReadBlockName() {
			string name = Encoding.ASCII.GetString(this.ReadBytes(4));

			return name;
		}

		public string ReadASCIIString(int size) {
			string res;

			res = Encoding.ASCII.GetString(this.ReadBytes(size));
			int pos = res.IndexOf('\0');
			if (pos > 0) res = res.Substring(0, pos);

			return res;
		}

		public string ReadUnicodeString(int size) {
			string res;

			res = Encoding.Unicode.GetString(this.ReadBytes(size * 2));
			int pos = res.IndexOf('\0');
			if (pos > 0) res = res.Substring(0, pos);

			return res;
		}

		public DateTime ReadDateTime() {
			long timestamp = this.ReadInt32();
			System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
			dateTime = dateTime.AddSeconds(timestamp);
			return dateTime;
		}

		public long Seek(long offset, SeekOrigin origin) {
			return this.BaseStream.Seek(offset, origin);
		}

		public void Skip(long offset) {
			this.Seek(offset, SeekOrigin.Current);
		}
	}
}