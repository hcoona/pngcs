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
    /// sRGB chunk: http://www.w3.org/TR/PNG/#11sRGB
    /// </summary>
    public class PngChunkSRGB : PngChunkSingle {
        public const String ID = ChunkHelper.sRGB;

        public const int RENDER_INTENT_Perceptual = 0;
        public const int RENDER_INTENT_Relative_colorimetric = 1;
        public const int RENDER_INTENT_Saturation = 2;
        public const int RENDER_INTENT_Absolute_colorimetric = 3;

        public int Intent { get; set; }

        public PngChunkSRGB(ImageInfo info)
            : base(ID, info) {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint() {
            return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
        }

        public override ChunkRaw CreateRawChunk() {
            ChunkRaw c = null;
            c = createEmptyChunk(1, true);
            c.Data[0] = (byte)Intent;
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c) {
            if (c.Len != 1)
                throw new PngjException("bad chunk length " + c);
            Intent = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
        }


        public override void CloneDataFromRead(PngChunk other) {
            PngChunkSRGB otherx = (PngChunkSRGB)other;
            Intent = otherx.Intent;
        }


    }
}
