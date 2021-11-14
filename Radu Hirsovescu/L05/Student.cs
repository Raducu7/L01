namespace L05
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class Student : TableEntity
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Year { get; set; }
        public string PhoneNumber { get; set; }
        public string Faculty { get; set; }
        public Student(string university, string cnp)
        {
            this.PartitionKey = university;
            this.RowKey = cnp;
        }
        public Student()
        {
        }
    }
}
