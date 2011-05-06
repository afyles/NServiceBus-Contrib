using System;
using System.Windows.Forms;

namespace NServiceBus.Contrib.Templates.Wizard
{
    public partial class CollectUserInputForm : Form
    {
        private String libPath;

        public CollectUserInputForm()
        {
            InitializeComponent();
        }

        public String LibraryPath
        {
            get { return this.libPath; }
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            DialogResult result = this.browseToLibPathDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.libPath = this.browseToLibPathDialog.SelectedPath;
                this.pathLabel.Text = this.libPath;
            }
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
