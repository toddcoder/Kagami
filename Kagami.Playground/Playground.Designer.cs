namespace Kagami.Playground
{
   partial class Playground
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
         this.table = new System.Windows.Forms.TableLayoutPanel();
         this.textEditor = new Kagami.Playground.DrawableRichTextBox();
         this.textConsole = new Kagami.Playground.DrawableRichTextBox();
         this.labelStatus = new System.Windows.Forms.Label();
         this.table.SuspendLayout();
         this.SuspendLayout();
         // 
         // table
         // 
         this.table.ColumnCount = 1;
         this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.table.Controls.Add(this.textEditor, 0, 0);
         this.table.Controls.Add(this.textConsole, 0, 2);
         this.table.Controls.Add(this.labelStatus, 0, 1);
         this.table.Dock = System.Windows.Forms.DockStyle.Fill;
         this.table.Location = new System.Drawing.Point(0, 0);
         this.table.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.table.Name = "table";
         this.table.RowCount = 3;
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.table.Size = new System.Drawing.Size(1284, 988);
         this.table.TabIndex = 0;
         // 
         // textEditor
         // 
         this.textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textEditor.Location = new System.Drawing.Point(4, 5);
         this.textEditor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.textEditor.Name = "textEditor";
         this.textEditor.Size = new System.Drawing.Size(1276, 459);
         this.textEditor.TabIndex = 0;
         this.textEditor.Text = "";
         this.textEditor.TextChanged += new System.EventHandler(this.textEditor_TextChanged);
         this.textEditor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEditor_KeyPress);
         this.textEditor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textEditor_KeyUp);
         // 
         // textConsole
         // 
         this.textConsole.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textConsole.Location = new System.Drawing.Point(4, 523);
         this.textConsole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.textConsole.Name = "textConsole";
         this.textConsole.Size = new System.Drawing.Size(1276, 460);
         this.textConsole.TabIndex = 1;
         this.textConsole.Text = "";
         // 
         // labelStatus
         // 
         this.labelStatus.AutoSize = true;
         this.labelStatus.BackColor = System.Drawing.Color.SkyBlue;
         this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
         this.labelStatus.Font = new System.Drawing.Font("Anonymous Pro", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelStatus.Location = new System.Drawing.Point(4, 469);
         this.labelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.labelStatus.Name = "labelStatus";
         this.labelStatus.Size = new System.Drawing.Size(1276, 49);
         this.labelStatus.TabIndex = 2;
         this.labelStatus.Text = "ready";
         this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // Playground
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1284, 988);
         this.Controls.Add(this.table);
         this.KeyPreview = true;
         this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.Name = "Playground";
         this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
         this.Text = "Kagami Playground";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Playground_FormClosing);
         this.Load += new System.EventHandler(this.Playground_Load);
         this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Playground_KeyUp);
         this.table.ResumeLayout(false);
         this.table.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.TableLayoutPanel table;
      private DrawableRichTextBox textEditor;
      private DrawableRichTextBox textConsole;
      private System.Windows.Forms.Label labelStatus;
   }
}

