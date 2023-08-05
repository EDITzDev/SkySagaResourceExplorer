using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ResourceExplorer;

public partial class VertexFormat : UserControl
{
    public enum Type : int
    {
        Color = 1,
        UByte,
        Short,

        Dec = 5,

        Float16 = 7,
        Float
    }

    public enum TypeFlag : byte
    {
        None,

        Signed = 1 << 0,
        Normalized = 1 << 1,

        Both = Signed | Normalized
    }

    public enum Usage : int
    {
        Position,
        Normal,
        Binormal,
        Tangent,
        Color0,
        Color1,
        TexCoord0,
        TexCoord1,
        TexCoord2,
        TexCoord3,
        TexCoord4,
        TexCoord5,
        TexCoord6,
        TexCoord7,
        TexCoord8,
        TexCoord9,
        TexCoord10,
        TexCoord11,
        TexCoord12,
        TexCoord13,
        TexCoord14,
        TexCoord15
    }

    public class Entry
    {
        public byte Stream { get; set; }
        public byte Offset { get; set; }
        public byte Dimension { get; set; }
        public TypeFlag TypeFlags { get; set; }
        public int Unknown { get; set; }
        public Type Type { get; set; }
        public Usage Usage { get; set; }
        public int Unknown2 { get; set; }
    }

    public VertexFormat()
    {
        InitializeComponent();

        Dock = DockStyle.Fill;
    }

    public static VertexFormat? Create(ResourceFile file)
    {
        var entries = LoadFile(file);

        if(!entries.Any())
            return null;

        var vertexFormat = new VertexFormat();

        vertexFormat.dataGridView.DataSource = entries;

        return vertexFormat;
    }

    public static IEnumerable<Entry> LoadFile(ResourceFile file)
    {
        using var data = file.GetData();

        using var br = new BinaryReader(data);

        br.BaseStream.Position = 33;

        var entryCount = br.ReadByte();

        if (entryCount <= 0)
            return Enumerable.Empty<Entry>();

        br.BaseStream.Position += 6;

        var entries = new List<Entry>();

        for (var i = 0; i < entryCount; i++)
        {
            entries.Add(new Entry
            {
                Stream = br.ReadByte(),
                Offset = br.ReadByte(),
                Dimension = br.ReadByte(),
                TypeFlags = (TypeFlag)br.ReadByte(),
                Unknown = br.ReadInt32(),
                Type = (Type)br.ReadInt32(),
                Usage = (Usage)br.ReadInt32(),
                Unknown2 = br.ReadInt32(),
            });
        }

        return entries;
    }
}