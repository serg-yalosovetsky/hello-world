using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GKS2
{
   


    public class GraphButton : Button
    {
        // Fields
        public GraphButtonInfo buttonInfo;
        private bool formMoved = false;
        public int tabPageNumber;
        private int XPos = 0;
        private int YPos = 0;

        // Events
        public event dRedr RedrawNeaded;

        // Methods
        public GraphButton(GraphButtonInfo gri, int _tabpagenumber)
        {
            this.tabPageNumber = _tabpagenumber;
            base.FlatAppearance.BorderColor = Color.FromArgb(0x80, 0x80, 170);
            base.FlatAppearance.BorderSize = 2;
            base.FlatStyle = FlatStyle.Flat;
            this.Font = new Font("Comic Sans MS", 9.25f, FontStyle.Regular, GraphicsUnit.Point, 0xcc);
            this.ForeColor = Color.FromArgb(0x80, 0x80, 170);
            base.Location = new Point(gri.left, gri.top);
            base.Size = new Size(gri.width, gri.height);
            this.Text = gri.name;
            base.Name = gri.name;
            base.UseVisualStyleBackColor = true;
            base.MouseDown += new MouseEventHandler(this.GraphButton_MouseDown);
            base.MouseMove += new MouseEventHandler(this.GraphButton_MouseMove);
            base.MouseUp += new MouseEventHandler(this.GraphButton_MouseUp);
            this.buttonInfo = gri;
        }

        private void GraphButton_MouseDown(object sender, MouseEventArgs e)
        {
            this.XPos = e.X;
            this.YPos = e.Y;
            this.formMoved = true;
            base.BringToFront();
        }

        private void GraphButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.formMoved)
            {
                base.Top += e.Y - this.YPos;
                base.Left += e.X - this.XPos;
            }
        }

        private void GraphButton_MouseUp(object sender, MouseEventArgs e)
        {
            int width = base.Parent.Width;
            int height = base.Parent.Height;
            int num3 = 0;
            int num4 = 0;
            if (this.formMoved)
            {
                this.formMoved = false;
                if (base.Left > width)
                {
                    base.Left = width;
                }
                if (base.Left < num4)
                {
                    base.Left = num4;
                }
                if (base.Top > height)
                {
                    base.Top = height;
                }
                if (base.Top < num3)
                {
                    base.Top = num3;
                }
            }
            this.buttonInfo.left = base.Left;
            this.buttonInfo.top = base.Top;
            for (int i = 0; i < 8; i++)
            {
                this.buttonInfo.isTargetPointBusy[i] = false;
            }
            this.buttonInfo.InitCoordinates();
            this.RedrawNeaded(this.tabPageNumber, this);
        }
    }


}
