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
        [HttpPost("huffman/compress/{name}")]
        public FileResult Compress([FromRoute] string name,[FromForm] IFormFile File)
        {
            
            
            var reader = new StreamReader(File.OpenReadStream());
            string texto = reader.ReadToEnd();
            reader.Close();
            
            //COMPRIMIENDO TEXTO
            var huffman = new Huffman<char>(texto);
            var keys = huffman.appearancesDictionary.Keys;
            var values = huffman.appearancesDictionary.Values;
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
            var newCompression = new CompModel();
            string NombreOriginal = File.FileName;
            double bytesOriginal = Convert.ToDouble(File.Length);
            string NombreComprimido = ".huff";
            double factorDeCompresion = CalcularFactorCompresion(bytesOriginal, dataAsBytes.Length);
            double razonDeCompresion = CalcularRazonCompresion(bytesOriginal, dataAsBytes.Length);
            string PorcentajeReducion = CalcularPorcentajeReducción(razonDeCompresion);
            var url = Request.Path;
            var requestUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            string rutaYnombre = requestUrl + url + NombreComprimido; 
            newCompression.NombreOriginal = NombreOriginal;
            newCompression.NombreRutadeArchivo = rutaYnombre;
            newCompression.FactorCompresion = factorDeCompresion;
            newCompression.RazonCompresion = razonDeCompresion;
            newCompression.PorcentajeReduccion = PorcentajeReducion;

            Singleton.Instance.CompList.Add(newCompression);
            #endregion

            //ESCRIBIENDO ARCHIVO
            string text = bytearray + "\n" + dataAsBytes;
            byte[] nullarray = { default };


            return base.File(Combine(arrayLargo, nullarray, bytearray,nullarray,dataAsBytes), "compressedFile / huff", name + ".huff");
        }

        // PUT api/<CompressionsController>/5
        [HttpPost("huffman/decompress")]
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


            List<string> asciivalues = new List<string>();

            var encoder = Encoding.GetEncoding(28591);
            string currentletter = "";
            string  mensajeordenado =  message(apariciones);
            var huffman = new Huffman<char>(mensajeordenado);
            for (int k = 0; k< mensaje.Length; k++)
            {
                currentletter = mensaje[k].ToString();
               currentletter = encoder.GetString(ConvertToByte(currentletter));
                asciivalues.Add(currentletter);
                
                if (mensaje[k] == 0)
                {
                    break;
                }
            }

            List<int> decodeascii = huffman.decodeascii(asciivalues,byteTamaño[0]);
            List<char> decoding = huffman.Decode(decodeascii);
            var lista = Singleton.Instance.CompList;
            var stringFinal = new string(decoding.ToArray());
            byte[] bytesFinales = Encoding.GetEncoding(28591).GetBytes(stringFinal);
            string NombreOriginal;

            if (lista.Count == 0)
            {
                NombreOriginal = "default";
            }
            else
            {
                NombreOriginal = lista.ElementAt(lista.Count - 1).NombreOriginal;
            }

        
    
           
            return base.File(bytesFinales,"text/plain", NombreOriginal);
        }

        [HttpPost("lzw/compress/{name}")]
        public async Task<FileResult> LZWCompress([FromRoute] string name, [FromForm] IFormFile File)
        {
            var compressor = new LZW<string>();
           
            byte[] bytes;

            ORIGINALCONTENT = File.ContentType;
            ORIGINALFILENAME = File.FileName;
            using (var memory = new MemoryStream())
            {
                await File.CopyToAsync(memory);


                bytes = memory.ToArray();


            }
            byte[] archivoComprimido = compressor.Compress(bytes);

            //GUARDANDO COMPRESIÓN
            var newCompression = new CompModel();
            string NombreOriginal = File.FileName;
            double bytesOriginal = Convert.ToDouble(File.Length);
            string NombreComprimido = ".lzw";
            double factorDeCompresion = CalcularFactorCompresion(bytesOriginal, archivoComprimido.Length);
            double razonDeCompresion = CalcularRazonCompresion(bytesOriginal, archivoComprimido.Length);
            string PorcentajeReducion = CalcularPorcentajeReducción(razonDeCompresion);
            var url = Request.Path;
            var requestUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            string rutaYnombre = requestUrl + url + NombreComprimido;
            newCompression.NombreOriginal = NombreOriginal;
            newCompression.NombreRutadeArchivo = rutaYnombre;
            newCompression.FactorCompresion = factorDeCompresion;
            newCompression.RazonCompresion = razonDeCompresion;
            newCompression.PorcentajeReduccion = PorcentajeReducion;

            return base.File(archivoComprimido, "compressedFile / lzw", name + ".lzw");
        }

        [HttpPost("lzw/decompress")]
        public async Task<FileResult> LZWDecompress([FromForm] IFormFile File)
        {
            var compressor = new LZW<string>();
            byte[] bytes;
            using (var memory = new MemoryStream())
            {
                await File.CopyToAsync(memory);


                bytes = memory.ToArray();


            }


            var largodiccionario = new List<Byte>();
            var chardiccionario = new List<Byte>();
            var mensajecomprimido = new List<Byte>();
       
            var lista = Singleton.Instance.CompList;
            int i = 0;
          
         
          
       
            while (bytes[i] != 0)
            {

                largodiccionario.Add(bytes[i]);
                i++;

            }
            i++;
           int  largo = 0;
            foreach (byte c in largodiccionario)
            {
                largo += Convert.ToInt32(c);
            }
            int z = 0;
            while (largo != z)
            {

                chardiccionario.Add(bytes[i]);
                z++;
                i++;

            }

            while (i != bytes.Length)
            {
                mensajecomprimido.Add(bytes[i]);
                i++;
            }

            string NombreOriginal;
            if (lista.Count == 0)
            {
                NombreOriginal = "default";
            }
            else
            {
                NombreOriginal = lista.ElementAt(lista.Count - 1).NombreOriginal;
            }


            byte[] mensajcomprimido = compressor.Decompress( chardiccionario.ToArray(), mensajecomprimido.ToArray());

            return base.File(mensajcomprimido, ORIGINALCONTENT, ORIGINALFILENAME);

         
        }

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
