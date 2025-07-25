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
