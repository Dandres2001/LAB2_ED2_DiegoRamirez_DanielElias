using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibreriaRD3
{
    public class Huffman
    {
       

        public byte[] Compress(byte[] originalData)
        {
            Dictionary<byte, int> diccionarioFrecuencias = ObtenerFrecuencias(originalData);
            List<Node> bytesHojas = obtenerHojas(diccionarioFrecuencias);
            minHeap<Node> minHeap = llenarHeap(bytesHojas);
            Node root = crearArbol(minHeap);
            string nulo;
            Dictionary<byte, string> diccionarioBytes = llenarDiccionarioPrefijos(root, out nulo);

            MemoryStream reader = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(reader);
            byte[] heapBytes = heapToBytes(root);
            writer.Write(heapBytes);

            byte[] compressedData = ObtenerComprimido(originalData, diccionarioBytes, nulo);
            writer.Write(compressedData);
            writer.Flush();
            return reader.ToArray();
        }

        public byte[] Decompress(byte[] fullBytes)
        {
            byte[] lenghtBytes = { fullBytes[0], fullBytes[1], fullBytes[2], fullBytes[3] };

            int lenght = BitConverter.ToInt32(lenghtBytes, 0);
            List<HuffmanNode> huffmanNode = new List<HuffmanNode>();

            #region
            for (int i = 0; i < lenght; i++)
            {
                byte[] letterBytes = { fullBytes[4 + i * 9] };
                byte[] leftBytes = { fullBytes[4 + i * 9 + 1], fullBytes[4 + i * 9 + 2], fullBytes[4 + i * 9 + 3], fullBytes[4 + i * 9 + 4] };
                byte[] rightBytes = { fullBytes[4 + i * 9 + 5], fullBytes[4 + i * 9 + 6], fullBytes[4 + i * 9 + 7], fullBytes[4 + i * 9 + 8] };

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(letterBytes);
                    Array.Reverse(leftBytes);
                    Array.Reverse(rightBytes);
                }
                byte letter = letterBytes[0];
                int left = BitConverter.ToInt32(leftBytes, 0);
                int right = BitConverter.ToInt32(rightBytes, 0);

                huffmanNode.Add(new HuffmanNode(letter, left, right));
            }
            #endregion

            List<byte> datosDecompress = new List<byte>();

            int actual = 0;
            byte[] bytesComprimidos = new byte[fullBytes.Length - 4 - lenght * 9];
            for (int i = 0; i < bytesComprimidos.Length; i++)
            {
                bytesComprimidos[i] = fullBytes[4 + lenght * 9 + i];
            }

            BitArray arraybit = new BitArray(bytesComprimidos);
            for (int i = 0; i < arraybit.Length; i++)
            {
                if (arraybit[i])
                {
                    actual = huffmanNode[actual].hijoDer;
                    if (huffmanNode[actual].hijoIzq == huffmanNode[actual].hijoDer && huffmanNode[actual].hijoIzq == -1)
                    {
                        datosDecompress.Add(huffmanNode[actual]._Byte);
                        actual = 0;
                    }
                    else if (huffmanNode[actual].hijoIzq == huffmanNode[actual].hijoDer && huffmanNode[actual].hijoIzq == -2)
                    {
                        i = arraybit.Length - 1;
                    }
                }
                else
                {
                    actual = huffmanNode[actual].hijoIzq;
                    if (huffmanNode[actual].hijoIzq == huffmanNode[actual].hijoDer && huffmanNode[actual].hijoIzq == -1)
                    {
                        datosDecompress.Add(huffmanNode[actual]._Byte);
                        actual = 0;
                    }
                    else if (huffmanNode[actual].hijoIzq == huffmanNode[actual].hijoDer && huffmanNode[actual].hijoIzq == -2)
                    {
                        i = arraybit.Length - 1;
                    }
                }
            }
            return datosDecompress.ToArray();
        }
        private  byte[] ObtenerComprimido(byte[] data, Dictionary<byte, string> diccionarioPrefijos, string nulo)
        {
            List<byte> finalBytes = new List<byte>();
            List<bool> segmentos = new List<bool>();
            foreach (byte b in data)
            {
                string codigo = diccionarioPrefijos[b];
                foreach (char c in codigo)
                {
                    segmentos.Add(c == '1' ? true : false);
                    if (segmentos.Count == 8)
                    {
                        int boolToInt = Convert.ToByte(segmentos[0]) * 1 + Convert.ToByte(segmentos[1]) * 2 + Convert.ToByte(segmentos[2]) * 4 + Convert.ToByte(segmentos[3]) * 8
                            + Convert.ToByte(segmentos[4]) * 16 + Convert.ToByte(segmentos[5]) * 32 + Convert.ToByte(segmentos[6]) * 64 + Convert.ToByte(segmentos[7]) * 128;
                        finalBytes.Add((byte)boolToInt);
                        segmentos.Clear();
                    }
                }

            }
            foreach (char c in nulo)
            {
                segmentos.Add(c == '1' ? true : false);
                if (segmentos.Count == 8)
                {
                    int boolToInt = Convert.ToByte(segmentos[0]) * 1 + Convert.ToByte(segmentos[1]) * 2 + Convert.ToByte(segmentos[2]) * 4 + Convert.ToByte(segmentos[3]) * 8
                        + Convert.ToByte(segmentos[4]) * 16 + Convert.ToByte(segmentos[5]) * 32 + Convert.ToByte(segmentos[6]) * 64 + Convert.ToByte(segmentos[7]) * 128;
                    finalBytes.Add((byte)boolToInt);
                    segmentos.Clear();
                }
            }

            int ultimoByte = 0;
            int multiplicador = 1;
            for (int i = 0; i < segmentos.Count; i++)
            {
                if (segmentos[i])
                    ultimoByte += multiplicador;
                multiplicador *= 2;
            }

            finalBytes.Add((byte)ultimoByte);

            return finalBytes.ToArray();
        }

        private static Dictionary<byte, int> ObtenerFrecuencias(byte[] originalData)
        {
            Dictionary<byte, int> listaFrecuencias = new Dictionary<byte, int>();
            foreach (byte a in originalData)
            {
                if (listaFrecuencias.ContainsKey(a))
                    listaFrecuencias[a] += 1;
                else
                    listaFrecuencias.Add(a, 1);
            }
            return listaFrecuencias;
        }
        private static List<Node> obtenerHojas(Dictionary<byte, int> diccionarioFrecuencias)
        {
            List<Node> letterLeaves = new List<Node>();
            foreach (KeyValuePair<byte, int> a in diccionarioFrecuencias)
            {
                Node n = new Node(a.Key, a.Value, null, null, true);
                letterLeaves.Add(n);
            }
            return letterLeaves;
        }
        private static minHeap<Node> llenarHeap(List<Node> nodes)
        {
            minHeap<Node> priorityQueue = new minHeap<Node>();
            foreach (Node n in nodes)
            {
                priorityQueue.push(n);
            }
            return priorityQueue;
        }
        private static Node crearArbol(minHeap<Node> heap)
        {
            heap.push(new Node(Byte.MinValue, 0, null, null, true));
            while (heap.Cont != 1)
            {
                Node primero = heap.Pop();
                Node sig = heap.Pop();
                Node newNode = new Node(Byte.MinValue, primero.frecuencias + sig.frecuencias, primero, sig, false);
                heap.push(newNode);
            }
            Node root = heap.Pop();
            return root;
        }

        private static Dictionary<byte, string> llenarDiccionarioPrefijos(Node root, out string nulo)
        {
         
            List<string> listaString = new List<string>();
            Dictionary<byte, string> diccionario = new Dictionary<byte, string>();
            CrearPrefijo(diccionario, root, "", listaString);
            nulo = listaString[0];
            return diccionario;
        }
        private static void CrearPrefijo(Dictionary<byte, string> diccionario, Node actual, string codigo, List<string> listOfOneString)
        {
            if (actual == null)
                return;

            string izquierda = codigo + "0";
            string derecha = codigo + "1";
            CrearPrefijo(diccionario, actual.HijoIzq, izquierda, listOfOneString);
            if (actual.isByte)
            {
                if (actual.frecuencias != 0) // this is needed, so that the '\0' terminating node wouldn't be mapped
                    diccionario.Add(actual._Byte, codigo);
                else
                    listOfOneString.Add(codigo);
            }
            CrearPrefijo(diccionario, actual.HijoDer, derecha, listOfOneString);
        }
        private static byte[] heapToBytes(Node root)
        {
            List<byte> listaBytes = new List<byte>();
            HuffmanNode[] arrayNodos = ArbolHuffman(root);
            byte[] lengthBytes = BitConverter.GetBytes(arrayNodos.Length);
            listaBytes.AddRange(lengthBytes);
            foreach (HuffmanNode n in arrayNodos)
            {
                byte letterByte = n._Byte;
                byte[] leftBytes = BitConverter.GetBytes(n.hijoIzq);
                byte[] rightBytes = BitConverter.GetBytes(n.hijoDer);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(leftBytes);
                    Array.Reverse(rightBytes);
                }
                listaBytes.Add(letterByte); listaBytes.AddRange(leftBytes); listaBytes.AddRange(rightBytes);
            }

            return listaBytes.ToArray();
        }
        private static HuffmanNode[] ArbolHuffman(Node root)
        {
            List<Node> listaNodos = new List<Node>();
            LlenarEspacios(root, listaNodos);
            for (int i = 0; i < listaNodos.Count; i++)
                listaNodos[i].id = i;
            HuffmanNode[] result = new HuffmanNode[listaNodos.Count];
            for (int i = 0; i < result.Length; i++)
            {
                if (listaNodos[i].frecuencias == 0)
                {
                    int izquierda = -2;
                    int derecha = -2;
                    result[i] = new HuffmanNode(listaNodos[i]._Byte, izquierda, derecha);
                }
                else
                {
                    int left = (listaNodos[i].HijoIzq == null) ? -1 : listaNodos[i].HijoIzq.id;
                    int right = (listaNodos[i].HijoDer == null) ? -1 : listaNodos[i].HijoDer.id;
                    result[i] = new HuffmanNode(listaNodos[i]._Byte, left, right);
                }

            }

            return result;
        }
        private static void LlenarEspacios(Node currentNode, List<Node> nodes)
        {
            if (currentNode == null)
                return;
            nodes.Add(currentNode);
            LlenarEspacios(currentNode.HijoIzq, nodes);
            LlenarEspacios(currentNode.HijoDer, nodes);
        }
    }
}




