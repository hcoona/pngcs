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

    public class Adler32 {
        private UInt32 a = 1;
        private UInt32 b = 0;
        private const int _base = 65521; /* largest prime smaller than 65536 */
        private const int _nmax = 5550;
        private int pend = 0; // how many bytes have I read witouth computing modulus


        public void Update(byte data) {
            if (pend >= _nmax) updateModulus();
            a += data;
            b += a;
            pend++;
        }

        public void Update(byte[] data) {
            Update(data, 0, data.Length);
        }

        public void Update(byte[] data, int offset, int length) {
            int nextJToComputeModulus = _nmax - pend;
            for (int j = 0; j < length; j++) {
                if (j == nextJToComputeModulus) {
                    updateModulus();
                    nextJToComputeModulus = j + _nmax;
                }
                unchecked {
                    a += data[j + offset];
                }
                b += a;
                pend++;
            }
        }

        public void Reset() {
            a = 1;
            b = 0;
            pend = 0;
        }

        private void updateModulus() {
            a %= _base;
            b %= _base;
            pend = 0;
        }

        public UInt32 GetValue() {
            if (pend > 0) updateModulus();
            return (b << 16) | a;
        }
    }
}
