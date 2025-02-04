
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
            this.inputTxt.Location = new System.Drawing.Point(12, 12);
            this.inputTxt.Multiline = true;
            this.inputTxt.Name = "inputTxt";
            this.inputTxt.Size = new System.Drawing.Size(316, 426);
            this.inputTxt.TabIndex = 0;
            // 
            // outputTxt
            // 
            this.outputTxt.Location = new System.Drawing.Point(472, 12);
            this.outputTxt.Multiline = true;
            this.outputTxt.Name = "outputTxt";
            this.outputTxt.Size = new System.Drawing.Size(316, 426);
            this.outputTxt.TabIndex = 1;
            // 
            // assembleBtn
            // 
            this.assembleBtn.Location = new System.Drawing.Point(334, 12);
            this.assembleBtn.Name = "assembleBtn";
            this.assembleBtn.Size = new System.Drawing.Size(132, 34);
            this.assembleBtn.TabIndex = 2;
            this.assembleBtn.Text = "Assemble";
            this.assembleBtn.UseVisualStyleBackColor = true;
            this.assembleBtn.Click += new System.EventHandler(this.assembleBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
    }
}

