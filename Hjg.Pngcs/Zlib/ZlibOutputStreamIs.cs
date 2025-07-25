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
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
// ONLY IF SHARPZIPLIB IS AVAILABLE

namespace Hjg.Pngcs.Zlib {


    /// <summary>
    /// Zlib output (deflater) based on ShaprZipLib
    /// </summary>
    class ZlibOutputStreamIs : AZlibOutputStream {

        private DeflaterOutputStream ost;
        private Deflater deflater;
        public ZlibOutputStreamIs(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
            : base(st,compressLevel,strat,leaveOpen) {
                deflater=new Deflater(compressLevel);
            setStrat(strat);
            ost = new DeflaterOutputStream(st, deflater);
            ost.IsStreamOwner = !leaveOpen;
        }

        public void setStrat(EDeflateCompressStrategy strat) {
            if (strat == EDeflateCompressStrategy.Filtered)
                deflater.SetStrategy(DeflateStrategy.Filtered);
            else if (strat == EDeflateCompressStrategy.Huffman)
                deflater.SetStrategy(DeflateStrategy.HuffmanOnly);
            else deflater.SetStrategy(DeflateStrategy.Default);
        }

        public override void Write(byte[] buffer, int offset, int count) {
            ost.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value) {
            ost.WriteByte(value);
        }

 
        public override void Close() {
            ost.Close();
        }


        public override void Flush() {
            ost.Flush();
        }

        public override String getImplementationId() {
            return "Zlib deflater: SharpZipLib";
        }
    }
}

#endif
