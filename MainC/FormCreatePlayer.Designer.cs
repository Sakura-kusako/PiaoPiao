namespace MainC
{
    partial class FormCreatePlayer
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
            this.myTextBox1 = new MainC.MyTextBox();
            this.SuspendLayout();
            // 
            // myTextBox1
            // 
            this.myTextBox1.BackImage = null;
            this.myTextBox1.Location = new System.Drawing.Point(12, 12);
            this.myTextBox1.Multiline = true;
            this.myTextBox1.Name = "myTextBox1";
            this.myTextBox1.Size = new System.Drawing.Size(210, 115);
            this.myTextBox1.TabIndex = 0;
            // 
            // FormCreatePlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.myTextBox1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCreatePlayer";
            this.Text = "CreatePlayer";
            this.Load += new System.EventHandler(this.CreatePlayer_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormCreatePlayer_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyTextBox myTextBox1;
    }
}