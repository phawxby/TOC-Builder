using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Submission_TOC_Builder
{
    public partial class Builder : Form
    {
        private Builders.IBuilder _builder;

        public Builder()
        {
            InitializeComponent();

            // Should the need arise to build different types of TOC's or anything else we can change
            // this to a select box some such
            _builder = new Builders.NeesToc();
        }

        private void btn_SelectDirectory_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (!String.IsNullOrWhiteSpace(txt_Path.Text) && Directory.Exists(txt_Path.Text))
                {
                    dlg.SelectedPath = txt_Path.Text;
                }

                dlg.ShowDialog();

                SetDir(dlg.SelectedPath);
            }
        }

        private void updateFoundLbl()
        {
            if (_builder.ValidDir)
            {
                    lbl_Found.Text = String.Format("Found {0} pdfs", _builder.FoundFiles);
            }
            else
            {
                lbl_Found.Text = "";
            }
        }

        private void btn_Build_Click(object sender, EventArgs e)
        {
            Build();
        }

        private void Build()
        {
            try
            {
                _builder.Build();

                lbl_Found.Text = "Done!";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error building pdf: " + ex.Message + "\n\nThis is likely due to an invalid document structure");
            }
        }

        private void txt_Path_TextChanged(object sender, EventArgs e)
        {
            SetDir(txt_Path.Text, true);
        }

        private void Builder_Load(object sender, EventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                SetDir(args[1]);
            }
        }

        private void SetDir(String path, Boolean textBoxInput = false)
        {
            _builder.SetDir(path);

            btn_Build.Enabled = _builder.ValidDir && _builder.FoundFiles > 0;

            if (_builder.ValidDir)
            {
                if (!textBoxInput)
                    txt_Path.Text = path;

                updateFoundLbl();
            }
            else
            {
                MessageBox.Show("Selected path is invalid");
            }
        }
    }
}
