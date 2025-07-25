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

namespace Hjg.Pngcs
{

    using System;
    using System.Diagnostics.CodeAnalysis;


    /// <summary>
    /// Lightweight wrapper for an image scanline, for read and write
    /// </summary>
    /// <remarks>It can be (usually it is) reused while iterating over the image lines
    /// See <c>scanline</c> field doc, to understand the format.
    ///</remarks>
    public class ImageLine
    {
        /// <summary>
        /// ImageInfo (readonly inmutable)
        /// </summary>
        public ImageInfo ImgInfo { get; private set; }
        /// <summary>
        /// Samples of an image line
        /// </summary>
        /// <remarks>
        ///
        /// The 'scanline' is an array of integers, corresponds to an image line (row)
        /// Except for 'packed' formats (gray/indexed with 1-2-4 bitdepth) each int is a
        /// "sample" (one for channel), (0-255 or 0-65535) in the respective PNG sequence
        /// sequence : (R G B R G B...) or (R G B A R G B A...) or (g g g ...) or ( i i i
        /// ) (palette index)
        ///
        /// For bitdepth 1/2/4 ,and if samplesUnpacked=false, each value is a PACKED byte! To get an unpacked copy,
        /// see <c>Pack()</c> and its inverse <c>Unpack()</c>
        ///
        /// To convert a indexed line to RGB balues, see ImageLineHelper.PalIdx2RGB()
        /// (cant do the reverse)
        /// </remarks>
        public int[] Scanline { get; private set; }

        /// <summary>
        /// Same as Scanline, but with one byte per sample. Only one of Scanline and ScanlineB is valid - this depends
        /// on SampleType}
        /// </summary>
        public byte[] ScanlineB { get; private set; }

        /// <summary>
        /// tracks the current row number (from 0 to rows-1)
        /// </summary>
        public int Rown { get; set; }

        internal readonly int channels; // copied from imgInfo, more handy
        internal readonly int bitDepth; // copied from imgInfo, more handy
        /// <summary>
        /// Hown many elements has the scanline array
        /// =imgInfo.samplePerRowPacked, if packed, imgInfo.samplePerRow elsewhere
        /// </summary>
        public int ElementsPerRow { get; private set; }
        /// <summary>
        /// Maximum sample value that this line admits: typically 255; less if bitdepth less than 8, 65535 if 16bits
        /// </summary>
        public int maxSampleVal { get; private set; }

        [SuppressMessage(
            "Naming",
            "CA1720:Identifier contains type name",
            Justification = "Align with the specification.")]
        public enum ESampleType
        {
            INT, // 4 bytes per sample
            BYTE // 1 byte per sample
        }
        /// <summary>
        /// Determines if samples are stored in integers or in bytes
        /// </summary>
        public ESampleType SampleType { get; private set; }

        /// <summary>
        /// True: each scanline element is a sample.
        /// False: each scanline element has severals samples packed in a byte
        /// </summary>
        public bool SamplesUnpacked { get; private set; }

        /// <summary>
        /// informational only ; filled by the reader
        /// </summary>
        public FilterType FilterUsed { get; set; }

        public ImageLine(ImageInfo imgInfo)
            : this(imgInfo, ESampleType.INT, false)
        {
        }

        public ImageLine(ImageInfo imgInfo, ESampleType stype)
            : this(imgInfo, stype, false)
        {
        }

        /// <summary>
        /// Constructs an ImageLine
        /// </summary>
        /// <param name="imgInfo">Inmutable copy of PNG ImageInfo</param>
        /// <param name="stype">Storage for samples:INT (default) or BYTE</param>
        /// <param name="unpackedMode">If true and bitdepth less than 8, samples are unpacked. This has no effect if biddepth 8 or 16</param>
        public ImageLine(ImageInfo imgInfo, ESampleType stype, bool unpackedMode)
            : this(imgInfo, stype, unpackedMode, null, null)
        {
        }

