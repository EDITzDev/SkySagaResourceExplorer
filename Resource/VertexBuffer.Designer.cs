namespace ResourceExplorer
{
    partial class VertexBuffer
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
            vertexdataGridView = new System.Windows.Forms.DataGridView();
            splitContainer = new System.Windows.Forms.SplitContainer();
            formatdataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)vertexdataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)formatdataGridView).BeginInit();
            SuspendLayout();
            // 
            // vertexdataGridView
            // 
            vertexdataGridView.AllowUserToAddRows = false;
            vertexdataGridView.AllowUserToDeleteRows = false;
            vertexdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            vertexdataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            vertexdataGridView.Location = new System.Drawing.Point(0, 0);
            vertexdataGridView.Name = "vertexdataGridView";
            vertexdataGridView.ReadOnly = true;
            vertexdataGridView.RowHeadersVisible = false;
            vertexdataGridView.RowTemplate.Height = 25;
            vertexdataGridView.Size = new System.Drawing.Size(1227, 634);
            vertexdataGridView.TabIndex = 1;
            // 
            // splitContainer
            // 
            splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer.Location = new System.Drawing.Point(0, 0);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(formatdataGridView);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(vertexdataGridView);
            splitContainer.Size = new System.Drawing.Size(1227, 850);
            splitContainer.SplitterDistance = 212;
            splitContainer.TabIndex = 2;
            // 
            // formatdataGridView
            // 
            formatdataGridView.AllowUserToAddRows = false;
            formatdataGridView.AllowUserToDeleteRows = false;
            formatdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            formatdataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            formatdataGridView.Location = new System.Drawing.Point(0, 0);
            formatdataGridView.Name = "formatdataGridView";
            formatdataGridView.ReadOnly = true;
            formatdataGridView.RowHeadersVisible = false;
            formatdataGridView.RowTemplate.Height = 25;
            formatdataGridView.Size = new System.Drawing.Size(1227, 212);
            formatdataGridView.TabIndex = 2;
            // 
            // VertexBuffer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainer);
            Name = "VertexBuffer";
            ((System.ComponentModel.ISupportInitialize)vertexdataGridView).EndInit();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)formatdataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView vertexdataGridView;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataGridView formatdataGridView;
    }
}
