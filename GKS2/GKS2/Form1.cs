using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace GKS2
{
    public delegate void dRedr(int pagenumber, GraphButton button);
    public delegate void dRes(bool res);
    public delegate void dStatus(string str);

    public partial class Form1 : Form
    {
        private FormMore formMore;
        private List<Group> groups;
        private List<Module> modules;
        private List<List<string>> detailsOperationsMatrix;
        private List<mSystem> nSystem;
        private int numberOfDetails;
        private List<string> uniqueOperations;
        private static int compareСounter;

        private int[,] detailsMatchOpsMatrix;
        private int[,] detailsQuadMatrix;

        public Form1()
        {
            this.groups = new List<Group>();
            this.modules = new List<Module>();
            this.detailsOperationsMatrix = new List<List<string>>();
            this.uniqueOperations = new List<string>();
            this.numberOfDetails = 0;
            this.nSystem = new List<mSystem>();
            this.formMore = new FormMore();
            this.components = null;

            InitializeComponent();
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            this.runToolStripMenuItem.Enabled = true;
        }

        private void OPENGKSToolItem_Click(object sender, EventArgs e)
        {
            this.NewData();
            RichTextBox box = new RichTextBox();

            box.Text = Resources.Default;
            this.toolStrip1.Items[0].Text = "Пример входных данных";

            string[] strArray = new string[10];
            int num = 0;
            this.dataGridView1.RowCount = box.Lines.Length;
            foreach (string str in box.Lines)
            {
                strArray = str.Split(new char[] { ',', '.', ';', '|', ' ' });
                for (int i = 0; i < strArray.Length; i++)
                {
                    this.dataGridView1[i, num].Value = strArray[i];
                }
                this.dataGridView1.Rows[num].HeaderCell.Value = (num + 1).ToString();
                num++;
            }
            this.runToolStripMenuItem.Enabled = true;
        }

        private void RunAnalisys_Click(object sender, EventArgs e)
        {
            this.richTextBoxInfo.Text = "";
            new Thread(new ThreadStart(this.RunAnalisys)).Start();
        }

        private void ToolItemMore_Click(object sender, EventArgs e)
        {
            RichTextBox box;
            Label label;
            this.formMore = new FormMore();
            this.formMore.Show();
            DataGridView view = new DataGridView
            {
                ColumnCount = this.uniqueOperations.Count,
                RowCount = this.numberOfDetails
            };
            for (int c = 0; c < view.ColumnCount; c++)
            {
                view.Columns[c].Width = 0x23;
                view.Columns[c].ReadOnly = true;
                view.Columns[c].HeaderText = this.uniqueOperations[c];
                for (int r = 0; r < view.RowCount; r++ )
                {
                    view.Rows[r].HeaderCell.Value = (r + 1).ToString();
                    view[c, r].Value = this.detailsMatchOpsMatrix[r, c];
                }
            }
            view.RowHeadersWidth = 50;
            view.Width = (view.ColumnCount * 0x23) + 0x37;
            view.Height = (view.RowCount * 0x16) + 40;
            view.BackgroundColor = this.BackColor;
            DataGridView view2 = new DataGridView
            {
                ColumnCount = this.numberOfDetails,
                RowCount = this.numberOfDetails
            };

            for (int c = 0; c < view2.ColumnCount; c++ )
            {
                view2.Columns[c].Width = 0x23;
                view2.Columns[c].ReadOnly = true;
                view2.Columns[c].HeaderText = (c + 1).ToString();

                for (int r = 0; r < view2.RowCount; r++ )
                {
                    view2.Rows[r].HeaderCell.Value = (r + 1).ToString();
                    view2[c, r].Value = this.detailsQuadMatrix[c, r];
                }
            }
            view2.RowHeadersWidth = 50;
            view2.Width = (view2.ColumnCount * 0x23) + 0x37;
            view2.Height = (view2.RowCount * 0x16) + 40;
            view2.BackgroundColor = this.BackColor;
            this.formMore.tabControl1.TabPages[0].Controls.Add(view);
            this.formMore.tabControl1.TabPages[1].Controls.Add(view2);
            for (int g = 0; g < this.groups.Count; g++)
            {
                this.formMore.tabControl2.TabPages.Add("Группа " + this.groups[g].Name + ":  " + this.groups[g].ToString());
                this.formMore.tabControl2.TabPages[g].BackColor = this.BackColor;
                Panel panel = new Panel();
                Panel panel2 = new Panel();
                panel.Height = this.formMore.tabControl2.Height;
                panel.Width = this.formMore.tabControl2.Width;
                panel.Name = "pgr";
                panel2.Height = this.formMore.tabControl2.Height;
                panel2.Width = this.formMore.tabControl2.Width;
                panel2.Name = "pmod";
                this.formMore.tabControl2.TabPages[g].Controls.Add(panel);
                this.formMore.tabControl2.TabPages[g].Controls.Add(panel2);

                for (int num = 0; num < this.groups[g].graphButtonsInfo.Count; num++ )
                {
                    GraphButton button = new GraphButton(this.groups[g].graphButtonsInfo[num], g);
                    button.RedrawNeaded += new dRedr(this.grbut_RedrawNeeded);
                    panel.Controls.Add(button);
                }

                this.formMore.Redraw(g, false);

                for (int num = 0; num < this.groups[g].modulesButtonsInfo.Count; num++ )
                {
                    GraphButton button2 = new GraphButton(this.groups[g].modulesButtonsInfo[num], g);
                    button2.RedrawNeaded += new dRedr(this.grbut_RedrawNeeded);
                    panel2.Controls.Add(button2);
                }

                this.formMore.Redraw(g, true);
                box = new RichTextBox
                {
                    Top = 300,
                    Left = 0x228
                };

                for (int m = 0; m < this.groups[g].modulesList.Count; m++ )
                {
                    box.Text = box.Text + "M" + this.groups[g].Name + this.groups[g].modulesList[m].Number.ToString() + ": " + this.groups[g].modulesList[m].ToString() + "\n";
                }
                box.Width = 150;
                box.Height = 150;
                panel2.Controls.Add(box);
                label = new Label();
                panel2.Controls.Add(label);
                label.Text = "Операции для группы: " + this.groups[g].operations.ToString();
                label.Top = this.formMore.tabControl2.Height - 40;
                label.AutoSize = true;
            }

            for (int g = 0; g < this.groups.Count; g++)
            {
                this.formMore.tabControl3.TabPages.Add("Группа " + this.groups[g].Name + ":  " + this.groups[g].ToString());
                this.formMore.tabControl3.TabPages[g].BackColor = this.BackColor; // = Color.FromArgb(0xff, 0xff, 0xc0);
                view2 = new DataGridView
                {
                    ColumnCount = this.groups[g].graphUniqueOperations.Count,
                    RowCount = this.groups[g].graphUniqueOperations.Count
                };
                for (int c = 0; c < view2.ColumnCount; c++ )
                {
                    view2.Columns[c].Width = 0x23;
                    view2.Columns[c].ReadOnly = true;
                    view2.Columns[c].HeaderText = this.groups[g].graphUniqueOperations[c];
                    for (int r = 0; r < view2.RowCount; r++ )
                    {
                        view2.Rows[r].HeaderCell.Value = this.groups[g].graphUniqueOperations[r];
                        view2[c, r].Value = this.groups[g].graphMatrix[r, c];
                    }
                }
                view2.RowHeadersWidth = 60;
                view2.Width = (view2.ColumnCount * 0x23) + 0x41;
                view2.Height = (view2.RowCount * 0x16) + 40;
                view2.BackgroundColor = this.BackColor; // = Color.FromArgb(0xff, 0xff, 0xc0);
                this.formMore.tabControl3.TabPages[g].Controls.Add(view2);
                label = new Label();
                this.formMore.tabControl3.TabPages[g].Controls.Add(label);
                label.Top = this.formMore.tabControl2.Height - 40;
                label.AutoSize = true;
                label.Text = "Операции для группы: " + this.groups[g].operations.ToString();
            }
            for (int g = 0; g < this.groups.Count; g++)
            {
                this.formMore.tabControl4.TabPages.Add("Группа " + this.groups[g].Name + ":  " + this.groups[g].ToString());
                this.formMore.tabControl4.TabPages[g].BackColor = this.BackColor; // = Color.FromArgb(0xff, 0xff, 0xc0);
                view2 = new DataGridView
                {
                    ColumnCount = this.groups[g].modulesList.Count,
                    RowCount = this.groups[g].modulesList.Count
                };
                for (int c=0; c < view2.ColumnCount; c++ )
                {
                    view2.Columns[c].Width = 0x23;
                    view2.Columns[c].ReadOnly = true;
                    view2.Columns[c].HeaderText = "M" + this.groups[g].Name + this.groups[g].modulesList[c].Number.ToString();
                    for (int r = 0; r < view2.RowCount; r++ )
                    {
                        view2.Rows[r].HeaderCell.Value = "M" + this.groups[g].Name + this.groups[g].modulesList[r].Number.ToString();
                        view2[c, r].Value = this.groups[g].moduleMatrix[r, c];
                    }
                }
                view2.RowHeadersWidth = 60;
                view2.Width = ((view2.ColumnCount * 0x23) + 0x41) + 40;
                view2.Height = ((view2.RowCount * 0x16) + 40) + 50;
                view2.BackgroundColor = this.BackColor;// = Color.FromArgb(0xff, 0xff, 0xc0);
                this.formMore.tabControl4.TabPages[g].Controls.Add(view2);
                box = new RichTextBox
                {
                    Top = 280
                };
                for (int i = 0; i < this.groups[g].modulesList.Count; i++ )
                {
                    box.Text = box.Text + "M" + this.groups[g].Name + this.groups[g].modulesList[i].Number.ToString() + ": " + this.groups[g].modulesList[i].ToString() + "\n";
                }
                box.Width = 200;
                box.Height = 150;
                this.formMore.tabControl4.TabPages[g].Controls.Add(box);
                label = new Label();
                this.formMore.tabControl4.TabPages[g].Controls.Add(label);
                label.Top = this.formMore.tabControl2.Height - 40;
                label.AutoSize = true;
                label.Text = "Операции для группы: " + this.groups[g].operations.ToString();
            }

            int result = 1;
            int.TryParse(this.textBox1.Text.ToString(), out result);
            result--;

            for (int i = 0; i < this.nSystem.Count; i++)
            {
                this.formMore.tabControl5.TabPages.Add("Система " + (i + 1).ToString());
                this.formMore.tabControl5.TabPages[i].BackColor = this.BackColor;// = Color.FromArgb(0xff, 0xff, 0xc0);
                view2 = new DataGridView
                {
                    Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top,
                    ColumnCount = this.nSystem[i].OptimalStruct[result].modModules.Count + 2,
                    RowCount = this.nSystem[i].OptimalStruct[result].modModules.Count + 2
                };
                for (int c = 0; c < view2.ColumnCount; c++ )
                {
                    view2.Columns[c].Width = 100;
                    view2.Rows[c].Height = 0x23;
                    view2.Columns[c].ReadOnly = true;
                    if (c == 0)
                    {
                        view2.Columns[c].HeaderText = "Вход".ToString();
                    }
                    else if (c == (this.nSystem[i].OptimalStruct[result].modModules.Count + 1))
                    {
                        view2.Columns[c].HeaderText = "Выход".ToString();
                    }
                    else
                    {
                        view2.Columns[c].HeaderText = this.nSystem[i].OptimalStruct[result].modModules[c - 1].ToString();
                    }
                    for (int r = 0; r < view2.RowCount; r++ )
                    {
                        if (r == 0)
                        {
                            view2.Rows[r].HeaderCell.Value = "Вход".ToString();
                        }
                        else if (r == (this.nSystem[i].OptimalStruct[result].modModules.Count + 1))
                        {
                            view2.Rows[r].HeaderCell.Value = "Выход".ToString();
                        }
                        else
                        {
                            view2.Rows[r].HeaderCell.Value = this.nSystem[i].OptimalStruct[result].modModules[r - 1].ToString();
                        }
                        string str = "";
                        foreach (int num6 in this.nSystem[i].OptimalStruct[result].modMatrix[r, c])
                        {
                            str = str + num6;
                            str = str + ", ";
                        }
                        view2[c, r].Value = str;
                    }
                }

                view2.RowHeadersWidth = 80;
                view2.Width = (view2.ColumnCount * 100) + 0x55;
                view2.Height = (view2.RowCount * 0x23) + 40;
                view2.BackgroundColor = this.BackColor;
                DataGridViewCellStyle style = new DataGridViewCellStyle(view2[0, 0].Style)
                {
                    BackColor = Color.Gray
                };
                for (int c = 0; c < view2.ColumnCount; c++)
                {
                    for (int cc = 0; cc < view2.ColumnCount; cc++)
                    {
                        if ((((c == cc) || (c == 0)) || ((cc == 0) || (c == (view2.ColumnCount - 1)))) || (cc == (view2.ColumnCount - 1)))
                        {
                            view2[c, cc].Style = style;
                            if (c == cc)
                            {
                                view2[c, c].Value = "";
                            }
                        }
                    }
                }
                view2[view2.ColumnCount - 1, 0].Value = "";
                this.formMore.tabControl5.TabPages[i].Controls.Add(view2);
            }

        }




        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Name == "ToolItemNew")
            {
                this.NewData();
            }
            if (((ToolStripMenuItem)sender).Name == "ToolItemLoad")
            {
                this.LoadData();
            }
            if (((ToolStripMenuItem)sender).Name == "ToolItemSave")
            {
                this.SaveData();
            }
        }

        #region Какие-то полезные функции

        public void AddInfo(string _status)
        {
            if (this.richTextBoxInfo.InvokeRequired)
            {
                dStatus method = new dStatus(this.AddInfo);
                base.Invoke(method, new object[] { _status });
            }
            else
            {
                this.richTextBoxInfo.Text = this.richTextBoxInfo.Text + _status;
                int firstCharIndexFromLine = this.richTextBoxInfo.GetFirstCharIndexFromLine(this.richTextBoxInfo.Lines.Length - 1);
                this.richTextBoxInfo.Select(firstCharIndexFromLine, 0);
                this.richTextBoxInfo.ScrollToCaret();
            }
        }

        private void AnalysisCoefProximity()
        {

            this.uniqueOperations.Clear();
            
            this.numberOfDetails = this.dataGridView1.RowCount;

            this.detailsOperationsMatrix.Clear();
            this.detailsOperationsMatrix.Add(new List<string>());

            // Get data from dataGridView1 and fill detailsOperationsMatrix

            for (int r = 0; r < this.dataGridView1.RowCount; r++)
            {
                this.dataGridView1.Rows[r].HeaderCell.Value = (r + 1).ToString();
                for (int c = 0; c < this.dataGridView1.ColumnCount; c++ )
                {
                    if ((this.dataGridView1[c, r].Value != null) && (this.dataGridView1[c, r].Value.ToString().Trim() != ""))
                    {
                        
                        string currentOperation = this.dataGridView1[c, r].Value.ToString();

                        this.detailsOperationsMatrix[this.detailsOperationsMatrix.Count - 1].Add(currentOperation);

                        if (this.uniqueOperations.IndexOf(this.dataGridView1[c, r].Value.ToString()) == -1)
                        {
                            this.uniqueOperations.Add(this.dataGridView1[c, r].Value.ToString());
                        }
                    }
                }
                this.detailsOperationsMatrix.Add(new List<string>());
            }

            // Print Num of details and unique operations count.

            this.AddInfo(string.Concat(new object[] { "Результаты: \nДеталей: ", this.numberOfDetails, ", \nУникальных операций: ", this.uniqueOperations.Count.ToString(), " : \n" }));

            // Print unique operations as string.

            foreach (string str in this.uniqueOperations)
            {
                this.AddInfo(string.Format(" {0}, ", str));
            }

            // Check if there are details and operations.

            if ((this.numberOfDetails != 0) && (this.uniqueOperations.Count != 0))
            {

                // Fill arr.

                this.detailsMatchOpsMatrix = new int[this.numberOfDetails, this.uniqueOperations.Count];

                for (int detNum = 0; detNum < this.numberOfDetails; detNum++)
                {
                    for (int opNum = 0; opNum < this.uniqueOperations.Count; opNum++)
                    {
                        this.detailsMatchOpsMatrix[detNum, opNum] = 0;

                        for (int colInViewTable = 0; colInViewTable < this.dataGridView1.ColumnCount; colInViewTable++)
                        {

                            object operationFromTable = this.dataGridView1[colInViewTable, detNum].Value;
                            string uniqueOperation = this.uniqueOperations[opNum];

                            if ((operationFromTable != null) && (operationFromTable.ToString() == uniqueOperation))
                            {
                                this.detailsMatchOpsMatrix[detNum, opNum] = 1;
                                break;
                            }
                        }

                    }
                }

                // Print array.
                
                for (int r = 0; r < this.numberOfDetails; r++)
                {
                    this.AddInfo(string.Format("\nДеталь: {0}: \n ", r+1));
                    for (int c = 0; c < this.uniqueOperations.Count; c++)
                    {
                        this.AddInfo(string.Format(" {0},   ", this.detailsMatchOpsMatrix[r, c]));
                    }
                }

                this.detailsQuadMatrix = new int[this.numberOfDetails, this.numberOfDetails];

                for (int detailLeft = 0; detailLeft < this.numberOfDetails; detailLeft++)
                {
                    for (int detailTop = detailLeft + 1; detailTop < this.numberOfDetails; detailTop++)
                    {
                        int detailsOperationsDiff = 0;

                        for (int opNum = 0; opNum < this.uniqueOperations.Count; opNum++)
                        {
                            int currentOperationDiff = this.detailsMatchOpsMatrix[detailLeft, opNum] + this.detailsMatchOpsMatrix[detailTop, opNum];
                            if (currentOperationDiff == 2) currentOperationDiff = 0;
                            detailsOperationsDiff += currentOperationDiff;
                        }

                        this.detailsQuadMatrix[detailLeft, detailTop] = this.uniqueOperations.Count - detailsOperationsDiff;
                        this.detailsQuadMatrix[detailTop, detailLeft] = this.uniqueOperations.Count - detailsOperationsDiff;
                        this.detailsQuadMatrix[detailLeft, detailLeft] = 0;
                        this.detailsQuadMatrix[detailLeft + 1, detailLeft + 1] = 0;
                    }
                }

                // Print array.

                this.AddInfo("\nКвадрат: \n");
                for (int r = 0; r < this.numberOfDetails; r++)
                {
                    //this.AddInfo(string.Format("\nДеталь: {0}: \n ", r + 1));
                    for (int c = 0; c < this.numberOfDetails; c++)
                    {
                        this.AddInfo(string.Format(" {0},   ", this.detailsQuadMatrix[r, c]));
                    }
                    this.AddInfo("\n");
                }

            }
        }

        private void AnalysisGroupsCreate()
        {

            this.groups.Clear();
            int rows = this.numberOfDetails;

            do
            {

                this.groups.Add(new Group());

                int val = -1;
                int currId = 0;
                int nextId = 0;

                for (int detId = 0; detId < this.numberOfDetails; detId++)
                {
                    for (int nextDetId = detId + 1; nextDetId < this.numberOfDetails; nextDetId++)
                    {
                        if (!(this.checkIfDetailInGroup(nextDetId) || this.checkIfDetailInGroup(detId)) && 
                             (this.detailsQuadMatrix[detId, nextDetId] > val))
                        {
                            val = this.detailsQuadMatrix[detId, nextDetId];
                            currId = detId;
                            nextId = nextDetId;
                        }

                    }
                }

                if (val != -1)
                {
                    this.groups[this.groups.Count - 1].AddDetail(currId, this.grOperString(currId));
                    this.groups[this.groups.Count - 1].AddDetail(nextId, this.grOperString(nextId));
                    rows -= 2;
                }

                else
                {
                    for (int r = 0; r < this.numberOfDetails; r++)
                    {
                        if (!this.checkIfDetailInGroup(r))
                        {
                            this.groups[this.groups.Count - 1].AddDetail(r, this.grOperString(r));
                            rows--;
                        }
                    }
                }

                for (int dId = 0; dId < this.groups[this.groups.Count - 1].details.Count; dId++)
                {
                    int detailId = this.groups[this.groups.Count - 1].details[dId].id;

                    for (int ddId = 0; ddId < this.numberOfDetails; ddId++)
                    {
                        if (this.detailsQuadMatrix[ddId, detailId] == val && !this.checkIfDetailInGroup(ddId))
                            {
                                this.groups[this.groups.Count - 1].AddDetail(ddId, this.grOperString(ddId));
                                rows--;
                            }
                    }
                }
            }

            while (rows > 0);

            // After while loop
            this.groups.Sort();

            // Print groups
            this.AddInfo("\n\nГруппы: \n");
            for (int g = 0; g < this.groups.Count; g++)
            {
                this.AddInfo((g + 1).ToString() + ": ");
                this.AddInfo(this.groups[g].ToString());
            }

            this.AddInfo("\nОперации по группам: \n");
            for (int g = 0; g < this.groups.Count; g++)
            {
                this.AddInfo((g + 1).ToString() + ": ");
                this.AddInfo(this.groups[g].operations.ToString());
            }
        }

        private void AnalysysGraph()
        {
            for (int gg = 0; gg < this.groups.Count; gg++)
            {
                this.groups[gg].GraphCreate();
                this.groups[gg].CreateModules();
            }

            this.modules.Clear();
            for (int g = 0; g < this.groups.Count; g++)
            {
                for (int m=0; m < this.groups[g].modulesList.Count; m++)
                {
                    Module item = new Module(this.groups[g].modulesList[m].Number);
                    item.AddOperations(this.groups[g].modulesList[m].operations);
                    this.modules.Add(item);
                }
            }

            bool progress = true;
            bool recheckModules = false;

            while(progress)
            {

                progress = false;
                recheckModules = false;
            
                //this.modules.Sort();

                for (int modCurr = 0; modCurr < this.modules.Count; modCurr++)
                {

                    for (int modNext = modCurr + 1; modNext < this.modules.Count; modNext++)
                    {
                        List<string> sameOperationsList = new List<string>();

                        for (int i = 0; i < this.modules[modNext].operations.Count; i++)
                        {
                            // Check if CurrentModule has Operations From NextModule
                            if (this.modules[modCurr].operations.Contains(this.modules[modNext].operations[i])) 
                                sameOperationsList.Add(this.modules[modNext].operations[i]);
                        }
                    
                        // if All operations from next module are in current module, we don't need next module, so remove it.
                        if (sameOperationsList.Count == this.modules[modNext].operations.Count)
                        {
                            this.modules.RemoveAt(modNext);
                            recheckModules = true;
                            break;
                        }

                        // if some operations are in both modules, remove duplicates from current module.
                        for (int op = 0; op < sameOperationsList.Count; op++) this.modules[modCurr].operations.Remove(sameOperationsList[op]);
                    }

                    if (recheckModules)
                    {
                        progress = true;
                        break;
                    }
                }

            }

            this.modules.Sort();

            this.AddInfo("\nМодули: \n");

            for (int num = 0; num < this.modules.Count; num++)
            {
                this.AddInfo("М" + (num + 1) + ": ");
                this.AddInfo(this.modules[num].ToString() + "\n");
            }
        }

        private void AnalysysNewGroups()
        {
            compareСounter = 0;
            this.CompareWithGroups();
            this.CompareWithDetails();

            this.AddInfo("\nУточненные группы: \n");

            for (int i = 0; i < this.groups.Count; i++)
            {
                this.AddInfo(((i + 1)).ToString() + ": ");
                this.AddInfo(this.groups[i].ToString());
            }
        }

        private void AnalysysStruct()
        {
            this.nSystem.Clear();

            mStruct struct2 = new mStruct(this.modules, this.detailsOperationsMatrix);
            
            List<List<int>> matrixOfModulesIds = new List<List<int>>();

            for (int m = 0; m < this.modules.Count; m++)
            {
                bool flag = false;

                for (int i = 0; i < matrixOfModulesIds.Count; i++ )
                {
                    if (matrixOfModulesIds[i].Contains(m))
                    {
                        flag = true;
                    }
                }

                if (!flag)
                {
                    List<int> listOfModulesIds = new List<int>();
                    listOfModulesIds.Add(m);
                    matrixOfModulesIds.Add(listOfModulesIds);
                }

                for (int i = 0; i < matrixOfModulesIds.Count; i++)
                {
                    for (int j = 0; j < matrixOfModulesIds[i].Count; j++)
                    {
                        for (int k = 0; k < this.modules.Count; k++ )
                        {
                            if (( (struct2.modMatrix[matrixOfModulesIds[i][j] + 1, k + 1].Count != 0) || 
                                  (struct2.modMatrix[k + 1, matrixOfModulesIds[i][j] + 1].Count != 0)) && !matrixOfModulesIds[i].Contains(k))
                            {
                                matrixOfModulesIds[i].Add(k);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < matrixOfModulesIds.Count; i++ )
            {
                List<Module> modulesList = new List<Module>();
                for (int j = 0; j < matrixOfModulesIds[i].Count; j++)
                {
                    modulesList.Add(this.modules[matrixOfModulesIds[i][j]]);
                }

                this.nSystem.Add(new mSystem(modulesList, this.detailsOperationsMatrix, this.StructSimple.Checked));
                this.nSystem[this.nSystem.Count - 1].Loading += new dStatus(this.Loading1);
                this.nSystem[this.nSystem.Count - 1].Optimize();
            }

            for (int i = 0; i < this.nSystem.Count; i++)
            {
                this.AddInfo("\n\n*******\nСистема: " + (i + 1).ToString());
                this.AddInfo("\nОптимизированная структура модулей: \n");
                for (int k = 0; k < this.nSystem[i].OptimalModules.Count; k++)
                {
                    this.AddInfo("\n\nОптимизированная структура модулей #" + ((k + 1)).ToString() + ": \n");
                    for (int z = 0; z < this.nSystem[i].OptimalModules[k].Count; z++)
                    {
                        this.AddInfo((z + 1) + ": ");
                        this.AddInfo(this.nSystem[i].OptimalModules[k][z].ToString() + "\n");
                    }
                }
                this.AddInfo("\nИнформация: Исходные модули:");
                this.AddInfo("\nПрямых связей:   " + this.nSystem[i].DefaultStruct.DirectConnectionsNumber.ToString());
                this.AddInfo("\nОбратных связей: " + this.nSystem[i].DefaultStruct.InversConnectionsNumber.ToString());
                this.AddInfo("\n\nИнформация: Оптимизированная структура:");
                this.AddInfo("\nПрямых связей:   " + this.nSystem[i].OptimalStruct[0].DirectConnectionsNumber.ToString());
                this.AddInfo("\nОбратных связей: " + this.nSystem[i].OptimalStruct[0].InversConnectionsNumber.ToString());
                this.AddInfo("\n* Итераций: " + this.nSystem[i].iterations.ToString());
            }
        }

        public void CompareWithDetails()
        {
            compareСounter++;
            if (compareСounter == 0x27)
            {
                compareСounter = 0;
            }
            else
            {
                for (int i = 0; i < (this.groups.Count - 1); i++)
                {
                    for (int j = i + 1; j < this.groups.Count; j++)
                    {
                        for (int k = 0; k < this.groups[j].details.Count; k++)
                        {
                            if (this.groups[i].operations.ContainsOperations(this.groups[j].details[k].operations))
                            {
                                this.groups[i].AddDetail(this.groups[j].details[k].id, this.groups[j].details[k].operations);
                                this.groups[j].details.RemoveAt(k);
                                this.groups[j].ReOrganizeOperations();
                                this.groups.Sort();
                                this.CompareWithDetails();
                            }
                        }
                    }
                }
            }
        }

        public void CompareWithGroups()
        {
            for (int i = 0; i < (this.groups.Count - 1); i++)
            {
                for (int j = i + 1; j < this.groups.Count; j++)
                {
                    if (this.groups[i].operations.ContainsOperations(this.groups[j].operations.GroupUniqueOperations))
                    {
                        for (int k = 0; k < this.groups[j].details.Count; k++)
                        {
                            this.groups[i].AddDetail(this.groups[j].details[k].id, this.groups[j].details[k].operations);
                        }
                        this.groups.RemoveAt(j);
                        this.groups[i].ReOrganizeOperations();
                        this.groups.Sort();
                        this.CompareWithGroups();
                    }
                }
            }
        }

        private void grbut_RedrawNeeded(int _pagenumber, GraphButton _but)
        {
            this.formMore.Redraw(_pagenumber, _but, _but.buttonInfo.isModule);
        }

        private List<string> grOperString(int detId)
        {
            List<string> list = new List<string>();
            for (int operInForm = 0; operInForm < this.dataGridView1.ColumnCount; operInForm++)
            {
                object valFromTable = this.dataGridView1[operInForm, detId].Value;
                if ((valFromTable != null) && (valFromTable.ToString().Trim() != "")) list.Add(valFromTable.ToString());
            }
            return list;
        }

        private bool checkIfDetailInGroup(int id)
        {
            foreach (Group group in this.groups)
            {
                if (group.ContainsDetail(id)) return true;
            }
            return false;
        }

        private void LoadData()
        {
            this.NewData();
            this.openFileDialog1.ShowDialog();
            if (this.openFileDialog1.FileName != "")
            {
                RichTextBox box = new RichTextBox();
                box.LoadFile(this.openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                this.toolStrip1.Items[0].Text = this.openFileDialog1.FileName;
                string[] strArray = new string[10];
                int num = 0;
                this.dataGridView1.RowCount = box.Lines.Length;
                foreach (string str in box.Lines)
                {
                    strArray = str.Split(new char[] { ',', '.', ';', '|', ' ' });
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        this.dataGridView1[i, num].Value = strArray[i];
                    }
                    this.dataGridView1.Rows[num].HeaderCell.Value = (num + 1).ToString();
                    num++;
                }
                this.runToolStripMenuItem.Enabled = true;
            }
        }

        private void Loading1(string str)
        {
            this.AddInfo(str);
        }

        private void NewData()
        {
            for (int i = 0; i < this.dataGridView1.ColumnCount; i++)
            {
                for (int j = 0; j < this.dataGridView1.RowCount; j++)
                {
                    this.dataGridView1[i, j].Value = "";
                }
            }
            this.dataGridView1.RowCount = 1;
            this.toolStrip1.Items[0].Text = "";
            this.richTextBoxInfo.Text = "Результаты: недоступно.";
            this.runToolStripMenuItem.Enabled = false;
            this.ToolItemMore.Enabled = false;
        }

        private void RunAnalisys()
        {
            try
            {
                if (this.dataGridView1[0, 0].Value == null)
                {
                    MessageBox.Show("Данные не загружены!");
                }
                else
                {
                    this.AnalysisCoefProximity();
                    this.AnalysisGroupsCreate();
                    this.AnalysysNewGroups();
                    for (int i = 0; i < this.groups.Count; i++)
                    {
                        this.groups[i].Name = (i + 1).ToString();
                    }
                    this.AnalysysGraph();
                    //

                    this.AddInfo("\n***\nСинтезирую оптимальную структуру\n");
                    this.AnalysysStruct();

                    //
                    this.ToolStripEnable(true);

                    this.AddInfo("\n***\nЗавершено успешно");
                }
            }
            catch (Exception exception)
            {
                this.AddInfo("\n***\nОшибка. Результаты: недоступно\n" + exception.Message.ToString());
                this.ToolStripEnable(false);
            }
        }

        private void SaveData()
        {
            this.saveFileDialog1.ShowDialog();
            if (this.saveFileDialog1.FileName != "")
            {
                this.toolStrip1.Items[0].Text = this.saveFileDialog1.FileName;
                StreamWriter writer = new StreamWriter(this.saveFileDialog1.FileName, false, Encoding.GetEncoding("windows-1251"), 0x4e20);
                for (int i = 0; i < this.dataGridView1.RowCount; i++)
                {
                    string str = "";
                    if (this.dataGridView1[0, i].Value != null)
                    {
                        str = this.dataGridView1[0, i].Value.ToString();
                    }
                    for (int j = 1; j < this.dataGridView1.ColumnCount; j++)
                    {
                        if ((this.dataGridView1[j, i].Value != null) && (this.dataGridView1[j, i].Value.ToString() != ""))
                        {
                            str = str + "," + this.dataGridView1[j, i].Value.ToString();
                        }
                    }
                    writer.WriteLine(str);
                }
                writer.Close();
            }
        }

        public void ToolStripEnable(bool _res)
        {
            if (this.richTextBoxInfo.InvokeRequired)
            {
                dRes method = new dRes(this.ToolStripEnable);
                base.Invoke(method, new object[] { _res });
            }
            else
            {
                this.ToolItemMore.Enabled = _res;
            }
        }

        #endregion

        private void splitContainer1Vertical_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
