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
    /// <summary>
    /// Wraps a set of rows from a image, read in a single operation, stored in a int[][] or byte[][] matrix
    ///
    /// They can be a subset of the total rows, but in this case they are equispaced.
    ///
    /// See also ImageLine
    /// </summary>
    public class ImageLines
    {

        public ImageInfo ImgInfo { get; private set; }
        public ImageLine.ESampleType sampleType { get; private set; }
        public bool SamplesUnpacked { get; private set; }
        public int RowOffset { get; private set; }
        public int Nrows { get; private set; }
        public int RowStep { get; private set; }
        internal readonly int channels;
        internal readonly int bitDepth;
        internal readonly int elementsPerRow;
        public int[][] Scanlines { get; private set; }
        public byte[][] ScanlinesB { get; private set; }

        public ImageLines(global::Hjg.Pngcs.ImageInfo ImgInfo, global::Hjg.Pngcs.ImageLine.ESampleType sampleType, bool unpackedMode, int rowOffset, int nRows, int rowStep)
        {
            this.ImgInfo = ImgInfo;
            channels = ImgInfo.Channels;
            bitDepth = ImgInfo.BitDepth;
            this.sampleType = sampleType;
            this.SamplesUnpacked = unpackedMode || !ImgInfo.Packed;
            this.RowOffset = rowOffset;
            this.Nrows = nRows;
            this.RowStep = rowStep;
            elementsPerRow = unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked;
            if (sampleType == ImageLine.ESampleType.INT)
            {
                Scanlines = new int[nRows][];
                for (int i = 0; i < nRows; i++) Scanlines[i] = new int[elementsPerRow];
                ScanlinesB = null;
            }
            else if (sampleType == ImageLine.ESampleType.BYTE)
            {
                ScanlinesB = new byte[nRows][];
                for (int i = 0; i < nRows; i++) ScanlinesB[i] = new byte[elementsPerRow];
                Scanlines = null;
            }
            else
                throw new PngjExceptionInternal("bad ImageLine initialization");
        }

        /// <summary>
        /// Translates from image row number to matrix row.
        /// If you are not sure if this image row in included, use better ImageRowToMatrixRowStrict
        ///
        /// </summary>
        /// <param name="imrow">Row number in the original image (from 0) </param>
        /// <returns>Row number in the wrapped matrix. Undefined result if invalid</returns>
        public int ImageRowToMatrixRow(int imrow)
        {
            int r = (imrow - RowOffset) / RowStep;
            return r < 0 ? 0 : (r < Nrows ? r : Nrows - 1);
        }

        /// <summary>
        /// translates from image row number to matrix row
        /// </summary>
        /// <param name="imrow">Row number in the original image (from 0) </param>
        /// <returns>Row number in the wrapped matrix. Returns -1 if invalid</returns>
        public int ImageRowToMatrixRowStrict(int imrow)
        {
            imrow -= RowOffset;
            int mrow = imrow >= 0 && imrow % RowStep == 0 ? imrow / RowStep : -1;
            return mrow < Nrows ? mrow : -1;
        }

        /// <summary>
        /// Translates from matrix row number to real image row number
        /// </summary>
        /// <param name="mrow"></param>
        /// <returns></returns>
        public int MatrixRowToImageRow(int mrow)
        {
            return mrow * RowStep + RowOffset;
        }
        /// <summary>
        /// Constructs and returns an ImageLine object backed by a matrix row.
        /// This is quite efficient, no deep copy.
        /// </summary>
        /// <param name="mrow">Row number inside the matrix</param>
        /// <returns></returns>
        public ImageLine GetImageLineAtMatrixRow(int mrow)
        {
            if (mrow < 0 || mrow > Nrows)
                throw new PngjException("Bad row " + mrow + ". Should be positive and less than "
                        + Nrows);
            ImageLine imline = sampleType == ImageLine.ESampleType.INT ? new ImageLine(ImgInfo, sampleType,
                    SamplesUnpacked, Scanlines[mrow], null) : new ImageLine(ImgInfo, sampleType,
                    SamplesUnpacked, null, ScanlinesB[mrow]);
            imline.Rown = MatrixRowToImageRow(mrow);
            return imline;
        }
    }
}
