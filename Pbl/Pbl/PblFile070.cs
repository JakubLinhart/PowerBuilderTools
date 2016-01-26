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
using System.Collections;
using System.Text;

namespace PowerDoc {
	public sealed class PblFile070 : PblFile {
		internal static readonly string BLOCK_NAME_HDR = "HDR*";
		internal static readonly string BLOCK_NAME_NOD = "NOD*";
		internal static readonly string BLOCK_NAME_ENT = "ENT*";
		internal static readonly string VERSION_SIGANTURE = "PowerBuilder";

		internal static readonly int LIBRARY_COMMENTS_SIZE = 511;
		internal static readonly int OFFSET_FIRST_NOD = 0x400;
		internal static readonly int OFFSET_FIRST_ENT = 28;

		private static Encoding EncCP1250 = Encoding.GetEncoding(1250);

		public PblFile070(string file_name) : base(file_name) {
		}

		public override bool Unicode {
			get { return false; }
		}

		public override string LoadEntrySyntax(string name) {
			return EncCP1250.GetString(LoadEntry(name));
		}

		public override byte[] LoadEntry(string name) {
			PblEntry entry = this.entries[name] as PblEntry;
			if (entry == null)
				throw new PblFileException("Entry " + name + " was not found.");

			if (entry.data != null)
				return entry.data;

			entry.data = new byte[entry.size - entry.commentsLength];
			long offset = entry.dataOffset;
			int size = 0;
			int index = 0;

			byte[] buff = new byte[512];
			// the first block may contain a comment
			if (entry.commentsLength > 0) {
				offset = LoadBlockData(buff, index, offset, ref size);
				entry.comments = EncCP1250.GetString(buff, 0, entry.commentsLength);
				Array.Copy(buff, entry.commentsLength, entry.data, 0, size - entry.commentsLength);
				index += (size - entry.commentsLength);
			}

			while (offset != 0) {
				offset = LoadBlockData(entry.data, index, offset, ref size);
				index += size;
			}

			return entry.data;
		}

		public override void LoadDirectory() {
			LoadLibraryHeader();
			LoadNodes();
			LoadEntries();
		}

		private void LoadLibraryHeader() {
			reader.Seek(0, SeekOrigin.Begin);
			string hdr = reader.ReadBlockName();
			if (hdr != PblFile070.BLOCK_NAME_HDR)
				throw new PblFileException("Wrong PBL signature. Signature " + PblFile070.BLOCK_NAME_HDR + " expected.");

			// signatura: "PowerBuilder\0\0"
			hdr = reader.ReadASCIIString(VERSION_SIGANTURE.Length);
			if (hdr != PblFile070.VERSION_SIGANTURE)
				throw new PblFileException("Wrong PBL signature. Signature " + PblFile070.VERSION_SIGANTURE + " expected");

			reader.Skip(2);

			string version = reader.ReadASCIIString(4);
			DateTime dt = reader.ReadDateTime();
			reader.Skip(2);
			this.comments = reader.ReadASCIIString(LIBRARY_COMMENTS_SIZE);
		}

		private void LoadNodes() {
			ArrayList nods = new ArrayList();

			PblNod root = LoadNod(PblFile070.OFFSET_FIRST_NOD);
			root.parentNod = null;
			LoadNodes(nods, root, root.offset);

			this.nods = ( PblNod[] ) nods.ToArray(System.Type.GetType("PowerDoc.PblNod"));
		}

		private PblNod LoadNodes(ArrayList nods, PblNod root, long offset) {
			PblNod nod = LoadNod(offset);
			nod.parentNod = root;
			nods.Add(nod);
			if (nod.nextOffset != 0)
				nod.leftNod = LoadNodes(nods, nod, nod.nextOffset);
			if (nod.prevOffset != 0)
				nod.rightNod = LoadNodes(nods, nod, nod.prevOffset);

			return nod;
		}

		private PblNod LoadNod(long offset) {
			if (offset > reader.BaseStream.Length)
                throw new PblFileException("PBL corrupted. NOD section was not found.");

			reader.Seek(offset, SeekOrigin.Begin);

			// kontrola, zda se opravdu jedna o NOD sekci
			string hdr = reader.ReadBlockName();
			if (hdr != PblFile070.BLOCK_NAME_NOD)
                throw new PblFileException("PBL corrupted. ENT section was not found.");

			int offset_previous = reader.ReadInt32(); // predchozi NOD
			int offset_parent = reader.ReadInt32(); // parent NOD ???
			int offset_next = reader.ReadInt32(); // nasledujici NOD
			int unknown = reader.ReadInt32(); // ???????
			short entries_count = reader.ReadInt16(); // pocet ENT (entries) patricich tomuto NOD

			PblNod nod = new PblNod();
			nod.nextOffset = offset_next;
			nod.prevOffset = offset_previous;
			nod.parentOffset = offset_parent;
			nod.entCount = entries_count;
			nod.offset = offset;
			nod.entries = new PblEntry[entries_count];

			return nod;
		}

		private void LoadEntries() {
			foreach (PblNod nod in this.nods) {
				LoadEntries(nod);
			}
		}

		private void LoadEntries(PblNod nod) {
			reader.Seek(nod.offset, SeekOrigin.Begin);
			string hdr = reader.ReadBlockName();
			if (hdr != PblFile070.BLOCK_NAME_NOD)
				throw new PblFileException("Chyba v souboru PBL knihovny. Nebyla nalezena NOD sekce.");

			reader.Seek(OFFSET_FIRST_ENT, SeekOrigin.Current);

			for (int i = 0; i < nod.entCount; i++) {
				long offset = reader.Position;
				hdr = reader.ReadBlockName();
				if (hdr != PblFile070.BLOCK_NAME_ENT)
					throw new PblFileException("Chyba v souboru PBL knihovny. Nebyla nalezena ENT sekce.");

				string unknown = reader.ReadASCIIString(4);
				long dat_offset = reader.ReadInt32();
				int entry_size = reader.ReadInt32();
				DateTime dt = reader.ReadDateTime();
				short comments_length = reader.ReadInt16();
				short name_length = reader.ReadInt16();
				string name = reader.ReadASCIIString(name_length);

				PblEntry ent = new PblEntry();
				ent.offset = offset;
				ent.dataOffset = dat_offset;
				ent.size = entry_size;
				ent.time = dt;
				ent.commentsLength = comments_length;
				ent.name = name;
				nod.entries[i] = ent;
				this.entries.Add(ent.name, ent);
			}
		}

		private long LoadBlockData(byte[] buff, int index, long offset, ref int size) {
			reader.Seek(offset, SeekOrigin.Begin);
			string hdr = reader.ReadBlockName();

			long next_offset = reader.ReadInt32();
			size = reader.ReadInt16();

			reader.Read(buff, index, size);

			return next_offset;
		}
	}
}
