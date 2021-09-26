using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace LibreriaRD3
{
    public class LZW
    {
        public static Byte[] GetBytesFromBinaryString(string binary)
        {
            var list = new List<Byte>();

            list.Add(Convert.ToByte(binary, 2));

            return list.ToArray();
        }
        public static Byte[] ConvertToByte(string binary)
        {

            var list = new List<Byte>();
            int number = Convert.ToInt32(binary);

            while (number > 255)
            {
                list.Add(Convert.ToByte(255));
                number = number - 255;
            }
    
                list.Add(Convert.ToByte(number));
            
           

            return list.ToArray();
        }
        public static string Encodeascii(List<string> values)
        {
            while (values.Count % 8 != 0)
            {
                values.Add("0");

            }
            var returvalue = new List<string>();
            string bytearray = "";
            string completeline = "";
            while (values != null)
            {

                for (int i = 0; i < 8; i++)
                {
                    if (values.Count > i)
                    {
                        bytearray += values[i];
                    }

                }
                if (values.Count > 8)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        values.RemoveAt(0);
                    }
                }
                else
                {
                    values = null;

                }

                var encEncoder = System.Text.Encoding.GetEncoding(28591);

                string str = encEncoder.GetString(GetBytesFromBinaryString(bytearray));
                bytearray = "";

                 completeline += str;

                returvalue.Add(str);

            }

            return completeline;
        }
        public static byte[] Compress(string uncompressed)
        {
            
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            string current = "";
            string currencharacter = "";
            string result = "";
            string finalresult = "";
            string aux = "";
            var encEncoder = System.Text.Encoding.GetEncoding(28591);
            List<int> compressed = new List<int>();
            int i = 0;
            byte[] separador = {default};

            foreach (char z in uncompressed)
            {
                if (!dictionary.ContainsKey(z.ToString()))
                {
                    dictionary.Add(z.ToString(), dictionary.Count + 1);
                    currencharacter += z.ToString();

                    i++;
                }

            }
            string str = encEncoder.GetString(ConvertToByte(Convert.ToString(dictionary.Count)));
    
            byte[] largoDiccionario = encEncoder.GetBytes(str);
            byte[] DiccionarioOriginal = encEncoder.GetBytes(currencharacter);

            foreach (char c in uncompressed)
            {

                string chain = current + c;
                if (dictionary.ContainsKey(chain))
                {
                    current = chain;
                }
                else
                {


                    dictionary.Add(chain, dictionary.Count + 1);
                    compressed.Add(dictionary[current]);
                    current = c.ToString();


                }
            }


            if (!string.IsNullOrEmpty(current))
            {
                compressed.Add(dictionary[current]);
            }

            int maxbits = Convert.ToInt32(Math.Log2(dictionary.Count + 1));
            str = encEncoder.GetString(ConvertToByte(Convert.ToString(maxbits)));
            
            byte[] maximosbits = encEncoder.GetBytes(str);
            str = encEncoder.GetString(ConvertToByte(Convert.ToString(compressed.Count)));
            byte[] repeticiones = encEncoder.GetBytes(str);
            for (int k = 0; k < compressed.Count; k++)
            {

                result = Convert.ToString(Convert.ToInt32(compressed[k].ToString(), 10), 2);
                for (int m=0; m < (maxbits - result.Length);m++){

                    aux += "0";

                }
               
                finalresult += aux+ result;
                aux = "";
            }
           
            List<string> binary = new List<string>();
            foreach (char l in finalresult)
            {
                binary.Add(l.ToString());
            }
      
            byte[] mensajebytes = encEncoder.GetBytes(Encodeascii(binary));
            byte[] mensajefinal = Combine(maximosbits,separador,repeticiones, separador,largoDiccionario,separador, DiccionarioOriginal,mensajebytes);
            return mensajefinal;
        }
        public static string Decompress(int maxbits, int repeticiones, List<byte> diccionario, List<byte> mensaje)
        {
            
            Dictionary<int,string> dictionary = new Dictionary<int, string>();
            var encoder = Encoding.GetEncoding(28591);
            string diccionariostring = encoder.GetString(diccionario.ToArray());
            foreach(char c in diccionariostring)
            {
                dictionary.Add( dictionary.Count + 1, c.ToString());
            }

            List<int> numerosmensaje=  decodeascii(mensaje, maxbits,repeticiones);

            string w = dictionary[numerosmensaje[0]];
            numerosmensaje.RemoveAt(0);
            string decompressed =  w;

            foreach (int k in numerosmensaje)
            {
                string entry = "";
                if (dictionary.ContainsKey(k))
                {
                    entry = dictionary[k];
                }
                else if (k == dictionary.Count)
                {
                    entry = w + w[0];
                }
                decompressed+= entry;


                dictionary.Add(dictionary.Count+1, w + entry[0]);

                w = entry;
            }

            return decompressed;

         
        }
        public static List<int> decodeascii(List<byte> value, int  maxbits, int repeticiones)
        {
            var returvalue = new List<int>();
            byte[] bytearray = value.ToArray();
            List<int> cadenanumeros = new List<int>();
            string byte1= "";
            for (int i = 0; i <bytearray.Length; i++)
            {
                for (int j=0; j<8; j++)
                {
                    returvalue.Add((bytearray[i] & 0x80) >0 ? 1:0);
                    bytearray[i] <<= 1;
                }
            }
            while (returvalue != null)
            {

                for (int i = 0; i < maxbits; i++)
                {
                    if (returvalue.Count > i)
                    {
                        byte1 += returvalue[i];
                    }

                }
                if (returvalue.Count > maxbits)
                {
                    for (int i = 0; i < maxbits; i++)
                    {
                      returvalue.RemoveAt(0);
                    }
                }
                else
                {
                   returvalue = null;

                }

                var encEncoder = System.Text.Encoding.GetEncoding(28591);

                //string str = encEncoder.GetString(GetBytesFromBinaryString(bytearray));
               string  result = Convert.ToString(Convert.ToInt32(byte1, 2), 10);
                byte1 = "";
                if (cadenanumeros.Count < repeticiones) {
                    cadenanumeros.Add(Convert.ToInt32(result));
                }
        

            }
          
       

            return cadenanumeros;
        }
     




          
        
    




    
        public static byte[] Combine(params byte[][] arrays)
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
