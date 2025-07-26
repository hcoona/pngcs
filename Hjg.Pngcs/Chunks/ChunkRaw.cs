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
    using Hjg.Pngcs.Zlib;

    /// <summary>
    /// Wraps the raw chunk data
    /// </summary>
    /// <remarks>
    /// Short lived object, to be created while
    /// serialing/deserializing
    ///
    /// Do not reuse it for different chunks
    ///
    /// See http://www.libpng.org/pub/png/spec/1.2/PNG-Chunks.html
    ///</remarks>
    public class ChunkRaw
    {
        /// <summary>
        /// The length counts only the data field, not itself, the chunk type code, or the CRC. Zero is a valid length.
        /// Although encoders and decoders should treat the length as unsigned, its value must not exceed 2^31-1 bytes.
        /// </summary>
        public readonly int Len;
        /// <summary>
        /// Chunk Id, as array of 4 bytes
        /// </summary>
        public readonly byte[] IdBytes;
        public readonly string Id;
        /// <summary>
        /// Raw data, crc not included
        /// </summary>
        public byte[] Data;
        private int crcval;

        /// <summary>
        /// Creates an empty raw chunk
        /// </summary>
        internal ChunkRaw(int length, string idb, bool alloc)
        {
            this.Id = idb;
            this.IdBytes = ChunkHelper.ToBytes(Id);
            this.Data = null;
            this.crcval = 0;
            this.Len = length;
            if (alloc)
                AllocData();
        }

        internal ChunkRaw(int length, byte[] idbytes, bool alloc) : this(length, ChunkHelper.ToString(idbytes), alloc)
        {
        }

        /// <summary>
        /// Called after setting data, before writing to os
        /// </summary>
        private int ComputeCrc()
        {
            CRC32 crcengine = PngHelperInternal.GetCRC();
            crcengine.Reset();
            crcengine.Update(IdBytes, 0, 4);
            if (Len > 0)
                crcengine.Update(Data, 0, Len); //
            return (int)crcengine.GetValue();
        }


        internal void WriteChunk(Stream os)
        {
            if (IdBytes.Length != 4)
                throw new PngjOutputException("bad chunkid [" + ChunkHelper.ToString(IdBytes) + "]");
            crcval = ComputeCrc();
            PngHelperInternal.WriteInt4(os, Len);
            PngHelperInternal.WriteBytes(os, IdBytes);
            if (Len > 0)
                PngHelperInternal.WriteBytes(os, Data, 0, Len);
            //Console.WriteLine("writing chunk " + this.ToString() + "crc=" + crcval);
            PngHelperInternal.WriteInt4(os, crcval);
        }

        /// <summary>
        /// Position before: just after chunk id. positon after: after crc Data should
        /// be already allocated. Checks CRC Return number of byte read.
        /// </summary>
        ///
        internal int ReadChunkData(Stream stream, bool checkCrc)
        {
            PngHelperInternal.ReadBytes(stream, Data, 0, Len);
            crcval = PngHelperInternal.ReadInt4(stream);
            if (checkCrc)
            {
                int crc = ComputeCrc();
                if (crc != crcval)
                    throw new PngjBadCrcException("crc invalid for chunk " + ToString() + " calc="
                            + crc + " read=" + crcval);
            }
            return Len + 4;
        }

        internal MemoryStream GetAsByteStream()
        { // only the data
            return new MemoryStream(Data);
        }

        private void AllocData()
        {
            if (Data == null || Data.Length < Len)
                Data = new byte[Len];
        }
        /// <summary>
        /// Just id and length
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "chunkid=" + ChunkHelper.ToString(IdBytes) + " len=" + Len;
        }
    }
}
