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
         this.textEditor = new Core.WinForms.Controls.ExRichTextBox();
         this.textConsole = new Core.WinForms.Controls.ExRichTextBox();
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
         this.table.Margin = new System.Windows.Forms.Padding(4);
         this.table.Name = "table";
         this.table.RowCount = 3;
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.table.Size = new System.Drawing.Size(1141, 790);
         this.table.TabIndex = 0;
         // 
         // textEditor
         // 
         this.textEditor.AnnotationFont = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
         this.textEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textEditor.Location = new System.Drawing.Point(4, 4);
         this.textEditor.Margin = new System.Windows.Forms.Padding(4);
         this.textEditor.Name = "textEditor";
         this.textEditor.Size = new System.Drawing.Size(1133, 367);
         this.textEditor.TabIndex = 0;
         this.textEditor.Text = "";
         this.textEditor.SelectionChanged += new System.EventHandler(this.textEditor_SelectionChanged);
         this.textEditor.TextChanged += new System.EventHandler(this.textEditor_TextChanged);
         this.textEditor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEditor_KeyPress);
         this.textEditor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textEditor_KeyUp);
         // 
         // textConsole
         // 
         this.textConsole.AnnotationFont = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
         this.textConsole.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.textConsole.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textConsole.Location = new System.Drawing.Point(4, 418);
         this.textConsole.Margin = new System.Windows.Forms.Padding(4);
         this.textConsole.Name = "textConsole";
         this.textConsole.Size = new System.Drawing.Size(1133, 368);
         this.textConsole.TabIndex = 1;
         this.textConsole.Text = "";
         // 
         // labelStatus
         // 
         this.labelStatus.AutoSize = true;
         this.labelStatus.BackColor = System.Drawing.Color.SkyBlue;
         this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
         this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelStatus.Location = new System.Drawing.Point(4, 375);
         this.labelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.labelStatus.Name = "labelStatus";
         this.labelStatus.Size = new System.Drawing.Size(1133, 39);
         this.labelStatus.TabIndex = 2;
         this.labelStatus.Text = "ready";
         this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // Playground
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1141, 790);
         this.Controls.Add(this.table);
         this.KeyPreview = true;
         this.Margin = new System.Windows.Forms.Padding(4);
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
      private Core.WinForms.Controls.ExRichTextBox textEditor;
      private Core.WinForms.Controls.ExRichTextBox textConsole;
      private System.Windows.Forms.Label labelStatus;
   }
}

