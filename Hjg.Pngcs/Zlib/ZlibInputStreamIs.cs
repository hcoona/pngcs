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
