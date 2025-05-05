using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MVC_PW.Controllers
{
    public class CarroController : Controller
    {
        private static List<Carro> _carros = new List<Carro>
        {
            new Carro { Id = 1, Marca = "Toyota", Modelo = "Corolla", Ano = 2022, Preco = 120000, DataFabricacao = new DateTime(2021, 05, 15) },
            new Carro { Id = 2, Marca = "Honda", Modelo = "Civic", Ano = 2021, Preco = 110000, DataFabricacao = new DateTime(2020, 07, 10) },
            new Carro { Id = 3, Marca = "Ford", Modelo = "Focus", Ano = 2020, Preco = 90000, DataFabricacao = new DateTime(2019, 03, 20) }
        };

        public IActionResult Index() => View(_carros);
        public IActionResult Visualizar(int id)
        {
            var carro = _carros.FirstOrDefault(c => c.Id == id);
            return carro == null ? NotFound() : View(carro);
        }
        public IActionResult Editar(int id)
        {
            var carro = _carros.FirstOrDefault(c => c.Id == id);
            return carro == null ? NotFound() : View(carro);
        }

        [HttpPost]
        public IActionResult Editar(Carro carro)
        {
            var carroExistente = _carros.FirstOrDefault(c => c.Id == carro.Id);
            if (carroExistente != null && ModelState.IsValid)
            {
                carroExistente.Marca = carro.Marca;
                carroExistente.Modelo = carro.Modelo;
                carroExistente.Ano = carro.Ano;
                carroExistente.Preco = carro.Preco;
                carroExistente.DataFabricacao = carro.DataFabricacao;
                return RedirectToAction("Index");
            }
            return View(carro);
        }

        public IActionResult Excluir(int id)
        {
            var carro = _carros.FirstOrDefault(c => c.Id == id);
            return carro == null ? NotFound() : View(carro);
        }

        [HttpPost]
        public IActionResult ExcluirConfirmado(int id)
        {
            var carro = _carros.FirstOrDefault(c => c.Id == id);
            if (carro != null)
            {
                _carros.Remove(carro);
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Carro carro)
        {
            if (ModelState.IsValid)
            {
                carro.Id = _carros.Any() ? _carros.Max(c => c.Id) + 1 : 1;
                _carros.Add(carro);
                return RedirectToAction("Index");
            }
            return View(carro);
        }
        public IActionResult GerarPDF()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Font tableFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                doc.Add(new Paragraph("Lista de Carros\n\n", titleFont));

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1, 2, 2, 1.5f, 2 });

                table.AddCell(new PdfPCell(new Phrase("ID", tableFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Marca", tableFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Modelo", tableFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Ano", tableFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Data de Fabricação", tableFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                foreach (var carro in _carros)
                {
                    table.AddCell(new PdfPCell(new Phrase(carro.Id.ToString(), tableFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(carro.Marca, tableFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(carro.Modelo, tableFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(carro.Ano.ToString(), tableFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(carro.DataFabricacao.ToString("dd/MM/yyyy"), tableFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                }

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "ListaCarros.pdf");
            }
        }
    }
}
