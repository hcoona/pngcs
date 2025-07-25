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
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Utility functions for C# porting
    /// </summary>
    ///
    internal class PngCsUtils {
        internal static bool arraysEqual4(byte[] ar1, byte[] ar2) {
            return (ar1[0] == ar2[0]) &&
                   (ar1[1] == ar2[1]) &&
                   (ar1[2] == ar2[2]) &&
                   (ar1[3] == ar2[3]);
        }


        internal static bool arraysEqual(byte[] a1, byte[] a2) {
            if (a1.Length != a2.Length) return false;
            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i]) return false;
            return true;
        }
    }
}
