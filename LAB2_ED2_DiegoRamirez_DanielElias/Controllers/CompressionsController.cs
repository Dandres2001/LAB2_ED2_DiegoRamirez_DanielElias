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
            var encoder = Encoding.GetEncoding(28591);
            string aux = "";
            for (int i = 0; i < keys.Count(); i++)
            {
                apariciones.Add(keys.ElementAt(i).ToString());
                aux = encoder.GetString(huffman.ConvertToByte(values.ElementAt(i).ToString()));
                apariciones.Add(aux);
            }
            byte[] bytearray = apariciones.SelectMany(s=> Encoding.GetEncoding(28591).GetBytes(s)).ToArray();
            List<int> encoding = huffman.Encode(texto);
            string largo = encoder.GetString(huffman.ConvertToByte(encoding.Count.ToString()));
            byte[] arrayLargo = Encoding.GetEncoding(28591).GetBytes(largo).ToArray();
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

            //ESCRIBIENDO ARCHIVO
            string text = bytearray + "\n" + dataAsBytes;
            byte[] nullarray = { default };


            return base.File(Combine(arrayLargo, nullarray, bytearray,nullarray,dataAsBytes), "compressedFile / huff", name + ".huff");
        }

        // PUT api/<CompressionsController>/5
        [HttpPost("decompress")]
        public async Task<FileResult> Decompress([FromForm] IFormFile File)
        {
            byte[] bytes;
            using (var memory = new MemoryStream())
            {
                await File.CopyToAsync(memory);
                bytes = memory.ToArray();
                List<byte> aux = bytes.OfType<byte>().ToList();

            }
            int i = 0;
            byte[] byteTamaño = new byte[bytes.Length];
            byte[] apariciones = new byte[bytes.Length];
            byte[] mensaje = new byte[bytes.Length];
            int j = 0;
            while (bytes[i] != 0)
            {
                
                byteTamaño[j] = bytes[i];
                i++;
                j++;
            }
            i++;
            j = 0;
            while (bytes[i] != 0)
            {
                
                apariciones[j] = bytes[i];
                i++;
                j++;
            }
            i++;
            j = 0;
            while (i != bytes.Length)
            {
               
                mensaje[j] = bytes[i];
                i++;
                j++;
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

        private byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}
