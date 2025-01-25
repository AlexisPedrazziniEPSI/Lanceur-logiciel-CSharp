using System.Reflection.Emit;

namespace Lanceur_logiciel
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public System.Windows.Forms.Label Label1 { get; private set; }

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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form2";

            Label1 = new System.Windows.Forms.Label();
            Label1.AutoSize = true;
            Label1.Location = new System.Drawing.Point(12, 9);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(35, 13);
            Label1.TabIndex = 0;
            Label1.Text = "Emplacement du logiciel";
        }

        #endregion
    }
}