        internal ImageLine(ImageInfo imgInfo, ESampleType stype, bool unpackedMode, int[] sci, byte[] scb)
        {
            this.ImgInfo = imgInfo;
            channels = imgInfo.Channels;
            this.bitDepth = imgInfo.BitDepth;
            this.FilterUsed = FilterType.FILTER_UNKNOWN;
            this.SampleType = stype;
            this.SamplesUnpacked = unpackedMode || !imgInfo.Packed;
            ElementsPerRow = this.SamplesUnpacked ? imgInfo.SamplesPerRow
                : imgInfo.SamplesPerRowPacked;
            if (stype == ESampleType.INT)
            {
                Scanline = sci != null ? sci : new int[ElementsPerRow];
                ScanlineB = null;
                maxSampleVal = bitDepth == 16 ? 0xFFFF : GetMaskForPackedFormatsLs(bitDepth);
            }
            else if (stype == ESampleType.BYTE)
            {
                ScanlineB = scb != null ? scb : new byte[ElementsPerRow];
                Scanline = null;
                maxSampleVal = bitDepth == 16 ? 0xFF : GetMaskForPackedFormatsLs(bitDepth);
            }
            else
                throw new PngjExceptionInternal("bad ImageLine initialization");
            this.Rown = -1;
        }

        static internal void unpackInplaceInt(ImageInfo iminfo, int[] src, int[] dst,
            bool Scale)
        {
            int bitDepth = iminfo.BitDepth;
            if (bitDepth >= 8)
                return; // nothing to do
            int mask0 = GetMaskForPackedFormatsLs(bitDepth);
            int scalefactor = 8 - bitDepth;
            int offset0 = 8 * iminfo.SamplesPerRowPacked - bitDepth * iminfo.SamplesPerRow;
            int mask, offset, v;
            if (offset0 != 8)
            {
                mask = mask0 << offset0;
                offset = offset0; // how many bits to shift the mask to the right to recover mask0
            }
            else
            {
                mask = mask0;
                offset = 0;
            }
            for (int j = iminfo.SamplesPerRow - 1, i = iminfo.SamplesPerRowPacked - 1; j >= 0; j--)
            {
                v = (src[i] & mask) >> offset;
                if (Scale)
                    v <<= scalefactor;
                dst[j] = v;
                mask <<= bitDepth;
                offset += bitDepth;
                if (offset == 8)
                {
                    mask = mask0;
                    offset = 0;
                    i--;
                }
            }
        }

        static internal void packInplaceInt(ImageInfo iminfo, int[] src, int[] dst,
            bool scaled)
        {
            int bitDepth = iminfo.BitDepth;
            if (bitDepth >= 8)
                return; // nothing to do
            int mask0 = GetMaskForPackedFormatsLs(bitDepth);
            int scalefactor = 8 - bitDepth;
            int offset0 = 8 - bitDepth;
            int v, v0;
            int offset = 8 - bitDepth;
            v0 = src[0]; // first value is special for in place
            dst[0] = 0;
            if (scaled)
                v0 >>= scalefactor;
            v0 = ((v0 & mask0) << offset);
            for (int i = 0, j = 0; j < iminfo.SamplesPerRow; j++)
            {
                v = src[j];
                if (scaled)
                    v >>= scalefactor;
                dst[i] |= ((v & mask0) << offset);
                offset -= bitDepth;
                if (offset < 0)
                {
                    offset = offset0;
                    i++;
                    dst[i] = 0;
                }
            }
            dst[0] |= v0;
        }

        static internal void unpackInplaceByte(ImageInfo iminfo, byte[] src,
            byte[] dst, bool scale)
        {
            int bitDepth = iminfo.BitDepth;
            if (bitDepth >= 8)
                return; // nothing to do
            int mask0 = GetMaskForPackedFormatsLs(bitDepth);
            int scalefactor = 8 - bitDepth;
            int offset0 = 8 * iminfo.SamplesPerRowPacked - bitDepth * iminfo.SamplesPerRow;
            int mask, offset, v;
            if (offset0 != 8)
            {
                mask = mask0 << offset0;
                offset = offset0; // how many bits to shift the mask to the right to recover mask0
            }
            else
            {
                mask = mask0;
                offset = 0;
            }
            for (int j = iminfo.SamplesPerRow - 1, i = iminfo.SamplesPerRowPacked - 1; j >= 0; j--)
            {
                v = (src[i] & mask) >> offset;
                if (scale)
                    v <<= scalefactor;
                dst[j] = (byte)v;
                mask <<= bitDepth;
                offset += bitDepth;
                if (offset == 8)
                {
                    mask = mask0;
                    offset = 0;
                    i--;
                }
            }
        }

