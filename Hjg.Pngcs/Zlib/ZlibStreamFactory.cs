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
using System.IO;
using System.Text;

namespace Hjg.Pngcs.Zlib {


    public class ZlibStreamFactory {        
        public static AZlibInputStream createZlibInputStream(Stream st, bool leaveOpen) {
#if NET45
                return new ZlibInputStreamMs(st,leaveOpen);
#endif
#if SHARPZIPLIB
            return new ZlibInputStreamIs(st, leaveOpen);
#endif
        }

        public static AZlibInputStream createZlibInputStream(Stream st) {
            return createZlibInputStream(st, false);
        }

        public static AZlibOutputStream createZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen) {
#if NET45
                return new ZlibOutputStreamMs( st, compressLevel,strat, leaveOpen);
#endif
#if SHARPZIPLIB
            return new ZlibOutputStreamIs(st, compressLevel, strat, leaveOpen);
#endif
        }

        public static AZlibOutputStream createZlibOutputStream(Stream st) {
            return createZlibOutputStream(st, false);
        }

        public static AZlibOutputStream createZlibOutputStream(Stream st, bool leaveOpen) {
            return createZlibOutputStream(st, DeflateCompressLevel.DEFAULT, EDeflateCompressStrategy.Default, leaveOpen);
        }
    }
}
