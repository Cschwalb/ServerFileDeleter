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
        if (sServiceName.Length <= 0 || string.IsNullOrWhiteSpace(sServiceName))
        {
            return false;
        }
        
        if (OperatingSystem.IsWindows())
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C net stop {sServiceName}", // was \C net stop {sServiceName}
                    RedirectStandardOutput = true,
                    RedirectStandardError = true, // Capture errors too
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "RunAs"
                }
            };
            if (!process.Start())
            {
                return false;
            }
            string sOutput = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            Console.WriteLine(error);
            Console.WriteLine(sOutput);
            //todo write handling for output
            if (error.Contains("invalid"))
            {
                return false;
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}