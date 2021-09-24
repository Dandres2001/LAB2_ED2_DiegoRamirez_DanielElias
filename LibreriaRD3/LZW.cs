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

            list.Add(Convert.ToByte(binary));

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
            byte[] mensajefinal = Combine(maximosbits,separador, largoDiccionario,separador,  DiccionarioOriginal,mensajebytes);
            return mensajefinal;
        }
        public static string Decompress(List<int> compressed)
        {
            
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(i, ((char)i).ToString());

            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);

            foreach (int k in compressed)
            {
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];

                decompressed.Append(entry);

                
                dictionary.Add(dictionary.Count, w + entry[0]);

                w = entry;
            }

            return decompressed.ToString();
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
