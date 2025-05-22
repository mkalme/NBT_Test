using System.IO;
using System.IO.Compression;
using zlib;

namespace WorldEditor
{
    public class Compression {
        //ZLib
        public static byte[] ZLib_Compress(byte[] bytes) {
            byte[] compressedBytes = { };

            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_DEFAULT_COMPRESSION))
            using (Stream inMemoryStream = new MemoryStream(bytes)) {
                CopyTo(inMemoryStream, outZStream);
                outZStream.finish();
                compressedBytes = outMemoryStream.ToArray();
            }

            return compressedBytes;
        }
        public static byte[] ZLib_Decompress(byte[] bytes) {
            byte[] decompressedBytes = { };

            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
            using (Stream inMemoryStream = new MemoryStream(bytes)) {
                CopyTo(inMemoryStream, outZStream);
                outZStream.finish();
                decompressedBytes = outMemoryStream.ToArray();
            }

            return decompressedBytes;
        }

        //GZip
        public static byte[] GZip_Compress(byte[] bytes) {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }
        public static byte[] GZip_Decompress(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                return mso.ToArray();
            }
        }

        //Stream Copy To
        public static void CopyTo(Stream input, Stream output)
        {
            byte[] buffer = new byte[10000];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}
