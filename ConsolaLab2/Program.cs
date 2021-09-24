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
            var lzw = new LZW();
            byte[] compressed = LZW.Compress("WABBAWABBAWABBAWABBA");
            Console.WriteLine(string.Join(", ", compressed));
            //string decompressed = LZW.Decompress(compressed);
            //Console.WriteLine(decompressed);
            Console.ReadKey();
           
            
        }
        
    }
}
