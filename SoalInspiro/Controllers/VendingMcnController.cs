using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SoalInspiro.Models;
using Newtonsoft.Json;

namespace SoalInspiro.Controllers
{
    public class VendingMcnController : Controller
    {
        private IHostingEnvironment _environment;
        private List<Makanan> DataMakanan = new List<Makanan>();
        private Output _result = new Output();
        public VendingMcnController(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        public IActionResult Index()
        {
            this.LoadDataJson();
            ViewBag.ListMakanan = DataMakanan;
            return View();
        }

        public JsonResult Prosess(List<Makanan> models, List<Makanan> dataMakanan)
        {
            try
            {

                dataMakanan.ForEach(delegate (Makanan data)
                {
                    var list = models.Where(w => w.Nama.Contains(data.Nama)).ToList();
                    if (data.Stock < list.Count)
                    {
                        throw new Exception(string.Format("Stock {0} kurang dari permintaan.",data.Nama));
                    }
                    data.Stock = data.Stock - list.Count;
                });

                DataMakanan = dataMakanan;
                ViewBag.ListMakanan = DataMakanan;
                this.WriteDataJson();
                //return RedirectToAction("Index", "VendingMcn");
            }
            catch (Exception ex)
            {
                _result.IsError = true;
                _result.Pesan = ex.Message;
                return Json(_result);
            }
            _result.Pesan = "Silahkan ambil makanan dan kembalian anada.";
            return Json(_result);
        }
        private void LoadDataJson()
        {
            if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
            {
                _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            string path = Path.Combine(_environment.WebRootPath, @"makanan.json");
            //string[] files = File.ReadAllLines(path);
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                DataMakanan = JsonConvert.DeserializeObject<List<Makanan>>(json);
            }
        }
        private void WriteDataJson()
        {
            if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
            {
                _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            string path = Path.Combine(_environment.WebRootPath, @"makanan.json");
            using (StreamWriter file = System.IO.File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, DataMakanan);
            }
        }
    }
}