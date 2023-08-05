using System.Windows.Forms;

namespace ResourceExplorer;

partial class Main
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
        TreeNode treeNode1 = new TreeNode("Data", 0, 0);
        iconImageList = new ImageList(components);
        mainStatusStrip = new StatusStrip();
        packCountToolStripStatusLabel = new ToolStripStatusLabel();
        fileCountToolStripStatusLabel = new ToolStripStatusLabel();
        nameListCountToolStripStatusLabel = new ToolStripStatusLabel();
        mainMenuStrip = new MenuStrip();
        fileToolStripMenuItem = new ToolStripMenuItem();
        openToolStripMenuItem = new ToolStripMenuItem();
        viewToolStripMenuItem = new ToolStripMenuItem();
        consoleLogToolStripMenuItem = new ToolStripMenuItem();
        closeAllTabsToolStripMenuItem = new ToolStripMenuItem();
        sortFilesSlowToolStripMenuItem = new ToolStripMenuItem();
        hideEmptyPacksToolStripMenuItem = new ToolStripMenuItem();
        exportToolStripMenuItem = new ToolStripMenuItem();
        actorsToolStripMenuItem = new ToolStripMenuItem();
        texturesToolStripMenuItem = new ToolStripMenuItem();
        stringTablesToolStripMenuItem = new ToolStripMenuItem();
        rawFilesToolStripMenuItem = new ToolStripMenuItem();
        BottomToolStripPanel = new ToolStripPanel();
        TopToolStripPanel = new ToolStripPanel();
        RightToolStripPanel = new ToolStripPanel();
        LeftToolStripPanel = new ToolStripPanel();
        ContentPanel = new ToolStripContentPanel();
        mainTabControl = new TabControl();
        mainTreeView = new TreeView();
        fileContextMenuStrip = new ContextMenuStrip(components);
        exportFileToolStripMenuItem = new ToolStripMenuItem();
        saveRawToolStripMenuItem = new ToolStripMenuItem();
        filterComboBox = new ComboBox();
        filterLabel = new Label();
        searchLabel = new Label();
        searchTextBox = new TextBox();
        mainSplitContainer = new SplitContainer();
        searchButton = new Button();
        colorDialog1 = new ColorDialog();
        mainStatusStrip.SuspendLayout();
        mainMenuStrip.SuspendLayout();
        fileContextMenuStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
        mainSplitContainer.Panel1.SuspendLayout();
        mainSplitContainer.Panel2.SuspendLayout();
        mainSplitContainer.SuspendLayout();
        SuspendLayout();
        // 
        // iconImageList
        // 
        iconImageList.ColorDepth = ColorDepth.Depth8Bit;
        iconImageList.ImageStream = (ImageListStreamer)resources.GetObject("iconImageList.ImageStream");
        iconImageList.TransparentColor = System.Drawing.Color.Transparent;
        iconImageList.Images.SetKeyName(0, "Folder.png");
        iconImageList.Images.SetKeyName(1, "FolderOpened.png");
        iconImageList.Images.SetKeyName(2, "FolderEmpty.png");
        iconImageList.Images.SetKeyName(3, "Default.png");
        iconImageList.Images.SetKeyName(4, "Texture.png");
        iconImageList.Images.SetKeyName(5, "Actor.png");
        iconImageList.Images.SetKeyName(6, "StringTable.png");
        iconImageList.Images.SetKeyName(7, "VertexFormat.png");
        iconImageList.Images.SetKeyName(8, "VertexBuffer.png");
        iconImageList.Images.SetKeyName(9, "VertexIndex.png");
        iconImageList.Images.SetKeyName(10, "MaterialAppearance.png");
        // 
        // mainStatusStrip
        // 
        mainStatusStrip.Items.AddRange(new ToolStripItem[] { packCountToolStripStatusLabel, fileCountToolStripStatusLabel, nameListCountToolStripStatusLabel });
        mainStatusStrip.Location = new System.Drawing.Point(0, 739);
        mainStatusStrip.Name = "mainStatusStrip";
        mainStatusStrip.Size = new System.Drawing.Size(1008, 22);
        mainStatusStrip.TabIndex = 6;
        mainStatusStrip.Text = "statusStrip1";
        // 
        // packCountToolStripStatusLabel
        // 
        packCountToolStripStatusLabel.Name = "packCountToolStripStatusLabel";
        packCountToolStripStatusLabel.Size = new System.Drawing.Size(49, 17);
        packCountToolStripStatusLabel.Text = "Packs: 0";
        // 
        // fileCountToolStripStatusLabel
        // 
        fileCountToolStripStatusLabel.Name = "fileCountToolStripStatusLabel";
        fileCountToolStripStatusLabel.Size = new System.Drawing.Size(42, 17);
        fileCountToolStripStatusLabel.Text = "Files: 0";
        // 
        // nameListCountToolStripStatusLabel
        // 
        nameListCountToolStripStatusLabel.Name = "nameListCountToolStripStatusLabel";
        nameListCountToolStripStatusLabel.Size = new System.Drawing.Size(56, 17);
        nameListCountToolStripStatusLabel.Text = "Names: 0";
        // 
        // mainMenuStrip
        // 
        mainMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, exportToolStripMenuItem });
        mainMenuStrip.Location = new System.Drawing.Point(0, 0);
        mainMenuStrip.Name = "mainMenuStrip";
        mainMenuStrip.Size = new System.Drawing.Size(1008, 24);
        mainMenuStrip.TabIndex = 7;
        mainMenuStrip.Text = "menuStrip1";
        // 
        // fileToolStripMenuItem
        // 
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
        fileToolStripMenuItem.Text = "&File";
        // 
        // openToolStripMenuItem
        // 
        openToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("openToolStripMenuItem.Image");
        openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
        openToolStripMenuItem.Name = "openToolStripMenuItem";
        openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
        openToolStripMenuItem.Text = "&Open";
        openToolStripMenuItem.Click += openToolStripMenuItem_Click;
        // 
        // viewToolStripMenuItem
        // 
        viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { consoleLogToolStripMenuItem, closeAllTabsToolStripMenuItem, sortFilesSlowToolStripMenuItem, hideEmptyPacksToolStripMenuItem });
        viewToolStripMenuItem.Name = "viewToolStripMenuItem";
        viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
        viewToolStripMenuItem.Text = "View";
        // 
        // consoleLogToolStripMenuItem
        // 
        consoleLogToolStripMenuItem.Name = "consoleLogToolStripMenuItem";
        consoleLogToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
        consoleLogToolStripMenuItem.Text = "Console Log";
        consoleLogToolStripMenuItem.Click += consoleLogToolStripMenuItem_Click;
        // 
        // closeAllTabsToolStripMenuItem
        // 
        closeAllTabsToolStripMenuItem.Name = "closeAllTabsToolStripMenuItem";
        closeAllTabsToolStripMenuItem.ShortcutKeyDisplayString = "";
        closeAllTabsToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
        closeAllTabsToolStripMenuItem.Text = "Close All Tabs";
        closeAllTabsToolStripMenuItem.Click += closeAllTabsToolStripMenuItem_Click;
        // 
        // sortFilesSlowToolStripMenuItem
        // 
        sortFilesSlowToolStripMenuItem.CheckOnClick = true;
        sortFilesSlowToolStripMenuItem.Name = "sortFilesSlowToolStripMenuItem";
        sortFilesSlowToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
        sortFilesSlowToolStripMenuItem.Text = "Sort Files (Slow)";
        sortFilesSlowToolStripMenuItem.CheckedChanged += sortFilesSlowToolStripMenuItem_CheckedChanged;
        // 
        // hideEmptyPacksToolStripMenuItem
        // 
        hideEmptyPacksToolStripMenuItem.Checked = true;
        hideEmptyPacksToolStripMenuItem.CheckOnClick = true;
        hideEmptyPacksToolStripMenuItem.CheckState = CheckState.Checked;
        hideEmptyPacksToolStripMenuItem.Name = "hideEmptyPacksToolStripMenuItem";
        hideEmptyPacksToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
        hideEmptyPacksToolStripMenuItem.Text = "Hide Empty Packs";
        hideEmptyPacksToolStripMenuItem.CheckedChanged += hideEmptyPacksToolStripMenuItem_CheckedChanged;
        // 
        // exportToolStripMenuItem
        // 
        exportToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { actorsToolStripMenuItem, texturesToolStripMenuItem, stringTablesToolStripMenuItem, rawFilesToolStripMenuItem });
        exportToolStripMenuItem.Name = "exportToolStripMenuItem";
        exportToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
        exportToolStripMenuItem.Text = "Export";
        // 
        // actorsToolStripMenuItem
        // 
        actorsToolStripMenuItem.Name = "actorsToolStripMenuItem";
        actorsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        actorsToolStripMenuItem.Text = "Actors";
        actorsToolStripMenuItem.Click += actorsToolStripMenuItem_Click;
        // 
        // texturesToolStripMenuItem
        // 
        texturesToolStripMenuItem.Name = "texturesToolStripMenuItem";
        texturesToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        texturesToolStripMenuItem.Text = "Textures";
        texturesToolStripMenuItem.Click += texturesToolStripMenuItem_Click;
        // 
        // stringTablesToolStripMenuItem
        // 
        stringTablesToolStripMenuItem.Name = "stringTablesToolStripMenuItem";
        stringTablesToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        stringTablesToolStripMenuItem.Text = "String Tables";
        stringTablesToolStripMenuItem.Click += stringTablesToolStripMenuItem_Click;
        // 
        // rawFilesToolStripMenuItem
        // 
        rawFilesToolStripMenuItem.Name = "rawFilesToolStripMenuItem";
        rawFilesToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
        rawFilesToolStripMenuItem.Text = "Raw Files (Visible Only)";
        rawFilesToolStripMenuItem.Click += rawFilesToolStripMenuItem_Click;
        // 
        // BottomToolStripPanel
        // 
        BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
        BottomToolStripPanel.Name = "BottomToolStripPanel";
        BottomToolStripPanel.Orientation = Orientation.Horizontal;
        BottomToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
        BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
        // 
        // TopToolStripPanel
        // 
        TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
        TopToolStripPanel.Name = "TopToolStripPanel";
        TopToolStripPanel.Orientation = Orientation.Horizontal;
        TopToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
        TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
        // 
        // RightToolStripPanel
        // 
        RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
        RightToolStripPanel.Name = "RightToolStripPanel";
        RightToolStripPanel.Orientation = Orientation.Horizontal;
        RightToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
        RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
        // 
        // LeftToolStripPanel
        // 
        LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
        LeftToolStripPanel.Name = "LeftToolStripPanel";
        LeftToolStripPanel.Orientation = Orientation.Horizontal;
        LeftToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
        LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
        // 
        // ContentPanel
        // 
        ContentPanel.Size = new System.Drawing.Size(1008, 690);
        // 
        // mainTabControl
        // 
        mainTabControl.Dock = DockStyle.Fill;
        mainTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
        mainTabControl.Location = new System.Drawing.Point(0, 0);
        mainTabControl.Name = "mainTabControl";
        mainTabControl.SelectedIndex = 0;
        mainTabControl.Size = new System.Drawing.Size(672, 715);
        mainTabControl.TabIndex = 1;
        mainTabControl.DrawItem += mainTabControl_DrawItem;
        mainTabControl.MouseDown += mainTabControl_MouseDown;
        // 
        // mainTreeView
        // 
        mainTreeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        mainTreeView.ImageIndex = 0;
        mainTreeView.ImageList = iconImageList;
        mainTreeView.Location = new System.Drawing.Point(2, 65);
        mainTreeView.Margin = new Padding(0);
        mainTreeView.Name = "mainTreeView";
        treeNode1.ImageIndex = 0;
        treeNode1.Name = "Data";
        treeNode1.SelectedImageIndex = 0;
        treeNode1.Text = "Data";
        mainTreeView.Nodes.AddRange(new TreeNode[] { treeNode1 });
        mainTreeView.SelectedImageIndex = 0;
        mainTreeView.ShowNodeToolTips = true;
        mainTreeView.Size = new System.Drawing.Size(331, 649);
        mainTreeView.TabIndex = 3;
        mainTreeView.AfterCollapse += mainTreeView_AfterCollapse;
        mainTreeView.AfterExpand += mainTreeView_AfterExpand;
        mainTreeView.NodeMouseClick += mainTreeView_NodeMouseClick;
        mainTreeView.DoubleClick += mainTreeView_DoubleClick;
        // 
        // fileContextMenuStrip
        // 
        fileContextMenuStrip.Items.AddRange(new ToolStripItem[] { exportFileToolStripMenuItem, saveRawToolStripMenuItem });
        fileContextMenuStrip.Name = "fileContextMenuStrip";
        fileContextMenuStrip.ShowImageMargin = false;
        fileContextMenuStrip.Size = new System.Drawing.Size(99, 48);
        // 
        // exportFileToolStripMenuItem
        // 
        exportFileToolStripMenuItem.Name = "exportFileToolStripMenuItem";
        exportFileToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
        exportFileToolStripMenuItem.Text = "Export";
        exportFileToolStripMenuItem.Click += exportFileToolStripMenuItem_Click;
        // 
        // saveRawToolStripMenuItem
        // 
        saveRawToolStripMenuItem.Name = "saveRawToolStripMenuItem";
        saveRawToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
        saveRawToolStripMenuItem.Text = "Save Raw";
        saveRawToolStripMenuItem.Click += saveRawToolStripMenuItem_Click;
        // 
        // filterComboBox
        // 
        filterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        filterComboBox.FormattingEnabled = true;
        filterComboBox.Items.AddRange(new object[] { "Texture", "Actor", "Sample", "Font", "String Table", "Splash", "Collision Mesh", "Audio Stream", "Lip Sync", "Stream Data", "Reflection Data", "Sound Bank", "Sound Patch Old", "Light Matrix", "Simulation Data", "Subtitle", "Material", "Vertex Shader", "Pixel Shader", "Post Proc Pipe", "Render Target", "Anim Tree", "Anim Graph", "Vertex Format", "Facial Animation", "Speech Grammar", "User Properties", "Lua Script", "Lua VM", "Vertex Buffer", "Index Buffer", "Terrain Tile", "Sound", "Sound Patch", "Curve", "Sequence", "Material Appearance", "Dictionary", "User Interface", "Custom Data", "All" });
        filterComboBox.Location = new System.Drawing.Point(55, 7);
        filterComboBox.Name = "filterComboBox";
        filterComboBox.Size = new System.Drawing.Size(273, 23);
        filterComboBox.TabIndex = 4;
        filterComboBox.SelectedIndexChanged += resourceTypeComboBox_SelectedIndexChanged;
        // 
        // filterLabel
        // 
        filterLabel.AutoSize = true;
        filterLabel.Location = new System.Drawing.Point(16, 10);
        filterLabel.Name = "filterLabel";
        filterLabel.Size = new System.Drawing.Size(33, 15);
        filterLabel.TabIndex = 5;
        filterLabel.Text = "Filter";
        // 
        // searchLabel
        // 
        searchLabel.AutoSize = true;
        searchLabel.Location = new System.Drawing.Point(7, 39);
        searchLabel.Name = "searchLabel";
        searchLabel.Size = new System.Drawing.Size(42, 15);
        searchLabel.TabIndex = 6;
        searchLabel.Text = "Search";
        // 
        // searchTextBox
        // 
        searchTextBox.Location = new System.Drawing.Point(55, 36);
        searchTextBox.Name = "searchTextBox";
        searchTextBox.Size = new System.Drawing.Size(244, 23);
        searchTextBox.TabIndex = 7;
        // 
        // mainSplitContainer
        // 
        mainSplitContainer.Dock = DockStyle.Fill;
        mainSplitContainer.Location = new System.Drawing.Point(0, 24);
        mainSplitContainer.Margin = new Padding(0);
        mainSplitContainer.Name = "mainSplitContainer";
        // 
        // mainSplitContainer.Panel1
        // 
        mainSplitContainer.Panel1.Controls.Add(searchButton);
        mainSplitContainer.Panel1.Controls.Add(mainTreeView);
        mainSplitContainer.Panel1.Controls.Add(searchTextBox);
        mainSplitContainer.Panel1.Controls.Add(searchLabel);
        mainSplitContainer.Panel1.Controls.Add(filterComboBox);
        mainSplitContainer.Panel1.Controls.Add(filterLabel);
        mainSplitContainer.Panel1MinSize = 334;
        // 
        // mainSplitContainer.Panel2
        // 
        mainSplitContainer.Panel2.Controls.Add(mainTabControl);
        mainSplitContainer.Size = new System.Drawing.Size(1008, 715);
        mainSplitContainer.SplitterDistance = 334;
        mainSplitContainer.SplitterWidth = 2;
        mainSplitContainer.TabIndex = 8;
        // 
        // searchButton
        // 
        searchButton.Location = new System.Drawing.Point(303, 35);
        searchButton.Name = "searchButton";
        searchButton.Size = new System.Drawing.Size(25, 25);
        searchButton.TabIndex = 9;
        searchButton.Text = "🔎︎";
        searchButton.UseVisualStyleBackColor = true;
        searchButton.Click += searchButton_Click;
        // 
        // Main
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1008, 761);
        Controls.Add(mainSplitContainer);
        Controls.Add(mainStatusStrip);
        Controls.Add(mainMenuStrip);
        MinimumSize = new System.Drawing.Size(1024, 800);
        Name = "Main";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Resource Explorer v0.0.0.0 | SkySaga: Infinite Isles";
        FormClosed += Main_FormClosed;
        mainStatusStrip.ResumeLayout(false);
        mainStatusStrip.PerformLayout();
        mainMenuStrip.ResumeLayout(false);
        mainMenuStrip.PerformLayout();
        fileContextMenuStrip.ResumeLayout(false);
        mainSplitContainer.Panel1.ResumeLayout(false);
        mainSplitContainer.Panel1.PerformLayout();
        mainSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
        mainSplitContainer.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private StatusStrip mainStatusStrip;
    private MenuStrip mainMenuStrip;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem openToolStripMenuItem;
    private ImageList iconImageList;
    private ToolStripMenuItem exportToolStripMenuItem;
    private ToolStripMenuItem texturesToolStripMenuItem;
    private ToolStripMenuItem actorsToolStripMenuItem;
    private ToolStripPanel BottomToolStripPanel;
    private ToolStripPanel TopToolStripPanel;
    private ToolStripPanel RightToolStripPanel;
    private ToolStripPanel LeftToolStripPanel;
    private ToolStripContentPanel ContentPanel;
    private TextBox searchTextBox;
    private Label searchLabel;
    private Label filterLabel;
    private ComboBox filterComboBox;
    private TreeView mainTreeView;
    private TabControl mainTabControl;
    private SplitContainer mainSplitContainer;
    private Button searchButton;
    private ToolStripStatusLabel fileCountToolStripStatusLabel;
    private ToolStripStatusLabel packCountToolStripStatusLabel;
    private ToolStripMenuItem viewToolStripMenuItem;
    private ToolStripMenuItem closeAllTabsToolStripMenuItem;
    private ToolStripMenuItem hideEmptyPacksToolStripMenuItem;
    private ToolStripMenuItem stringTablesToolStripMenuItem;
    private ToolStripMenuItem consoleLogToolStripMenuItem;
    private ToolStripStatusLabel nameListCountToolStripStatusLabel;
    private ToolStripMenuItem sortFilesSlowToolStripMenuItem;
    private ToolStripMenuItem rawFilesToolStripMenuItem;
    private ContextMenuStrip fileContextMenuStrip;
    private ToolStripMenuItem exportFileToolStripMenuItem;
    private ToolStripMenuItem saveRawToolStripMenuItem;
    private ColorDialog colorDialog1;
}