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
    using System.IO;
    using Hjg.Pngcs;

    /// <summary>
    /// sPLT chunk: http://www.w3.org/TR/PNG/#11sPLT
    /// </summary>
    public class PngChunkSPLT : PngChunkMultiple
    {
        public const string ID = ChunkHelper.sPLT;
        /// <summary>
        /// Must be unique in image
        /// </summary>
        public string PalName { get; set; }
        /// <summary>
        /// 8-16
        /// </summary>
        public int SampleDepth { get; set; }
        /// <summary>
        /// 5 elements per entry
        /// </summary>
        public int[] Palette { get; set; }

        public PngChunkSPLT(ImageInfo info)
            : base(ID, info)
        {
            PalName = "";
        }


        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.BEFORE_IDAT;
        }
        public override ChunkRaw CreateRawChunk()
        {
            MemoryStream ba = new MemoryStream();
            ChunkHelper.WriteBytesToStream(ba, ChunkHelper.ToBytes(PalName));
            ba.WriteByte(0); // separator
            ba.WriteByte((byte)SampleDepth);
            int nentries = GetNentries();
            for (int n = 0; n < nentries; n++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (SampleDepth == 8)
                        PngHelperInternal.WriteByte(ba, (byte)Palette[n * 5 + i]);
                    else
                        PngHelperInternal.WriteInt2(ba, Palette[n * 5 + i]);
                }
                PngHelperInternal.WriteInt2(ba, Palette[n * 5 + 4]);
            }
            byte[] b = ba.ToArray();
            ChunkRaw chunk = createEmptyChunk(b.Length, false);
            chunk.Data = b;
            return chunk;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            int t = -1;
            for (int i = 0; i < c.Data.Length; i++)
            { // look for first zero
                if (c.Data[i] == 0)
                {
                    t = i;
                    break;
                }
            }
            if (t <= 0 || t > c.Data.Length - 2)
                throw new PngjException("bad sPLT chunk: no separator found");
            PalName = ChunkHelper.ToString(c.Data, 0, t);
            SampleDepth = PngHelperInternal.ReadInt1fromByte(c.Data, t + 1);
            t += 2;
            int nentries = (c.Data.Length - t) / (SampleDepth == 8 ? 6 : 10);
            Palette = new int[nentries * 5];
            int r, g, b, a, f, ne;
            ne = 0;
            for (int i = 0; i < nentries; i++)
            {
                if (SampleDepth == 8)
                {
                    r = PngHelperInternal.ReadInt1fromByte(c.Data, t++);
                    g = PngHelperInternal.ReadInt1fromByte(c.Data, t++);
                    b = PngHelperInternal.ReadInt1fromByte(c.Data, t++);
                    a = PngHelperInternal.ReadInt1fromByte(c.Data, t++);
                }
                else
                {
                    r = PngHelperInternal.ReadInt2fromBytes(c.Data, t);
                    t += 2;
                    g = PngHelperInternal.ReadInt2fromBytes(c.Data, t);
                    t += 2;
                    b = PngHelperInternal.ReadInt2fromBytes(c.Data, t);
                    t += 2;
                    a = PngHelperInternal.ReadInt2fromBytes(c.Data, t);
                    t += 2;
                }
                f = PngHelperInternal.ReadInt2fromBytes(c.Data, t);
                t += 2;
                Palette[ne++] = r;
                Palette[ne++] = g;
                Palette[ne++] = b;
                Palette[ne++] = a;
                Palette[ne++] = f;
            }
        }

        public override void CloneDataFromRead(PngChunk other)
        {
            PngChunkSPLT otherx = (PngChunkSPLT)other;
            PalName = otherx.PalName;
            SampleDepth = otherx.SampleDepth;
            Palette = new int[otherx.Palette.Length];
            Array.Copy(otherx.Palette, 0, Palette, 0, Palette.Length);

        }

        public int GetNentries()
        {
            return Palette.Length / 5;
        }

    }
}
