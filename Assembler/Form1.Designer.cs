
namespace Assembler
{
    partial class Form1
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
            this.inputTxt = new System.Windows.Forms.TextBox();
            this.outputTxt = new System.Windows.Forms.TextBox();
            this.assembleBtn = new System.Windows.Forms.Button();
            this.ABCompileBtn = new System.Windows.Forms.Button();
            this.VMCompileBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // inputTxt
            // 
            this.inputTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.inputTxt.Location = new System.Drawing.Point(12, 12);
            this.inputTxt.Multiline = true;
            this.inputTxt.Name = "inputTxt";
            this.inputTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.inputTxt.Size = new System.Drawing.Size(316, 426);
            this.inputTxt.TabIndex = 0;
            // 
            // outputTxt
            // 
            this.outputTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTxt.Location = new System.Drawing.Point(472, 12);
            this.outputTxt.Multiline = true;
            this.outputTxt.Name = "outputTxt";
            this.outputTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTxt.Size = new System.Drawing.Size(316, 426);
            this.outputTxt.TabIndex = 1;
            // 
            // assembleBtn
            // 
            this.assembleBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.assembleBtn.Location = new System.Drawing.Point(334, 12);
            this.assembleBtn.Name = "assembleBtn";
            this.assembleBtn.Size = new System.Drawing.Size(132, 34);
            this.assembleBtn.TabIndex = 2;
            this.assembleBtn.Text = "Assemble";
            this.assembleBtn.UseVisualStyleBackColor = true;
            this.assembleBtn.Click += new System.EventHandler(this.assembleBtn_Click);
            // 
            // ABCompileBtn
            // 
            this.ABCompileBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ABCompileBtn.Location = new System.Drawing.Point(334, 92);
            this.ABCompileBtn.Name = "ABCompileBtn";
            this.ABCompileBtn.Size = new System.Drawing.Size(132, 34);
            this.ABCompileBtn.TabIndex = 3;
            this.ABCompileBtn.Text = "AB Compile";
            this.ABCompileBtn.UseVisualStyleBackColor = true;
            this.ABCompileBtn.Click += new System.EventHandler(this.ABCompileBtn_Click);
            // 
            // VMCompileBtn
            // 
            this.VMCompileBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.VMCompileBtn.Location = new System.Drawing.Point(334, 52);
            this.VMCompileBtn.Name = "VMCompileBtn";
            this.VMCompileBtn.Size = new System.Drawing.Size(132, 34);
            this.VMCompileBtn.TabIndex = 4;
            this.VMCompileBtn.Text = "VM Compile";
            this.VMCompileBtn.UseVisualStyleBackColor = true;
            this.VMCompileBtn.Click += new System.EventHandler(this.VMCompileBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 449);
            this.Controls.Add(this.VMCompileBtn);
            this.Controls.Add(this.ABCompileBtn);
            this.Controls.Add(this.assembleBtn);
            this.Controls.Add(this.outputTxt);
            this.Controls.Add(this.inputTxt);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputTxt;
        private System.Windows.Forms.TextBox outputTxt;
        private System.Windows.Forms.Button assembleBtn;
        private System.Windows.Forms.Button ABCompileBtn;
        private System.Windows.Forms.Button VMCompileBtn;
    }
}

