using System;
using System.Collections.Generic;
using System.Text;

namespace GKS2
{
    public class OperationsList : IComparable
    {
        // Fields
        private List<string> unOperations = new List<string>();

        // Methods
        public bool AddUniqueOperation(string oper)
        {
            if (this.unOperations.Contains(oper))
            {
                return false;
            }
            this.unOperations.Add(oper);
            return true;
        }

        internal void Clear()
        {
            this.unOperations.Clear();
        }

        public int CompareTo(object obj)
        {
            if (obj is OperationsList)
            {
                OperationsList list = (OperationsList)obj;
                return (-1 * this.NumberOfUniqueOperations.CompareTo(list.NumberOfUniqueOperations));
            }
            return -1;
        }

        public bool ContainsOperations(List<string> compList)
        {
            foreach (string str in compList)
            {
                if (!this.unOperations.Contains(str))
                {
                    return false;
                }
            }
            return true;
        }

        public string GetOperation(int oper)
        {
            return this.unOperations[oper];
        }

        public override string ToString()
        {
            string str = "";
            foreach (string str2 in this.unOperations)
            {
                str = str + str2;
                str = str + ", ";
            }
            return ((str + ";").Replace(", ;", ";") + "\n");
        }

        // Properties
        public List<string> GroupUniqueOperations
        {
            get
            {
                return this.unOperations;
            }
        }

        public int NumberOfUniqueOperations
        {
            get
            {
                return this.unOperations.Count;
            }
        }
    }


}
