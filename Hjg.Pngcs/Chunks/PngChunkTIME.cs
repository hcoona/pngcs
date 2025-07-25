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

namespace Hjg.Pngcs.Chunks {

    using Hjg.Pngcs;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// tIME chunk: http://www.w3.org/TR/PNG/#11tIME
    /// </summary>
    public class PngChunkTIME : PngChunkSingle {
        public const String ID = ChunkHelper.tIME;
    
        private int year, mon, day, hour, min, sec;

        public PngChunkTIME(ImageInfo info)
            : base(ID, info) {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint() {
            return ChunkOrderingConstraint.NONE;
        }

        public override ChunkRaw CreateRawChunk() {
            ChunkRaw c = createEmptyChunk(7, true);
            Hjg.Pngcs.PngHelperInternal.WriteInt2tobytes(year, c.Data, 0);
            c.Data[2] = (byte)mon;
            c.Data[3] = (byte)day;
            c.Data[4] = (byte)hour;
            c.Data[5] = (byte)min;
            c.Data[6] = (byte)sec;
            return c;
        }

        public override void ParseFromRaw(ChunkRaw chunk) {
            if (chunk.Len != 7)
                throw new PngjException("bad chunk " + chunk);
            year = Hjg.Pngcs.PngHelperInternal.ReadInt2fromBytes(chunk.Data, 0);
            mon = Hjg.Pngcs.PngHelperInternal.ReadInt1fromByte(chunk.Data, 2);
            day = Hjg.Pngcs.PngHelperInternal.ReadInt1fromByte(chunk.Data, 3);
            hour = Hjg.Pngcs.PngHelperInternal.ReadInt1fromByte(chunk.Data, 4);
            min = Hjg.Pngcs.PngHelperInternal.ReadInt1fromByte(chunk.Data, 5);
            sec = Hjg.Pngcs.PngHelperInternal.ReadInt1fromByte(chunk.Data, 6);
        }

        public override void CloneDataFromRead(PngChunk other) {
            PngChunkTIME x = (PngChunkTIME)other;
            year = x.year;
            mon = x.mon;
            day = x.day;
            hour = x.hour;
            min = x.min;
            sec = x.sec;
        }

        public void SetNow(int secsAgo) {
            DateTime d1 = DateTime.Now;
            year = d1.Year;
            mon = d1.Month;
            day = d1.Day;
            hour = d1.Hour;
            min = d1.Minute;
            sec = d1.Second;
        }

        internal void SetYMDHMS(int yearx, int monx, int dayx, int hourx, int minx, int secx) {
            year = yearx;
            mon = monx;
            day = dayx;
            hour = hourx;
            min = minx;
            sec = secx;
        }

        public int[] GetYMDHMS() {
            return new int[] { year, mon, day, hour, min, sec };
        }

        /** format YYYY/MM/DD HH:mm:SS */
        public String GetAsString() {
            return String.Format("%04d/%02d/%02d %02d:%02d:%02d", year, mon, day, hour, min, sec);
        }

    }
}
