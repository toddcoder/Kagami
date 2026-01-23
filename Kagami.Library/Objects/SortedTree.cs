using System.Collections;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class SortedTree : IEnumerable<IObject>
{
   protected class Node(IObject key)
   {
      public IObject Key { get; set; } = key;

      public Maybe<Node> Left { get; set; } = nil;

      public Maybe<Node> Right { get; set; } = nil;

      public int Height { get; set; } = 1;
   }

   protected static int defaultCompare(IObject a, IObject b)
   {
      if (ReferenceEquals(a, b))
      {
         return 0;
      }

      if (a is IComparable<IObject> ic1)
      {
         return ic1.CompareTo(b);
      }

      if (b is IComparable<IObject> ic2)
      {
         return -ic2.CompareTo(a);
      }

      if (a is IComparable ia)
      {
         return ia.CompareTo(b);
      }

      if (b is IComparable ib)
      {
         return -ib.CompareTo(a);
      }

      return string.Compare(a.AsString, b.AsString, StringComparison.Ordinal);
   }

   protected Maybe<Node> _root = nil;
   protected int count;
   protected readonly System.Comparison<IObject> compare;

   public SortedTree(System.Comparison<IObject> comparer)
   {
      compare = comparer;
   }

   public SortedTree()
   {
      compare = defaultCompare;
   }

   protected static int height(Maybe<Node> _node) => _node.Map(n => n.Height) | 0;

   protected static int balanceFactor(Maybe<Node> _node) => _node.Map(n => height(n.Left) - height(n.Right)) | 0;

   protected static void updateHeight(Node node) => node.Height = height(node.Left).MaxOf(height(node.Right) + 1);

   protected Maybe<Node> rotateRight(Node y)
   {
      if (y.Left is (true, var x))
      {
         y.Left = x.Right;
         x.Right = y;
         updateHeight(y);
         updateHeight(x);
      }

      return y.Left;
   }

   protected Maybe<Node> rotateLeft(Node x)
   {
      if (x.Right is (true, var y))
      {
         x.Right = y.Left;
         y.Left = x;
         updateHeight(x);
         updateHeight(y);
      }

      return x.Right;
   }

   protected Node balance(Node node)
   {
      updateHeight(node);
      var bf = balanceFactor(node);
      switch (bf)
      {
         case > 1:
         {
            if (balanceFactor(node.Left) < 0)
            {
               node.Left = node.Left.Map(rotateLeft);
            }

            return rotateRight(node);
         }
         case < 1:
         {
            if (balanceFactor(node.Right) > 0)
            {
               node.Right = node.Right.Map(rotateRight);
            }

            return rotateLeft(node);
         }
         default:
            return node;
      }
   }

   public bool Add(IObject key)
   {
      var (node, added) = Insert(_root, key);
      _root = node;
      if (added)
      {
         count++;
      }

      return added;
   }

   protected (Node node, bool added) Insert(Maybe<Node> _node, IObject key)
   {
      if (_node is (true, var node))
      {
         var cmp = compare(key, node.Key);

         switch (cmp)
         {
            case < 0:
            {
               var (newLeft, added) = Insert(node.Left, key);
               node.Left = newLeft;
               return (balance(node), added);
            }
            case > 0:
            {
               var (newRight, added) = Insert(node.Right, key);
               node.Right = newRight;
               return (balance(node), added);
            }
            default:
               return (node, false);
         }
      }
      else
      {
         return (new Node(key), true);
      }
   }

   public bool Contains(IObject key) => contains(_root, key);

   protected bool contains(Maybe<Node> _node, IObject key)
   {
      while (_node is (true, var node))
      {
         var cmp = compare(key, node.Key);
         if (cmp == 0)
         {
            return true;
         }

         _node = cmp < 0 ? node.Left : node.Right;
      }

      return false;
   }

   public bool Remove(IObject key)
   {
      var (node, removed) = remove(_root, key);
      _root = node;
      if (removed)
      {
         --count;
      }

      return removed;
   }

   protected (Maybe<Node> node, bool removed) remove(Maybe<Node> _node, IObject key)
   {
      if (_node is (true, var node))
      {
         var cmp = compare(key, node.Key);
         switch (cmp)
         {
            case < 0:
            {
               var (newLeft, removed) = remove(node.Left, key);
               node.Left = newLeft;
               return (balance(node), removed);
            }
            case > 0:
            {
               var (newRight, removed) = remove(node.Right, key);
               node.Right = newRight;
               return (balance(node), removed);
            }
            default:
            {
               var removed = true;
               if (!node.Left || !node.Right)
               {
                  return (node.Left | node.Right, removed);
               }
               else
               {
                  if (node.Right is (true, var successor))
                  {
                     while (successor.Left is (true, var left))
                     {
                        successor = left;
                     }

                     node.Key = successor.Key;
                     (var _removedNode, removed) = remove(node.Right, successor.Key);
                     node.Key = successor.Key;
                     node.Right = _removedNode;

                     return (balance(node), removed);
                  }
                  else
                  {
                     return (nil, false);
                  }
               }
            }
         }
      }
      else
      {
         return (nil, false);
      }
   }

   public int Count => count;

   public Maybe<IObject> Min()
   {
      if (_root is (true, var root))
      {
         var node = root;
         while (node.Left is (true, var left))
         {
            node = left;
         }

         return node.Key.Some();
      }
      else
      {
         return nil;
      }
   }

   public Maybe<IObject> Max()
   {
      if (_root is (true, var root))
      {
         var node = root;
         while (node.Right is (true, var right))
         {
            node = right;
         }

         return node.Key.Some();
      }
      else
      {
         return nil;
      }
   }

   protected IEnumerable<IObject> inOrder(Maybe<Node> _node)
   {
      if (_node is (true, var node))
      {
         foreach (var leftKey in inOrder(node.Left))
         {
            yield return leftKey;
         }

         yield return node.Key;

         foreach (var rightKey in inOrder(node.Right))
         {
            yield return rightKey;
         }
      }
   }

   public IEnumerator<IObject> GetEnumerator() => inOrder(_root).GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}