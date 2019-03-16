using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using electron.Models;
using ElectronNET.API;
using ElectronNET.API.Entities;
using System.IO;
using System.Text;
using ChoETL;

namespace electron.Controllers
{
    public class HomeController : Controller
    {


        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                Electron.IpcMain.On("select-directory", async (args) => {
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    var options = new OpenDialogOptions
                    {
                        Properties = new OpenDialogProperty[] {
                        OpenDialogProperty.openFile
                    }
                    };
                 

                    string[] files = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);

                    string csv = System.IO.File.ReadAllText(files[0]);
                    StringBuilder sb = new StringBuilder();
                    using (var p = ChoCSVReader.LoadText(csv)
                        .WithFirstLineHeader()
                        )
                    {
                        using (var w = new ChoJSONWriter(sb))
                            w.Write(p);
                    }
                    string json = sb.ToString();
                    Electron.IpcMain.Send(mainWindow, "select-directory-reply", json);
                });
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
