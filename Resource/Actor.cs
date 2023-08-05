using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ResourceExplorer;

public partial class Actor : UserControl
{
    public class Mesh
    {
        public ulong IndexBufferHash { get; set; }
        public ulong VertexBufferHash { get; set; }
        public ulong MaterialAppearanceHash { get; set; }
    }

    public Actor()
    {
        InitializeComponent();

        Dock = DockStyle.Fill;
    }

    public static Actor? Create(ResourceFile file)
    {
        var meshes = LoadFile(file);

        if (meshes is null)
            return null;

        var actor = new Actor();

        actor.lodLabel.Text = $"Lod: {meshes.GetLength(0)}";
        actor.qualityLabel.Text = $"Quality: {meshes.GetLength(1)}";

        actor.indexBufferLabel.Text = $"IndexBuffer: {meshes[0, 0].IndexBufferHash}";
        actor.vertexBufferLabel.Text = $"VertexBuffer: {meshes[0, 0].VertexBufferHash}";
        actor.materialAppearanceLabel.Text = $"MaterialAppearance: {meshes[0, 0].MaterialAppearanceHash}";

        return actor;
    }

    public static Mesh[,]? LoadFile(ResourceFile file)
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
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;

        var meshCount = br.ReadInt16();

        var qualityCount = br.ReadByte();
        var lodCount = br.ReadByte();

        if (meshOffset <= 0)
            return null;

        var meshes = new Mesh[lodCount, qualityCount];

        br.BaseStream.Position = meshOffset;

        var meshOffsets = new int[lodCount, qualityCount];

        for (var l = 0; l < lodCount; l++)
        {
            for (var q = 0; q < qualityCount; q++)
            {
                meshOffsets[l, q] = br.ReadInt32(); br.BaseStream.Position += 4;
            }
        }

        for (var l = 0; l < lodCount; l++)
        {
            for (var q = 0; q < qualityCount; q++)
            {
                var mesh = new Mesh();

                br.BaseStream.Position = meshOffsets[l, q];

                _ = br.ReadInt32(); br.BaseStream.Position += 4;
                var offset1 = br.ReadInt32(); br.BaseStream.Position += 4;
                _ = br.ReadInt32(); br.BaseStream.Position += 4;
                _ = br.ReadInt32(); br.BaseStream.Position += 4;
                _ = br.ReadInt32(); br.BaseStream.Position += 4;
                _ = br.ReadInt32(); br.BaseStream.Position += 4;

                mesh.IndexBufferHash = file.Pack.Version == 6 ? br.ReadUInt64() : br.ReadUInt32();

                br.BaseStream.Position = offset1;

                if (file.Pack.Version != 6)
                    br.BaseStream.Position += 4;

                mesh.VertexBufferHash = file.Pack.Version == 6 ? br.ReadUInt64() : br.ReadUInt32();

                br.BaseStream.Position += file.Pack.Version == 6 ? 40 : 32;

                mesh.MaterialAppearanceHash = file.Pack.Version == 6 ? br.ReadUInt64() : br.ReadUInt32();

                meshes[l, q] = mesh;
            }
        }

