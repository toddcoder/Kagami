namespace Kagami.GuiTests
{
   partial class Tests
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
         tableLayoutPanel = new TableLayoutPanel();
         listViewTests = new ListView();
         columnHeaderTest = new ColumnHeader();
         columnHeaderExpected = new ColumnHeader();
         columnHeaderResults = new ColumnHeader();
         tableLayoutPanel.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel
         // 
         tableLayoutPanel.ColumnCount = 2;
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Controls.Add(listViewTests, 0, 0);
         tableLayoutPanel.Dock = DockStyle.Fill;
         tableLayoutPanel.Location = new Point(0, 0);
         tableLayoutPanel.Name = "tableLayoutPanel";
         tableLayoutPanel.RowCount = 2;
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Size = new Size(800, 450);
         tableLayoutPanel.TabIndex = 0;
         // 
         // listViewTests
         // 
         listViewTests.Columns.AddRange(new ColumnHeader[] { columnHeaderTest, columnHeaderExpected, columnHeaderResults });
         listViewTests.Dock = DockStyle.Fill;
         listViewTests.Location = new Point(3, 3);
         listViewTests.Name = "listViewTests";
         listViewTests.Size = new Size(394, 219);
         listViewTests.TabIndex = 0;
         listViewTests.UseCompatibleStateImageBehavior = false;
         listViewTests.View = View.Details;
         // 
         // columnHeaderTest
         // 
         columnHeaderTest.Text = "Test";
         // 
         // columnHeaderExpected
         // 
         columnHeaderExpected.Text = "Expected";
         // 
         // columnHeaderResults
         // 
         columnHeaderResults.Text = "Results";
         // 
         // Tests
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(800, 450);
         Controls.Add(tableLayoutPanel);
         Name = "Tests";
         StartPosition = FormStartPosition.CenterScreen;
         Text = "Kagami Tests";
         tableLayoutPanel.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel;
      private ListView listViewTests;
      private ColumnHeader columnHeaderTest;
      private ColumnHeader columnHeaderExpected;
      private ColumnHeader columnHeaderResults;
   }
}
