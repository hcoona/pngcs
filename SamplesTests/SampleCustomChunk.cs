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

using System;
using System.IO;
using System.Xml.Serialization;
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
/**
 * This example shows how to register a custom chunk.
 */
namespace SamplesTests
{

    public class SampleCustomChunk
    {

        public static void testWrite(string src, string target)
        {
            // for writing is not necesary to register
            DummyClass c = new DummyClass();
            c.name = "Hernán";
            c.age = 45;

            PngReader pngr = FileHelper.CreatePngReader(src);
            PngWriter pngw = FileHelper.CreatePngWriter(target, pngr.ImgInfo, true);
            pngw.CopyChunksFirst(pngr, ChunkCopyBehaviour.COPY_ALL_SAFE);
            PngChunkSERI mychunk = new PngChunkSERI(pngw.ImgInfo);
            mychunk.SetObj(c);
            mychunk.Priority = true; // if we want it to be written as soon as possible
            pngw.GetChunksList().Queue(mychunk);
            for (int row = 0; row < pngr.ImgInfo.Rows; row++)
            {
                ImageLine l1 = pngr.ReadRow(row);
                pngw.WriteRow(l1, row);
            }
            pngw.CopyChunksLast(pngr, ChunkCopyBehaviour.COPY_ALL);
            pngr.End();
            pngw.End();
            Console.Out.WriteLine("Done. Writen : " + target);
        }

        public static void testRead(string file)
        {
            // register with factory chunk
            PngChunk.FactoryRegister(PngChunkSERI.ID, typeof(PngChunkSERI));
            // read all file
            PngReader pngr = FileHelper.CreatePngReader(file);
            pngr.ReadSkippingAllRows();
            pngr.End();
            // we assume there can be at most one chunk of this type...
            PngChunk chunk = pngr.GetChunksList().GetById1(PngChunkSERI.ID); // This would work even if not registered, but then PngChunk would be of type PngChunkUNKNOWN
            Console.Out.WriteLine(chunk);
            // the following would fail if we had not register the chunk
            PngChunkSERI chunkprop = (PngChunkSERI)chunk;
            string name = chunkprop.GetObj().name;
            int age = chunkprop.GetObj().age;
            Console.Out.WriteLine("Done. Name: " + name + " age=" + age);
        }

    }

    // Dummy class to exemplify the serialization/deserialization
    public class DummyClass
    {
        public string name;
        public int age;
    }

    // Example chunk: this stores a serializable object
    public class PngChunkSERI : PngChunkSingle
    {
        // ID must follow the PNG conventions: four ascii letters,
        // ID[0] : lowercase (ancillary)
        // ID[1] : lowercase if private, upppecase if public
        // ID[3] : uppercase if "safe to copy"
        public readonly static string ID = "seRi";

        private DummyClass obj;

        public PngChunkSERI(ImageInfo info)
            : base(ID, info)
        {
            obj = new DummyClass();
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            // change this if you don't require this chunk to be before IDAT, etc
            return ChunkOrderingConstraint.BEFORE_IDAT;
        }

        // in this case, we have that the chunk data corresponds to the serialized object
        public override ChunkRaw CreateRawChunk()
        {
            ChunkRaw c = null;
            XmlSerializer serializerObj = new XmlSerializer(typeof(DummyClass));
            MemoryStream bos = new MemoryStream();
            serializerObj.Serialize(bos, obj);
            byte[] arr = bos.ToArray();
            c = createEmptyChunk(arr.Length, true);
            c.Data = arr;
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            XmlSerializer serializerObj = new XmlSerializer(typeof(DummyClass));
            obj = (DummyClass)serializerObj.Deserialize(new MemoryStream(c.Data));
        }

        public override void CloneDataFromRead(PngChunk other)
        {
            PngChunkSERI otherx = (PngChunkSERI)other;
            this.obj = otherx.obj; // shallow clone, we could implement other copying
        }

        public DummyClass GetObj()
        {
            return obj;
        }

        public void SetObj(DummyClass o)
        {
            this.obj = o;
        }

    }

}
