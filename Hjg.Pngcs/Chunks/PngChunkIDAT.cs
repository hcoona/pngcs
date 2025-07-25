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
    /// IDAT chunk http://www.w3.org/TR/PNG/#11IDAT
    /// 
    /// This object is dummy placeholder - We treat this chunk in a very different way than ancillary chnks
    /// </summary>
    public class PngChunkIDAT : PngChunkMultiple {
        public const String ID = ChunkHelper.IDAT;

        public PngChunkIDAT(ImageInfo i,int len, long offset)
            : base(ID, i) {
            this.Length = len;
            this.Offset = offset;
        }

        public override ChunkOrderingConstraint GetOrderingConstraint() {
            return ChunkOrderingConstraint.NA;
        }

        public override ChunkRaw CreateRawChunk() {// does nothing
            return null;
        }

        public override void ParseFromRaw(ChunkRaw c) { // does nothing
        }

        public override void CloneDataFromRead(PngChunk other) {
        }
    }
}
