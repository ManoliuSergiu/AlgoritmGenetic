using System;
using System.Windows;

namespace AlgGenTestWPF
{
    class Node
    {
        private Point location;
        public double frictionPercentage;
        private Vector speed;
        public int gCount;
        public Shape shape;
        public Node()
        {

            speed = new Vector(0, 0);
            location = new Point(((Engine.rnd.NextDouble()*10-5) * Engine.nodeSize), (Engine.rnd.NextDouble() * 5) * Engine.nodeSize);
            frictionPercentage = Engine.rnd.NextDouble();
        }
        public Node(double x, double y, double ff)
        {
            speed = new Vector(0, 0);
            location = new Point(x* Engine.nodeSize, y * Engine.nodeSize);
            frictionPercentage = 0.05+ 0.95 * ff;
        }
        public Node(Node toClone)
        {
            speed = new Vector(0, 0);
            location = toClone.GetPoint();
            frictionPercentage = toClone.frictionPercentage;
        }
        public Point GetNormalPoint()
        {
            return new Point(Engine.widthA/2+ shape.LocationRelativeToTheCenter(this)*Engine.scale, Engine.heightA - (location.Y + Engine.nodeSize) * Engine.scale);
        }
        public Point GetPoint()
        {
            return new Point(location.X, location.Y);
        }
        public void SetPoint(Point location)
        {
            location.X = location.X;
            location.Y = location.Y;
        }
        public void MovePoint()
        {
            Vector aux = speed + new Vector(0, -gCount * Engine.g)/Engine.tickrate;
            speed = aux * (1 - 1.0 / Engine.tickrate);
            if (Math.Floor(location.Y)<=0)
            {
                speed -= new Vector(speed.X * frictionPercentage, speed.Y);
                location.Y = 0;
            }
            location += speed/Engine.tickrate;

            
        }
        public void SetSpeed(Vector s)
        {
            speed = s;
        }
        public Vector GetSpeed()
        {
            return speed;
        }

    }
}
