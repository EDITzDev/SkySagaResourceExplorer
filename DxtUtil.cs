using System.IO;
using System.Runtime.CompilerServices;

namespace ResourceExplorer;

public static class DxtUtil
{
    internal static byte[] DecompressDxt1(Stream stream, int width, int height)
    {
        var data = new byte[width * height * 4];

        using var reader = new BinaryReader(stream);

        var blockCountX = (width + 3) / 4;
        var blockCountY = (height + 3) / 4;

        for (var y = 0; y < blockCountY; y++) 
            for (var x = 0; x < blockCountX; x++)
                DecompressDxt1Block(reader, x, y, blockCountX, width, height, ref data);

        return data;
    }

    private static void DecompressDxt1Block(BinaryReader reader, int x, int y, int blockCountX, int width, int height, ref byte[] imageData)
    {
        var c0 = reader.ReadUInt16();
        var c1 = reader.ReadUInt16();

        ConvertRgb565ToRgb888(c0, out var r0, out var g0, out var b0);
        ConvertRgb565ToRgb888(c1, out var r1, out var g1, out var b1);

        var lookupTable = reader.ReadUInt32();

        for (var blockY = 0; blockY < 4; blockY++)
        {
            for (var blockX = 0; blockX < 4; blockX++)
            {
                byte a = 255;

                var index = (lookupTable >> 2 * (4 * blockY + blockX)) & 0x03;

                byte r = index switch
                {
                    0 => r0,
                    1 => r1,
                    2 => (byte)(c0 > c1 ? ((2 * r0 + r1) / 3) : ((r0 + r1) / 2)),
                    3 => (byte)(c0 > c1 ? ((r0 + 2 * r1) / 3) : 0),
                    _ => 0
                };

                byte g = index switch
                {
                    0 => g0,
                    1 => g1,
                    2 => (byte)(c0 > c1 ? ((2 * g0 + g1) / 3) : (byte)((g0 + g1) / 2)),
                    3 => (byte)(c0 > c1 ? ((g0 + 2 * g1) / 3) : 0),
                    _ => 0
                };

                byte b = index switch
                {
                    0 => b0,
                    1 => b1,
                    2 => (byte)(c0 > c1 ? ((2 * b0 + b1) / 3) : (byte)((b0 + b1) / 2)),
                    3 => (byte)(c0 > c1 ? ((b0 + 2 * b1) / 3) : 0),
                    _ => 0
                };

                var px = (x << 2) + blockX;
                var py = (y << 2) + blockY;

                if ((px < width) && (py < height))
                {
                    var offset = ((py * width) + px) << 2;

                    imageData[offset] = r;
                    imageData[offset + 1] = g;
                    imageData[offset + 2] = b;
                    imageData[offset + 3] = a;
                }
            }
        }
    }

    internal static byte[] DecompressDxt3(Stream stream, int width, int height)
    {
        var data = new byte[width * height * 4];

        using var reader = new BinaryReader(stream);

        var blockCountX = (width + 3) / 4;
        var blockCountY = (height + 3) / 4;

        for (var y = 0; y < blockCountY; y++)
            for (var x = 0; x < blockCountX; x++)
                DecompressDxt3Block(reader, x, y, blockCountX, width, height, ref data);

        return data;
    }

    private static void DecompressDxt3Block(BinaryReader imageReader, int x, int y, int blockCountX, int width, int height, ref byte[] imageData)
    {
        var a0 = imageReader.ReadByte();
        var a1 = imageReader.ReadByte();
        var a2 = imageReader.ReadByte();
        var a3 = imageReader.ReadByte();
        var a4 = imageReader.ReadByte();
        var a5 = imageReader.ReadByte();
        var a6 = imageReader.ReadByte();
        var a7 = imageReader.ReadByte();

        var c0 = imageReader.ReadUInt16();
        var c1 = imageReader.ReadUInt16();

        ConvertRgb565ToRgb888(c0, out var r0, out var g0, out var b0);
        ConvertRgb565ToRgb888(c1, out var r1, out var g1, out var b1);

        var lookupTable = imageReader.ReadUInt32();

        var alphaIndex = 0;

        for (var blockY = 0; blockY < 4; blockY++)
        {
            for (var blockX = 0; blockX < 4; blockX++)
            {
                var index = (lookupTable >> 2 * (4 * blockY + blockX)) & 0x03;

                byte a = alphaIndex switch
                {
                    0 => (byte)((a0 & 0x0F) | ((a0 & 0x0F) << 4)),
                    1 => (byte)((a0 & 0xF0) | ((a0 & 0xF0) >> 4)),
                    2 => (byte)((a1 & 0x0F) | ((a1 & 0x0F) << 4)),
                    3 => (byte)((a1 & 0xF0) | ((a1 & 0xF0) >> 4)),
                    4 => (byte)((a2 & 0x0F) | ((a2 & 0x0F) << 4)),
                    5 => (byte)((a2 & 0xF0) | ((a2 & 0xF0) >> 4)),
                    6 => (byte)((a3 & 0x0F) | ((a3 & 0x0F) << 4)),
                    7 => (byte)((a3 & 0xF0) | ((a3 & 0xF0) >> 4)),
                    8 => (byte)((a4 & 0x0F) | ((a4 & 0x0F) << 4)),
                    9 => (byte)((a4 & 0xF0) | ((a4 & 0xF0) >> 4)),
                    10 => (byte)((a5 & 0x0F) | ((a5 & 0x0F) << 4)),
                    11 => (byte)((a5 & 0xF0) | ((a5 & 0xF0) >> 4)),
                    12 => (byte)((a6 & 0x0F) | ((a6 & 0x0F) << 4)),
                    13 => (byte)((a6 & 0xF0) | ((a6 & 0xF0) >> 4)),
                    14 => (byte)((a7 & 0x0F) | ((a7 & 0x0F) << 4)),
                    15 => (byte)((a7 & 0xF0) | ((a7 & 0xF0) >> 4)),
                    _ => 255
                };

                ++alphaIndex;

                byte r = index switch
                {
                    0 => r0,
                    1 => r1,
                    2 => (byte)((2 * r0 + r1) / 3),
                    3 => (byte)((r0 + 2 * r1) / 3),
                    _ => 0
                };

                byte g = index switch
                {
                    0 => g0,
                    1 => g1,
                    2 => (byte)((2 * g0 + g1) / 3),
                    3 => (byte)((g0 + 2 * g1) / 3),
                    _ => 0
                };

                byte b = index switch
                {
                    0 => b0,
                    1 => b1,
                    2 => (byte)((2 * b0 + b1) / 3),
                    3 => (byte)((b0 + 2 * b1) / 3),
                    _ => 0
                };

                var px = (x << 2) + blockX;
                var py = (y << 2) + blockY;

                if ((px < width) && (py < height))
                {
                    var offset = ((py * width) + px) << 2;

                    imageData[offset] = r;
                    imageData[offset + 1] = g;
                    imageData[offset + 2] = b;
                    imageData[offset + 3] = a;
                }
            }
        }
    }

