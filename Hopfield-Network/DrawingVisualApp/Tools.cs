using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DrawingVisualApp
{
    class Tools
    {
        public static bool IsEqual(int[] A, List<int> B)
        {
            int num = A.Length;
            int count = B.Count;
            if (num != count)
            {
                throw new Exception("Non-conformable matrices in MatrixAreEqual");
            }

            for (int i = 0; i < num; i++)
            {
                if (A[i] != B[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
