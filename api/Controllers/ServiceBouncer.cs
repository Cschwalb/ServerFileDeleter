using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace api.Controllers;


[ApiController]
[Route("[controller]")]
public class ServiceBouncer : ControllerBase
{
    [HttpPost(Name = "turnOffService")]
    public bool Get(string sServiceName)
    {
        if (sServiceName.Length <= 0)
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
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "RunAs"
                }
            };
            process.Start();
            string sOutput = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine(sOutput);
            return true;
        }
        else
        {
            return false;
        }
    }
}