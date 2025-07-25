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

namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// match if have same id and, if Text (or SPLT) if have the asame key
    /// </summary>
    /// <remarks>
    /// This is the same as ChunkPredicateEquivalent, the only difference is that does not requires
    /// a chunk at construction time
    /// </remarks>
    internal class ChunkPredicateId2 : ChunkPredicate
    {

        private readonly string id;
        private readonly string innerid;
        public ChunkPredicateId2(string id, string inner)
        {
            this.id = id;
            this.innerid = inner;
        }
        public bool Matches(PngChunk c)
        {
            if (!c.Id.Equals(id))
                return false;
            if (c is PngChunkTextVar && !((PngChunkTextVar)c).GetKey().Equals(innerid))
                return false;
            if (c is PngChunkSPLT && !((PngChunkSPLT)c).PalName.Equals(innerid))
                return false;

            return true;
        }

    }
}
