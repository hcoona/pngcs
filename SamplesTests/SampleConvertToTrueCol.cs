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
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;

namespace SamplesTests {

    class SampleConvertToTrueCol {

        public static void doit(String orig) {
            string copy= TestsHelper.addSuffixToName(orig, "_tc");

            PngReader pngr = FileHelper.CreatePngReader(orig);
            if (!pngr.ImgInfo.Indexed)
                throw new Exception("Not indexed image");
            PngChunkPLTE plte = pngr.GetMetadata().GetPLTE();
            PngChunkTRNS trns = pngr.GetMetadata().GetTRNS(); // transparency metadata, can be null
            bool alpha = trns != null;
            ImageInfo im2 = new ImageInfo(pngr.ImgInfo.Cols, pngr.ImgInfo.Rows, 8, alpha);
            PngWriter pngw = FileHelper.CreatePngWriter(copy, im2, true);
            pngw.CopyChunksFirst(pngr, ChunkCopyBehaviour.COPY_ALL_SAFE);
            int[] buf = null;
            for (int row = 0; row < pngr.ImgInfo.Rows; row++) {
                ImageLine line = pngr.ReadRowInt(row);
                buf = ImageLineHelper.Palette2rgb(line, plte, trns, buf);
                pngw.WriteRowInt(buf, row);
            }
            pngw.CopyChunksLast(pngr, ChunkCopyBehaviour.COPY_ALL_SAFE);
            pngr.End();
            pngw.End();
            Console.WriteLine("True color: " + copy);
        }


    }

}
