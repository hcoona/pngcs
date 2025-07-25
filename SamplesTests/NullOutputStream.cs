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

namespace SamplesTests {
    public  class NullOutputStream : System.IO.MemoryStream {
        private int cont = 0;

        public void Write(int arg0) {
            // nothing!
            cont++;
        }

        public override void Write(byte[] b, int off, int len) {
            cont += len;
        }

        public override void WriteByte(byte b) {
            cont++;
        }

        public int getCont() {
            return cont;
        }
    }

}
