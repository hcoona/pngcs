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

using System.Diagnostics.CodeAnalysis;

namespace Hjg.Pngcs
{
    /// <summary>
    /// Simple immutable wrapper for basic image info
    /// </summary>
    /// <remarks>
    /// Some parameters are clearly redundant
    /// The constructor requires an 'ortogonal' subset
    /// http://www.w3.org/TR/PNG/#11IHDR
    /// </remarks>
    public class ImageInfo
    {
        private const int MAX_COLS_ROWS_VAL = 400000; // very big value, but no so ridiculous as 2^32

        /// <summary>
        /// Image width, in pixels
        /// </summary>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly int Cols;

        /// <summary>
        /// Image height, in pixels
        /// </summary>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly int Rows;

        /// <summary>
        /// Bits per sample (per channel) in the buffer.
        /// </summary>
        /// <remarks>
        /// This is 8 or 16 for RGB/ARGB images.
        /// For grayscale, it's 8 (or 1 2 4 ).
        /// For indexed images, number of bits per palette index (1 2 4 8).
        ///</remarks>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly int BitDepth;

        /// <summary>
        /// Number of channels, used in the buffer
        /// </summary>
        /// <remarks>
        /// WARNING: This is 3-4 for rgb/rgba, but 1 for palette/gray !
        ///</remarks>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly int Channels;

        /// <summary>
        /// Bits used for each pixel in the buffer
        /// </summary>
        /// <remarks>equals <c>channels * bitDepth</c>
        /// </remarks>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly int BitspPixel;

        /// <summary>
        /// Bytes per pixel, rounded up
        /// </summary>
        /// <remarks>This is mainly for internal use (filter)</remarks>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly int BytesPixel;

        /// <summary>
        /// Bytes per row, rounded up
        /// </summary>
        /// <remarks>equals <c>ceil(bitspp*cols/8)</c></remarks>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly int BytesPerRow;

        /// <summary>
        /// Samples (scalar values) per row
        /// </summary>
        /// <remarks>
        /// Equals <c>cols * channels</c>
        /// </remarks>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly int SamplesPerRow;

        /// <summary>
        /// Number of values in our scanline, which might be packed.
        /// </summary>
        /// <remarks>
        /// Equals samplesPerRow if not packed. Elsewhere, it's lower
        /// For internal use, mostly.
        /// </remarks>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly int SamplesPerRowPacked;
        /// <summary>
        /// flag: has alpha channel
        /// </summary>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly bool Alpha;
        /// <summary>
        /// flag: is grayscale (G/GA)
        /// </summary>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly bool Greyscale;
        /// <summary>
        /// flag: has palette
        /// </summary>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly bool Indexed;
        /// <summary>
        /// flag: less than one byte per sample (bit depth 1-2-4)
        /// </summary>
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        public readonly bool Packed;


        /// <summary>
        /// Simple constructor: only for RGB/RGBA
        /// </summary>
        public ImageInfo(int cols, int rows, int bitdepth, bool alpha)
            : this(cols, rows, bitdepth, alpha, false, false)
        {
        }

        /// <summary>
        /// General Constructor
        /// </summary>
        /// <param name="cols">Width in pixels</param>
        /// <param name="rows">Height in pixels</param>
        /// <param name="bitdepth">Bits per sample per channel</param>
        /// <param name="alpha">Has alpha channel</param>
        /// <param name="grayscale">Is grayscale</param>
        /// <param name="palette">Has palette</param>
        public ImageInfo(int cols, int rows, int bitdepth, bool alpha, bool grayscale,
                bool palette)
        {
            this.Cols = cols;
            this.Rows = rows;
            this.Alpha = alpha;
            this.Indexed = palette;
            this.Greyscale = grayscale;
            if (Greyscale && palette)
                throw new PngjException("palette and greyscale are exclusive");
            this.Channels = (grayscale || palette) ? ((alpha) ? 2 : 1) : ((alpha) ? 4 : 3);
            // http://www.w3.org/TR/PNG/#11IHDR
            this.BitDepth = bitdepth;
            this.Packed = bitdepth < 8;
            this.BitspPixel = (Channels * this.BitDepth);
            this.BytesPixel = (BitspPixel + 7) / 8;
            this.BytesPerRow = (BitspPixel * cols + 7) / 8;
            this.SamplesPerRow = Channels * this.Cols;
            this.SamplesPerRowPacked = (Packed) ? BytesPerRow : SamplesPerRow;
            // checks
            switch (this.BitDepth)
            {
                case 1:
                case 2:
                case 4:
                    if (!(this.Indexed || this.Greyscale))
                        throw new PngjException("only indexed or grayscale can have bitdepth="
                                + this.BitDepth);
                    break;
                case 8:
                    break;
                case 16:
                    if (this.Indexed)
                        throw new PngjException("indexed can't have bitdepth=" + this.BitDepth);
                    break;
                default:
                    throw new PngjException("invalid bitdepth=" + this.BitDepth);
            }
            if (cols < 1 || cols > MAX_COLS_ROWS_VAL)
                throw new PngjException("invalid cols=" + cols + " ???");
            if (rows < 1 || rows > MAX_COLS_ROWS_VAL)
                throw new PngjException("invalid rows=" + rows + " ???");
        }

        /// <summary>
        /// General information, for debugging
        /// </summary>
        /// <returns>Summary</returns>
        public override string ToString()
        {
            return "ImageInfo [cols=" + Cols + ", rows=" + Rows + ", bitDepth=" + BitDepth
                    + ", channels=" + Channels + ", bitspPixel=" + BitspPixel + ", bytesPixel="
                    + BytesPixel + ", bytesPerRow=" + BytesPerRow + ", samplesPerRow="
                    + SamplesPerRow + ", samplesPerRowP=" + SamplesPerRowPacked + ", alpha=" + Alpha
                    + ", greyscale=" + Greyscale + ", indexed=" + Indexed + ", packed=" + Packed
                    + "]";
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + ((Alpha) ? 1231 : 1237);
            result = prime * result + BitDepth;
            result = prime * result + Channels;
            result = prime * result + Cols;
            result = prime * result + ((Greyscale) ? 1231 : 1237);
            result = prime * result + ((Indexed) ? 1231 : 1237);
            result = prime * result + Rows;
            return result;
        }

        public override bool Equals(object obj)
        {
            if ((object)this == obj)
                return true;
            if (obj == null)
                return false;
            if ((object)GetType() != (object)obj.GetType())
                return false;
            ImageInfo other = (ImageInfo)obj;
            if (Alpha != other.Alpha)
                return false;
            if (BitDepth != other.BitDepth)
                return false;
            if (Channels != other.Channels)
                return false;
            if (Cols != other.Cols)
                return false;
            if (Greyscale != other.Greyscale)
                return false;
            if (Indexed != other.Indexed)
                return false;
            if (Rows != other.Rows)
                return false;
            return true;
        }
    }
}
