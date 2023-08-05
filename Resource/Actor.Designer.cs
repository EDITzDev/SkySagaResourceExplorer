namespace ResourceExplorer
{
    partial class Actor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            indexBufferLabel = new System.Windows.Forms.Label();
            lodLabel = new System.Windows.Forms.Label();
            qualityLabel = new System.Windows.Forms.Label();
            vertexBufferLabel = new System.Windows.Forms.Label();
            materialAppearanceLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // indexBufferLabel
            // 
            indexBufferLabel.AutoSize = true;
            indexBufferLabel.Location = new System.Drawing.Point(28, 52);
            indexBufferLabel.Name = "indexBufferLabel";
            indexBufferLabel.Size = new System.Drawing.Size(80, 15);
            indexBufferLabel.TabIndex = 0;
            indexBufferLabel.Text = "IndexBuffer: 0";
            // 
            // lodLabel
            // 
            lodLabel.AutoSize = true;
            lodLabel.Location = new System.Drawing.Point(28, 22);
            lodLabel.Name = "lodLabel";
            lodLabel.Size = new System.Drawing.Size(39, 15);
            lodLabel.TabIndex = 1;
            lodLabel.Text = "Lod: 0";
            // 
            // qualityLabel
            // 
            qualityLabel.AutoSize = true;
            qualityLabel.Location = new System.Drawing.Point(28, 37);
            qualityLabel.Name = "qualityLabel";
            qualityLabel.Size = new System.Drawing.Size(57, 15);
            qualityLabel.TabIndex = 2;
            qualityLabel.Text = "Quality: 0";
            // 
            // vertexBufferLabel
            // 
            vertexBufferLabel.AutoSize = true;
            vertexBufferLabel.Location = new System.Drawing.Point(28, 67);
            vertexBufferLabel.Name = "vertexBufferLabel";
            vertexBufferLabel.Size = new System.Drawing.Size(83, 15);
            vertexBufferLabel.TabIndex = 3;
            vertexBufferLabel.Text = "VertexBuffer: 0";
            // 
            // materialAppearanceLabel
            // 
            materialAppearanceLabel.AutoSize = true;
            materialAppearanceLabel.Location = new System.Drawing.Point(28, 82);
            materialAppearanceLabel.Name = "materialAppearanceLabel";
            materialAppearanceLabel.Size = new System.Drawing.Size(125, 15);
            materialAppearanceLabel.TabIndex = 3;
            materialAppearanceLabel.Text = "MaterialAppearance: 0";
            // 
            // Actor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(materialAppearanceLabel);
            Controls.Add(vertexBufferLabel);
            Controls.Add(qualityLabel);
            Controls.Add(lodLabel);
            Controls.Add(indexBufferLabel);
            Name = "Actor";
            Size = new System.Drawing.Size(1227, 850);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label indexBufferLabel;
        private System.Windows.Forms.Label lodLabel;
        private System.Windows.Forms.Label qualityLabel;
        private System.Windows.Forms.Label vertexBufferLabel;
        private System.Windows.Forms.Label materialAppearanceLabel;
    }
}
