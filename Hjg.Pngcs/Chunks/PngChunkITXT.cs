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
    using System.IO;
    using Hjg.Pngcs;
    /// <summary>
    /// iTXt chunk:  http://www.w3.org/TR/PNG/#11iTXt
    /// One of the three text chunks
    /// </summary>
    public class PngChunkITXT : PngChunkTextVar
    {
        public const string ID = ChunkHelper.iTXt;

        private bool compressed = false;
        private string langTag = "";
        private string translatedTag = "";

        public PngChunkITXT(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkRaw CreateRawChunk()
        {
            if (key.Length == 0)
                throw new PngjException("Text chunk key must be non empty");
            MemoryStream ba = new MemoryStream();
            ChunkHelper.WriteBytesToStream(ba, ChunkHelper.ToBytes(key));
            ba.WriteByte(0); // separator
            ba.WriteByte(compressed ? (byte)1 : (byte)0);
            ba.WriteByte(0); // compression method (always 0)
            ChunkHelper.WriteBytesToStream(ba, ChunkHelper.ToBytes(langTag));
            ba.WriteByte(0); // separator
            ChunkHelper.WriteBytesToStream(ba, ChunkHelper.ToBytesUTF8(translatedTag));
            ba.WriteByte(0); // separator
            byte[] textbytes = ChunkHelper.ToBytesUTF8(val);
            if (compressed)
            {
                textbytes = ChunkHelper.compressBytes(textbytes, true);
            }
            ChunkHelper.WriteBytesToStream(ba, textbytes);
            byte[] b = ba.ToArray();
            ChunkRaw chunk = createEmptyChunk(b.Length, false);
            chunk.Data = b;
            return chunk;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            int nullsFound = 0;
            int[] nullsIdx = new int[3];
            for (int k = 0; k < c.Data.Length; k++)
            {
                if (c.Data[k] != 0)
                    continue;
                nullsIdx[nullsFound] = k;
                nullsFound++;
                if (nullsFound == 1)
                    k += 2;
                if (nullsFound == 3)
                    break;
            }
            if (nullsFound != 3)
                throw new PngjException("Bad formed PngChunkITXT chunk");
            key = ChunkHelper.ToString(c.Data, 0, nullsIdx[0]);
            int i = nullsIdx[0] + 1;
            compressed = c.Data[i] == 0 ? false : true;
            i++;
            if (compressed && c.Data[i] != 0)
                throw new PngjException("Bad formed PngChunkITXT chunk - bad compression method ");
            langTag = ChunkHelper.ToString(c.Data, i, nullsIdx[1] - i);
            translatedTag = ChunkHelper.ToStringUTF8(c.Data, nullsIdx[1] + 1, nullsIdx[2] - nullsIdx[1] - 1);
            i = nullsIdx[2] + 1;
            if (compressed)
            {
                byte[] bytes = ChunkHelper.compressBytes(c.Data, i, c.Data.Length - i, false);
                val = ChunkHelper.ToStringUTF8(bytes);
            }
            else
            {
                val = ChunkHelper.ToStringUTF8(c.Data, i, c.Data.Length - i);
            }
        }

        public override void CloneDataFromRead(PngChunk other)
        {
            PngChunkITXT otherx = (PngChunkITXT)other;
            key = otherx.key;
            val = otherx.val;
            compressed = otherx.compressed;
            langTag = otherx.langTag;
            translatedTag = otherx.translatedTag;
        }

        public bool IsCompressed()
        {
            return compressed;
        }

        public void SetCompressed(bool compressed)
        {
            this.compressed = compressed;
        }

        public string GetLangtag()
        {
            return langTag;
        }

        public void SetLangtag(string langtag)
        {
            this.langTag = langtag;
        }

        public string GetTranslatedTag()
        {
            return translatedTag;
        }

        public void SetTranslatedTag(string translatedTag)
        {
            this.translatedTag = translatedTag;
        }
    }
}
