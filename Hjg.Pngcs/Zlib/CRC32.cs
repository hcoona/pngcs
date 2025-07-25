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

namespace Hjg.Pngcs.Zlib {
        
    public class CRC32 { // based on http://damieng.com/blog/2006/08/08/calculating_crc32_in_c_and_net

            private const UInt32 defaultPolynomial = 0xedb88320;
            private const UInt32 defaultSeed = 0xffffffff;
            private static UInt32[] defaultTable;

            private UInt32 hash;
            private UInt32 seed;
            private UInt32[] table;

            public CRC32()
                : this(defaultPolynomial, defaultSeed) {
            }

            public CRC32(UInt32 polynomial, UInt32 seed) {
                table = InitializeTable(polynomial);
                this.seed = seed;
                this.hash = seed;
            }

            public void Update(byte[] buffer) {
                Update(buffer, 0, buffer.Length);
            }

            public void Update(byte[] buffer, int start, int length) {
                for (int i = 0, j = start; i < length; i++, j++) {
                    unchecked {
                        hash = (hash >> 8) ^ table[buffer[j] ^ hash & 0xff];
                    }
                }
            }

            public UInt32 GetValue() {
                return ~hash;
            }

            public void Reset() {
                this.hash = seed;
            }
        
            private static UInt32[] InitializeTable(UInt32 polynomial) {
                if (polynomial == defaultPolynomial && defaultTable != null)
                    return defaultTable;
                UInt32[] createTable = new UInt32[256];
                for (int i = 0; i < 256; i++) {
                    UInt32 entry = (UInt32)i;
                    for (int j = 0; j < 8; j++)
                        if ((entry & 1) == 1)
                            entry = (entry >> 1) ^ polynomial;
                        else
                            entry = entry >> 1;
                    createTable[i] = entry;
                }
                if (polynomial == defaultPolynomial)
                    defaultTable = createTable;
                return createTable;
            }

        }
}
