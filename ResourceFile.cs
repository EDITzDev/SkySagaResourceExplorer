using System;
using System.IO;
using System.Windows.Forms;

namespace ResourceExplorer;

public class ResourceFile : TreeNode
{
    public ResourcePack Pack { get; }

    public long Offset { get; }

    public DateTime? FileTime { get; }

    public int NameOffset { get; }

    public ulong Hash { get; }

    public int Size { get; }

    public bool Compressed { get; }
    public int CompressedSize { get; }
    public int UncompressedSize { get; }

    public ResourceType Type { get; }

    public ResourceFile(ResourcePack pack, long offset, DateTime? fileTime, ulong hash, int nameOffset, int size, bool compressed, int compressedSize, int uncompressedSize, ResourceType type) : base(hash.ToString())
    {
        Pack = pack;

        Offset = offset;

        FileTime = fileTime;

        Hash = hash;

        NameOffset = nameOffset;

        Size = size;

        Compressed = compressed;
        CompressedSize = compressedSize;
        UncompressedSize = uncompressedSize;

        Type = type;

        ToolTipText = $"""
                       Offset: {Offset}
                       File Time: {FileTime}
                       Hash: {Hash}
                       Name Offset: {NameOffset}
                       Size: {Size}
                       Compressed: {Compressed}
                       Compressed Size: {CompressedSize}
                       Uncompressed Size: {UncompressedSize}
                       Type: {Type}
                       """;

        ImageIndex = SelectedImageIndex = type switch
        {
            ResourceType.Texture => 4,
            ResourceType.Actor => 5,
            ResourceType.Stringtable => 6,
            ResourceType.VertexFormat => 7,
            ResourceType.VertexBuffer => 8,
            ResourceType.IndexBuffer => 9,
            ResourceType.MaterialAppearance => 10,
            _ => 3,
        };
    }

    public Stream GetData()
    {
        return Pack.ReadData(Compressed ? UncompressedSize : Size, Offset, Compressed);
    }
}