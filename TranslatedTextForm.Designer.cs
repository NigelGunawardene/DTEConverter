namespace DTEConverter
{
    partial class TranslatedTextForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CloseButton = new System.Windows.Forms.Button();
            this.DutchLabel = new System.Windows.Forms.Label();
            this.EnglishLabel = new System.Windows.Forms.Label();
            this.DutchTextBox = new DTEConverter.SynchronizedScrollRichTextBox();
            this.EnglishTextBox = new DTEConverter.SynchronizedScrollRichTextBox();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(273, 393);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // DutchLabel
            // 
            this.DutchLabel.AutoSize = true;
            this.DutchLabel.Location = new System.Drawing.Point(12, 19);
            this.DutchLabel.Name = "DutchLabel";
            this.DutchLabel.Size = new System.Drawing.Size(42, 15);
            this.DutchLabel.TabIndex = 3;
            this.DutchLabel.Text = "Dutch:";
            // 
            // EnglishLabel
            // 
            this.EnglishLabel.AutoSize = true;
            this.EnglishLabel.Location = new System.Drawing.Point(190, 19);
            this.EnglishLabel.Name = "EnglishLabel";
            this.EnglishLabel.Size = new System.Drawing.Size(48, 15);
            this.EnglishLabel.TabIndex = 4;
            this.EnglishLabel.Text = "English:";
            // 
            // DutchTextBox
            // 
            this.DutchTextBox.Location = new System.Drawing.Point(12, 37);
            this.DutchTextBox.Name = "DutchTextBox";
            this.DutchTextBox.Size = new System.Drawing.Size(158, 336);
            this.DutchTextBox.TabIndex = 5;
            this.DutchTextBox.Text = "";
            // 
            // EnglishTextBox
            // 
            this.EnglishTextBox.Location = new System.Drawing.Point(190, 37);
            this.EnglishTextBox.Name = "EnglishTextBox";
            this.EnglishTextBox.Size = new System.Drawing.Size(158, 336);
            this.EnglishTextBox.TabIndex = 6;
            this.EnglishTextBox.Text = "";
            // 
            // TranslatedTextForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 426);
            this.Controls.Add(this.EnglishTextBox);
            this.Controls.Add(this.DutchTextBox);
            this.Controls.Add(this.EnglishLabel);
            this.Controls.Add(this.DutchLabel);
            this.Controls.Add(this.CloseButton);
            this.Name = "TranslatedTextForm";
            this.Text = "Translate Dutch To English";
            this.Load += new System.EventHandler(this.TranslatedTextForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Label DutchLabel;
        private System.Windows.Forms.Label EnglishLabel;
        private SynchronizedScrollRichTextBox DutchTextBox;
        private SynchronizedScrollRichTextBox EnglishTextBox;
    }
}
