using System.IO;

namespace NN
{
    class Files
    {
        public static void FileRead(Map map, string path)
        {
            string[] readText = File.ReadAllLines(path);

            int z = 0;
            for (int i = 0; i < map.rows; i++)
            {
                for (int j = 0; j < map.cols; j++)
                {
                    if (readText[z++] == "1")
                    {
                        map[i, j] = 1;
                    }
                    else
                    {
                        map[i, j] = 0;
                    }
                }
            }
        }

        public static void SaveToFile(Map map, string path)
        {
            int z = 0;
            string[] arr = new string[map.rows * map.cols];
            for (int i = 0; i < map.rows; i++)
                for (int j = 0; j < map.cols; j++)
                {
                    arr[z++] = (map[i, j] == 1) ? "1" : "0";
                }
            
            if (!File.Exists(path)) File.Create(path).Close();

            File.WriteAllLines(path, arr);
        }
    }
}
