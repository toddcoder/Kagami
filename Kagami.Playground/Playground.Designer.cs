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
         components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Playground));
         table = new TableLayoutPanel();
         textEditor = new Core.WinForms.Controls.ExRichTextBox();
         textConsole = new Core.WinForms.Controls.ExRichTextBox();
         labelStatus = new Label();
         timerIdle = new System.Windows.Forms.Timer(components);
         table.SuspendLayout();
         SuspendLayout();
         // 
         // table
         // 
         table.ColumnCount = 1;
         table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         table.Controls.Add(textEditor, 0, 0);
         table.Controls.Add(textConsole, 0, 2);
         table.Controls.Add(labelStatus, 0, 1);
         table.Dock = DockStyle.Fill;
         table.Location = new Point(0, 0);
         table.Margin = new Padding(4, 3, 4, 3);
         table.Name = "table";
         table.RowCount = 3;
         table.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         table.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
         table.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         table.Size = new Size(999, 741);
         table.TabIndex = 0;
         // 
         // textEditor
         // 
         textEditor.AnnotationFont = new Font("Calibri", 12F, FontStyle.Bold);
         textEditor.BorderStyle = BorderStyle.None;
         textEditor.DetectUrls = false;
         textEditor.Dock = DockStyle.Fill;
         textEditor.Location = new Point(4, 3);
         textEditor.Margin = new Padding(4, 3, 4, 3);
         textEditor.ModificationGlyphLeftMargin = 2;
         textEditor.ModificationGlyphWidth = 4F;
         textEditor.ModificationLocked = false;
         textEditor.ModifiedGlyphColor = Color.Gold;
         textEditor.Name = "textEditor";
         textEditor.ObjectId = 0L;
         textEditor.SavedGlyphColor = Color.Green;
         textEditor.Selection = ((int, int))resources.GetObject("textEditor.Selection");
         textEditor.Size = new Size(991, 346);
         textEditor.TabIndex = 0;
         textEditor.Text = "";
         textEditor.SelectionChanged += textEditor_SelectionChanged;
         textEditor.TextChanged += textEditor_TextChanged;
         textEditor.KeyPress += textEditor_KeyPress;
         textEditor.KeyUp += textEditor_KeyUp;
         // 
         // textConsole
         // 
         textConsole.AnnotationFont = new Font("Calibri", 12F, FontStyle.Bold);
         textConsole.BorderStyle = BorderStyle.None;
         textConsole.DetectUrls = false;
         textConsole.Dock = DockStyle.Fill;
         textConsole.Location = new Point(4, 392);
         textConsole.Margin = new Padding(4, 3, 4, 3);
         textConsole.ModificationGlyphLeftMargin = 2;
         textConsole.ModificationGlyphWidth = 4F;
         textConsole.ModificationLocked = false;
         textConsole.ModifiedGlyphColor = Color.Gold;
         textConsole.Name = "textConsole";
         textConsole.ObjectId = 0L;
         textConsole.SavedGlyphColor = Color.Green;
         textConsole.Selection = ((int, int))resources.GetObject("textConsole.Selection");
         textConsole.Size = new Size(991, 346);
         textConsole.TabIndex = 1;
         textConsole.Text = "";
         // 
         // labelStatus
         // 
         labelStatus.AutoSize = true;
         labelStatus.BackColor = Color.SkyBlue;
         labelStatus.Dock = DockStyle.Fill;
         labelStatus.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
         labelStatus.Location = new Point(4, 352);
         labelStatus.Margin = new Padding(4, 0, 4, 0);
         labelStatus.Name = "labelStatus";
         labelStatus.Size = new Size(991, 37);
         labelStatus.TabIndex = 2;
         labelStatus.Text = "ready";
         labelStatus.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // timerIdle
         // 
         timerIdle.Enabled = true;
         timerIdle.Interval = 500;
         timerIdle.Tick += timerIdle_Tick;
         // 
         // Playground
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(999, 741);
         Controls.Add(table);
         KeyPreview = true;
         Margin = new Padding(4, 3, 4, 3);
         Name = "Playground";
         StartPosition = FormStartPosition.WindowsDefaultBounds;
         Text = "Kagami Playground";
         FormClosing += Playground_FormClosing;
         Load += Playground_Load;
         KeyUp += Playground_KeyUp;
         table.ResumeLayout(false);
         table.PerformLayout();
         ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.TableLayoutPanel table;
      private Core.WinForms.Controls.ExRichTextBox textEditor;
      private Core.WinForms.Controls.ExRichTextBox textConsole;
      private System.Windows.Forms.Label labelStatus;
      private System.Windows.Forms.Timer timerIdle;
   }
}

