namespace Submission_TOC_Builder
{
    partial class Builder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Builder));
            this.txt_Path = new System.Windows.Forms.TextBox();
            this.btn_SelectDirectory = new System.Windows.Forms.Button();
            this.btn_Build = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_Found = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt_Path
            // 
            this.txt_Path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Path.Location = new System.Drawing.Point(70, 12);
            this.txt_Path.Name = "txt_Path";
            this.txt_Path.Size = new System.Drawing.Size(357, 20);
            this.txt_Path.TabIndex = 0;
            this.txt_Path.TextChanged += new System.EventHandler(this.txt_Path_TextChanged);
            // 
            // btn_SelectDirectory
            // 
            this.btn_SelectDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_SelectDirectory.Location = new System.Drawing.Point(433, 10);
            this.btn_SelectDirectory.Name = "btn_SelectDirectory";
            this.btn_SelectDirectory.Size = new System.Drawing.Size(114, 23);
            this.btn_SelectDirectory.TabIndex = 2;
            this.btn_SelectDirectory.Text = "Select Directory";
            this.btn_SelectDirectory.UseVisualStyleBackColor = true;
            this.btn_SelectDirectory.Click += new System.EventHandler(this.btn_SelectDirectory_Click);
            // 
            // btn_Build
            // 
            this.btn_Build.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Build.Enabled = false;
            this.btn_Build.Location = new System.Drawing.Point(433, 38);
            this.btn_Build.Name = "btn_Build";
            this.btn_Build.Size = new System.Drawing.Size(114, 23);
            this.btn_Build.TabIndex = 3;
            this.btn_Build.Text = "Build";
            this.btn_Build.UseVisualStyleBackColor = true;
            this.btn_Build.Click += new System.EventHandler(this.btn_Build_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Directory:";
            // 
            // lbl_Found
            // 
            this.lbl_Found.AutoSize = true;
            this.lbl_Found.Location = new System.Drawing.Point(67, 43);
            this.lbl_Found.MaximumSize = new System.Drawing.Size(200, 0);
            this.lbl_Found.Name = "lbl_Found";
            this.lbl_Found.Size = new System.Drawing.Size(0, 13);
            this.lbl_Found.TabIndex = 5;
            // 
            // Builder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 70);
            this.Controls.Add(this.lbl_Found);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Build);
            this.Controls.Add(this.btn_SelectDirectory);
            this.Controls.Add(this.txt_Path);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(575, 108);
            this.MinimumSize = new System.Drawing.Size(575, 108);
            this.Name = "Builder";
            this.Text = "Submission TOC Builder";
            this.Load += new System.EventHandler(this.Builder_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_Path;
        private System.Windows.Forms.Button btn_SelectDirectory;
        private System.Windows.Forms.Button btn_Build;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_Found;

    }
}

