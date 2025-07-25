// Copyright 2012    Hernán J. González    hgonzalez@gmail.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hjg.Pngcs.Zlib {

    public abstract class AZlibOutputStream : Stream {
        readonly protected Stream rawStream;
        readonly protected bool leaveOpen;
        protected int compressLevel;
        protected EDeflateCompressStrategy strategy;

        public AZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen) {
            rawStream = st;
            this.leaveOpen = leaveOpen;
            this.strategy = strat;
            this.compressLevel = compressLevel;
        }

        public override void SetLength(long value) {
            throw new NotImplementedException();
        }


        public override bool CanSeek {
            get { return false; }
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotImplementedException();
        }

        public override long Position {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override long Length {
            get { throw new NotImplementedException(); }
        }


        public override int Read(byte[] buffer, int offset, int count) {
            throw new NotImplementedException();
        }

        public override bool CanRead {
            get { return false; }
        }

        public override bool CanWrite {
            get { return true; }
        }

        public override bool CanTimeout {
            get {
                return false;
            }
        }

        /// <summary>
        /// mainly for debugging
        /// </summary>
        /// <returns></returns>
        public abstract String getImplementationId();
    }
}
