using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GKS2
{
    public class GraphButtonInfo
    {
        // Fields
        public Point centerPoint;
        public List<int> childrens;
        public List<Point> childrensPoint;
        public readonly int height;
        public int id;
        public bool isModule;
        internal List<bool> isTargetPointBusy;
        public int left;
        public string name;
        public List<string> operations;
        internal List<Point> targetPoints;
        public int top;
        public readonly int width;

        // Methods
        public GraphButtonInfo(int _id, string _name, int _left, int _top)
        {
            this.operations = new List<string>();
            this.isModule = false;
            this.width = 0x30;
            this.height = 0x26;
            this.childrens = new List<int>();
            this.childrensPoint = new List<Point>();
            this.targetPoints = new List<Point>();
            this.isTargetPointBusy = new List<bool>();
            this.id = _id;
            this.name = _name;
            this.operations.Add(this.name);
            this.top = _top;
            this.left = _left;
            this.InitCoordinates();
        }

        public GraphButtonInfo(int _id, string _name, List<string> _oper, int _left, int _top)
        {
            this.operations = new List<string>();
            this.isModule = false;
            this.width = 0x30;
            this.height = 0x26;
            this.childrens = new List<int>();
            this.childrensPoint = new List<Point>();
            this.targetPoints = new List<Point>();
            this.isTargetPointBusy = new List<bool>();
            this.id = _id;
            this.name = _name;
            this.operations.AddRange(_oper);
            this.isModule = true;
            this.top = _top;
            this.left = _left;
            this.InitCoordinates();
        }

        public Point CreateConnectionPoint(int _parent, Point _parentCenter)
        {
            Point point = new Point();
            List<Point> list = new List<Point>();
            for (int i = 0; i < 8; i++)
            {
                if (!this.isTargetPointBusy[i])
                {
                    list.Add(this.targetPoints[i]);
                }
            }
            return this.MinDistance(_parentCenter, list);
        }

        private double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((double)(((p2.X - p1.X) * (p2.X - p1.X)) + ((p2.Y - p1.Y) * (p2.Y - p1.Y))));
        }

        internal void InitCoordinates()
        {
            this.centerPoint = new Point(this.left + (this.width / 2), this.top + (this.height / 2));
            for (int i = 0; i < 8; i++)
            {
                Point item = new Point();
                this.targetPoints.Add(item);
                this.isTargetPointBusy.Add(false);
            }
            this.targetPoints[0] = new Point(this.left, this.top);
            this.targetPoints[1] = new Point(this.left + (this.width / 2), this.top);
            this.targetPoints[2] = new Point(this.left + this.width, this.top);
            this.targetPoints[3] = new Point(this.left + this.width, this.top + (this.height / 2));
            this.targetPoints[4] = new Point(this.left + this.width, this.top + this.height);
            this.targetPoints[5] = new Point(this.left + (this.width / 2), this.top + this.height);
            this.targetPoints[6] = new Point(this.left, this.top + this.height);
            this.targetPoints[7] = new Point(this.left, this.top + (this.height / 2));
        }

        private Point MinDistance(Point _start, List<Point> _finish)
        {
            Point item = new Point();
            double num = 1000.0;
            for (int i = 0; i < _finish.Count; i++)
            {
                if (this.Distance(_start, _finish[i]) < num)
                {
                    item = _finish[i];
                    num = this.Distance(_start, _finish[i]);
                }
            }
            this.isTargetPointBusy[this.targetPoints.IndexOf(item)] = true;
            return item;
        }
    }


}
