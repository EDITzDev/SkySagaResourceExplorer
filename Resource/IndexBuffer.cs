using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ResourceExplorer;

public partial class IndexBuffer : UserControl
{
    public IndexBuffer()
    {
        InitializeComponent();

        Dock = DockStyle.Fill;
    }

    public static IndexBuffer? Create(ResourceFile file)
    {
        var indexList = LoadFile(file);

        if(!indexList.Any())
            return null;

        var indexBuffer = new IndexBuffer();

        foreach (var index in indexList)
            indexBuffer.dataGridView.Rows.Add(new object[] { index });

        return indexBuffer;
    }

    public static IList<int> LoadFile(ResourceFile file)
    {
        using var data = file.GetData();

        using var br = new BinaryReader(data);

        br.BaseStream.Position = 64;

        var offset = br.ReadInt32();

        br.BaseStream.Position += 36;

        var count = br.ReadInt32();
        var size = br.ReadInt32();

        br.BaseStream.Position = offset;

        var indexBuffer = new List<int>();

        for (var i = 0; i < count; i++)
            indexBuffer.Add(size == 2 ? br.ReadInt16() : br.ReadInt32());

        return indexBuffer;
    }
}