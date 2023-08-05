using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ResourceExplorer;

public partial class StringTable : UserControl
{
    public StringTable()
    {
        InitializeComponent();

        Dock = DockStyle.Fill;
    }

    public static StringTable? Create(ResourceFile file)
    {
        var stringList = LoadFile(file);

        if(!stringList.Any())
            return null;

        var stringTable = new StringTable();

        foreach (var stringRow in stringList)
            stringTable.dataGridView.Rows.Add(stringRow.Key, stringRow.Value);

        return stringTable;
    }

    public static IDictionary<uint, string> LoadFile(ResourceFile file)
    {
        using var data = file.GetData();

        using var br = new BinaryReader(data);

        br.BaseStream.Position = 24;

        var dataOffset = br.ReadInt32();

        br.BaseStream.Position += 20;

        var stringCount = br.ReadInt32();

        var flags = br.ReadByte();

        // TODO
        if ((flags & 1) != 0)
            return ImmutableDictionary<uint, string>.Empty;

        br.BaseStream.Position = dataOffset;

        var stringTable = new Dictionary<uint, string>();

        for (var i = 0; i < stringCount; i++)
        {
            br.BaseStream.Position += 8;

            var stringOffset = br.ReadInt32();

            br.BaseStream.Position += 20;

            var stringLength = br.ReadInt32();
            var stringHash = br.ReadUInt32();

            var tempPosition = br.BaseStream.Position;

            br.BaseStream.Position = stringOffset;

            var stringBytes = br.ReadBytes(stringLength * 2);

            stringTable.Add(stringHash, Encoding.Unicode.GetString(stringBytes));

            br.BaseStream.Position = tempPosition;
        }

        return stringTable;
    }

    public static void Export(string directory, ResourceFile file)
    {
        var stringTable = LoadFile(file);

        if (!stringTable.Any())
            return;

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var savePath = Path.Combine(directory, $"{file.Text}.txt");

        File.WriteAllLines(savePath, stringTable.Values);
    }
}