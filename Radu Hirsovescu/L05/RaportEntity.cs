namespace L05
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;


    public class RaportEntity : TableEntity
    {
        public RaportEntity(string university, int count)
        {
            this.PartitionKey = university;
            this.RowKey = DateTime.Now.ToString("HH:mm:ss");
            this.Count = count;
        }

        public RaportEntity()
        {
        }

        public int Count { get; set; }
    }
}
