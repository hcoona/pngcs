// Pngcs is a CSharp implementation of PNG binary reader and writer.
// Copyright (C) 2025 Shuai Zhang <zhangshuai.ustc@gmail.com>
//
// Based on original work:
//   Copyright 2012    Hernán J. González    hgonzalez@gmail.com
//   Licensed under the Apache License, Version 2.0
//
//   You should have received a copy of the Apache License 2.0
//   along with the program.
//   If not, see <http://www.apache.org/licenses/LICENSE-2.0>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

namespace Hjg.Pngcs.Chunks
{

    using System;
    using Hjg.Pngcs;
    /// <summary>
    /// general class for textual chunks
    /// </summary>
    public abstract class PngChunkTextVar : PngChunkMultiple
    {
        protected internal String key; // key/val: only for tEXt. lazy computed
        protected internal String val;

        protected internal PngChunkTextVar(String id, ImageInfo info)
            : base(id, info)
        {
        }

        public const String KEY_Title = "Title"; // Short (one line) title or caption for image
        public const String KEY_Author = "Author"; // Name of image's creator
        public const String KEY_Description = "Description"; // Description of image (possibly long)
        public const String KEY_Copyright = "Copyright"; // Copyright notice
        public const String KEY_Creation_Time = "Creation Time"; // Time of original image creation
        public const String KEY_Software = "Software"; // Software used to create the image
        public const String KEY_Disclaimer = "Disclaimer"; // Legal disclaimer
        public const String KEY_Warning = "Warning"; // Warning of nature of content
        public const String KEY_Source = "Source"; // Device used to create the image
        public const String KEY_Comment = "Comment"; // Miscellaneous comment

        public class PngTxtInfo
        {
            public String title;
            public String author;
            public String description;
            public String creation_time;// = (new Date()).toString();
            public String software;
            public String disclaimer;
            public String warning;
            public String source;
            public String comment;
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.NONE;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public String GetKey()
        {
            return key;
        }

        public String GetVal()
        {
            return val;
        }

        public void SetKeyVal(String key, String val)
        {
            this.key = key;
            this.val = val;
        }
    }
}
