using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tema
{
    class Program
    {
        private static readonly string StorageConnectionString = Environment.GetEnvironmentVariable("accesString");

        private static CloudTableClient tableClient;
        private static CloudTable studentsTable;
        private static ITableEntity studenti;

        static async Task AddStudent()
        {
            var student = new Student("UPT","354");
            student.Nume = "Popescu";
            student.Prenume = "Andrei";
            student.An = 4;
            student.Nr = "0783809342";
            student.Facultate = "AC";

            var insertOperation = TableOperation.Insert(student);

            await studentsTable.ExecuteAsync(insertOperation);
        }

        private static async Task DisplayStudents()
        {
            Console.WriteLine("Universitate\tID\tNume\tPrenume\tNr\tAn");
            TableQuery<Student> query = new TableQuery<Student>();

            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<Student> resultSegment = await studentsTable.ExecuteQuerySegmentedAsync(query,token);
                token = resultSegment.ContinuationToken;

                foreach (Student entity in resultSegment.Results)
                {
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", entity.PartitionKey, entity.RowKey, entity.Nume,
                        entity.Prenume, entity.An, entity.Nr, entity.Facultate);
                }
            } while (token != null);
        }

        private static async Task GetStudent(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey);
            await studentsTable.ExecuteAsync(retrieveOperation);

        }

        private static async Task UpdateStudent(ITableEntity studenti)
        {
            var updateOperation = TableOperation.InsertOrReplace(studenti);
            await studentsTable.ExecuteAsync(updateOperation);
        }

        private static async Task DeleteStudent(ITableEntity studenti)
        {
            var deleteOperation = TableOperation.Delete(studenti);
            await studentsTable.ExecuteAsync(deleteOperation);
        }

        static void Main(string[] args)
        {
            Console.WriteLine(StorageConnectionString);
        }

        static async Task Initialize()
        {
            var account = CloudStorageAccount.Parse(StorageConnectionString);
            tableClient = account.CreateCloudTableClient();
            studentsTable = tableClient.GetTableReference("studenti");

            await studentsTable.CreateIfNotExistsAsync();
            await AddStudent();
            await DisplayStudents();
        }
    }
}
