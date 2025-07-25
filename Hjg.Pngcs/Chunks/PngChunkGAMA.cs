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
    /// gAMA chunk, see http://www.w3.org/TR/PNG/#11gAMA
    /// </summary>
    public class PngChunkGAMA : PngChunkSingle {
        public const String ID = ChunkHelper.gAMA;

        private double gamma;

        public PngChunkGAMA(ImageInfo info)
            : base(ID, info) {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint() {
            return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
        }

        public override ChunkRaw CreateRawChunk() {
            ChunkRaw c = createEmptyChunk(4, true);
            int g = (int)(gamma * 100000 + 0.5d);
            Hjg.Pngcs.PngHelperInternal.WriteInt4tobytes(g, c.Data, 0);
            return c;
        }

        public override void ParseFromRaw(ChunkRaw chunk) {
            if (chunk.Len != 4)
                throw new PngjException("bad chunk " + chunk);
            int g = Hjg.Pngcs.PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0);
            gamma = ((double)g) / 100000.0d;
        }

        public override void CloneDataFromRead(PngChunk other) {
            gamma = ((PngChunkGAMA)other).gamma;
        }

        public double GetGamma() {
            return gamma;
        }

        public void SetGamma(double gamma) {
            this.gamma = gamma;
        }
    }
}
