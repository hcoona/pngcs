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

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// stream that outputs to memory and allows to flush fragments every 'size'
    /// bytes to some other destination
    /// </summary>
    ///
    abstract internal class ProgressiveOutputStream : MemoryStream {
        private readonly int size;
        private long countFlushed = 0;

        public ProgressiveOutputStream(int size_0) {
            this.size = size_0;
            if (size < 8) throw new PngjException("bad size for ProgressiveOutputStream: " + size);
        }

        public override void Close() {
            Flush();
            base.Close();
        }

        public override void Flush() {
            base.Flush();
            CheckFlushBuffer(true);
        }

        public override void Write(byte[] b, int off, int len) {
            base.Write(b, off, len);
            CheckFlushBuffer(false);
        }

        public void Write(byte[] b) {
            Write(b, 0, b.Length);
            CheckFlushBuffer(false);
        }


        /// <summary>
        /// if it's time to flush data (or if forced==true) calls abstract method
        /// flushBuffer() and cleans those bytes from own buffer
        /// </summary>
        ///
        private void CheckFlushBuffer(bool forced) {
            int count = (int)Position;
            byte[] buf = GetBuffer();
            while (forced || count >= size) {
                int nb = size;
                if (nb > count)
                    nb = count;
                if (nb == 0)
                    return;
                FlushBuffer(buf, nb);
                countFlushed += nb;
                int bytesleft = count - nb;
                count = bytesleft;
                Position = count;
                if (bytesleft > 0)
                    System.Array.Copy((Array)(buf), nb, (Array)(buf), 0, bytesleft);
            }
        }

        protected abstract void FlushBuffer(byte[] b, int n);

        public long GetCountFlushed() {
            return countFlushed;
        }
    }
}
