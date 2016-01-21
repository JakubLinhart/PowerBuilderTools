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
using System.IO;

namespace PowerDoc {
	public class PblNod {
		internal long offset;
		internal long nextOffset;
		internal long prevOffset;
		internal long parentOffset;
		internal short entCount;

		internal PblNod leftNod;
		internal PblNod rightNod;
		internal PblNod parentNod;

		internal PblEntry[] entries;
	}

	public class PblEntry {
		internal long offset;
		internal long dataOffset;
		internal int size;
		internal DateTime time;
		internal string comments;
		internal int commentsLength;
		internal string name;

		internal byte[] data = null;

		public string Name {
			get { return this.name; }
		}

		public string Comments {
			get { return this.comments; }
		}

		public int Size {
			get { return this.size; }
		}
	}

	public abstract class PblFile {
		public enum PblVersion {Unknown, PBL070, PBL105 };
		private static long IDENTIFICATION_OFFSET = 4;

		protected PblReader reader;
		protected bool unicode = true;

		protected PblNod[] nods;
		protected Hashtable entries = new Hashtable();
		protected string fileName;
		protected string comments;

		public string Comments {
			get { return this.comments; }
		}

		public static PblFile OpenPbl(string name) {
			FileStream f = new FileStream(name, FileMode.Open);
			PblFile pbl_file = null;
			PblVersion version = PblVersion.Unknown;

			try {
				f.Seek(IDENTIFICATION_OFFSET, SeekOrigin.Begin);
				int b = f.ReadByte();
				if (b == 0x50) {
					b = f.ReadByte();
					if (b == 0x00) {
						version = PblVersion.PBL105;
					} else if (b == 0x6F) {
						version = PblVersion.PBL070;
					}
				}
			} finally {
				f.Close();
			}

			switch (version) {
				case PblVersion.PBL070 :
					pbl_file = new PblFile070(name);
					break;
				case PblVersion.PBL105 :
					pbl_file = new PblFile105(name);
					break;
				default :
					throw new PblFileException("Neznámý typ souboru PBL knihovny: " + name);
			}

			return pbl_file;
		}

		public PblFile(string file_name) {
			this.reader = new PblReader(file_name);
			this.fileName = file_name;
		}

		public virtual bool Unicode {
			get { return true; }
		}

		public IEnumerable Entries {
			get { return entries.Values; }
		}

		public string FileName {
			get { return this.fileName; }
		}

		public virtual void Close() {
			reader.Close();
		}

		public abstract byte[] LoadEntry(string name);
		public abstract string LoadEntrySyntax(string name);
		public abstract void LoadDirectory();
	}
}