    internal static byte[] DecompressDxt5(Stream stream, int width, int height)
    {
        var data = new byte[width * height * 4];

        using var reader = new BinaryReader(stream);

        var blockCountX = (width + 3) / 4;
        var blockCountY = (height + 3) / 4;

        for (var y = 0; y < blockCountY; y++)
            for (var x = 0; x < blockCountX; x++)
                DecompressDxt5Block(reader, x, y, blockCountX, width, height, ref data);

        return data;
    }

    private static void DecompressDxt5Block(BinaryReader imageReader, int x, int y, int blockCountX, int width, int height, ref byte[] imageData)
    {
        var alpha0 = imageReader.ReadByte();
        var alpha1 = imageReader.ReadByte();

        ulong alphaMask = (ulong)imageReader.ReadByte();
        alphaMask += (ulong)imageReader.ReadByte() << 8;
        alphaMask += (ulong)imageReader.ReadByte() << 16;
        alphaMask += (ulong)imageReader.ReadByte() << 24;
        alphaMask += (ulong)imageReader.ReadByte() << 32;
        alphaMask += (ulong)imageReader.ReadByte() << 40;

        var c0 = imageReader.ReadUInt16();
        var c1 = imageReader.ReadUInt16();

        ConvertRgb565ToRgb888(c0, out var r0, out var g0, out var b0);
        ConvertRgb565ToRgb888(c1, out var r1, out var g1, out var b1);

        var lookupTable = imageReader.ReadUInt32();

        for (var blockY = 0; blockY < 4; blockY++)
        {
            for (var blockX = 0; blockX < 4; blockX++)
            {
                var index = (lookupTable >> 2 * (4 * blockY + blockX)) & 0x03;

                var alphaIndex = (uint)((alphaMask >> 3 * (4 * blockY + blockX)) & 0x07);

                byte a;

                if (alphaIndex == 0)
                {
                    a = alpha0;
                }
                else if (alphaIndex == 1)
                {
                    a = alpha1;
                }
                else if (alpha0 > alpha1)
                {
                    a = (byte)(((8 - alphaIndex) * alpha0 + (alphaIndex - 1) * alpha1) / 7);
                }
                else if (alphaIndex == 6)
                {
                    a = 0;
                }
                else if (alphaIndex == 7)
                {
                    a = 0xff;
                }
                else
                {
                    a = (byte)(((6 - alphaIndex) * alpha0 + (alphaIndex - 1) * alpha1) / 5);
                }

                byte r = index switch
                {
                    0 => r0,
                    1 => r1,
                    2 => (byte)((2 * r0 + r1) / 3),
                    3 => (byte)((r0 + 2 * r1) / 3),
                    _ => 0
                };

                byte g = index switch
                {
                    0 => g0,
                    1 => g1,
                    2 => (byte)((2 * g0 + g1) / 3),
                    3 => (byte)((g0 + 2 * g1) / 3),
                    _ => 0
                };

                byte b = index switch
                {
                    0 => b0,
                    1 => b1,
                    2 => (byte)((2 * b0 + b1) / 3),
                    3 => (byte)((b0 + 2 * b1) / 3),
                    _ => 0
                };

                var px = (x << 2) + blockX;
                var py = (y << 2) + blockY;

                if ((px < width) && (py < height))
                {
                    var offset = ((py * width) + px) << 2;

                    imageData[offset] = r;
                    imageData[offset + 1] = g;
                    imageData[offset + 2] = b;
                    imageData[offset + 3] = a;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ConvertRgb565ToRgb888(ushort color, out byte r, out byte g, out byte b)
    {
        var temp = (color >> 11) * 255 + 16;
        r = (byte)((temp / 32 + temp) / 32);

        temp = ((color & 0x07E0) >> 5) * 255 + 32;
        g = (byte)((temp / 64 + temp) / 64);

        temp = (color & 0x001F) * 255 + 16;
        b = (byte)((temp / 32 + temp) / 32);
    }
}