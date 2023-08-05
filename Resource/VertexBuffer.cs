using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ResourceExplorer;

public partial class VertexBuffer : UserControl
{
    public VertexBuffer()
    {
        InitializeComponent();

        Dock = DockStyle.Fill;
    }

    public static VertexBuffer? Create(ResourceFile file)
    {
        var vertexList = LoadFile(file, out var formatEntries);

        if(!vertexList.Any())
            return null;

        var vertexBuffer = new VertexBuffer();

        var primaryFormatEntries = formatEntries.Where(x => x.Stream == 0);

        vertexBuffer.formatdataGridView.DataSource = primaryFormatEntries.ToList();

        foreach (var entry in primaryFormatEntries)
        {
            var columnName = entry.Usage.ToString();

            var column = new DataGridViewTextBoxColumn
            {
                Name = columnName,
                HeaderText = columnName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            vertexBuffer.vertexdataGridView.Columns.Add(column);
        }

        foreach (var vertex in vertexList)
        {
            var index = vertexBuffer.vertexdataGridView.Rows.Add();

            var row = vertexBuffer.vertexdataGridView.Rows[index];

            foreach (var entry in primaryFormatEntries)
            {
                var columnName = entry.Usage.ToString();

                switch (entry.Type)
                {
                    case VertexFormat.Type.Color:
                        if (entry.Dimension == 4)
                        {
                            var a = vertex[entry.Offset];
                            var r = vertex[entry.Offset + 1];
                            var g = vertex[entry.Offset + 2];
                            var b = vertex[entry.Offset + 3];
                            row.Cells[columnName].Value = Color.FromArgb(a, r, g, b);
                        }
                        break;

                    case VertexFormat.Type.UByte:
                        row.Cells[columnName].Value = vertex[entry.Offset];
                        break;

                    case VertexFormat.Type.Short:
                        row.Cells[columnName].Value = BitConverter.ToInt16(vertex, entry.Offset);
                        break;

                    case VertexFormat.Type.Dec:
                        break;

                    case VertexFormat.Type.Float16:
                        {
                            var floats = new float[entry.Dimension];

                            for (var i = 0; i < entry.Dimension; i++)
                                floats[i] = (float)BitConverter.ToHalf(vertex, entry.Offset + (i * 2));

                            row.Cells[columnName].Value = entry.Dimension switch
                            {
                                2 => new Vector2(floats),
                                3 => new Vector3(floats),
                                4 => new Vector4(floats),
                                _ => new NotImplementedException()
                            };
                        }
                        break;

                    case VertexFormat.Type.Float:
                        {
                            var floats = new float[entry.Dimension];

                            for (var i = 0; i < entry.Dimension; i++)
                                floats[i] = BitConverter.ToSingle(vertex, entry.Offset + (i * 4));

                            row.Cells[columnName].Value = entry.Dimension switch
                            {
                                2 => new Vector2(floats),
                                3 => new Vector3(floats),
                                4 => new Vector4(floats),
                                _ => new NotImplementedException()
                            };
                        }
                        break;
                }
            }
        }

        return vertexBuffer;
    }

    public static IEnumerable<byte[]> LoadFile(ResourceFile file, out IEnumerable<VertexFormat.Entry> formatEntries)
    {
        using var data = file.GetData();

        using var br = new BinaryReader(data);

        br.BaseStream.Position = 52;

        var count = br.ReadInt32();

        var offset = br.ReadInt32();

        br.BaseStream.Position += 52;

        var size = br.ReadInt32();

        br.BaseStream.Position += file.Pack.Version == 6 ? 12 : 8;

        var formatHash = file.Pack.Version == 6 ? br.ReadUInt64() : br.ReadUInt32();

        br.BaseStream.Position = offset;

        var vertexBuffer = new List<byte[]>();

        for (var i = 0; i < count; i++)
            vertexBuffer.Add(br.ReadBytes(size));

        // Load format

        var format = Util.FindFile(formatHash);

        if (format is null)
        {
            formatEntries = Enumerable.Empty<VertexFormat.Entry>();
            return Enumerable.Empty<byte[]>();
        }

        formatEntries = VertexFormat.LoadFile(format);

        return vertexBuffer;
    }
}