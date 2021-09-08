using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaRD3
{
    public interface ICompressor<T>
    {
        public void Encode(T value, List<int> encoding);

        public List<T> Decode(List<int> bitString);
    }
}
