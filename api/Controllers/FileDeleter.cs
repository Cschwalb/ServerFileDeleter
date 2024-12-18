using System.Net;
using Microsoft.AspNetCore.Mvc;
namespace api.Controllers;


public class Files
{
    public string Path { get; set; }
    public string ID { get; set; }
    public string Extension { get; set; }
    
    public long Size { get; set; }

    public Files(string path, string id)
    {
        Path = path;
        ID = id;
        var split = path.Split('.');
        Extension = split[1];
        FileInfo fi = new FileInfo(path);
        Size = fi.Length;
    }
}

[ApiController]
[Route("[controller]")]
public class FileDeleter : ControllerBase
{
    [HttpGet(Name = "GetFilesToDelete")]
    public List<Files> Get(string sDirectory)
    {
        Console.WriteLine("Getting Files to Delete!");
        var fileListMP3 = Directory.GetFiles(sDirectory, "*.mp3");
        var fileListWEBM = Directory.GetFiles(sDirectory, "*.WEBM");
        int counter = 0;
        var ListOfFilesToDelete = fileListWEBM.Union(fileListMP3);
        // if I knew more regex this union wouldn't be needed.
        List<Files> LOF = new List<Files>();
        
        foreach (var item in ListOfFilesToDelete)
        {
            var file = new Files(item, counter.ToString());
            FileInfo getSize = new FileInfo(item);
            file.Size = getSize.Length;
            counter++;
            LOF.Add(file);
        }
        
        return LOF;
    }

    [HttpPost(Name = "DeleteFiles")]
    public async Task<HttpResponseMessage> Post(string sDirectory)
    {
        // check if file directory is there
        if (!Directory.Exists(sDirectory) || string.IsNullOrEmpty(sDirectory))
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Directory Doesn't Exist!")
            };
        }
        // get list of files
        List<Files> listToDelete = Get(sDirectory);
        // check to make sure list isn't empty
        if (listToDelete.Count == 0)
        {
            return new HttpResponseMessage(HttpStatusCode.AlreadyReported)
            {
                Content = new StringContent("No Items in list to delete!")
            };
        }
        foreach (var item in listToDelete)
        {
            if (System.IO.File.Exists(item.Path))
            {
                System.IO.File.Delete(item.Path);
                Console.WriteLine("Deleted Item:  " + item.Path);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Could not find Item to delete!")
                };
            }
        }

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Files were deleted!")
        };
    }
}