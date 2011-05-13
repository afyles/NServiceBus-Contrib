using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace NServiceBus.Contrib.Templates.Wizard
{
    public partial class CollectUserInputForm : Form
    {
        private const String AppDataSubDirectory = "NServiceBus\\Templates";
        private const String TemplateStateFileName = "state.xml";
        private const String LastLibraryPathNodeName = "LastLibraryPath";
        private String templateStateFilePath;
        private String appDataDirectory;
        private String libPath;

        public CollectUserInputForm()
        {
            InitializeComponent();

            this.appDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppDataSubDirectory);

            this.templateStateFilePath = Path.Combine(
                this.appDataDirectory,
                TemplateStateFileName);

            this.LoadLastPath();
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
            this.SaveLastPath();
            this.Dispose();
        }

        private void relPathTextBox_TextChanged(object sender, EventArgs e)
        {
            this.libPath = this.relPathTextBox.Text;
            this.pathLabel.Text = this.libPath;
        }


        private void LoadLastPath()
        {
            if (File.Exists(this.templateStateFilePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(this.templateStateFilePath);
                XmlNode lastLibPathNode = doc.SelectSingleNode(LastLibraryPathNodeName);

                if (null != lastLibPathNode)
                {
                    this.libPath = lastLibPathNode.InnerText;
                    this.pathLabel.Text = this.libPath;
                }
            }
        }

        private void SaveLastPath()
        {
            try
            {
                if (!Directory.Exists(this.appDataDirectory))
                    Directory.CreateDirectory(this.appDataDirectory);

                XmlDocument doc = new XmlDocument();
                XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", null, null);
                doc.AppendChild(declaration);
                XmlElement libPath = doc.CreateElement(LastLibraryPathNodeName);
                libPath.InnerText = this.libPath;
                doc.AppendChild(libPath);
                doc.Save(this.templateStateFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}: {1}","There was an error saving the last path chosen", ex.Message), "Error", MessageBoxButtons.OK);
            }
        }
    }
}
