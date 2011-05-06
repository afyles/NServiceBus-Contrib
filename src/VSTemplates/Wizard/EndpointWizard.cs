using System;
using Microsoft.VisualStudio.TemplateWizard;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NServiceBus.Contrib.Templates.Wizard
{
    public class EndpointWizard : IWizard
    {
        private CollectUserInputForm inputForm;
        private String libPath;

        #region IWizard Members

        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
        }

        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            try
            {
                this.inputForm = new CollectUserInputForm();
                this.inputForm.ShowDialog();
                this.libPath = this.inputForm.LibraryPath;

                replacementsDictionary.Add("$librarypath$", this.libPath);
            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        #endregion
    }
}
