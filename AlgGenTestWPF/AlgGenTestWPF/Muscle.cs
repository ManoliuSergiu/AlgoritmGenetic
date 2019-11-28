using System;
using System.Windows;

namespace AlgGenTestWPF
{
    class Muscle
    {
        public Node[] nodes = new Node[2];
        public double extendedLength;
        public double contractionSizePrecentage;
        public int timeToContract;
        public int timeToExtend;
        public int timeIdle;
        public int cylceDuration;
        public int currentTick = 0;
        public int state = -1;
        public Vector force = new Vector(0,0);

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">First node of the muscle</param>
        /// <param name="b">Second node of the muscle</param>
        /// <param name="strength">Ratio of the size of the muscle when it's contracted</param>
        /// <param name="contractionTime">Time in seconds to contract</param>
        /// <param name="extentionTime">Time in seconds to extend</param>
        /// <param name="idleTime">Time in seconds for idle</param>
        public Muscle(Node a, Node b, double strength, double contractionTime, double extentionTime, double idleTime)
        {
            nodes[0] = a;
            nodes[1] = b;
            extendedLength = Engine.GetLength(a, b);
            contractionSizePrecentage = 0.5 * strength;
            timeToContract = Math.Max((int)(Engine.tickrate * contractionTime), 1); ;
            timeToExtend = Math.Max((int)(Engine.tickrate * extentionTime), 1);
            timeIdle = (int)(Engine.tickrate * idleTime);
            cylceDuration = timeToContract + timeToExtend + timeIdle;
        }

        public Muscle(Node a, Node b)
        {
            nodes[0] = a;
            nodes[1] = b;
            extendedLength = Engine.GetLength(a, b);
            contractionSizePrecentage = 0.5 * Engine.rnd.NextDouble();
            timeToContract = Engine.rnd.Next(1, Engine.tickrate + 1);
            timeToExtend = Engine.rnd.Next(1, Engine.tickrate + 1);
            timeIdle = Engine.rnd.Next(0, Engine.tickrate + 1);
            cylceDuration = timeToContract + timeToExtend + timeIdle;

        }
        public Muscle(Node a, Node b, Muscle toClone)
        {
            nodes[0] = a;
            nodes[1] = b;
            extendedLength = toClone.extendedLength;
            contractionSizePrecentage = toClone.contractionSizePrecentage;
            timeToContract = toClone.timeToContract;
            timeToExtend = toClone.timeToExtend;
            timeIdle = toClone.timeIdle;
            cylceDuration = toClone.cylceDuration;
        }
        #endregion






        internal void Tick()
        {

            double distx = Math.Abs(Math.Round(nodes[1].GetPoint().X, 3) - Math.Round(nodes[0].GetPoint().X));
            double disty = Math.Abs(Math.Round(nodes[1].GetPoint().Y, 3) - Math.Round(nodes[0].GetPoint().Y));
            double currentDistance = Engine.GetLength(this);
            if (double.IsNaN(currentDistance))
            {
                currentDistance = 0.1;
            }
            double angle = Engine.Angle(this);
            double scalex = Math.Cos(angle);
            double scaley = Math.Sin(angle);
            double a;
            if (currentTick == 0)
            {
                a = Math.Max(currentDistance / (extendedLength * contractionSizePrecentage),1);
                a /= timeToContract;
                Contract(new Vector(a * scalex, a * scaley));
                state = -1;
            }
            else if (currentTick == timeToContract)
            {
                a = extendedLength / currentDistance;
                a /= timeToExtend;
                Extend(new Vector(a * scalex, a * scaley));
                state = 1;
            }
            else if (currentTick == timeToContract+timeToExtend)
            {
                state = 0;
            }
            if (currentDistance > extendedLength)
            {
                Vector delta = (nodes[0].GetSpeed() + nodes[1].GetSpeed()) / 2;
                nodes[0].SetSpeed(delta);
                nodes[1].SetSpeed(delta);

            }
            currentTick++;
            if (currentTick == cylceDuration)
                currentTick = 0;
        }

        public void Contract(Vector vector)
        {
            if (nodes[0].GetPoint().Y > nodes[1].GetPoint().Y)
            {
                vector.Y = -vector.Y;
            }
            if (nodes[0].GetPoint().X > nodes[1].GetPoint().X)
            {
                vector.X = -vector.X;
            }

            force = vector;
        }
        private void Extend(Vector vector)
        {

            if (nodes[0].GetPoint().Y > nodes[1].GetPoint().Y)
            {
                vector.Y = -vector.Y;
            }
            if (nodes[0].GetPoint().X < nodes[1].GetPoint().X)
            {
                vector.X = -vector.X;
            }

            force = vector;
        }


        public bool HasNode(Node a)
        {
            if (nodes[0] == a || nodes[1] == a)
            {
                return true;
            }
            return false;
        }

    }
}
