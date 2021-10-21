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
using Microsoft.AspNetCore.Routing;
using System.Text.Json;
using System.Web;
using System.Net;
using System.Net.Http.Headers;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LAB2_ED2_DiegoRamirez_DanielElias.Controllers
{
    [Route("api")]
    [ApiController]
    public class CompressionsController : ControllerBase
    {
        static string ORIGINALFILENAME;
        static string ORIGINALCONTENT;
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        // GET api/<CompressionsController>/5
        
        [HttpGet("compressions")]
        public string GetCompressions([FromRoute] string recorrido)
        {
            string returnJson = "";

            try
            {
                for (int i = 0; i < Singleton.Instance.CompList.Count; i++)
                {
                    returnJson += JsonSerializer.Serialize<CompModel>(Singleton.Instance.CompList.ElementAt(i));
                }

            }
            catch
            {


            }
            return returnJson;
        }

        // POST api/<CompressionsController>
        [HttpPost("compress/{name}")]
        public async Task<FileResult> CompressAsync([FromRoute] string name,[FromForm] IFormFile File)
        {

            var huffman = new Huffman();
            byte[] fileData;
            using (var memory = new MemoryStream())
            {
                await File.CopyToAsync(memory);


                fileData = memory.ToArray();


            }

            //COMPRIMIENDO TEXTO
            byte[] compressedData = huffman.Compress(fileData);
          
            //ARCHIVANDO COMPRESION
            #region
            var newCompression = new CompModel();
            string NombreOriginal = File.FileName;
            double bytesOriginal = Convert.ToDouble(File.Length);
            string NombreComprimido = ".huff";
            double factorDeCompresion = CalcularFactorCompresion(bytesOriginal, compressedData.Length);
            double razonDeCompresion = CalcularRazonCompresion(bytesOriginal, compressedData.Length);
            string PorcentajeReducion = CalcularPorcentajeReducción(razonDeCompresion);
            var url = Request.Path;
            var requestUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            string rutaYnombre = requestUrl + url + NombreComprimido; 
            newCompression.NombreOriginal = NombreOriginal;
            newCompression.NombreRutadeArchivo = rutaYnombre;
            newCompression.FactorCompresion = factorDeCompresion;
            newCompression.RazonCompresion = razonDeCompresion;
            newCompression.PorcentajeReduccion = PorcentajeReducion;
            ORIGINALFILENAME = File.FileName;
            ORIGINALCONTENT = File.ContentType;
            Singleton.Instance.CompList.Add(newCompression);
            #endregion

            //ESCRIBIENDO ARCHIVO
            


            return base.File(compressedData, "compressedFile / huff", name + ".huff");
        }

        // PUT api/<CompressionsController>/5
        [HttpPost("decompress")]
        public async Task<FileResult> Decompress([FromForm] IFormFile File)
        {

            var huffman = new Huffman();
            byte[] fileData;
            using (var memory = new MemoryStream())
            {
                await File.CopyToAsync(memory);
                
                
                fileData = memory.ToArray();
              
            }
            byte[] decompressedData = huffman.Decompress(fileData);


            var lista = Singleton.Instance.CompList;

           
            return base.File(decompressedData, ORIGINALCONTENT, ORIGINALFILENAME);
        }


//        [HttpPost("lzw/compress/{name}")]
//        public FileResult LZWCompress([FromRoute] string name, [FromForm] IFormFile File)
//        {
//            var compressor = new LZW<string>();
//            var reader = new StreamReader(File.OpenReadStream());
//            string texto = reader.ReadToEnd();
//            reader.Close();
//            byte[] archivoComprimido = compressor.Compress(texto);

//            //GUARDANDO COMPRESIÓN
//            var newCompression = new CompModel();
//            string NombreOriginal = File.FileName;
//            double bytesOriginal = Convert.ToDouble(File.Length);
//            string NombreComprimido = ".lzw";
//            double factorDeCompresion = CalcularFactorCompresion(bytesOriginal, archivoComprimido.Length);
//            double razonDeCompresion = CalcularRazonCompresion(bytesOriginal, archivoComprimido.Length);
//            string PorcentajeReducion = CalcularPorcentajeReducción(razonDeCompresion);
//            var url = Request.Path;
//            var requestUrl = $"{Request.Scheme}://{Request.Host.Value}/";
//            string rutaYnombre = requestUrl + url + NombreComprimido;
//            newCompression.NombreOriginal = NombreOriginal;
//            newCompression.NombreRutadeArchivo = rutaYnombre;
//            newCompression.FactorCompresion = factorDeCompresion;
//            newCompression.RazonCompresion = razonDeCompresion;
//            newCompression.PorcentajeReduccion = PorcentajeReducion;

//            return base.File(archivoComprimido, "compressedFile / lzw", name + ".lzw");
//        }

//        [HttpPost("lzw/decompress")]
//        public async Task<FileResult> LZWDecompress([FromForm] IFormFile File)
//        {
//            var compressor = new LZW<string>();
//            byte[] bytes;
//            using (var memory = new MemoryStream())
//            {
//                await File.CopyToAsync(memory);


//                bytes = memory.ToArray();
//                List<byte> aux = bytes.OfType<byte>().ToList();

//            }
//            int i = 0;
       
//            var maxBits = new List<Byte>();
//            var largodiccionario = new List<Byte>();
//            var repeticiones = new List<Byte>();
//            var chardiccionario = new List<Byte>();
//            var mensajecomprimido= new List<Byte>();
//            var encoder = Encoding.GetEncoding(28591);
//            var lista = Singleton.Instance.CompList;
           
//            while (bytes[i] != 0)
//            {

//                maxBits.Add(bytes[i]);
//                i++;
             
//            }
//            i++;
//            string max = encoder.GetString(maxBits.ToArray());
//            int bitsmax = 0;
//            foreach (char c in max)
//            {
//                bitsmax = bitsmax + c;
//            }
//            while (bytes[i] != 0)
//            {
//                repeticiones.Add(bytes[i]);
//                i++;
//            }
//            string repe = encoder.GetString(repeticiones.ToArray());
//            int repetir = 0;
//            foreach (char c in repe)
//            {
//                repetir = repetir + c;
//            }
//            i++;
//            while (bytes[i] != 0)
//            {

//                 largodiccionario.Add(bytes[i]);
//                i++;

//            }
//            i++;
//            string tamañoDiccionario = encoder.GetString(largodiccionario.ToArray());
//            int largo = 0;
//            foreach (char c in tamañoDiccionario)
//            {
//                largo = largo + c;
//            }
//            int z = 0;
//            while (largo !=z )
//            {
               
//                 chardiccionario.Add( bytes[i]);
//                z++;
//                i++;
                
//            }

//            while(i != bytes.Length)
//            {
//                mensajecomprimido.Add(bytes[i]);
//                i++;
//            }

//            string NombreOriginal;
//            if (lista.Count == 0)
//            {
//                NombreOriginal = "default";
//            }
//            else
//            {
//                NombreOriginal = lista.ElementAt(lista.Count - 1).NombreOriginal;
//            }


//            string mensajcomprimido =  compressor.Decompress(bitsmax, repetir, chardiccionario, mensajecomprimido);
//            byte[] bytesFinales = encoder.GetBytes(mensajcomprimido);
//            return base.File(bytesFinales, "text/plain", NombreOriginal+ ".txt");

         
//        }

//>>>>>>> Stashed changes
        public string  message(byte[] aparaciones)
        {
            string mensaje = "";
            string currentletter = "";
            string currentthing = "";
            int frecuencia = 0;
            int i = 0;
            var encoder = Encoding.GetEncoding(28591);
            while  (aparaciones[i] != 0)
            {
                if  (i != 0)
                {
                    if (i %2 == 0)
                    {
                        currentletter = aparaciones[i].ToString();
                      currentthing = encoder.GetString(ConvertToByte(currentletter));
                    }
                    else
                    {
                        frecuencia = aparaciones[i];
                        for (int j = 0; j < frecuencia; j++)
                        {
                            mensaje += currentthing;
                        }
                    }
                }
                else
                {
                    currentletter = aparaciones[i].ToString();
                    currentthing = encoder.GetString(ConvertToByte(currentletter));
                }

               
                i++;
            }
            return mensaje;
        }
        //Algoritmo para convertir de string a binario
        public Byte[] ConvertToByte(string binary)
        {
            var list = new List<Byte>();

            list.Add(Convert.ToByte(binary));

            return list.ToArray();
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
        public string CalcularPorcentajeReducción(double factorDeCompresión)
        {
            double Porcentaje = Math.Round((1 - factorDeCompresión)*100);
            string resultado = Porcentaje + "%";
            return resultado;
        }

        //Algoritmo para combinar arrays de tipo byte
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
