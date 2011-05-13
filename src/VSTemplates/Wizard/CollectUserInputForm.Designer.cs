namespace NServiceBus.Contrib.Templates.Wizard
{
    partial class CollectUserInputForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectUserInputForm));
            this.browseToLibPathDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.selectButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.instructionsLabel = new System.Windows.Forms.Label();
            this.pathLabel = new System.Windows.Forms.Label();
            this.relPathTextBox = new System.Windows.Forms.TextBox();
            this.fullPathBox = new System.Windows.Forms.GroupBox();
            this.relativePathBox = new System.Windows.Forms.GroupBox();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.fullPathBox.SuspendLayout();
            this.relativePathBox.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectButton
            // 
            this.selectButton.Location = new System.Drawing.Point(6, 23);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(75, 23);
            this.selectButton.TabIndex = 0;
            this.selectButton.Text = "&Select";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // finishButton
            // 
            this.finishButton.Location = new System.Drawing.Point(100, 204);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 1;
            this.finishButton.Text = "&Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // instructionsLabel
            // 
            this.instructionsLabel.AutoSize = true;
            this.instructionsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.instructionsLabel.Location = new System.Drawing.Point(0, 0);
            this.instructionsLabel.Margin = new System.Windows.Forms.Padding(3);
            this.instructionsLabel.Name = "instructionsLabel";
            this.instructionsLabel.Padding = new System.Windows.Forms.Padding(3);
            this.instructionsLabel.Size = new System.Drawing.Size(239, 19);
            this.instructionsLabel.TabIndex = 2;
            this.instructionsLabel.Text = "Select or enter the Hint path to the NSB libraries";
            // 
            // pathLabel
            // 
            this.pathLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathLabel.Location = new System.Drawing.Point(5, 129);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Padding = new System.Windows.Forms.Padding(5);
            this.pathLabel.Size = new System.Drawing.Size(274, 45);
            this.pathLabel.TabIndex = 3;
            this.pathLabel.Text = "/Path/To/NSB/Libraries";
            // 
            // relPathTextBox
            // 
            this.relPathTextBox.Location = new System.Drawing.Point(6, 24);
            this.relPathTextBox.Name = "relPathTextBox";
            this.relPathTextBox.Size = new System.Drawing.Size(245, 20);
            this.relPathTextBox.TabIndex = 4;
            this.relPathTextBox.TextChanged += new System.EventHandler(this.relPathTextBox_TextChanged);
            // 
            // fullPathBox
            // 
            this.fullPathBox.Controls.Add(this.selectButton);
            this.fullPathBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.fullPathBox.Location = new System.Drawing.Point(5, 5);
            this.fullPathBox.Name = "fullPathBox";
            this.fullPathBox.Size = new System.Drawing.Size(274, 62);
            this.fullPathBox.TabIndex = 5;
            this.fullPathBox.TabStop = false;
            this.fullPathBox.Text = "Full Path";
            // 
            // relativePathBox
            // 
            this.relativePathBox.Controls.Add(this.relPathTextBox);
            this.relativePathBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.relativePathBox.Location = new System.Drawing.Point(5, 67);
            this.relativePathBox.Name = "relativePathBox";
            this.relativePathBox.Size = new System.Drawing.Size(274, 62);
            this.relativePathBox.TabIndex = 6;
            this.relativePathBox.TabStop = false;
            this.relativePathBox.Text = "Relative Path";
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.pathLabel);
            this.mainPanel.Controls.Add(this.relativePathBox);
            this.mainPanel.Controls.Add(this.fullPathBox);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainPanel.Location = new System.Drawing.Point(0, 19);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(5);
            this.mainPanel.Size = new System.Drawing.Size(284, 179);
            this.mainPanel.TabIndex = 7;
            // 
            // CollectUserInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 236);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.instructionsLabel);
            this.Controls.Add(this.finishButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CollectUserInputForm";
            this.Text = "NSB Template Wizard";
            this.fullPathBox.ResumeLayout(false);
            this.relativePathBox.ResumeLayout(false);
            this.relativePathBox.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog browseToLibPathDialog;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.Label instructionsLabel;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.TextBox relPathTextBox;
        private System.Windows.Forms.GroupBox fullPathBox;
        private System.Windows.Forms.GroupBox relativePathBox;
        private System.Windows.Forms.Panel mainPanel;
    }
}