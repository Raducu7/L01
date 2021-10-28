using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace tema
{
    public class Program
    {
        protected static string[] scopes = { DriveService.Scope.Drive };
        protected static UserCredential credential;
        static string ApplicationName = "tema";
        protected static DriveService service;

        public static void Main(string[] args)
        {

            FileStream file =
                File.OpenRead(@"C:\Users\Radu\Desktop\New folder\tema\file.txt");
            DriveService service;
            service = DriveApiService();
            ListEntities();
            string id = CreateFolder("root", "test", service);
            System.Threading.Thread.Sleep(5000);
            Remove(id);
            Console.Read();


        }

        public static void Remove(string id)
        {
            service.Files.Delete(id).Execute();
        }


        public static DriveService DriveApiService()
        {
            using (var stream =
                new FileStream("C:\\Users\\Radu\\Desktop\\New folder\\tema\\credential.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

                Console.WriteLine("Credential file saved to: " + credPath);
            }

            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });


            UploadTxtFile(service);

            return service;
        }


        public async Task<Google.Apis.Drive.v3.Data.File> Upload(IFormFile file, string documentId)
        {
            var name = ($"{DateTime.UtcNow.ToString()}.{Path.GetExtension(file.FileName)}");
            var mimeType = file.ContentType;

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name,
                MimeType = mimeType,
                Parents = new[] { documentId }
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = file.OpenReadStream())
            {
                request = service.Files.Create(
                    fileMetadata, stream, mimeType);
                request.Fields = "id, name, parents, createdTime, modifiedTime, mimeType, thumbnailLink";
                await request.UploadAsync();
            }


            return request.ResponseBody;
        }

        public static IList<Google.Apis.Drive.v3.Data.File> ListEntities(string id = "root")
        {
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 100;
            listRequest.Fields = "nextPageToken, files(id, name, parents, createdTime, modifiedTime, mimeType)";
            listRequest.Q = $"'{id}' in parents";

            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
            Console.Read();

            return listRequest.Execute().Files;
        }

        public static string CreateFolder(string parent, string folderName, DriveService service_)
        {

            var driveFolder = new Google.Apis.Drive.v3.Data.File();
            driveFolder.Name = folderName;
            driveFolder.MimeType = "application/vnd.google-apps.folder";
            driveFolder.Parents = new string[] { parent };
            var command = service.Files.Create(driveFolder);
            var file = command.Execute();
            return file.Id;
        }

        public async Task<Stream> Download(string fileId)
        {
            Stream outputstream = new MemoryStream();
            var request = service.Files.Get(fileId);

            await request.DownloadAsync(outputstream);

            outputstream.Position = 0;

            return outputstream;
        }

        static async void UploadTxtFile(DriveService service)
        {
            string filepath = @"C:\Users\Radu\Desktop\New folder\tema\file.txt";
            if (System.IO.File.Exists(filepath.ToString()))
            {
                using var uploadStream = System.IO.File.OpenRead(filepath);

                using (StreamReader sr = File.OpenText(filepath))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }

                Google.Apis.Drive.v3.Data.File driveFile = new Google.Apis.Drive.v3.Data.File
                {
                    Name = "file.txt"
                };

                FilesResource.CreateMediaUpload insertRequest = service.Files.Create(
                    driveFile, uploadStream, "text/plain");

                Console.WriteLine(insertRequest.Body.Id);

                insertRequest.ProgressChanged += Upload_ProgressChanged;
                insertRequest.ResponseReceived += Upload_ResponseReceived;

                await insertRequest.UploadAsync();
            }
        }
        static void Upload_ProgressChanged(Google.Apis.Upload.IUploadProgress progress) =>
                    Console.WriteLine(progress.Status + " " + progress.BytesSent);

        static void Upload_ResponseReceived(Google.Apis.Drive.v3.Data.File file) =>
                Console.WriteLine(file.Name + " was uploaded successfully");

    }
}