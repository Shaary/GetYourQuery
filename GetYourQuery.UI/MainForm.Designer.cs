namespace GetYourQuery.UI
{
    partial class MainForm
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
            this.spTypeBox = new System.Windows.Forms.GroupBox();
            this.rbDelete = new System.Windows.Forms.RadioButton();
            this.rbAdd = new System.Windows.Forms.RadioButton();
            this.rbGet = new System.Windows.Forms.RadioButton();
            this.rbUpdate = new System.Windows.Forms.RadioButton();
            this.intoLabel = new System.Windows.Forms.Label();
            this.schemaBox = new System.Windows.Forms.ComboBox();
            this.query = new System.Windows.Forms.TextBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.schemaLabel = new System.Windows.Forms.Label();
            this.spTypeBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // spTypeBox
            // 
            this.spTypeBox.Controls.Add(this.rbDelete);
            this.spTypeBox.Controls.Add(this.rbAdd);
            this.spTypeBox.Controls.Add(this.rbGet);
            this.spTypeBox.Controls.Add(this.rbUpdate);
            this.spTypeBox.Location = new System.Drawing.Point(35, 22);
            this.spTypeBox.Name = "spTypeBox";
            this.spTypeBox.Size = new System.Drawing.Size(368, 93);
            this.spTypeBox.TabIndex = 1;
            this.spTypeBox.TabStop = false;
            this.spTypeBox.Text = "Choose type";
            // 
            // rbDelete
            // 
            this.rbDelete.AutoSize = true;
            this.rbDelete.Location = new System.Drawing.Point(273, 26);
            this.rbDelete.Name = "rbDelete";
            this.rbDelete.Padding = new System.Windows.Forms.Padding(10);
            this.rbDelete.Size = new System.Drawing.Size(94, 44);
            this.rbDelete.TabIndex = 3;
            this.rbDelete.TabStop = true;
            this.rbDelete.Text = "Delete";
            this.rbDelete.UseVisualStyleBackColor = true;
            // 
            // rbAdd
            // 
            this.rbAdd.AutoSize = true;
            this.rbAdd.Location = new System.Drawing.Point(84, 26);
            this.rbAdd.Name = "rbAdd";
            this.rbAdd.Padding = new System.Windows.Forms.Padding(10);
            this.rbAdd.Size = new System.Drawing.Size(78, 44);
            this.rbAdd.TabIndex = 1;
            this.rbAdd.TabStop = true;
            this.rbAdd.Text = "Add";
            this.rbAdd.UseVisualStyleBackColor = true;
            // 
            // rbGet
            // 
            this.rbGet.AutoSize = true;
            this.rbGet.Location = new System.Drawing.Point(5, 26);
            this.rbGet.Name = "rbGet";
            this.rbGet.Padding = new System.Windows.Forms.Padding(10);
            this.rbGet.Size = new System.Drawing.Size(73, 44);
            this.rbGet.TabIndex = 0;
            this.rbGet.TabStop = true;
            this.rbGet.Text = "Get";
            this.rbGet.UseVisualStyleBackColor = true;
            // 
            // rbUpdate
            // 
            this.rbUpdate.AutoSize = true;
            this.rbUpdate.Location = new System.Drawing.Point(168, 26);
            this.rbUpdate.Name = "rbUpdate";
            this.rbUpdate.Padding = new System.Windows.Forms.Padding(10);
            this.rbUpdate.Size = new System.Drawing.Size(99, 44);
            this.rbUpdate.TabIndex = 2;
            this.rbUpdate.TabStop = true;
            this.rbUpdate.Text = "Update";
            this.rbUpdate.UseVisualStyleBackColor = true;
            // 
            // intoLabel
            // 
            this.intoLabel.AutoSize = true;
            this.intoLabel.Location = new System.Drawing.Point(40, 134);
            this.intoLabel.Name = "intoLabel";
            this.intoLabel.Size = new System.Drawing.Size(132, 20);
            this.intoLabel.TabIndex = 2;
            this.intoLabel.Text = "Choose your query";
            // 
            // schemaBox
            // 
            this.schemaBox.FormattingEnabled = true;
            this.schemaBox.Location = new System.Drawing.Point(40, 157);
            this.schemaBox.Name = "schemaBox";
            this.schemaBox.Size = new System.Drawing.Size(486, 28);
            this.schemaBox.TabIndex = 3;
            // 
            // query
            // 
            this.query.Location = new System.Drawing.Point(40, 223);
            this.query.Multiline = true;
            this.query.Name = "query";
            this.query.ReadOnly = true;
            this.query.Size = new System.Drawing.Size(486, 185);
            this.query.TabIndex = 4;
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(565, 156);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(94, 29);
            this.btnFind.TabIndex = 5;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            // 
            // schemaLabel
            // 
            this.schemaLabel.AutoSize = true;
            this.schemaLabel.Location = new System.Drawing.Point(412, 22);
            this.schemaLabel.Name = "schemaLabel";
            this.schemaLabel.Size = new System.Drawing.Size(112, 20);
            this.schemaLabel.TabIndex = 6;
            this.schemaLabel.Text = "Choose schema";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.schemaLabel);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.query);
            this.Controls.Add(this.schemaBox);
            this.Controls.Add(this.intoLabel);
            this.Controls.Add(this.spTypeBox);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.spTypeBox.ResumeLayout(false);
            this.spTypeBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox spTypeBox;
        private System.Windows.Forms.RadioButton rbDelete;
        private System.Windows.Forms.RadioButton rbAdd;
        private System.Windows.Forms.RadioButton rbGet;
        private System.Windows.Forms.RadioButton rbUpdate;
        private System.Windows.Forms.Label intoLabel;
        private System.Windows.Forms.ComboBox schemaBox;
        private System.Windows.Forms.TextBox query;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label schemaLabel;
    }
}

