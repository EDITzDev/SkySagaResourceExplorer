using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ResourceExplorer;

public partial class MaterialAppearance : UserControl
{
    // Extracted from pixel shaders.
    public enum Type : uint
    {
        PatternMap = 67115434u,
        ColourLUT = 140546881u,
        DiffuseSampler = 142695404u,
        MaskMap = 330639273u,
        SpecularMap = 590123184u,
        OverlayTex = 742161934u,
        EdgeMapTex = 1092829288u,
        DistanceMap = 1526201522u,
        EyeEnvTexture = 1777162241u,
        DiffuseTexture = 2110104344u,
        ColourGradient = 2253552177u,
        GlowLUT = 2273823985u,
        ColorLUTSampler = 2281016862u,
        TeleportMask = 2427858469u,
        NormalMap = 2681500227u,
        LUTTexture = 2708546583u,
        ShadowTex = 2715872652u,
        BlockNormalMap = 2716815414u,
        DiffuseMap = 2853631434u,
        IrisTexture = 2867627092u,
        ShapeMaskTexture = 3288604865u,
        ParticleLUT = 3595746625u,
        MaterialMaskSampler = 4085146453u,
        StarTexture = 4145218147u,
        RGBPatternMap = 4179060177u
    }

    public class Texture
    {
        public ulong Hash { get; set; }
        public Type Type { get; set; }
    }

    public MaterialAppearance()
    {
        InitializeComponent();

        Dock = DockStyle.Fill;
    }

    public static MaterialAppearance? Create(ResourceFile file)
    {
        var textures = LoadFile(file);

        if(!textures.Any())
            return null;

        var materialAppearance = new MaterialAppearance();

        materialAppearance.dataGridView.DataSource = textures;

        return materialAppearance;
    }

    public static IEnumerable<Texture> LoadFile(ResourceFile file)
    {
        using var data = file.GetData();

        using var br = new BinaryReader(data);

        br.BaseStream.Position = 24;

        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        var texturesOffset = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;

        if (file.Pack.Version == 170)
            br.BaseStream.Position += 22;
        else if(file.Pack.Version == 6)
            br.BaseStream.Position += 36;
        else
            br.BaseStream.Position += 32;

        var textureSetCount = br.ReadInt16();

        br.BaseStream.Position = texturesOffset;

        var textures = new List<Texture>();

        for (var i = 0; i < textureSetCount; i++)
        {
            textures.Add(new Texture
            {
                Hash = file.Pack.Version == 6 ? br.ReadUInt64() : br.ReadUInt32(),
                Type = (Type)br.ReadUInt32()
            });

            br.BaseStream.Position += file.Pack.Version == 6 ? 28 : 24;
        }

        return textures;
    }
}