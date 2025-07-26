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
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Hjg.Pngcs.Zlib
{

    public abstract class AZlibInputStream : Stream
    {
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        protected readonly Stream rawStream;
        [SuppressMessage(
            "Design",
            "CA1051:Do not declare visible instance fields",
            Justification = "Allow public readonly fields.")]
        protected readonly bool leaveOpen;

        public AZlibInputStream(Stream st, bool leaveOpen)
        {
            rawStream = st;
            this.leaveOpen = leaveOpen;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }


        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanTimeout
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// mainly for debugging
        /// </summary>
        /// <returns></returns>
        public abstract string getImplementationId();
    }
}
