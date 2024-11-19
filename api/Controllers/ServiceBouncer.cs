using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace api.Controllers;


[ApiController]
[Route("[controller]")]
public class ServiceBouncer : ControllerBase
{
    [HttpGet(Name = "turnOffService")]
    public bool Get(string sServiceName)
    {
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"net stop {sServiceName}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        string sOutput = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        Console.WriteLine(sOutput);
        return true;
    }
}