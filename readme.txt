 ==== PNGCS : A small library to read/write huge PNG files in C# ===

PngCs is a C# to read/write PNG images. 

It provides a simple API for progressive (sequential line-oriented) reading and writing. 
It's specially suitable for huge images, which one does not want to load fully in memory.

It supports all PNG spec color models and bitdepths: RGB8/RGB16/RGBA8/RGBA16, G8/4/2/1,
GA8/4/2/1, PAL8/4/2/1,  all filters/compressions settings. It does not support interlaced images. 
It also has support for Chunks (metadata).

This is a port of the PngJ library (Java): http://code.google.com/p/pngj/
the API, documentation and samples from PNGJ apply also
to this PngCs library: http://code.google.com/p/pngj/wiki/Overview

The distribution of this library includes documentation in folder docs/

See also the included sample projects, 

--------------------------------------------------------------

NOTE: Since version 1.1.4 two assemblies are provided for different environments:

1 For .Net 4.5 :  dotnet45/png45.dll
This uses an internal Zlib  implementation based on the new .Net 4.5 DeflateStream

Advantages:
     Single dll, does not requires SharpZipLib dll
     Better speed

2 .Net 2.0 compatible: dotnet20/png.dll 
This requires an extra dll (included) ICSharpCode.SharpZipLib.dll

Advantages:
     Works with old .net versions (2.0 and above)
     Slightly better compression


------------------------------------------------------------------------

The ICSharpCode.SharpZipLib.dll assembly, provided with this library,
must be referenced together with pngcs.dll by client projects.
Because SharpZipLib is released  under the GPL license with an exception
that allows to link it with independent modules, PNGCS relies on that
exception and is released under the Apache license. See LICENSE.txt

-----------------------------------------------------------------------------

History: 

See changes.txt

Hernan J Gonzalez - hgonzalez@gmail.com -  http://stackoverflow.com/users/277304/leonbloy

---------------------------------------------------------------------------------

## License

This project is licensed under the GNU General Public License v3.0 or later (GPL v3+).  
See `LICENSE.GPL.txt` for details.

Portions of the code are derived from an open source project originally licensed under the Apache License, Version 2.0 by Hernán J. González.  
The original copyright and license notice are preserved in accordance with the Apache License.

For more information, see the files `LICENSE.GPL.txt` (GPL v3+) and `LICENSE.APACHE.txt` (Apache 2.0) in this repository.
