using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ResourceExplorer;

public partial class Main : Form
{
    private readonly ParallelOptions _parallelOptions = new();

    private readonly HashSet<ResourceType> _loadedTypes = new();

    private int _lastSearchIndex;
    private string _lastSearchText = string.Empty;
    private List<TreeNode> _lastSearchResults = new List<TreeNode>();

    public Main()
    {
        InitializeComponent();

        filterComboBox.Items.Clear();

        nameListCountToolStripStatusLabel.Text = $"Names: {Program.Names.Count}";

        Text = $"Resource Explorer v{Application.ProductVersion} | SkySaga: Infinite Isles";

        _parallelOptions.MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.50) * 1.0));
    }

    private void PopulateTreeNode()
    {
        if (filterComboBox.SelectedItem is not ResourceType resourceType)
            return;

        mainTreeView.BeginUpdate();

        var dataNode = mainTreeView.Nodes["Data"];

        if (dataNode is null)
            return;

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

        clearSearchResults();

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

        _loadedTypes.Clear();
        filterComboBox.Items.Clear();

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

        foreach (var pack in Program.Packs)
        {
            foreach (var file in pack.Files)
            {
                if (file.Type >= ResourceType.All)
                    continue;

                if (!_loadedTypes.Contains(file.Type))
                    _loadedTypes.Add(file.Type);
            }
        }

        if (_loadedTypes.Count > 0)
        {
            foreach (var loadedType in _loadedTypes)
                filterComboBox.Items.Add(loadedType);

            filterComboBox.Items.Add(ResourceType.All);
            filterComboBox.SelectedItem = ResourceType.All;
        }
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
    private void firstLodAndMaterialToolStripMenuItem_Click(object sender, EventArgs e)
    {
        exportActors();
    }

    private void includeAllLodsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        exportActors(allLods: true);
    }

    private void includeAllMaterialsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        exportActors(allMaterials: true);
    }

    private void includeAllLodsAndMaterialsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        exportActors(allLods: true, allMaterials: true);
    }

    private void exportActors(bool allLods = false, bool allMaterials = false)
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
                    Actor.Export(saveDirectory, file, allLods, allMaterials);
            });
        });
    }

    // Export - Textures
    private void allTexturesToolStripMenuItem_Click(object sender, EventArgs e)
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
    private void allStringTablesToolStripMenuItem_Click(object sender, EventArgs e)
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
    private void allRawFilesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var folderBrowserDialog = new FolderBrowserDialog();

        if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            return;

        var dataNode = mainTreeView.Nodes["Data"];

        if (dataNode is null)
            return;

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
    private void filterComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateTreeNode();
    }

    // UI - Search
    private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
            return;

        e.Handled = true;
        e.SuppressKeyPress = true;

        searchTreeView();
    }

    private void searchButton_Click(object sender, EventArgs e)
    {
        searchTreeView();
    }

    private void searchTreeView()
    {
        if (string.IsNullOrEmpty(searchTextBox.Text))
            return;

        var dataNode = mainTreeView.Nodes["Data"];

        if (dataNode is null)
            return;

        var searchText = searchTextBox.Text;

        var isHash = ulong.TryParse(searchText, out var searchHash);

        if (searchText != _lastSearchText)
        {
            clearSearchResults();

            _lastSearchText = searchText;

            recursivelySearchTreeView(dataNode, searchText, isHash ? searchHash : null);
        }

        if (_lastSearchResults.Count == 0)
        {
            _lastSearchIndex = 0;

            SystemSounds.Exclamation.Play();

            return;
        }

        if (_lastSearchIndex == _lastSearchResults.Count)
            _lastSearchIndex = 0;

        var selectedNode = _lastSearchResults[_lastSearchIndex++];

        mainTreeView.SelectedNode = selectedNode;
        mainTreeView.SelectedNode.Expand();
        mainTreeView.Focus();
    }

    private void clearSearchResults()
    {
        _lastSearchIndex = 0;
        _lastSearchResults.Clear();
        _lastSearchText = string.Empty;
    }

    private void recursivelySearchTreeView(TreeNode? startNode, string searchText, ulong? searchHash)
    {
        while (startNode is not null)
        {
            if (startNode.Text.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                || startNode is ResourceFile resourceFile && resourceFile.Hash == searchHash)
            {
                _lastSearchResults.Add(startNode);
            }

            if (startNode.Nodes.Count > 0)
                recursivelySearchTreeView(startNode.Nodes[0], searchText, searchHash);

            startNode = startNode.NextNode;
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
                control = Actor.Create(file, mainTabControl);
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
                control = MaterialAppearance.Create(file, mainTabControl);
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

        if (mainTabControl.SelectedTab is not null && closeRect.Contains(e.Location))
            mainTabControl.TabPages.Remove(mainTabControl.SelectedTab);
    }

    private void consoleLogToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Util.AllocConsole();
    }
}