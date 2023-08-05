using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ResourceExplorer;

public partial class Main : Form
{
    private readonly ParallelOptions _parallelOptions = new();

    public Main()
    {
        InitializeComponent();

        filterComboBox.SelectedIndex = (int)ResourceType.All;

        nameListCountToolStripStatusLabel.Text = $"Names: {Program.Names.Count}";

        Text = $"Resource Explorer v{Application.ProductVersion} | SkySaga: Infinite Isles";

        _parallelOptions.MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.50) * 1.0));
    }

    private void PopulateTreeNode()
    {
        var resourceType = (ResourceType)filterComboBox.SelectedIndex;

        mainTreeView.BeginUpdate();

        var dataNode = mainTreeView.Nodes["Data"];

        foreach (ResourcePack pack in dataNode.Nodes)
            pack.Nodes.Clear();

        dataNode.Nodes.Clear();

        foreach (var pack in Program.Packs)
        {
            foreach (var file in pack.Files)
            {
                if (resourceType == ResourceType.All || file.Type == resourceType)
                    pack.Nodes.Add(file);
            }

            if (hideEmptyPacksToolStripMenuItem.Checked && pack.Nodes.Count == 0)
                continue;

            pack.ImageIndex = pack.SelectedImageIndex = pack.Nodes.Count > 0 ? 0 : 2;

            dataNode.Nodes.Add(pack);
        }

        dataNode.Expand();

        if (sortFilesSlowToolStripMenuItem.Checked)
            mainTreeView.Sort();

        mainTreeView.EndUpdate();

        var packCount = dataNode.GetNodeCount(false);
        var fileCount = dataNode.GetNodeCount(true) - packCount;

        packCountToolStripStatusLabel.Text = $"Packs: {packCount}";
        fileCountToolStripStatusLabel.Text = $"Files: {fileCount}";
    }

    private void Main_FormClosed(object sender, FormClosedEventArgs e)
    {
        mainTreeView.Nodes.Clear();
    }

    // File
    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var folderBrowserDialog = new FolderBrowserDialog();

        if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            return;

        mainTabControl.TabPages.Clear();

        Program.Packs.Clear();

        Parallel.ForEach(Directory.EnumerateFiles(folderBrowserDialog.SelectedPath, "*.pc"), _parallelOptions, pcFile =>
        {
            var resourcePack = ResourcePack.Load(pcFile);

            if (resourcePack is null)
            {
                Console.WriteLine($"Failed to load resource pack. \"{pcFile}\"");
                return;
            }

            Program.Packs.Add(resourcePack);
        });

        Parallel.ForEach(Directory.EnumerateFiles(folderBrowserDialog.SelectedPath, "*.bpc"), _parallelOptions, bpcFile =>
        {
            var resourcePack = ResourcePack.Load(bpcFile);

            if (resourcePack is null)
            {
                Console.WriteLine($"Failed to load resource pack. \"{bpcFile}\"");
                return;
            }

            Program.Packs.Add(resourcePack);
        });

        PopulateTreeNode();
    }

    // View
    private void closeAllTabsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        mainTabControl.TabPages.Clear();
    }

    private void sortFilesSlowToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
    {
        PopulateTreeNode();
    }

    private void hideEmptyPacksToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
    {
        PopulateTreeNode();
    }

    // Export - Actors
    private void actorsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var folderBrowserDialog = new FolderBrowserDialog();

        if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            return;

        Parallel.ForEach(Program.Packs, pack =>
        {
            var saveDirectory = Path.Combine(folderBrowserDialog.SelectedPath, pack.Text);

            Parallel.ForEach(pack.Files, file =>
            {
                if (file.Type == ResourceType.Actor)
                    Actor.Export(saveDirectory, file);
            });
        });

    }

    // Export - Textures
    private void texturesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var folderBrowserDialog = new FolderBrowserDialog();

        if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            return;

        Parallel.ForEach(Program.Packs, _parallelOptions, pack =>
        {
            var saveDirectory = Path.Combine(folderBrowserDialog.SelectedPath, pack.Text);

            Parallel.ForEach(pack.Files, _parallelOptions, file =>
            {
                if (file.Type == ResourceType.Texture)
                    Texture.Export(saveDirectory, file);
            });
        });
    }

    // Export - String Tables
    private void stringTablesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var folderBrowserDialog = new FolderBrowserDialog();

        if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            return;

        Parallel.ForEach(Program.Packs, _parallelOptions, pack =>
        {
            var saveDirectory = Path.Combine(folderBrowserDialog.SelectedPath, pack.Text);

            Parallel.ForEach(pack.Files, _parallelOptions, file =>
            {
                if (file.Type == ResourceType.Stringtable)
                    StringTable.Export(saveDirectory, file);
            });
        });
    }

    // Export - Raw Files (Visible)
    private void rawFilesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var folderBrowserDialog = new FolderBrowserDialog();

        if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            return;

        var dataNode = mainTreeView.Nodes["Data"];

        Parallel.ForEach(dataNode.Nodes.OfType<ResourcePack>(), _parallelOptions, pack =>
        {
            var saveDirectory = Path.Combine(folderBrowserDialog.SelectedPath, pack.Text);

            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            Parallel.ForEach(pack.Nodes.OfType<ResourceFile>(), _parallelOptions, file =>
            {
                var savePath = Path.Combine(saveDirectory, $"{file.Text}.dat");

                using var fileStream = File.OpenWrite(savePath);

                using var dataStream = file.GetData();

                dataStream.CopyTo(fileStream);
            });
        });
    }

    // UI - Filter
    private void resourceTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateTreeNode();
    }

    // UI - Search
    private void searchButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(searchTextBox.Text))
            return;

        var isHash = ulong.TryParse(searchTextBox.Text, out var hash);

        var dataNode = mainTreeView.Nodes["Data"];

        foreach (ResourcePack packNode in dataNode.Nodes)
        {
            foreach (ResourceFile fileNode in packNode.Nodes)
            {
                if (fileNode.Text.Contains(searchTextBox.Text) || (isHash && fileNode.Hash == hash))
                {
                    mainTreeView.Focus();
                    mainTreeView.SelectedNode = fileNode;

                    break;
                }
            }
        }
    }

    // UI - TreeView
    private void mainTreeView_AfterExpand(object sender, TreeViewEventArgs e)
    {
        if (e.Node is not null)
            e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
    }

    private void mainTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
    {
        if (e.Node is not null)
            e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
    }

    private void mainTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
            mainTreeView.SelectedNode = e.Node;

        if (e.Node.Level == 2)
            e.Node.ContextMenuStrip = fileContextMenuStrip;
    }

    private void mainTreeView_DoubleClick(object sender, EventArgs e)
    {
        if (mainTreeView.SelectedNode is not ResourceFile file)
            return;

        Control? control = null;

        switch (file.Type)
        {
            case ResourceType.Texture:
                control = Texture.Create(file);
                break;

            case ResourceType.Actor:
                control = Actor.Create(file);
                break;

            case ResourceType.Stringtable:
                control = StringTable.Create(file);
                break;

            case ResourceType.VertexFormat:
                control = VertexFormat.Create(file);
                break;

            case ResourceType.VertexBuffer:
                control = VertexBuffer.Create(file);
                break;

            case ResourceType.IndexBuffer:
                control = IndexBuffer.Create(file);
                break;

            case ResourceType.MaterialAppearance:
                control = MaterialAppearance.Create(file);
                break;
        }

        if (control is null)
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        var tabPage = new TabPage
        {
            Text = $"{file.Text}    "
        };

        tabPage.Controls.Add(control);

        mainTabControl.TabPages.Add(tabPage);

        mainTabControl.SelectedTab = tabPage;
    }

    // UI - TreeView - Right Click
    private void exportFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (mainTreeView.SelectedNode is not ResourceFile file)
            return;

        using var folderBrowserDialog = new FolderBrowserDialog();

        if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            return;

        switch (file.Type)
        {
            case ResourceType.Texture:
                Texture.Export(folderBrowserDialog.SelectedPath, file);
                break;

            case ResourceType.Actor:
                Actor.Export(folderBrowserDialog.SelectedPath, file);
                break;

            case ResourceType.Stringtable:
                StringTable.Export(folderBrowserDialog.SelectedPath, file);
                break;
        }
    }

    private void saveRawToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (mainTreeView.SelectedNode is not ResourceFile file)
            return;

        using var folderBrowserDialog = new FolderBrowserDialog();

        if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            return;

        var savePath = Path.Combine(folderBrowserDialog.SelectedPath, $"{file.Text}.dat");

        using var fileStream = File.OpenWrite(savePath);

        using var dataStream = file.GetData();

        dataStream.CopyTo(fileStream);
    }

    // UI - Tabs
    private void mainTabControl_DrawItem(object sender, DrawItemEventArgs e)
    {
        var tabPage = mainTabControl.TabPages[e.Index];
        var tabRect = mainTabControl.GetTabRect(e.Index);

        tabRect.Inflate(-2, -2);

        TextRenderer.DrawText(e.Graphics, "X", tabPage.Font, new Point(tabRect.Right - 14, tabRect.Top + (tabRect.Height - 14) / 2), tabPage.ForeColor);
        TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font, tabRect, tabPage.ForeColor, TextFormatFlags.Left);
    }

    private void mainTabControl_MouseDown(object sender, MouseEventArgs e)
    {
        var tabRect = mainTabControl.GetTabRect(mainTabControl.SelectedIndex);

        tabRect.Inflate(-2, -2);

        var closeRect = new Rectangle(tabRect.Right - 14, tabRect.Top + (tabRect.Height - 14) / 2, 14, 14);

        if (closeRect.Contains(e.Location))
            mainTabControl.TabPages.Remove(mainTabControl.SelectedTab);
    }

    private void consoleLogToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Util.AllocConsole();
    }
}