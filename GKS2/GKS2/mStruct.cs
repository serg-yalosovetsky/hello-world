using System;
using System.Collections.Generic;
using System.Text;

namespace GKS2
{
    public class mStruct
    {
        // Fields
        private int elements;

        public int[] enters;
        public int[] exits;

        private int maxentern;
        private int maxexitn;

        public List<int>[,] modMatrix;
        public List<Module> modModules;
        
        private List<List<string>> modObjectsOperations;

        // Methods
        public mStruct(List<Module> _mod, List<List<string>> _oper)
        {
            int num;
            this.modModules = new List<Module>();
            this.modObjectsOperations = new List<List<string>>();
            this.elements = 0;
            this.maxexitn = 0;
            this.maxentern = 0;
            this.modModules = _mod;
            this.modObjectsOperations = _oper;
            this.elements = this.modModules.Count + 2;
            this.modMatrix = new List<int>[this.elements, this.elements];
            for (num = 0; num < this.elements; num++)
            {
                for (int i = 0; i < this.elements; i++)
                {
                    this.modMatrix[num, i] = new List<int>();
                    this.modMatrix[num, i].Clear();
                }
            }
            this.FillModMatrix();
            this.enters = new int[this.elements];
            this.exits = new int[this.elements];
            for (num = 0; num < this.elements; num++)
            {
                this.enters[num] += this.modMatrix[0, num].Count;
                this.exits[num] += this.modMatrix[num, this.elements - 1].Count;
            }
            int num3 = 0;
            int num4 = 0;
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            for (num = 0; num < this.enters.Length; num++)
            {
                if (this.enters[num] > num3)
                {
                    num3 = this.enters[num];
                    list.Clear();
                    list.Add(num);
                }
                else if (this.enters[num] == num3)
                {
                    list.Add(num);
                }
            }
            for (num = 0; num < this.exits.Length; num++)
            {
                if (this.exits[num] > num4)
                {
                    if (!list.Contains(num))
                    {
                        num4 = this.exits[num];
                        list2.Clear();
                        list2.Add(num);
                    }
                }
                else if ((this.exits[num] == num4) && !list.Contains(num))
                {
                    list2.Add(num);
                }
            }
            if (list.Count == 1)
            {
                this.maxentern = list[0];
            }
            else
            {
                this.maxentern = list[0];
            }
            if (list2.Count == 1)
            {
                this.maxexitn = list2[0];
            }
            else
            {
                this.maxexitn = list2[list2.Count - 1];
            }
        }

        private void FillModMatrix()
        {
            for (int i = 0; i < (this.modObjectsOperations.Count - 1); i++)
            {
                int z = 0;
                for (int j = 0; j < this.modObjectsOperations[i].Count; j++)
                {
                    for (int k = 0; k < this.modModules.Count; k++)
                    {
                        if (this.modModules[k].operations.Contains(this.modObjectsOperations[i][j]) && !this.modMatrix[z, k + 1].Contains(i + 1))
                        {
                            this.modMatrix[z, k + 1].Add(i + 1);
                            z = k + 1;
                        }
                    }
                }
                this.modMatrix[z, this.modModules.Count + 1].Add(i + 1);
            }
        }

        // Properties
        public int DirectConnectionsNumber
        {
            get
            {
                int connections = 0;
                for (int i = 1; i < (this.elements - 1); i++)
                    for (int j = 1; j < i; j++)
                        connections += this.modMatrix[j, i].Count;
                
                return connections;
            }
        }

        public int InversConnectionsNumber
        {
            get
            {
                int connections = 0;
                for (int i = 0; i < this.elements; i++)
                    for (int j = 0; j < i; j++)
                        connections += this.modMatrix[i, j].Count;
                
                return connections;
            }
        }

        public int MaxEnterModuleNumber
        {
            get
            {
                return this.maxentern;
            }
        }

        public int MaxExitModuleNumber
        {
            get
            {
                return this.maxexitn;
            }
        }
    }


}
