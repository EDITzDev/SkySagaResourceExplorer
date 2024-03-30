using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;

namespace ResourceExplorer;

public partial class Actor : UserControl
{
    private readonly ResourcePack _pack;
    private readonly TabControl _tabControl;

    public class Mesh
    {
        public int Lod { get; set; }
        public int Material { get; set; }

        public ulong IndexBufferHash { get; set; }
        public ulong VertexBufferHash { get; set; }
        public ulong MaterialAppearanceHash { get; set; } // Quality needs to be applied, Eg: .ultra
    }

    public Actor(ResourcePack pack, TabControl tabControl)
    {
        _pack = pack;
        _tabControl = tabControl;

        InitializeComponent();

        Dock = DockStyle.Fill;
    }

    public static Actor? Create(ResourceFile file, TabControl tabControl)
    {
        var meshes = LoadFile(file);

        if (meshes is null)
            return null;

        var actor = new Actor(file.Pack, tabControl);

        actor.meshesLabel.Text = $"Meshes: {meshes.Count}";

        actor.dataGridView.DataSource = meshes;

        return actor;
    }

    private void dataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.ColumnIndex is not 2 and not 3 and not 4)
            return;

        var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

        if (cell is null || cell.Value is not ulong hash)
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        // Material Appearence
        if (e.ColumnIndex == 4)
            hash = hash < uint.MaxValue ? Util.ComputeCrc32(".ultra", (uint)hash) : Util.ComputeCrc64(".ultra", hash);

        var file = Util.FindFile(_pack, hash) ?? Util.FindFile(hash);

        if (file is null)
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        var control = e.ColumnIndex switch
        {
            2 => CreateIndexBuffer(file),
            3 => CreateVertexBuffer(file),
            4 => CreateMaterialAppearance(file),
            _ => null
        };

        if (control is null)
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        _tabControl.TabPages.Add(control);

        _tabControl.SelectedTab = control;
        _tabControl.SelectedTab.Focus();
    }

    private TabPage? CreateIndexBuffer(ResourceFile file)
    {
        var indexBuffer = IndexBuffer.Create(file);

        if (indexBuffer is null)
        {
            SystemSounds.Exclamation.Play();
            return null;
        }

        var tabPage = new TabPage
        {
            Text = $"{file.Text}    "
        };

        tabPage.Controls.Add(indexBuffer);

        return tabPage;
    }

    private TabPage? CreateVertexBuffer(ResourceFile file)
    {
        var vertexBuffer = VertexBuffer.Create(file);

        if (vertexBuffer is null)
        {
            SystemSounds.Exclamation.Play();
            return null;
        }

        var tabPage = new TabPage
        {
            Text = $"{file.Text}    "
        };

        tabPage.Controls.Add(vertexBuffer);

        return tabPage;
    }

    private TabPage? CreateMaterialAppearance(ResourceFile file)
    {
        var materialAppearance = MaterialAppearance.Create(file, _tabControl);

        if (materialAppearance is null)
        {
            SystemSounds.Exclamation.Play();
            return null;
        }

        var tabPage = new TabPage
        {
            Text = $"{file.Text}    "
        };

        tabPage.Controls.Add(materialAppearance);

        return tabPage;
    }

    public static ICollection<Mesh>? LoadFile(ResourceFile file)
    {
        // TODO: Old version stores index/vertex buffer in the actor data.
        if (file.Pack.Version == 170)
            return null;

        using var data = file.GetData();

        using var br = new BinaryReader(data);

        br.BaseStream.Position = file.Pack.Version == 6 ? 17 : 9;

        var version = br.ReadByte();

        if (version != 1)
            return null;

        br.BaseStream.Position = 24;

        var meshOffset = br.ReadInt32(); br.BaseStream.Position += 4;
        var meshOffset2 = br.ReadInt32(); br.BaseStream.Position += 4;

        br.BaseStream.Position += 96;

        var meshCount = br.ReadInt16();

        var lodCount = br.ReadByte();
        var materialCount = br.ReadByte();

        if (meshOffset > 0)
        {
            var meshes = new List<Mesh>();

            br.BaseStream.Position = meshOffset;

            var meshOffsets = new int[materialCount, lodCount];

            for (var m = 0; m < materialCount; m++)
            {
                for (var l = 0; l < lodCount; l++)
                {
                    meshOffsets[m, l] = br.ReadInt32(); br.BaseStream.Position += 4;
                }
            }

            for (var m = 0; m < materialCount; m++)
            {
                for (var l = 0; l < lodCount; l++)
                {
                    var mesh = ReadMesh(br, meshOffsets[m, l], file.Pack.Version);

                    mesh.Lod = l;
                    mesh.Material = m;

                    meshes.Add(mesh);
                }
            }

            return meshes;
        }

        if (meshOffset2 > 0)
        {
            var meshes = new List<Mesh>();

            br.BaseStream.Position = meshOffset2;

            br.BaseStream.Position += 184;

            if (br.ReadInt32() != lodCount || br.ReadInt32() != materialCount)
            {
                Console.WriteLine($"Mesh lod/quality count mismatch. {file.Text}");
                return null;
            }

            br.BaseStream.Position += 128;

            var meshOffsets = new int[materialCount];

            for (var m = 0; m < materialCount; m++)
            {
                meshOffsets[m] = br.ReadInt32(); br.BaseStream.Position += 4;
            }

            for (var m = 0; m < materialCount; m++)
            {
                var tempMeshOffset = meshOffsets[m];

                for (var l = 0; l < lodCount; l++)
                {
                    var mesh = ReadMesh(br, tempMeshOffset, file.Pack.Version);

                    mesh.Lod = l;
                    mesh.Material = m;

                    meshes.Add(mesh);

                    tempMeshOffset += 112;
                }
            }

            return meshes;
        }

        return null;
    }

    private static Mesh ReadMesh(BinaryReader br, int offset, int version)
    {
        var mesh = new Mesh();

        br.BaseStream.Position = offset;

        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        var offset1 = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        var offset2 = br.ReadInt32(); br.BaseStream.Position += 4;

        mesh.IndexBufferHash = version == 6 ? br.ReadUInt64() : br.ReadUInt32();

        br.BaseStream.Position = offset1;

        if (version != 6)
            br.BaseStream.Position += 4;

        mesh.VertexBufferHash = version == 6 ? br.ReadUInt64() : br.ReadUInt32();

        br.BaseStream.Position = offset2;

        br.BaseStream.Position += version == 6 ? 16 : 24;

        mesh.MaterialAppearanceHash = version == 6 ? br.ReadUInt64() : br.ReadUInt32();

        return mesh;
    }

    public static void Export(string directory, ResourceFile file, bool allLods = false, bool allMaterials = true)
    {
        var meshes = LoadFile(file);

        if (meshes is null)
        {
            Console.WriteLine($"No meshes found in actor. {file.Text}");
            return;
        }

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        foreach (var mesh in meshes.Where(x => (allLods || x.Lod == 0) && (allMaterials || x.Material == 0)))
        {
            var vertexBufferFile = Util.FindFile(file.Pack, mesh.VertexBufferHash) ?? Util.FindFile(mesh.VertexBufferHash);
            var indexBufferFile = Util.FindFile(file.Pack, mesh.IndexBufferHash) ?? Util.FindFile(mesh.IndexBufferHash);

            var materialAppearanceHash = mesh.MaterialAppearanceHash < uint.MaxValue
                ? Util.ComputeCrc32(".ultra", (uint)mesh.MaterialAppearanceHash)
                : Util.ComputeCrc64(".ultra", mesh.MaterialAppearanceHash);

            var materialAppearanceFile = Util.FindFile(file.Pack, materialAppearanceHash) ?? Util.FindFile(materialAppearanceHash);

            if (vertexBufferFile is null || indexBufferFile is null)
            {
                Console.WriteLine($"No mesh vertex/index buffer found in mesh. {file.Text} {mesh.Material}-{mesh.Lod}");
                continue;
            }

            var vertexBuffer = VertexBuffer.LoadFile(vertexBufferFile, out var formatEntries);
            var indexBuffer = IndexBuffer.LoadFile(indexBufferFile);

            var textureSets = Enumerable.Empty<MaterialAppearance.TextureSet>();

            if (materialAppearanceFile is not null)
                textureSets = MaterialAppearance.LoadFile(materialAppearanceFile, out var _);

            if (vertexBuffer is null || formatEntries is null || indexBuffer is null || textureSets is null)
                throw new NotImplementedException();

            // Save Diffuse Texture

            var diffuseTexture = textureSets.FirstOrDefault(x => x.Type == MaterialAppearance.Type.DiffuseMap);

            ResourceFile? diffuseTextureFile = null;

            if (materialAppearanceFile is not null && diffuseTexture is not null)
            {
                diffuseTextureFile = Util.FindFile(file.Pack, diffuseTexture.Hash) ?? Util.FindFile(diffuseTexture.Hash);

                if (diffuseTextureFile is not null)
                {
                    Texture.Export(directory, diffuseTextureFile, true);

                    // Save Material

                    using var materialStreamWriter = new StreamWriter(Path.Combine(directory, $"{materialAppearanceFile.Text}_{mesh.Material}-{mesh.Lod}.mtl"));

                    materialStreamWriter.WriteLine("# Material Exported by Resource Explorer");
                    materialStreamWriter.WriteLine();
                    materialStreamWriter.WriteLine($"newmtl {diffuseTextureFile.Text}");
                    materialStreamWriter.WriteLine($"map_Kd {diffuseTextureFile.Text}.png");
                }
            }

            // Save Mesh

            using var objectStreamWriter = new StreamWriter(Path.Combine(directory, $"{file.Text}_{mesh.Material}-{mesh.Lod}.obj"));

            objectStreamWriter.WriteLine("# Mesh Exported by Resource Explorer");
            objectStreamWriter.WriteLine();

            if (materialAppearanceFile is not null)
                objectStreamWriter.WriteLine($"mtllib {materialAppearanceFile.Text}_{mesh.Material}-{mesh.Lod}.mtl");

            objectStreamWriter.WriteLine();

            var positionEntry = formatEntries.SingleOrDefault(x => x.Usage == VertexFormat.Usage.Position);
            var texCoordEntry = formatEntries.SingleOrDefault(x => x.Usage == VertexFormat.Usage.TexCoord0);
            var normalEntry = formatEntries.SingleOrDefault(x => x.Usage == VertexFormat.Usage.Normal);

            if (positionEntry is null || texCoordEntry is null)
            {
                Console.WriteLine($"No position or texcoord entry found in actor mesh. {file.Text}");
                return;
            }

            // Vertex - Position
            foreach (var vertex in vertexBuffer)
            {
                if (positionEntry.Type == VertexFormat.Type.Float16)
                {
                    var floats = new float[positionEntry.Dimension];

                    for (var i = 0; i < positionEntry.Dimension; i++)
                        floats[i] = (float)BitConverter.ToHalf(vertex, positionEntry.Offset + (i * 2));

                    objectStreamWriter.WriteLine($"v  {string.Join(' ', floats.Select(x => x.ToString(CultureInfo.InvariantCulture)))}");
                }
                else if (positionEntry.Type == VertexFormat.Type.Float)
                {
                    var floats = new float[positionEntry.Dimension];

                    for (var i = 0; i < positionEntry.Dimension; i++)
                        floats[i] = BitConverter.ToSingle(vertex, positionEntry.Offset + (i * 4));

                    objectStreamWriter.WriteLine($"v  {string.Join(' ', floats.Select(x => x.ToString(CultureInfo.InvariantCulture)))}");
                }
                else
                    throw new NotImplementedException();
            }

            objectStreamWriter.WriteLine();

            // Vertex - TexCoord
            foreach (var vertex in vertexBuffer)
            {
                if (texCoordEntry.Type == VertexFormat.Type.Float16)
                {
                    var floats = new float[texCoordEntry.Dimension];

                    for (var i = 0; i < texCoordEntry.Dimension; i++)
                        floats[i] = (float)BitConverter.ToHalf(vertex, texCoordEntry.Offset + (i * 2));

                    objectStreamWriter.WriteLine($"vt  {string.Join(' ', floats.Select(x => x.ToString(CultureInfo.InvariantCulture)))}");
                }
                else if (texCoordEntry.Type == VertexFormat.Type.Float)
                {
                    var floats = new float[texCoordEntry.Dimension];

                    for (var i = 0; i < texCoordEntry.Dimension; i++)
                        floats[i] = BitConverter.ToSingle(vertex, texCoordEntry.Offset + (i * 4));

                    objectStreamWriter.WriteLine($"vt  {string.Join(' ', floats.Select(x => x.ToString(CultureInfo.InvariantCulture)))}");
                }
                else
                    throw new NotImplementedException();
            }

            objectStreamWriter.WriteLine();

            // Vertex - Normal
            if (normalEntry is not null)
            {
                foreach (var vertex in vertexBuffer)
                {
                    if (normalEntry.Type == VertexFormat.Type.Float16)
                    {
                        var floats = new float[normalEntry.Dimension];

                        for (var i = 0; i < normalEntry.Dimension; i++)
                            floats[i] = (float)BitConverter.ToHalf(vertex, normalEntry.Offset + (i * 2));

                        objectStreamWriter.WriteLine($"vn  {string.Join(' ', floats.Select(x => x.ToString(CultureInfo.InvariantCulture)))}");
                    }
                    else if (normalEntry.Type == VertexFormat.Type.Float)
                    {
                        var floats = new float[normalEntry.Dimension];

                        for (var i = 0; i < normalEntry.Dimension; i++)
                            floats[i] = BitConverter.ToSingle(vertex, normalEntry.Offset + (i * 4));

                        objectStreamWriter.WriteLine($"vn  {string.Join(' ', floats.Select(x => x.ToString(CultureInfo.InvariantCulture)))}");
                    }
                    else
                        throw new NotImplementedException();
                }
            }

            // Index

            if (diffuseTextureFile is not null)
            {
                objectStreamWriter.WriteLine();
                objectStreamWriter.WriteLine($"usemtl {diffuseTextureFile.Text}");
            }

            for (var i = 0; i < indexBuffer.Count; i += 3)
            {
                var f0 = indexBuffer[i + 0] + 1;
                var f1 = indexBuffer[i + 1] + 1;
                var f2 = indexBuffer[i + 2] + 1;

                objectStreamWriter.WriteLine(normalEntry is null
                    ? "f {0}/{0} {1}/{1} {2}/{2}"
                    : "f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", f0, f1, f2);
            }
        }
    }
}