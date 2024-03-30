using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Buffers.Binary;
using System.IO.Compression;
using System.Collections.Generic;

namespace ResourceExplorer;

public class ResourcePack : TreeNode, IDisposable
{
    private readonly string _path;
    private readonly Stream _stream;
    private readonly object _streamLock = new object();

    public int Version { get; private set; }

    private readonly Dictionary<ulong, ResourceFile> _files = new();
    public IReadOnlyCollection<ResourceFile> Files => _files.Values;

    protected ResourcePack(string path)
    {
        _path = path;
        _stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        ImageIndex = SelectedImageIndex = 0;

        Text = Path.GetFileNameWithoutExtension(path);
    }

    public Stream ReadData(int size, long offset, bool compressed)
    {
        lock (_streamLock)
        {
            var dataStream = new MemoryStream(size);

            _stream.Position = offset;

            if (compressed)
            {
                var zlibStream = new ZLibStream(_stream, CompressionMode.Decompress);

                zlibStream.CopyTo(dataStream);
            }
            else
            {
                _stream.CopyTo(dataStream);
            }

            dataStream.Seek(0, SeekOrigin.Begin);

            return dataStream;
        }
    }

    public static ResourcePack? Load(string path)
    {
        var extension = Path.GetExtension(path);

        var resourcePack = new ResourcePack(path);

        return extension switch
        {
            ".pc" when resourcePack.LoadPcFile() => resourcePack,
            ".bpc" when resourcePack.LoadBpcFile() => resourcePack,
            _ => null,
        };
    }

    private bool LoadPcFile()
    {
        var br = new BinaryReader(_stream);

        // Read Header

        br.BaseStream.Position = 12;

        var fileCount = br.ReadInt32();

        br.BaseStream.Position += 36;

        Version = br.ReadInt32();

        if (Version is not 170 and not 173)
        {
            Console.WriteLine($"Unknown pack version. {Version}");
            return false;
        }

        br.BaseStream.Position += 312;

        var dataOffset = br.ReadInt32();

        br.BaseStream.Position += 12;

        var readOffset = br.BaseStream.Position + dataOffset;

        // Read Files

        for (var i = 0; i < fileCount; i++)
        {
            br.BaseStream.Position += 4;

            var fileHash = br.ReadUInt32();
            var fileSize = br.ReadInt32();
            var fileCompressedSize = br.ReadInt32();
            var fileNameOffset = br.ReadInt32();

            var fileType = br.ReadInt32().ToResourceType();

            br.BaseStream.Position += 24;

            var compressed = fileSize != fileCompressedSize;

            if (!_files.TryAdd(fileHash, new ResourceFile(this, readOffset, null, fileHash, fileNameOffset, fileSize, compressed, fileCompressedSize, fileSize, fileType)))
                Console.WriteLine($"Duplicate hash found. ( Hash: {fileHash}, Type: {fileType})");

            readOffset += fileCompressedSize;

            const int paddingSize = 16;

            var padding = fileCompressedSize % paddingSize;

            if (padding > 0)
                readOffset += paddingSize - padding;
        }

        ProcessDictionary();

        var packHashString = Path.GetFileNameWithoutExtension(_path);
        var packHash = BinaryPrimitives.ReadUInt32BigEndian(Convert.FromHexString(packHashString));

        if (Program.Names.TryGetValue(packHash, out var packName))
            Text = Path.GetFileNameWithoutExtension(packName);

        return true;
    }

    private bool LoadBpcFile()
    {
        var br = new BinaryReader(_stream);

        // Read Header

        var signature = br.ReadUInt32();

        if (signature != 0xd4c54ef5)
        {
            Console.WriteLine($"Unknown pack signature. {signature:X4}");
            return false;
        }

        Version = br.ReadInt16();

        if (Version is not 2 and not 4 and not 5 and not 6)
        {
            Console.WriteLine($"Unknown pack version. {Version}");
            return false;
        }

        var resBankCount = br.ReadInt16();

        if (resBankCount != 64)
        {
            Console.WriteLine($"Invalid resource bank count. {resBankCount}");
            return false;
        }

        br.BaseStream.Position += 8;

        var packSize = br.ReadInt64();

        if (packSize != br.BaseStream.Length)
            return false;

        var dataOffset = br.ReadInt64();
        var dataSize = br.ReadInt32();
        var fileCount = br.ReadInt32();

        // Read Files

        br.BaseStream.Position = dataOffset;

        for (var i = 0; i < fileCount; i++)
        {
            var fileOffset = br.ReadInt64();

            var fileTime = DateTime.FromFileTime(br.ReadInt64());

            br.BaseStream.Position += 8;

            var fileHash = Version != 6 ? br.ReadUInt32() : br.ReadUInt64();

            var fileNameOffset = br.ReadInt32();

            var fileSize = br.ReadInt32();

            var fileCompressedSize = br.ReadInt32();

            var fileUncompressedSize = br.ReadInt32();

            br.BaseStream.Position += 20;

            var fileFlags = br.ReadInt32();

            if (Version is 2 or 4)
                br.BaseStream.Position += 4;
            else if (Version == 5)
                br.BaseStream.Position += 12;
            else if (Version == 6)
                br.BaseStream.Position += 16;

            var fileType = (ResourceType)fileFlags;

            var compressed = (fileFlags & 0x10000) != 0;

            if (!_files.TryAdd(fileHash, new ResourceFile(this, fileOffset, fileTime, fileHash, fileNameOffset, fileSize, compressed, fileCompressedSize, fileUncompressedSize, fileType)))
                Console.WriteLine($"Duplicate hash found. ( Hash: {fileHash}, Type: {fileType})");
        }

        ProcessManifest();

        return true;
    }

    private void ProcessDictionary()
    {
        var dictionaryFile = Files.SingleOrDefault(x => x.Type == ResourceType.Dictionary);

        if (dictionaryFile is null)
            return;

        using var data = dictionaryFile.GetData();

        using var br = new BinaryReader(data);

        br.BaseStream.Position = 40;

        var count = br.ReadInt32();

        foreach (var file in Files)
        {
            if (file.Type >= ResourceType.All || file.NameOffset == -1)
                continue;

            var nameOffset = 48 + count * 16 + file.NameOffset;

            br.BaseStream.Position = nameOffset;

            var stringBuilder = new StringBuilder();

            while (br.PeekChar() != '\0')
                stringBuilder.Append(br.ReadChar());

            file.Text = stringBuilder.ToString();
        }
    }

    private void ProcessManifest()
    {
        var manifestFile = Files.SingleOrDefault(x => (int)x.Type == 42);

        if (manifestFile is null)
            return;

        using var data = manifestFile.GetData();

        using var br = new BinaryReader(data);

        foreach (var file in Files)
        {
            if (file.Type >= ResourceType.All || file.NameOffset == -1)
                continue;

            br.BaseStream.Position = file.NameOffset;

            var stringBuilder = new StringBuilder();

            while (br.PeekChar() != '\0')
                stringBuilder.Append(br.ReadChar());

            file.Text = stringBuilder.ToString();
        }
    }

    public void Dispose()
    {
        _stream.Close();
    }
}