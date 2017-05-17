using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GKS2
{
    public partial class FormMore : Form
    {
        public FormMore()
        {
            this.components = null;
            InitializeComponent();
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int num;
            if (((TrackBar)sender).Value == 1)
            {
                for (num = 0; num < this.tabControl2.TabPages.Count; num++)
                {
                    this.tabControl2.TabPages[num].Controls["pmod"].Visible = false;
                    this.tabControl2.TabPages[num].Controls["pgr"].Visible = true;
                }
            }
            if (((TrackBar)sender).Value == 0)
            {
                for (num = 0; num < this.tabControl2.TabPages.Count; num++)
                {
                    this.tabControl2.TabPages[num].Controls["pmod"].Visible = true;
                    this.tabControl2.TabPages[num].Controls["pgr"].Visible = false;
                }
            }
        }
        public void Redraw(int _tabpagenumber, bool _isModule)
        {
            string str = "";
            if (_isModule)
            {
                str = "pmod";
            }
            else
            {
                str = "pgr";
            }
            Bitmap bitmap = new Bitmap(0x3e8, 0x3e8);
            this.tabControl2.TabPages[_tabpagenumber].Controls[str].BackgroundImage = bitmap;
            Graphics graphics = Graphics.FromImage(this.tabControl2.TabPages[_tabpagenumber].Controls[str].BackgroundImage);
            graphics.Clear(Color.White);
            for (int i = 0; i < this.tabControl2.TabPages[_tabpagenumber].Controls[str].Controls.Count; i++)
            {
                if (this.tabControl2.TabPages[_tabpagenumber].Controls[str].Controls[i] is GraphButton)
                {
                    Point centerPoint = ((GraphButton)this.tabControl2.TabPages[_tabpagenumber].Controls[str].Controls[i]).buttonInfo.centerPoint;
                    for (int j = 0; j < ((GraphButton)this.tabControl2.TabPages[_tabpagenumber].Controls[str].Controls[i]).buttonInfo.childrens.Count; j++)
                    {
                        Point point2 = ((GraphButton)this.tabControl2.TabPages[_tabpagenumber].Controls[str].Controls[i]).buttonInfo.childrensPoint[j];
                        graphics.DrawLine(new Pen(Color.Green, 1f), centerPoint, point2);
                        graphics.DrawPie(new Pen(Brushes.Red, 4f), point2.X - 2, point2.Y - 2, 4, 4, 0, 360);
                    }
                    this.tabControl2.TabPages[_tabpagenumber].Controls[str].Refresh();
                }
            }
        }
        public void Redraw(int _tabpagenumber, GraphButton _badchild, bool _isModule)
        {
            string str = "";
            if (_isModule)
            {
                str = "pmod";
            }
            else
            {
                str = "pgr";
            }
            Bitmap bitmap = new Bitmap(0x3e8, 0x3e8);
            this.tabControl2.TabPages[_tabpagenumber].Controls[str].BackgroundImage = bitmap;
            Graphics graphics = Graphics.FromImage(this.tabControl2.TabPages[_tabpagenumber].Controls[str].BackgroundImage);
            graphics.Clear(Color.White);
            for (int i = 0; i < this.tabControl2.TabPages[_tabpagenumber].Controls[str].Controls.Count; i++)
            {
                if (this.tabControl2.TabPages[_tabpagenumber].Controls[str].Controls[i] is GraphButton)
                {
                    GraphButton button = (GraphButton)this.tabControl2.TabPages[_tabpagenumber].Controls[str].Controls[i];
                    if (button.buttonInfo.childrens.Contains(_badchild.buttonInfo.id))
                    {
                        int index = button.buttonInfo.childrens.IndexOf(_badchild.buttonInfo.id);
                        Point point = _badchild.buttonInfo.CreateConnectionPoint(button.buttonInfo.id, button.buttonInfo.centerPoint);
                        button.buttonInfo.childrensPoint[index] = point;
                    }
                    Point centerPoint = button.buttonInfo.centerPoint;
                    for (int j = 0; j < button.buttonInfo.childrens.Count; j++)
                    {
                        Point point3 = button.buttonInfo.childrensPoint[j];
                        graphics.DrawLine(new Pen(Color.Green, 1f), centerPoint, point3);
                        graphics.DrawPie(new Pen(Brushes.Red, 4f), point3.X - 2, point3.Y - 2, 4, 4, 0, 360);
                    }
                }
            }
            this.tabControl2.TabPages[_tabpagenumber].Controls[str].Refresh();
        }
    }

}
