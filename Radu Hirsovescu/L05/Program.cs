namespace L05
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    internal class Program
    {
        private static CloudTable studentsTable_;
        private static CloudTable raportTable_;
        private static readonly string connectionString = Environment.GetEnvironmentVariable("connectionString");
        internal static async Task Main()
        {
            await InitializeRaportTable();
            OpenStudentTable();

            List<Student> students = await GetAllStudents();

            var mapStud = new Dictionary<string, int>();
            int cntGeneral = 0;
            foreach (Student s in students)
            {
                if (mapStud.ContainsKey(s.PartitionKey))
                    mapStud[s.PartitionKey]++;
                else
                    mapStud[s.PartitionKey] = 1;

                cntGeneral++;
            }

            Console.Write(DateTime.Now.ToString("HH:mm:ss") + ": ");
            foreach (KeyValuePair<string, int> s in mapStud)
            {
                RaportEntity raportEntity = new RaportEntity(s.Key, s.Value);
                await CreateRaport(raportEntity);
                Console.Write(s.Key + "->" + s.Value + ";  ");
            }
            RaportEntity raportEntity2 = new RaportEntity("General", cntGeneral);
            await CreateRaport(raportEntity2);
            Console.WriteLine("General->" + cntGeneral.ToString());
        }

        public static async Task<List<Student>> GetAllStudents()
        {
            var students = new List<Student>();

            TableQuery<Student> query = new TableQuery<Student>();

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<Student> resultSegment = await studentsTable_.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                students.AddRange(resultSegment.Results);
            } while (token != null);

            return students;
        }

        public static async Task CreateRaport(RaportEntity raport)
        {
            var insertOperation = TableOperation.Insert(raport);

            await raportTable_.ExecuteAsync(insertOperation);
        }

        private static void OpenStudentTable()
        {
            studentsTable_ = CloudStorageAccount.Parse(connectionString).CreateCloudTableClient().GetTableReference("studenti");
        }

        private static async Task InitializeRaportTable()
        {
            raportTable_ = CloudStorageAccount.Parse(connectionString).CreateCloudTableClient().GetTableReference("rapoarte");
            await raportTable_.CreateIfNotExistsAsync();
        }
    }
}
