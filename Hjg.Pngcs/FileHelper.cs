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

namespace Hjg.Pngcs {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// A few utility static methods to read and write files
    /// </summary>
    ///
    public class FileHelper {

        public static Stream OpenFileForReading(String file) {
            Stream isx = null;
            if (file == null || !File.Exists(file))
                throw new PngjInputException("Cannot open file for reading (" + file + ")");
            isx = new FileStream(file, FileMode.Open);
            return isx;
        }

        public static Stream OpenFileForWriting(String file, bool allowOverwrite) {
            Stream osx = null;
            if (File.Exists(file) && !allowOverwrite)
                throw new PngjOutputException("File already exists (" + file + ") and overwrite=false");
            osx = new FileStream(file, FileMode.Create);
            return osx;
        }

        /// <summary>
        /// Given a filename and a ImageInfo, produces a PngWriter object, ready for writing.</summary>
        /// <param name="fileName">Path of file</param>
        /// <param name="imgInfo">ImageInfo object</param>
        /// <param name="allowOverwrite">Flag: if false and file exists, a PngjOutputException is thrown</param>
        /// <returns>A PngWriter object, ready for writing</returns>
        public static PngWriter CreatePngWriter(String fileName, ImageInfo imgInfo, bool allowOverwrite) {
            return new PngWriter(OpenFileForWriting(fileName, allowOverwrite), imgInfo,
                    fileName);
        }

        /// <summary>
        /// Given a filename, produces a PngReader object, ready for reading.
        /// </summary>
        /// <param name="fileName">Path of file</param>
        /// <returns>PngReader, ready for reading</returns>
        public static PngReader CreatePngReader(String fileName) {
            return new PngReader(OpenFileForReading(fileName), fileName);
        }
    }
}
