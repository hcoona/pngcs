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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

#if SHARPZIPLIB

using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
// ONLY IF SHARPZIPLIB IS AVAILABLE

namespace Hjg.Pngcs.Zlib {


    /// <summary>
    /// Zip input (inflater) based on ShaprZipLib
    /// </summary>
    class ZlibInputStreamIs : AZlibInputStream {

        private InflaterInputStream ist;

        public ZlibInputStreamIs(Stream st, bool leaveOpen)
            : base(st, leaveOpen) {
            ist = new InflaterInputStream(st);
            ist.IsStreamOwner = !leaveOpen;
        }

        public override int Read(byte[] array, int offset, int count) {
            return ist.Read(array, offset, count);
        }

        public override int ReadByte() {
            return ist.ReadByte();
        }

        public override void Close() {
            ist.Close();
        }


        public override void Flush() {
            ist.Flush();
        }

        public override String getImplementationId() {
            return "Zlib inflater: SharpZipLib";
        }
    }
}

#endif
