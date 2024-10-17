using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

public class AzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobStorageService(string connectionString, string containerName)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = containerName;
    }

    public async Task UploadFileAsync(string filePath, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);

        await using FileStream uploadFileStream = File.OpenRead(filePath);
        await blobClient.UploadAsync(uploadFileStream, true);

        Console.WriteLine($"File {fileName} uploaded successfully.");
    }

    public async Task DownloadFileAsync(string fileName, string downloadFilePath)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        BlobDownloadInfo download = await blobClient.DownloadAsync();

        await using FileStream downloadFileStream = File.OpenWrite(downloadFilePath);
        await download.Content.CopyToAsync(downloadFileStream);

        Console.WriteLine($"File {fileName} downloaded successfully.");
    }

    public async Task ListFilesAsync()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            Console.WriteLine($"File Name: {blobItem.Name}");
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "YOUR_CONNECTION_STRING_HERE";
        string containerName = "myfiles";

        var blobService = new AzureBlobStorageService(connectionString, containerName);

        // Upload a file
        await blobService.UploadFileAsync("path/to/local/file.txt", "file.txt");

        // List all files
        await blobService.ListFilesAsync();

        // Download a file
        await blobService.DownloadFileAsync("file.txt", "path/to/download/file.txt");
    }
}
