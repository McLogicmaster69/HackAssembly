
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
            this.SuspendLayout();
            // 
            // inputTxt
            // 
            this.inputTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.inputTxt.Location = new System.Drawing.Point(8, 8);
            this.inputTxt.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.inputTxt.Multiline = true;
            this.inputTxt.Name = "inputTxt";
            this.inputTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.inputTxt.Size = new System.Drawing.Size(212, 278);
            this.inputTxt.TabIndex = 0;
            // 
            // outputTxt
            // 
            this.outputTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTxt.Location = new System.Drawing.Point(315, 8);
            this.outputTxt.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.outputTxt.Multiline = true;
            this.outputTxt.Name = "outputTxt";
            this.outputTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTxt.Size = new System.Drawing.Size(212, 278);
            this.outputTxt.TabIndex = 1;
            // 
            // assembleBtn
            // 
            this.assembleBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.assembleBtn.Location = new System.Drawing.Point(223, 8);
            this.assembleBtn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.assembleBtn.Name = "assembleBtn";
            this.assembleBtn.Size = new System.Drawing.Size(88, 22);
            this.assembleBtn.TabIndex = 2;
            this.assembleBtn.Text = "Assemble";
            this.assembleBtn.UseVisualStyleBackColor = true;
            this.assembleBtn.Click += new System.EventHandler(this.assembleBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 292);
            this.Controls.Add(this.assembleBtn);
            this.Controls.Add(this.outputTxt);
            this.Controls.Add(this.inputTxt);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputTxt;
        private System.Windows.Forms.TextBox outputTxt;
        private System.Windows.Forms.Button assembleBtn;
    }
}

