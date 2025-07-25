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

namespace Hjg.Pngcs {

    using Hjg.Pngcs.Chunks;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// outputs the stream for IDAT chunk , fragmented at fixed size (32k default).
    /// </summary>
    ///
    internal class PngIDatChunkOutputStream : ProgressiveOutputStream {
        private const int SIZE_DEFAULT = 32768;// 32k
        private readonly Stream outputStream;

        public PngIDatChunkOutputStream(Stream outputStream_0)
            : this(outputStream_0, SIZE_DEFAULT) {

        }

        public PngIDatChunkOutputStream(Stream outputStream_0, int size)
            : base(size > 8 ? size : SIZE_DEFAULT) {
            this.outputStream = outputStream_0;
        }

        protected override void FlushBuffer(byte[] b, int len) {
            ChunkRaw c = new ChunkRaw(len, Hjg.Pngcs.Chunks.ChunkHelper.b_IDAT, false);
            c.Data = b;
            c.WriteChunk(outputStream);
        }

        public override void Close() {
            // closing the IDAT stream only flushes it, it does not close the underlying stream
            Flush();
        }
    }
}
