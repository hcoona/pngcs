// Copyright 2012    Hernán J. González    hgonzalez@gmail.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Hjg.Pngcs.Chunks {

    using Hjg.Pngcs;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// IHDR chunk: http://www.w3.org/TR/PNG/#11IHDR
    /// </summary>
    public class PngChunkIHDR : PngChunkSingle {
        public const String ID = ChunkHelper.IHDR;
        public int Cols {get;set;}
        public int Rows { get; set; }
        public int Bitspc { get; set; }
        public int Colormodel { get; set; }
        public int Compmeth { get; set; }
        public int Filmeth { get; set; }
        public int Interlaced { get; set; }

        public PngChunkIHDR(ImageInfo info)
            : base(ID, info) {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint() {
            return ChunkOrderingConstraint.NA;
        }

        public override ChunkRaw CreateRawChunk() {
            ChunkRaw c = new ChunkRaw(13, ChunkHelper.b_IHDR, true);
            int offset = 0;
            Hjg.Pngcs.PngHelperInternal.WriteInt4tobytes(Cols, c.Data, offset);
            offset += 4;
            Hjg.Pngcs.PngHelperInternal.WriteInt4tobytes(Rows, c.Data, offset);
            offset += 4;
            c.Data[offset++] = (byte)Bitspc;
            c.Data[offset++] = (byte)Colormodel;
            c.Data[offset++] = (byte)Compmeth;
            c.Data[offset++] = (byte)Filmeth;
            c.Data[offset++] = (byte)Interlaced;
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c) {
            if (c.Len != 13)
                throw new PngjException("Bad IDHR len " + c.Len);
            MemoryStream st = c.GetAsByteStream();
            Cols = Hjg.Pngcs.PngHelperInternal.ReadInt4(st);
            Rows = Hjg.Pngcs.PngHelperInternal.ReadInt4(st);
            // bit depth: number of bits per channel
            Bitspc = Hjg.Pngcs.PngHelperInternal.ReadByte(st);
            Colormodel = Hjg.Pngcs.PngHelperInternal.ReadByte(st);
            Compmeth = Hjg.Pngcs.PngHelperInternal.ReadByte(st);
            Filmeth = Hjg.Pngcs.PngHelperInternal.ReadByte(st);
            Interlaced = Hjg.Pngcs.PngHelperInternal.ReadByte(st);
        }

        public override void CloneDataFromRead(PngChunk other) {
            PngChunkIHDR otherx = (PngChunkIHDR)other;
            Cols = otherx.Cols;
            Rows = otherx.Rows;
            Bitspc = otherx.Bitspc;
            Colormodel = otherx.Colormodel;
            Compmeth = otherx.Compmeth;
            Filmeth = otherx.Filmeth;
            Interlaced = otherx.Interlaced;
        }
    }
}
