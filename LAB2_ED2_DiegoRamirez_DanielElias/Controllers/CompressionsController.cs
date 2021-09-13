using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using LAB2_ED2_DiegoRamirez_DanielElias.Models;
using LAB2_ED2_DiegoRamirez_DanielElias.Data;
using LibreriaRD3;
using Microsoft.AspNetCore.Http;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LAB2_ED2_DiegoRamirez_DanielElias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompressionsController : ControllerBase
    {
       
         [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        // GET api/<CompressionsController>/5
   

        // POST api/<CompressionsController>
        [HttpPost("compress/{name}")]
        public FileResult Compress([FromRoute] string name,[FromForm] IFormFile File)
        {
           
            var reader = new StreamReader(File.OpenReadStream());
            string texto = reader.ReadToEnd();
            reader.Close();
            
            //COMPRIMIENDO TEXTO
            var huffman = new Huffman<char>(texto);
            var keys = huffman.countsDictionary.Keys;
            var values = huffman.countsDictionary.Values;

            List<string> apariciones = new List<string>();
           
    
            for (int i = 0; i < keys.Count(); i++)
            {
                apariciones.Add(keys.ElementAt(i).ToString());
                apariciones.Add(values.ElementAt(i).ToString());
            }
           byte[] bytearray = apariciones.SelectMany(s=>Encoding.GetEncoding(28591).GetBytes(s)).ToArray();
            var encoder = Encoding.GetEncoding(28591);
            //byte[] aparicionesBytes = 

            List<int> encoding = huffman.Encode(texto);
            List<string> compressed = huffman.Encodeascii(encoding);
            byte[] dataAsBytes = compressed.SelectMany(s => Encoding.GetEncoding(28591).GetBytes(s)).ToArray();
            //ARCHIVANDO COMPRESION
            #region
            var newCompression = new Compressions();
            string NombreOriginal = File.FileName;
            double bytesOriginal = Convert.ToDouble(File.Length);
            string NombreComprimido = name + ".huff";
            double factorDeCompresion = CalcularFactorCompresion(bytesOriginal, dataAsBytes.Length);
            double razonDeCompresion = CalcularRazonCompresion(bytesOriginal, dataAsBytes.Length);

            newCompression.NombreOriginal = NombreOriginal;
            newCompression.NombreRutadeArchivo = NombreComprimido;
            newCompression.FactorCompresion = factorDeCompresion;
            newCompression.RazonCompresion = razonDeCompresion;

            Singleton.Instance.CompList.Add(newCompression);
            #endregion

            return base.File(dataAsBytes, "compressedFile / huff", name + ".huff");
        }

        // PUT api/<CompressionsController>/5
        [HttpPost("decompress")]
        public async Task<FileResult> Decompress([FromForm] IFormFile File)
        {
            
            using (var memory = new MemoryStream())
            {
                await File.CopyToAsync(memory);
                byte[] bytes = memory.ToArray();
                List<byte> aux = bytes.OfType<byte>().ToList();
                
            }
            return null;
        }
        
        public double CalcularFactorCompresion(double BytesOriginal, double BytesComprimido)
        {
            double Factor = Math.Round(BytesOriginal / BytesComprimido, 3);
            return Factor;
        }
        public double CalcularRazonCompresion(double BytesOriginal, double BytesComprimido)
        {
            double Razon = Math.Round(BytesComprimido / BytesOriginal, 3);
            return Razon;
        }

        public static string StringToBinary(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }
    }
}
