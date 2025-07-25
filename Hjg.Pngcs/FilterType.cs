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
    /// Internal PNG predictor filter, or a strategy to select it.
    /// </summary>
    public enum FilterType {
        /// <summary>
        /// No filtering 
        /// </summary>
        FILTER_NONE = 0,
        /// <summary>
        /// SUB filter: uses same row
        /// </summary>
        FILTER_SUB = 1,
        /// <summary>
        ///  UP filter: uses previous row
        /// </summary>
        FILTER_UP = 2,
        /// <summary>
        ///AVERAGE filter: uses neighbors
        /// </summary>
        FILTER_AVERAGE = 3,
        /// <summary>
        /// PAETH predictor
        /// </summary>
        FILTER_PAETH = 4,

        /// <summary>
        /// Default strategy: select one of the standard filters depending on global image parameters
        /// </summary>
        FILTER_DEFAULT = -1, // 


        /// <summary>
        /// Aggressive strategy: select dinamically the filters, trying every 8 rows
        /// </summary>
        FILTER_AGGRESSIVE = -2,

        /// <summary>
        /// Very aggressive and slow strategy: tries all filters for each row
        /// </summary>
        FILTER_VERYAGGRESSIVE = -3,

        /// <summary>
        /// Uses all fiters, one for lines, in cyclic way. Only useful for testing.
        /// </summary>
        FILTER_CYCLIC = -50,

        /// <summary>
        /// Not specified, placeholder for unknown or NA filters. 
        /// </summary>
        FILTER_UNKNOWN = -100
    }


}
