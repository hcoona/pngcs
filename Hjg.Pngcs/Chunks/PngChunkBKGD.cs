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
    using Hjg.Pngcs;

    /// <summary>
    /// bKGD chunk, see http://www.w3.org/TR/PNG/#11bKGD
    /// </summary>
    public class PngChunkBKGD : PngChunkSingle
    {
        public const string ID = ChunkHelper.bKGD;

        // this chunk structure depends on the image type
        // only one of these is meaningful
        private int gray;
        private int red, green, blue;
        private int paletteIndex;

        public PngChunkBKGD(ImageInfo info)
            : base(ID, info)
        {
        }
        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
        }

        public override ChunkRaw CreateRawChunk()
        {
            ChunkRaw c = null;
            if (ImgInfo.Greyscale)
            {
                c = createEmptyChunk(2, true);
                PngHelperInternal.WriteInt2tobytes(gray, c.Data, 0);
            }
            else if (ImgInfo.Indexed)
            {
                c = createEmptyChunk(1, true);
                c.Data[0] = (byte)paletteIndex;
            }
            else
            {
                c = createEmptyChunk(6, true);
                PngHelperInternal.WriteInt2tobytes(red, c.Data, 0);
                PngHelperInternal.WriteInt2tobytes(green, c.Data, 0);
                PngHelperInternal.WriteInt2tobytes(blue, c.Data, 0);
            }
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (ImgInfo.Greyscale)
            {
                gray = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
            }
            else if (ImgInfo.Indexed)
            {
                paletteIndex = (int)(c.Data[0] & 0xff);
            }
            else
            {
                red = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
                green = PngHelperInternal.ReadInt2fromBytes(c.Data, 2);
                blue = PngHelperInternal.ReadInt2fromBytes(c.Data, 4);
            }
        }

        public override void CloneDataFromRead(PngChunk other)
        {
            PngChunkBKGD otherx = (PngChunkBKGD)other;
            gray = otherx.gray;
            red = otherx.red;
            green = otherx.red;
            blue = otherx.red;
            paletteIndex = otherx.paletteIndex;
        }

        /// <summary>
        /// Set gray value (0-255 if bitdept=8)
        /// </summary>
        /// <param name="gray"></param>
        public void SetGray(int gray)
        {
            if (!ImgInfo.Greyscale)
                throw new PngjException("only gray images support this");
            this.gray = gray;
        }
        /// <summary>
        /// Gets gray value
        /// </summary>
        /// <returns>gray value  (0-255 if bitdept=8)</returns>
        public int GetGray()
        {
            if (!ImgInfo.Greyscale)
                throw new PngjException("only gray images support this");
            return gray;
        }

        /// <summary>
        /// Set pallette index - only for indexed
        /// </summary>
        /// <param name="index"></param>
        public void SetPaletteIndex(int index)
        {
            if (!ImgInfo.Indexed)
                throw new PngjException("only indexed (pallete) images support this");
            this.paletteIndex = index;
        }

        /// <summary>
        /// Get pallette index - only for indexed
        /// </summary>
        /// <returns></returns>
        public int GetPaletteIndex()
        {
            if (!ImgInfo.Indexed)
                throw new PngjException("only indexed (pallete) images support this");
            return paletteIndex;
        }

        /// <summary>
        /// Sets rgb value, only for rgb images
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetRGB(int r, int g, int b)
        {
            if (ImgInfo.Greyscale || ImgInfo.Indexed)
                throw new PngjException("only rgb or rgba images support this");
            red = r;
            green = g;
            blue = b;
        }
        /// <summary>
        /// Gets rgb value, only for rgb images
        /// </summary>
        /// <returns>[r , g, b] array</returns>
        public int[] GetRGB()
        {
            if (ImgInfo.Greyscale || ImgInfo.Indexed)
                throw new PngjException("only rgb or rgba images support this");
            return new int[] { red, green, blue };
        }

    }
}
