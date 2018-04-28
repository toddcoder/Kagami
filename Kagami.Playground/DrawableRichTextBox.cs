using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kagami.Playground
{
   public class DrawableRichTextBox : RichTextBox
   {
      public class WindowExtender : NativeWindow
      {
         const int WM_PAINT = 15;

         DrawableRichTextBox baseControl;
         Bitmap canvas;
         Graphics bufferGraphics;
         Rectangle bufferClip;
         Graphics controlGraphics;
         bool canRender;

         public WindowExtender(DrawableRichTextBox baseControl)
         {
            this.baseControl = baseControl;
            canRender = false;
            ReinitializeCanvas();
         }

         protected override void WndProc(ref Message m)
         {
            if (m.Msg == WM_PAINT)
            {
               baseControl.Invalidate();
               base.WndProc(ref m);
               onPerformPaint();
            }
            else
               base.WndProc(ref m);
         }

         protected void onPerformPaint()
         {
            if (!canRender)
               return;

            bufferGraphics.Clear(Color.Transparent);
            baseControl.OnPaint(new PaintEventArgs(bufferGraphics, bufferClip));
            controlGraphics.DrawImageUnscaled(canvas, 0, 0);
         }

         public void ReinitializeCanvas()
         {
            lock (this)
            {
               TearDown();
               canRender = baseControl.Width > 0 && baseControl.Height > 0;
               if (!canRender)
                  return;

               canvas = new Bitmap(baseControl.Width, baseControl.Height);
               bufferGraphics = Graphics.FromImage(canvas);
               bufferClip = baseControl.ClientRectangle;
               bufferGraphics.Clip = new Region(bufferClip);
               controlGraphics = Graphics.FromHwnd(baseControl.Handle);
            }
         }

         public void TearDown()
         {
            controlGraphics?.Dispose();
            bufferGraphics?.Dispose();
            canvas?.Dispose();
         }
      }

      WindowExtender windowExtender;

      public new event EventHandler<PaintEventArgs> Paint;

      public DrawableRichTextBox()
      {
         windowExtender = new WindowExtender(this);
         windowExtender.AssignHandle(Handle);
      }

      protected override void Dispose(bool disposing)
      {
         windowExtender.ReleaseHandle();
         windowExtender.TearDown();

         base.Dispose(disposing);
      }

      protected override void OnPaint(PaintEventArgs e) => Paint?.Invoke(this, e);

      protected override void OnResize(EventArgs e)
      {
         base.OnResize(e);

         windowExtender.ReinitializeCanvas();
      }

      public void ReassignHandle() => windowExtender.AssignHandle(Handle);
   }
}