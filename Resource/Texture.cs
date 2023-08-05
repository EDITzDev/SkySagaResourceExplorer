using System;
using System.IO;
using System.Windows.Forms;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace ResourceExplorer;

public partial class Texture : UserControl
{
    public Texture()
    {
        InitializeComponent();

        Dock = DockStyle.Fill;
    }

    public static Texture? Create(ResourceFile file)
    {
        var image = LoadFile(file);

        if (image is null)
            return null;

        var texture = new Texture();

        texture.pictureBox.Width = image.Width;
        texture.pictureBox.Height = image.Height;

        texture.pictureBox.Image = image.ToNetImage();

        return texture;
    }

    public static Image? LoadFile(ResourceFile file)
    {
        using var data = file.GetData();

        using var br = new BinaryReader(data);

        br.BaseStream.Position = 84;

        var width = br.ReadUInt16();
        var height = br.ReadUInt16();
        var depth = br.ReadUInt16();
        var flags = br.ReadUInt16();
        var format = br.ReadByte();
        _ = br.ReadByte();
        var levels = br.ReadByte();
        _ = br.ReadByte();
        _ = br.ReadByte();

        var cubeMap = (flags & 2) != 0;

        // TODO
        if (cubeMap)
        {
            Console.WriteLine("Cube map textures are currently not supported.");
            return null;
        }

        br.BaseStream.Position = 124;

        // A8R8G8B8
        if (format == 1)
        {
            var pixelData = br.ReadBytes(width * height * 4);

            return Image.LoadPixelData<Bgra32>(pixelData, width, height);
        }

        // DXT1
        if (file.Pack.Version != 6 && format == 18 || file.Pack.Version == 6 && format == 19)
        {
            var pixelData = DxtUtil.DecompressDxt1(br.BaseStream, width, height);

            return Image.LoadPixelData<Rgba32>(pixelData, width, height);
        }

        // DXT3
        if (file.Pack.Version != 6 && format == 20 || file.Pack.Version == 6 && format == 21)
        {
            var pixelData = DxtUtil.DecompressDxt3(br.BaseStream, width, height);

            return Image.LoadPixelData<Rgba32>(pixelData, width, height);
        }

        // DXT5
        if (file.Pack.Version != 6 && format == 22 || file.Pack.Version == 6 && format == 23)
        {
            var pixelData = DxtUtil.DecompressDxt5(br.BaseStream, width, height);

            return Image.LoadPixelData<Rgba32>(pixelData, width, height);
        }

        // R16F
        if (file.Pack.Version != 6 && format == 82 || file.Pack.Version == 6 && format == 83)
        {
            var pixelData = br.ReadBytes(width * height * 2);

            return Image.LoadPixelData<HalfSingle>(pixelData, width, height);
        }

        // A16B16G16R16
        if (file.Pack.Version != 6 && format == 148 || file.Pack.Version == 6 && format == 150)
        {
            var pixelData = br.ReadBytes(width * height * 8);

            return Image.LoadPixelData<Rgba64>(pixelData, width, height);
        }

        Console.WriteLine($"Unknown texture format. Format: {format}");

        return null;
    }

    public static void Export(string directory, ResourceFile file, bool flipVertical = false)
    {
        var image = LoadFile(file);

        if (image is null)
            return;

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var savePath = Path.Combine(directory, $"{file.Text}.png");

        if (flipVertical)
            image.Mutate(x => x.Flip(FlipMode.Vertical));

        image.SaveAsPng(savePath, new PngEncoder
        {
            ColorType = PngColorType.Rgb
        });

        image.Dispose();
    }

    private void pictureBox_MouseWheel(object? sender, MouseEventArgs e)
    {
        const int zoomRatio = 10;

        var zoomWidth = pictureBox.Width * zoomRatio / 100;
        var zoomHeight = pictureBox.Height * zoomRatio / 100;

        if (e.Delta <= 0)
        {
            zoomWidth *= -1;
            zoomHeight *= -1;
        }

        pictureBox.Width += zoomWidth;
        pictureBox.Height += zoomHeight;
    }
}