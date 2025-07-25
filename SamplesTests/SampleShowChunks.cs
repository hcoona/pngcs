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

namespace SamplesTests {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Hjg.Pngcs;
    using Hjg.Pngcs.Chunks;

    /**
 * prints all chunks (remember that IDAT is shown as only one pseudo zero-length chunk)
 */
    public class SampleShowChunks {

        public static void showChunks(String file) {
            PngReader pngr = FileHelper.CreatePngReader(file);
            pngr.MaxTotalBytesRead = 1024 * 1024 * 1024L * 3; // 3Gb!
            pngr.ReadSkippingAllRows();
            Console.Out.WriteLine(pngr.ToString());
            Console.Out.WriteLine(pngr.GetChunksList().ToStringFull());
        }


    }
}