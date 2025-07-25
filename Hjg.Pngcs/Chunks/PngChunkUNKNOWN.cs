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
    /// Unknown (for our chunk factory) chunk type.
    /// </summary>
    public class PngChunkUNKNOWN : PngChunkMultiple { 

        private byte[] data;

        public PngChunkUNKNOWN(String id, ImageInfo info)
            : base(id, info) {
        }

        private PngChunkUNKNOWN(PngChunkUNKNOWN c, ImageInfo info)
            : base(c.Id, info) {
            System.Array.Copy(c.data, 0, data, 0, c.data.Length);
        }

        public override ChunkOrderingConstraint GetOrderingConstraint() {
            return ChunkOrderingConstraint.NONE;
        }


        public override ChunkRaw CreateRawChunk() {
            ChunkRaw p = createEmptyChunk(data.Length, false);
            p.Data = this.data;
            return p;
        }

        public override void ParseFromRaw(ChunkRaw c) {
            data = c.Data;
        }

        /* does not copy! */
        public byte[] GetData() {
            return data;
        }

        /* does not copy! */
        public void SetData(byte[] data_0) {
            this.data = data_0;
        }

        public override void CloneDataFromRead(PngChunk other) {
            // THIS SHOULD NOT BE CALLED IF ALREADY CLONED WITH COPY CONSTRUCTOR
            PngChunkUNKNOWN c = (PngChunkUNKNOWN)other;
            data = c.data; // not deep copy
        }
    }
}
