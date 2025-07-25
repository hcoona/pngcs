// Pngcs is a CSharp implementation of PNG binary reader and writer.
// Copyright (C) 2025 Shuai Zhang <zhangshuai.ustc@gmail.com>
//
// Based on original work:
//   Copyright 2012    Hernán J. González    hgonzalez@gmail.com
//   Licensed under the Apache License, Version 2.0
//
//   You should have received a copy of the Apache License 2.0
//   along with the program.
//   If not, see <http://www.apache.org/licenses/LICENSE-2.0>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

namespace Hjg.Pngcs.Chunks
{

    using System;
    using Hjg.Pngcs;

    /// <summary>
    /// PLTE Palette chunk: this is the only optional critical chunk
    ///
    /// http://www.w3.org/TR/PNG/#11PLTE
    /// </summary>
    public class PngChunkPLTE : PngChunkSingle
    {
        public const string ID = ChunkHelper.PLTE;

        private int nentries = 0;

        /// <summary>
        ///
        /// </summary>
        private int[] entries;

        public PngChunkPLTE(ImageInfo info)
            : base(ID, info)
        {
            this.nentries = 0;
        }


        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.NA;
        }

        public override ChunkRaw CreateRawChunk()
        {
            int len = 3 * nentries;
            int[] rgb = new int[3];
            ChunkRaw c = createEmptyChunk(len, true);
            for (int n = 0, i = 0; n < nentries; n++)
            {
                GetEntryRgb(n, rgb);
                c.Data[i++] = (byte)rgb[0];
                c.Data[i++] = (byte)rgb[1];
                c.Data[i++] = (byte)rgb[2];
            }
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            SetNentries(c.Len / 3);
            for (int n = 0, i = 0; n < nentries; n++)
            {
                SetEntry(n, (int)(c.Data[i++] & 0xff), (int)(c.Data[i++] & 0xff),
                        (int)(c.Data[i++] & 0xff));
            }
        }

        public override void CloneDataFromRead(PngChunk other)
        {
            PngChunkPLTE otherx = (PngChunkPLTE)other;
            this.SetNentries(otherx.GetNentries());
            Array.Copy((Array)(otherx.entries), 0, (Array)(entries), 0, nentries);
        }

        /// <summary>
        /// Also allocates array
        /// </summary>
        /// <param name="nentries">1-256</param>
        public void SetNentries(int nentries)
        {
            this.nentries = nentries;
            if (nentries < 1 || nentries > 256)
                throw new PngjException("invalid pallette - nentries=" + nentries);
            if (entries == null || entries.Length != nentries)
            { // alloc
                entries = new int[nentries];
            }
        }

        public int GetNentries()
        {
            return nentries;
        }

        public void SetEntry(int n, int r, int g, int b)
        {
            entries[n] = ((r << 16) | (g << 8) | b);
        }

        /// <summary>
        /// as packed RGB8
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int GetEntry(int n)
        {
            return entries[n];
        }


        /// <summary>
        /// Gets n'th entry, filling 3 positions of given array, at given offset
        /// </summary>
        /// <param name="index"></param>
        /// <param name="rgb"></param>
        /// <param name="offset"></param>
        public void GetEntryRgb(int index, int[] rgb, int offset)
        {
            int v = entries[index];
            rgb[offset] = ((v & 0xff0000) >> 16);
            rgb[offset + 1] = ((v & 0xff00) >> 8);
            rgb[offset + 2] = (v & 0xff);
        }

        /// <summary>
        /// shortcut: GetEntryRgb(index, int[] rgb, 0)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="rgb"></param>
        public void GetEntryRgb(int n, int[] rgb)
        {
            GetEntryRgb(n, rgb, 0);
        }

        /// <summary>
        /// minimum allowed bit depth, given palette size
        /// </summary>
        /// <returns>1-2-4-8</returns>
        public int MinBitDepth()
        {
            if (nentries <= 2)
                return 1;
            else if (nentries <= 4)
                return 2;
            else if (nentries <= 16)
                return 4;
            else
                return 8;
        }
    }

}