        return meshes;
    }

    public static void Export(string directory, ResourceFile file)
    {
        var meshes = LoadFile(file);

        if (meshes is null)
            return;

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // TODO: Export all qualities/lods

        var mesh = meshes[0, 0];

        var vertexBufferFile = Util.FindFile(file.Pack, mesh.VertexBufferHash) ?? Util.FindFile(mesh.VertexBufferHash);
        var indexBufferFile = Util.FindFile(file.Pack, mesh.IndexBufferHash) ?? Util.FindFile(mesh.IndexBufferHash);

        var materialAppearanceHash = mesh.MaterialAppearanceHash < uint.MaxValue
            ? Util.ComputeCrc32(".ultra", (uint)mesh.MaterialAppearanceHash)
            : Util.ComputeCrc64(".ultra", mesh.MaterialAppearanceHash);

        var materialAppearanceFile = Util.FindFile(file.Pack, materialAppearanceHash) ?? Util.FindFile(materialAppearanceHash);

        if (vertexBufferFile is null || indexBufferFile is null || materialAppearanceFile is null)
            return;

        var vertexBuffer = VertexBuffer.LoadFile(vertexBufferFile, out var formatEntries);
        var indexBuffer = IndexBuffer.LoadFile(indexBufferFile);
        var textures = MaterialAppearance.LoadFile(materialAppearanceFile);

        if (vertexBuffer is null || formatEntries is null || indexBuffer is null || textures is null)
            return;

        // Save Diffuse Texture

        var diffuseTexture = textures.SingleOrDefault(x => x.Type == MaterialAppearance.Type.DiffuseMap);

        ResourceFile? diffuseTextureFile = null;

        if (diffuseTexture is not null)
        {
            diffuseTextureFile = Util.FindFile(file.Pack, diffuseTexture.Hash) ?? Util.FindFile(diffuseTexture.Hash);

            if (diffuseTextureFile is not null)
            {
                Texture.Export(directory, diffuseTextureFile, true);

                // Save Material

                using var materialStreamWriter = new StreamWriter(Path.Combine(directory, $"{materialAppearanceFile.Text}.mtl"));

                materialStreamWriter.WriteLine("# Material Exported by Resource Explorer");
                materialStreamWriter.WriteLine();
                materialStreamWriter.WriteLine($"newmtl {diffuseTextureFile.Text}");
                materialStreamWriter.WriteLine($"map_Kd {diffuseTextureFile.Text}.png");
            }
        }

        // Save Mesh

        using var objectStreamWriter = new StreamWriter(Path.Combine(directory, $"{file.Text}.obj"));

        objectStreamWriter.WriteLine("# Mesh Exported by Resource Explorer");
        objectStreamWriter.WriteLine();
        objectStreamWriter.WriteLine($"mtllib {materialAppearanceFile.Text}.mtl");
        objectStreamWriter.WriteLine();

        var positionEntry = formatEntries.SingleOrDefault(x => x.Usage == VertexFormat.Usage.Position);
        var texCoordEntry = formatEntries.SingleOrDefault(x => x.Usage == VertexFormat.Usage.TexCoord0);
        var normalEntry = formatEntries.SingleOrDefault(x => x.Usage == VertexFormat.Usage.Normal);

        if (positionEntry is null || texCoordEntry is null)
            return;

        // Vertex - Position
        foreach (var vertex in vertexBuffer)
        {
            if (positionEntry.Type == VertexFormat.Type.Float16)
            {
                var floats = new float[positionEntry.Dimension];

                for (var i = 0; i < positionEntry.Dimension; i++)
                    floats[i] = (float)BitConverter.ToHalf(vertex, positionEntry.Offset + (i * 2));

                objectStreamWriter.WriteLine($"v  {string.Join(' ', floats)}");
            }
            else if (positionEntry.Type == VertexFormat.Type.Float)
            {
                var floats = new float[positionEntry.Dimension];

                for (var i = 0; i < positionEntry.Dimension; i++)
                    floats[i] = BitConverter.ToSingle(vertex, positionEntry.Offset + (i * 4));

                objectStreamWriter.WriteLine($"v  {string.Join(' ', floats)}");
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

                objectStreamWriter.WriteLine($"vt  {string.Join(' ', floats)}");
            }
            else if (texCoordEntry.Type == VertexFormat.Type.Float)
            {
                var floats = new float[texCoordEntry.Dimension];

                for (var i = 0; i < texCoordEntry.Dimension; i++)
                    floats[i] = BitConverter.ToSingle(vertex, texCoordEntry.Offset + (i * 4));

                objectStreamWriter.WriteLine($"vt  {string.Join(' ', floats)}");
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

                    objectStreamWriter.WriteLine($"vn  {string.Join(' ', floats)}");
                }
                else if (normalEntry.Type == VertexFormat.Type.Float)
                {
                    var floats = new float[normalEntry.Dimension];

                    for (var i = 0; i < normalEntry.Dimension; i++)
                        floats[i] = BitConverter.ToSingle(vertex, normalEntry.Offset + (i * 4));

                    objectStreamWriter.WriteLine($"vn  {string.Join(' ', floats)}");
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