        /** size original: samplesPerRow sizeFinal: samplesPerRowPacked (trailing elements are trash!) **/
        static internal void packInplaceByte(ImageInfo iminfo, byte[] src, byte[] dst,
                bool scaled)
        {
            int bitDepth = iminfo.BitDepth;
            if (bitDepth >= 8)
                return; // nothing to do
            byte mask0 = (byte)GetMaskForPackedFormatsLs(bitDepth);
            byte scalefactor = (byte)(8 - bitDepth);
            byte offset0 = (byte)(8 - bitDepth);
            byte v, v0;
            int offset = 8 - bitDepth;
            v0 = src[0]; // first value is special
            dst[0] = 0;
            if (scaled)
                v0 >>= scalefactor;
            v0 = (byte)((v0 & mask0) << offset);
            for (int i = 0, j = 0; j < iminfo.SamplesPerRow; j++)
            {
                v = src[j];
                if (scaled)
                    v >>= scalefactor;
                dst[i] |= (byte)((v & mask0) << offset);
                offset -= bitDepth;
                if (offset < 0)
                {
                    offset = offset0;
                    i++;
                    dst[i] = 0;
                }
            }
            dst[0] |= v0;
        }

        /// <summary>
        /// Makes a deep copy
        /// </summary>
        /// <remarks>You should rarely use this</remarks>
        /// <param name="b"></param>
        internal void SetScanLine(int[] b)
        { // makes copy
            Array.Copy((Array)(b), 0, (Array)(Scanline), 0, Scanline.Length);
        }

        /// <summary>
        /// Makes a deep copy
        /// </summary>
        /// <remarks>You should rarely use this</remarks>
        /// <param name="b"></param>
        internal int[] GetScanLineCopy(int[] b)
        {
            if (b == null || b.Length < Scanline.Length)
                b = new int[Scanline.Length];
            Array.Copy((Array)(Scanline), 0, (Array)(b), 0, Scanline.Length);
            return b;
        }

        public ImageLine unpackToNewImageLine()
        {
            ImageLine newline = new ImageLine(ImgInfo, SampleType, true);
            if (SampleType == ESampleType.INT)
                unpackInplaceInt(ImgInfo, Scanline, newline.Scanline, false);
            else
                unpackInplaceByte(ImgInfo, ScanlineB, newline.ScanlineB, false);
            return newline;
        }

        public ImageLine packToNewImageLine()
        {
            ImageLine newline = new ImageLine(ImgInfo, SampleType, false);
            if (SampleType == ESampleType.INT)
                packInplaceInt(ImgInfo, Scanline, newline.Scanline, false);
            else
                packInplaceByte(ImgInfo, ScanlineB, newline.ScanlineB, false);
            return newline;
        }

        public int[] GetScanlineInt()
        {
            return Scanline;
        }

        public byte[] GetScanlineByte()
        {
            return ScanlineB;
        }

        public bool IsInt()
        {
            return SampleType == ESampleType.INT;
        }

        public bool IsByte()
        {
            return SampleType == ESampleType.BYTE;
        }


        public override string ToString()
        {
            return "row=" + Rown + " cols=" + ImgInfo.Cols + " bpc=" + ImgInfo.BitDepth
                    + " size=" + Scanline.Length;
        }

        internal static int GetMaskForPackedFormats(int bitDepth)
        { // Utility function for pack/unpack
            if (bitDepth == 4) return 0xf0;
            else if (bitDepth == 2) return 0xc0;
            else if (bitDepth == 1) return 0x80;
            else return 0xff;

        }

        internal static int GetMaskForPackedFormatsLs(int bitDepth)
        { // Utility function for pack/unpack
            if (bitDepth == 4) return 0x0f;
            else if (bitDepth == 2) return 0x03;
            else if (bitDepth == 1) return 0x01;
            else return 0xff;
        }

    }
}
