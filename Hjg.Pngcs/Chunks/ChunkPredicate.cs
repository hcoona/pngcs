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

namespace Hjg.Pngcs.Chunks {
    /// <summary>
    /// Decides if another chunk "matches", according to some criterion
    /// </summary>
    public interface ChunkPredicate {
        /// <summary>
        /// The other chunk matches with this one
        /// </summary>
        /// <param name="chunk">The other chunk</param>
        /// <returns>true if matches</returns>
        bool Matches(PngChunk chunk);
    }
}
