using System;
using System.Collections.Generic;
using System.Text;

namespace GKS2
{
    public class mSystem
    {
        // Fields
        public mStruct DefaultStruct;
        public int iterations = 0;
        public int OptimalInverse;
        public List<Module> nModules;
        public List<List<Module>> OptimalModules;
        public List<mStruct> OptimalStruct;
        public List<List<string>> OptObjectOperations;
        private bool StructSimple = false;

        // Events
        public event dStatus Loading;

        // Methods
        public mSystem(List<Module> _nModules, List<List<string>> _nObjectsOperations, bool _StructSimple)
        {
            this.OptObjectOperations = _nObjectsOperations;
            this.DefaultStruct = null;
            this.DefaultStruct = new mStruct(_nModules, _nObjectsOperations);
            this.nModules = _nModules;
            this.StructSimple = _StructSimple;
        }

        private void mSystem_Loading(string str)
        {
        }

        public void Optimize()
        {
            Module module;
            int num = this.DefaultStruct.MaxEnterModuleNumber - 1;
            int num2 = this.DefaultStruct.MaxExitModuleNumber - 1;
            int inversConnectionsNumber = this.DefaultStruct.InversConnectionsNumber;
            int directConnectionsNumber = this.DefaultStruct.DirectConnectionsNumber;
            List<Module> list = new List<Module>();
            list.Clear();
            list.AddRange(this.nModules);
            if ((num != -2) && (this.nModules.Count > 3))
            {
                module = list[num];
                list[num] = list[0];
                list[0] = module;
                if (num2 == 0)
                {
                    num2 = num;
                }
            }
            if ((num2 != -2) && (this.nModules.Count > 3))
            {
                module = list[num2];
                list[num2] = list[list.Count - 1];
                list[list.Count - 1] = module;
            }
            mStruct item = new mStruct(list, this.OptObjectOperations);
            int num5 = item.InversConnectionsNumber;
            this.OptimalModules = null;
            this.OptimalStruct = null;
            this.OptimalInverse = 0;
            this.OptimalModules = new List<List<Module>>();
            this.OptimalStruct = new List<mStruct>();
            this.OptimalModules.Add(list);
            this.OptimalStruct.Add(item);
            this.OptimalInverse = num5;
            this.iterations = 0;
            int exit = 0;
            int k = 0;
            if (num != -2)
            {
                k = 1;
            }
            if (num2 != -2)
            {
                exit = 1;
            }
            if (this.StructSimple)
            {
                this.Permut(k, list, exit);
            }
            else
            {
                this.Permut(0, list, 0);
            }
        }

        private void Permut(int k, List<Module> lMod, int exit)
        {
            int count = lMod.Count;
            if (k < (count - exit))
            {
                for (int i = k; i < (count - exit); i++)
                {
                    Module module = lMod[i];
                    int num3 = i;
                    while (num3 > k)
                    {
                        lMod[num3] = lMod[num3 - 1];
                        num3--;
                    }
                    lMod[k] = module;
                    this.Permut(k + 1, lMod, exit);
                    for (num3 = k; num3 < i; num3++)
                    {
                        lMod[num3] = lMod[num3 + 1];
                    }
                    lMod[i] = module;
                }
            }
            else
            {
                this.iterations++;
                if ((this.iterations % 0x1388) == 0)
                {
                    this.Loading(">");
                }
                List<Module> list = new List<Module>();
                list.Clear();
                list.AddRange(lMod);
                mStruct item = new mStruct(list, this.OptObjectOperations);
                int inversConnectionsNumber = item.InversConnectionsNumber;
                if (inversConnectionsNumber < this.OptimalInverse)
                {
                    this.OptimalInverse = inversConnectionsNumber;
                    this.OptimalStruct.Clear();
                    this.OptimalModules.Clear();
                    this.OptimalStruct.Add(item);
                    this.OptimalModules.Add(list);
                }
                else if (inversConnectionsNumber == this.OptimalInverse)
                {
                    this.OptimalStruct.Add(item);
                    this.OptimalModules.Add(list);
                }
            }
        }
    }

 

}
