using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace L04
{
    class Student : TableEntity
    {
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public int An { get; set; }
        public string Nr { get; set; }
        public string Facultate { get; set; }


        public Student(string university, string id)
        {
            this.PartitionKey = university;
            this.RowKey = id;
        }

        public Student(){}
    }
}
