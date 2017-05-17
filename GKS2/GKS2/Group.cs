using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GKS2
{
    class Group : IComparable
    {
        // Fields
        public List<Detail> details = new List<Detail>();
        public List<GraphButtonInfo> graphButtonsInfo = new List<GraphButtonInfo>();
        public int[,] graphMatrix;
        public List<string> graphUniqueOperations = new List<string>();
        private string groupName;
        private string lastCheckedOperation;
        public int[,] moduleMatrix;
        public List<GraphButtonInfo> modulesButtonsInfo = new List<GraphButtonInfo>();
        public List<Module> modulesList = new List<Module>();
        public OperationsList operations = new OperationsList();
        private int[] RarrayX;
        private int[] RarrayY;
        private List<int> removedModules = new List<int>();

        // Methods
        public void AddDetail(int numb, List<string> operString)
        {
            this.details.Add(new Detail(numb, operString));
            for (int i = 0; i < operString.Count; i++)
            {
                this.operations.AddUniqueOperation(operString[i]);
            }
        }

        private void ButtonsPositionsCreate(out int[] x, out int[] y, int count)
        {
            x = new int[count];
            y = new int[count];
            int num = 210;
            int num2 = 180;
            int num3 = 360 / count;
            float num4 = 180f;
            for (int i = 0; i < count; i++)
            {
                x[i] = (int)((num * 1.3) + ((num2 * 1.3) * Math.Cos((num4 * 3.1415926535897931) / 180.0)));
                y[i] = num + ((int)(num2 * Math.Sin((num4 * 3.1415926535897931) / 180.0)));
                num4 += num3;
            }
        }

        public void Clear()
        {
            this.details.Clear();
            this.operations.Clear();
        }

        public int CompareTo(object obj)
        {
            if (obj is Group)
            {
                Group group = (Group)obj;
                return (-1 * this.operations.NumberOfUniqueOperations.CompareTo(group.operations.NumberOfUniqueOperations));
            }
            return -1;
        }

        public bool ContainsDetail(int _num)
        {
            foreach (Detail detail in this.details)
            {
                if (detail.id == _num)
                {
                    return true;
                }
            }
            return false;
        }

        public void CreateModules()
        {
            int num;
            int num2;
            int num3;
            this.modulesList.Clear();
            this.removedModules.Clear();
            for (num = 0; num < this.graphUniqueOperations.Count; num++)
            {
                this.modulesList.Add(new Module(num + 1, this.graphUniqueOperations[num]));
            }
            this.moduleMatrix = new int[this.modulesList.Count, this.modulesList.Count];
            for (num = 0; num < this.modulesList.Count; num++)
            {
                num2 = 0;
                while (num2 < this.modulesList.Count)
                {
                    this.moduleMatrix[num, num2] = this.graphMatrix[num, num2];
                    num2++;
                }
            }
        Label_00C8:
            num = 0;
            while (num < this.modulesList.Count)
            {
                num2 = 0;
                while (num2 < this.modulesList.Count)
                {
                    if (((!this.removedModules.Contains(num) && !this.removedModules.Contains(num2)) && ((this.moduleMatrix[num, num2] == this.moduleMatrix[num2, num]) && (this.moduleMatrix[num, num2] == 1))) && ((this.modulesList[num].operations.Count + this.modulesList[num2].operations.Count) <= 6))
                    {
                        this.ModuleMatrixRebuild(num, num2);
                        goto Label_00C8;
                    }
                    num2++;
                }
                num++;
            }
            for (num = 0; num < this.modulesList.Count; num++)
            {
                num2 = 0;
                while (num2 < this.modulesList.Count)
                {
                    if (this.moduleMatrix[num, num2] == 1)
                    {
                        num3 = 0;
                        while (num3 < this.modulesList.Count)
                        {
                            switch (this.MTransit(num, num2, num3, 3))
                            {
                                case 7:
                                    goto Label_0229;

                                case 1:
                                    goto Label_00C8;
                            }
                            num3++;
                        }
                    }
                Label_0229:
                    num2++;
                }
            }
            for (num = 0; num < this.modulesList.Count; num++)
            {
                num2 = 0;
                while (num2 < this.modulesList.Count)
                {
                    if (this.moduleMatrix[num, num2] == 1)
                    {
                        for (num3 = 0; num3 < this.modulesList.Count; num3++)
                        {
                            switch (this.MTransit(num, num2, num3, 1))
                            {
                                case 7:
                                    goto Label_033B;

                                case 1:
                                    goto Label_00C8;

                                case 2:
                                    for (int i = 0; i < this.modulesList.Count; i++)
                                    {
                                        switch (this.MTransit(num, num3, i, 2))
                                        {
                                            case 1:
                                                goto Label_00C8;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                Label_033B:
                    num2++;
                }
            }
            int index = 0;
            for (num = 0; num < this.modulesList.Count; num++)
            {
                if (!this.removedModules.Contains(num))
                {
                    int num7 = 0;
                    num2 = 0;
                    while (num2 < this.modulesList.Count)
                    {
                        if (!this.removedModules.Contains(num2))
                        {
                            this.moduleMatrix[index, num7] = this.moduleMatrix[num, num2];
                            num7++;
                        }
                        num2++;
                    }
                    this.RarrayX[index] = this.RarrayX[num];
                    this.RarrayY[index] = this.RarrayY[num];
                    index++;
                }
            }
            for (num = this.modulesList.Count - 1; num >= 0; num--)
            {
                if (this.removedModules.Contains(num))
                {
                    this.modulesList.RemoveAt(num);
                }
            }
            for (num = 0; num < this.modulesList.Count; num++)
            {
                this.modulesButtonsInfo.Add(new GraphButtonInfo(num, "M" + this.Name + this.modulesList[num].Number.ToString(), this.modulesList[num].operations, this.RarrayX[num], this.RarrayY[num]));
            }
            for (num = 0; num < this.modulesList.Count; num++)
            {
                for (num2 = 0; num2 < this.modulesList.Count; num2++)
                {
                    if (this.moduleMatrix[num, num2] == 1)
                    {
                        this.modulesButtonsInfo[num].childrens.Add(num2);
                        Point item = this.modulesButtonsInfo[num2].CreateConnectionPoint(num, this.modulesButtonsInfo[num].centerPoint);
                        this.modulesButtonsInfo[num].childrensPoint.Add(item);
                    }
                }
            }
        }

        public void GraphCreate()
        {
            int num;
            int num2;
            this.graphButtonsInfo = new List<GraphButtonInfo>();
            this.graphMatrix = new int[this.operations.NumberOfUniqueOperations, this.operations.NumberOfUniqueOperations];
            for (num = 0; num < this.operations.NumberOfUniqueOperations; num++)
            {
                num2 = 0;
                while (num2 < this.operations.NumberOfUniqueOperations)
                {
                    this.graphMatrix[num, num2] = 0;
                    num2++;
                }
            }
            this.RarrayX = new int[this.operations.NumberOfUniqueOperations];
            this.RarrayY = new int[this.operations.NumberOfUniqueOperations];
            this.ButtonsPositionsCreate(out this.RarrayX, out this.RarrayY, this.operations.NumberOfUniqueOperations);
            for (num = 0; num < this.details.Count; num++)
            {
                this.lastCheckedOperation = null;
                for (num2 = 0; num2 < this.details[num].operations.Count; num2++)
                {
                    int num4;
                    int num5;
                    Point point;
                    string item = this.details[num].operations[num2];
                    if (!this.graphUniqueOperations.Contains(item))
                    {
                        int index;
                        if (this.lastCheckedOperation == null)
                        {
                            this.graphUniqueOperations.Add(item);
                            index = this.graphUniqueOperations.IndexOf(item);
                            this.graphButtonsInfo.Add(new GraphButtonInfo(index, this.graphUniqueOperations[index], this.RarrayX[index], this.RarrayY[index]));
                            this.lastCheckedOperation = item;
                        }
                        else
                        {
                            this.graphUniqueOperations.Add(item);
                            index = this.graphUniqueOperations.IndexOf(item);
                            this.graphButtonsInfo.Add(new GraphButtonInfo(index, this.graphUniqueOperations[index], this.RarrayX[index], this.RarrayY[index]));
                            num4 = this.graphUniqueOperations.IndexOf(this.lastCheckedOperation);
                            num5 = this.graphUniqueOperations.IndexOf(item);
                            this.graphMatrix[num4, num5] = 1;
                            this.graphButtonsInfo[num4].childrens.Add(num5);
                            point = this.graphButtonsInfo[num5].CreateConnectionPoint(num4, this.graphButtonsInfo[num4].centerPoint);
                            this.graphButtonsInfo[num4].childrensPoint.Add(point);
                            this.lastCheckedOperation = item;
                        }
                    }
                    else if (this.lastCheckedOperation == null)
                    {
                        this.lastCheckedOperation = this.details[num].operations[num2];
                    }
                    else
                    {
                        num4 = this.graphUniqueOperations.IndexOf(this.lastCheckedOperation);
                        num5 = this.graphUniqueOperations.IndexOf(item);
                        this.lastCheckedOperation = item;
                        if (this.graphMatrix[num4, num5] != 1)
                        {
                            this.graphMatrix[num4, num5] = 1;
                            this.graphButtonsInfo[num4].childrens.Add(num5);
                            point = this.graphButtonsInfo[num5].CreateConnectionPoint(num4, this.graphButtonsInfo[num4].centerPoint);
                            this.graphButtonsInfo[num4].childrensPoint.Add(point);
                        }
                    }
                }
            }
        }

        private void ModuleMatrixRebuild(int i, int x)
        {
            for (int j = 0; j < this.modulesList[x].operations.Count; j++)
            {
                this.modulesList[i].operations.Add(this.modulesList[x].operations[j]);
            }
            for (int k = 0; k < this.modulesList.Count; k++)
            {
                if (this.moduleMatrix[x, k] == 1)
                {
                    this.moduleMatrix[i, k] = 1;
                    this.moduleMatrix[x, k] = 0;
                }
                if (this.moduleMatrix[k, x] == 1)
                {
                    this.moduleMatrix[k, i] = 1;
                    this.moduleMatrix[k, x] = 0;
                }
                this.moduleMatrix[k, k] = 0;
            }
            double d = (this.RarrayX[i] + this.RarrayX[x]) / 2;
            double num4 = (this.RarrayY[i] + this.RarrayY[x]) / 2;
            this.RarrayX[i] = Convert.ToInt32(Math.Floor(d));
            this.RarrayY[i] = Convert.ToInt32(Math.Floor(num4));
            this.removedModules.Add(x);
        }

        public int MTransit(int i, int j, int x, int mode)
        {
            if ((((mode == 3) && (x != j)) && (this.moduleMatrix[i, x] == 1)) && (this.moduleMatrix[x, j] == 1))
            {
                if (((this.modulesList[i].operations.Count + this.modulesList[j].operations.Count) + this.modulesList[x].operations.Count) >= 7)
                {
                    return 7;
                }
                bool flag = true;
                for (int k = 0; k < this.modulesList.Count; k++)
                {
                    if ((this.moduleMatrix[x, k] == 1) && (k != j))
                    {
                        flag = false;
                    }
                    if ((this.moduleMatrix[k, x] == 1) && (k != i))
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    if (x > j)
                    {
                        this.ModuleMatrixRebuild(i, x);
                        this.ModuleMatrixRebuild(i, j);
                    }
                    else
                    {
                        this.ModuleMatrixRebuild(i, j);
                        this.ModuleMatrixRebuild(i, x);
                    }
                    return 1;
                }
            }
            if ((mode != 3) && (this.moduleMatrix[j, x] == 1))
            {
                if (this.moduleMatrix[x, i] == 1)
                {
                    if (((this.modulesList[i].operations.Count + this.modulesList[j].operations.Count) + this.modulesList[x].operations.Count) >= 7)
                    {
                        return 7;
                    }
                    if (x > j)
                    {
                        this.ModuleMatrixRebuild(i, x);
                        this.ModuleMatrixRebuild(i, j);
                    }
                    else
                    {
                        this.ModuleMatrixRebuild(i, j);
                        this.ModuleMatrixRebuild(i, x);
                    }
                    return 1;
                }
                if (((this.modulesList[i].operations.Count + this.modulesList[j].operations.Count) + this.modulesList[x].operations.Count) < 5)
                {
                    return 2;
                }
            }
            return 0;
        }

        public void ReOrganizeOperations()
        {
            this.operations.Clear();
            for (int i = 0; i < this.details.Count; i++)
            {
                foreach (string str in this.details[i].operations)
                {
                    this.operations.AddUniqueOperation(str);
                }
            }
        }

        public override string ToString()
        {
            string str = "";
            foreach (Detail detail in this.details)
            {
                str = str + (detail.id + 1);
                str = str + ", ";
            }
            return ((str + ";").Replace(", ;", ";") + "\n");
        }

        // Properties
        public string Name
        {
            get
            {
                return this.groupName.ToString();
            }
            set
            {
                this.groupName = value;
            }
        }
    }

}
