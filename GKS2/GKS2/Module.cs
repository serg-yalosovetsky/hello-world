using System;
using System.Collections.Generic;
using System.Text;

namespace GKS2
{
    public class Module : IComparable
    {
        // Fields
        private int numb;
        internal List<string> operations;

        // Methods
        public Module(int _num)
        {
            this.operations = new List<string>();
            this.numb = _num;
        }

        public Module(int _num, string _oper)
        {
            this.operations = new List<string>();
            this.numb = _num;
            this.operations.Add(_oper);
        }

        public void AddOperations(List<string> _op)
        {
            this.operations.AddRange(_op);
        }

        public int CompareTo(object obj)
        {
            if (obj is Module)
            {
                Module module = (Module)obj;
                return (-1 * this.operations.Count.CompareTo(module.operations.Count));
            }
            return -1;
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < this.operations.Count; i++)
            {
                str = str + this.operations[i] + ", ";
            }
            return (str + ";").Replace(", ;", ";");
        }

        // Properties
        public int Number
        {
            get
            {
                return this.numb;
            }
        }
    }


}
