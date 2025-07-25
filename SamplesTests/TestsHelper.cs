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
using Hjg.Pngcs;


namespace SamplesTests
{

    public class TestsHelper
    {

        static string tempDir = "temp";

        public static void testEqual(string image1, string image2)
        {
            PngReader png1 = FileHelper.CreatePngReader(image1);
            PngHelperInternal.InitCrcForTests(png1);
            PngReader png2 = FileHelper.CreatePngReader(image2);
            PngHelperInternal.InitCrcForTests(png2);
            if (png1.IsInterlaced() != png2.IsInterlaced())
                fatalError("Cannot compare, one is interlaced, the other not:" + png1 + " " + png2,
                        png1, png2);
            if (!png1.ImgInfo.Equals(png2.ImgInfo))
                fatalError("Image are of different type", png1, png2);
            png1.ReadRow(png1.ImgInfo.Rows - 1);
            png2.ReadRow(png2.ImgInfo.Rows - 1);
            png1.End();
            png2.End();
            long crc1 = PngHelperInternal.GetCrctestVal(png1);
            long crc2 = PngHelperInternal.GetCrctestVal(png2);
            if (crc1 != crc2)
                fatalError("different crcs " + image1 + "=" + crc1 + " " + image2 + "=" + crc2,
                        png1, png2);
        }


        public static void testCrcEquals(string image1, long crc)
        {
            PngReader png1 = FileHelper.CreatePngReader(image1);
            PngHelperInternal.InitCrcForTests(png1);
            png1.ReadRow(png1.ImgInfo.Rows - 1);
            png1.End();
            long crc1 = PngHelperInternal.GetCrctestVal(png1);
            if (crc1 != crc)
                fatalError("different crcs", png1);
        }

        public static string getTmpFile(string suffix)
        {
            return tempDir + "/temp" + suffix + ".png";
        }

        /**
        * Creates a dummy temp png You should call endFileTmp after adding chunks, etc
        * */
        public static PngWriter prepareFileTmp(string suffix, ImageInfo imi)
        {
            PngWriter png = FileHelper.CreatePngWriter(getTmpFile(suffix), imi, true);
            return png;
        }

        public static PngWriter prepareFileTmp(string suffix, bool palette)
        {
            return prepareFileTmp(suffix, new ImageInfo(32, 32, 8, false, false, palette));
        }

        public static ImageLine generateNoiseLine(ImageInfo imi)
        { // byte format!
            ImageLine line = new ImageLine(imi, ImageLine.ESampleType.BYTE, true);
            Random r = new Random();
            r.NextBytes(line.ScanlineB);
            return line;
        }

        public static PngWriter prepareFileTmp(string suffix)
        {
            return prepareFileTmp(suffix, false);
        }

        public static void endFileTmp(PngWriter png)
        {
            ImageLine imline = new ImageLine(png.ImgInfo);
            for (int i = 0; i < png.ImgInfo.Rows; i++)
                png.WriteRow(imline, i);
            png.End();
        }

        public static PngReader getReaderTmp(string suffix)
        {
            PngReader p = FileHelper.CreatePngReader(getTmpFile(suffix));
            return p;
        }

        public static string addSuffixToName(string orig, string suffix)
        {
            string dest = System.Text.RegularExpressions.Regex.Replace(orig, @"\.png$", "");
            return dest + suffix + ".png";
        }


        public static string createWaves(string suffix, double scale, ImageInfo imi)
        {
            string f = getTmpFile(suffix);
            // open image for writing to a output stream
            PngWriter png = FileHelper.CreatePngWriter(f, imi, true);
            png.GetMetadata().SetText("key1", "val1");
            ImageLine iline = new ImageLine(imi, ImageLine.ESampleType.BYTE, true);
            for (int row = 0; row < png.ImgInfo.Rows; row++)
            {
                for (int x = 0; x < imi.Cols; x++)
                {
                    int r = (int)((Math.Sin((row + x) * 0.073 * scale) + 1) * 128);
                    int g = (int)((Math.Sin((row + x * 0.22) * 0.08 * scale) + 1) * 128);
                    int b = (int)((Math.Sin((row * 0.52 - x * 0.2) * 0.21 * scale) + 1) * 128);
                    iline.ScanlineB[x * imi.Channels] = (byte)r;
                    iline.ScanlineB[x * imi.Channels + 1] = (byte)g;
                    iline.ScanlineB[x * imi.Channels + 2] = (byte)b;
                    if (imi.Channels == 4)
                        iline.ScanlineB[x * imi.Channels + 3] = (byte)((b + g) / 2);
                }
                png.WriteRow(iline, row);
            }
            png.End();
            return f;
        }

        public static void fatalError(string s, PngReader png1, PngWriter png2)
        {
            try
            {
                png1.End();
                png2.End();
            }
            catch (Exception)
            {
            }
            throw new PngjException(s);
        }



        public static void fatalError(string s, PngReader png1, PngReader png2)
        {
            try
            {
                png1.End();
                png2.End();
            }
            catch (Exception)
            {
            }
            throw new PngjException(s);
        }
        public static void fatalError(string s, PngReader png1)
        {
            try
            {
                png1.End();
            }
            catch (Exception)
            {
            }
            throw new PngjException(s);
        }
    }
}
