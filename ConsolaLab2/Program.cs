using System;
using System.Collections.Generic;
using LibreriaRD3;
//using static LibreriaRD3.Huffmann;

namespace ConsolaLab2
{
    class Program
    {
        static void Main(string[] args)
        {
            string Example = "bbeecita";
            var huffman = new Huffman<char>(Example);
            
            List<int> encoding = huffman.Encode(Example);
            int largo = encoding.Count;
            List<int> decodeascii = huffman.decodeascii(huffman.Encodeascii(encoding), largo);
            
            List<char> decoding = huffman.Decode(decodeascii);
            string linea = "";
            var outString = new string(decoding.ToArray());
             for (int i=0; decoding.Count >i; i++)
            {
                linea += decoding[i];

            }
             
            Console.WriteLine(outString == Example ? "Encoding/decoding worked" : "Encoding/Decoding failed");
            Console.WriteLine(linea);
            var chars = new HashSet<char>(Example);
            foreach (char c in chars)
            {
                encoding = huffman.Encode(c);
                decoding = huffman.Decode(encoding);
                Console.Write("{0}:  ", c);
              
                foreach (int bit in encoding)
                {
                    Console.Write("{0}", bit);
                }
              
                Console.WriteLine();
            }



            Console.ReadKey(); ;
            
        }
        
    }
}
