using Kagami.Library.Objects;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Plots;

public class Plot : Form, IObject
{
   protected List<Action<Graphics>> actions = [];
   protected Color penColor;
   protected float penSize;
   //Color brushColor;

   public Plot()
   {
      penColor = Color.Black;
      penSize = 1;
      //brushColor = Color.Black;

      Paint += paint;
   }

   protected void paint(object sender, PaintEventArgs e)
   {
      foreach (var action in actions)
      {
         action(e.Graphics);
      }
   }

   public string ClassName => "Plot";

   public string AsString => "Plot";

   public string Image => "Plot()";

   public int Hash => GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Plot;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public IObject ShowPlot()
   {
      Show();
      return this;
   }

   protected Pen getPen() => new(penColor, penSize);

   public IObject Line(int x1, int y1, int x2, int y2)
   {
      actions.Add(g =>
      {
         using var pen = getPen();
         g.DrawLine(pen, x1, y1, x2, y2);
      });

      return this;
   }
}