using System;
using System.Collections.Generic;
using System.Text;

namespace GKS2
{
    class Detail
    {
        // Fields
        public int id;
        public List<string> operations = new List<string>();

        // Methods
        public Detail(int _id, List<string> _operationsList)
        {
            this.id = _id;
            for (int i = 0; i < _operationsList.Count; i++)
            {
                this.operations.Add(_operationsList[i]);
            }
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
    }

}
