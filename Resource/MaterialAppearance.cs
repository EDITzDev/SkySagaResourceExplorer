using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ResourceExplorer;

public partial class MaterialAppearance : UserControl
{
    private readonly ResourcePack _pack;
    private readonly TabControl _tabControl;

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

    public class TextureSet
    {
        public ulong Hash { get; set; }
        public Type Type { get; set; }
    }

    public MaterialAppearance(ResourcePack pack, TabControl tabControl)
    {
        _pack = pack;
        _tabControl = tabControl;

        InitializeComponent();

        Dock = DockStyle.Fill;
    }

    private void dataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.ColumnIndex != 0)
            return;

        var textureCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

        if (textureCell is null || textureCell.Value is not ulong textureHash)
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        var textureFile = Util.FindFile(_pack, textureHash) ?? Util.FindFile(textureHash);

        if (textureFile is null)
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        var texture = Texture.Create(textureFile);

        if (texture is null)
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        var tabPage = new TabPage
        {
            Text = $"{textureFile.Text}    "
        };

        tabPage.Controls.Add(texture);

        _tabControl.TabPages.Add(tabPage);

        _tabControl.SelectedTab = tabPage;
        _tabControl.SelectedTab.Focus();
    }

    public static MaterialAppearance? Create(ResourceFile file, TabControl mainTabControl)
    {
        var textureSets = LoadFile(file, out var materialHash);

        if (!textureSets.Any())
            return null;

        var materialAppearance = new MaterialAppearance(file.Pack, mainTabControl);

        materialAppearance.dataGridView.DataSource = textureSets;

        var materialFile = Util.FindFile(file.Pack, materialHash) ?? Util.FindFile(materialHash);

        if (materialFile is not null)
            materialAppearance.materialLabel.Text = $"Material: {materialFile.Text}";

        return materialAppearance;
    }

    public static IEnumerable<TextureSet> LoadFile(ResourceFile file, out ulong materialHash)
    {
        using var data = file.GetData();

        using var br = new BinaryReader(data);

        br.BaseStream.Position = 24;

        _ = br.ReadInt32(); br.BaseStream.Position += 4;
        var texturesOffset = br.ReadInt32(); br.BaseStream.Position += 4;
        _ = br.ReadInt32(); br.BaseStream.Position += 4;

        if (file.Pack.Version == 170)
        {
            materialHash = br.ReadUInt32();

            br.BaseStream.Position += 16;
        }
        else if (file.Pack.Version == 6)
        {
            br.BaseStream.Position += 24;

            materialHash = br.ReadUInt64();

            br.BaseStream.Position += 4;
        }
        else
        {
            br.BaseStream.Position += 24;

            materialHash = br.ReadUInt32();

            br.BaseStream.Position += 4;
        }

        var textureSetCount = br.ReadInt16();

        br.BaseStream.Position = texturesOffset;

        var textureSets = new List<TextureSet>();

        for (var i = 0; i < textureSetCount; i++)
        {
            textureSets.Add(new TextureSet
            {
                Hash = file.Pack.Version == 6 ? br.ReadUInt64() : br.ReadUInt32(),
                Type = (Type)br.ReadUInt32()
            });

            br.BaseStream.Position += file.Pack.Version == 6 ? 28 : 24;
        }

        return textureSets;
    }
}