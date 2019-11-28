using System;
using System.Collections.Generic;

namespace AlgGenTestWPF
{
    class Shape
    {
        public List<Muscle> muscles = new List<Muscle>();
        public List<Node> nodes = new List<Node>();
        public double center, startcenter=0;

        public Shape()
        {
            Random rnd = new Random();
            int nodeNr = rnd.Next(3, 7);
            for (int i = 0; i < nodeNr; i++)
            {
                nodes.Add(new Node());
                nodes[i].shape = this;
            }
            for (int i = 0; i < nodeNr; i++)
            {
                int muscleNr = rnd.Next(1, 5);
                for (int j = 0; j < nodeNr; j++)
                {
                    if (i!=j)
                    {
                        if (!Connected(nodes[i],nodes[j]))
                        {
                            muscles.Add(new Muscle(nodes[i], nodes[j]));
                        }
                    }
                    
                }
            }
            Normalize();
            CalculateCenter();
            startcenter = center;
            center -= startcenter;

        }
        public Shape(List<Muscle> a) 
        {
            muscles = a;
            foreach (var muscle in muscles)
            {
                foreach (var node in muscle.nodes)
                {
                    if (nodes.Find(x => x.Equals(node))== null)
                    {
                        node.shape = this;
                        nodes.Add(node);
                    }
                }
                
            }
            Normalize();
            CalculateCenter();
            startcenter = center;
            center -= startcenter;
        }

      
        public Shape(Shape toClone)
        {
            foreach (var node in toClone.nodes)
            {
                nodes.Add(new Node(node));
                nodes[nodes.Count - 1].shape = this;
            }
            foreach (var muscle in toClone.muscles)
            {
                int[] indexes = new int[2];
                for (int i = 0; i < 2; i++)
                {
                    indexes[i] = toClone.nodes.FindIndex(x => x.Equals(muscle.nodes[i]));
                }
                muscles.Add(new Muscle(nodes[indexes[0]], nodes[indexes[1]],muscle));
            }
            startcenter = toClone.startcenter;
            center = toClone.center;
        }

        public bool Connected(Node a,Node b)
        {
            foreach (var muscle in muscles)
            {
                if (muscle.HasNode(a) && muscle.HasNode(b)) 
                {
                    return true;
                }
            }
            return false;
        }

        private void Normalize()
        {
            double min = double.MaxValue;
            foreach (var node in nodes)
            {
                double y = node.GetPoint().Y;
                if (min > y)
                {
                    min = y;
                }
            }
            foreach (var node in nodes)
            {
                node.SetPoint(new System.Windows.Point(node.GetPoint().X, node.GetPoint().Y - min));
            }
        }

        public double Size()
        {
            double left = double.MaxValue;
            double right = double.MinValue;
            double top = double.MinValue;
            double bot = double.MaxValue;
            foreach (var node in nodes)
            {
                double x = node.GetPoint().X;
                double y = node.GetPoint().Y;
                if (left > x)
                    left = x;
                if (right < x)
                    right = x;
                if (top > y)
                    top = y;
                if (bot < y)
                    bot = y;
            }
            return Math.Max(right-left,top-0);
        }
        public void CalculateCenter()
        {
            double leftmost = double.MaxValue;
            double rightmost = double.MinValue;
            foreach (var node in nodes)
            {
                double x = node.GetPoint().X;
                if (leftmost > x) 
                    leftmost = x;
                if (rightmost < x) 
                    rightmost = x;
            }
            center = (leftmost+rightmost)/2-startcenter+Engine.nodeSize/2;
        }
        public double LocationRelativeToTheCenter(Node a)
        {
            if (nodes.Find(x => x.Equals(a)) == null)
            {
                throw new Exception();
            }
            else
            {
                return a.GetPoint().X - (startcenter + center);
            }
        }
    }
}
