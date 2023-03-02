using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace RestoreMessage
{
    internal class Program
    {
        private static List<string> listData = new List<string>();
        private static List<string> listReplacement = new List<string>();
        private static List<string> listDigitalReplacement = new List<string>();
        private static int count;
        private static int index;

        static void Main(string[] args)
        {
            ReadFile();
            while (listReplacement.Count != 0)
            {
                count = 0;
                index = 0;
                var dublicatesReplacement = listReplacement.Select((t, i) => new { Index = i, Text = t }).Where(g => g.Text == listReplacement[count]).Select(w => w.Index).ToList();
                var copyDublicatesReplacement = dublicatesReplacement.Select(g => g).ToList();
                for (int j = 0; j < listData.Count; j++)
                {
                    if (int.TryParse(listReplacement[count], out _))
                    {
                        IsDigitalReplacement();
                        continue;
                    }
                    else if (listData[j].Contains(listReplacement[count]) && listReplacement[count + 1] != "null")
                        IsNull(dublicatesReplacement, j);
                    else if (listData[j].Contains(listReplacement[count]) && listReplacement[count + 1] == "null")
                        IsNotNull(dublicatesReplacement, j);

                    if (j == listData.Count - 1)
                        for (int k = copyDublicatesReplacement.Count - 1; k >= 0; k--)
                            listReplacement.RemoveRange(copyDublicatesReplacement[k], 2);
                }
            }
            ReplaceDigitalReplacement();
            for (int i = listData.Count - 1; i >= 0; i--)
                if (!listData[i].Any(w => char.IsLetter(w)) && listData[i] != "]" && listData[i] != "[")
                    listData.RemoveAt(i);
            WriteFile();
            Console.WriteLine("Файл \"result.json\" был успешно создан.\nПуть: Breaking\\bin\\Debug\\result");
            Console.ReadLine();
        }
        static void IsDigitalReplacement()
        {
            listDigitalReplacement.Add(listReplacement[count]);
            listDigitalReplacement.Add(listReplacement[count + 1]);
            listReplacement.RemoveRange(0, 2);
        }
        static void IsNull(List<int> dublicates, int j)
        {
            index = listData[j].IndexOf(listReplacement[count]);
            listData[j] = listData[j].Remove(index, listReplacement[count].Length);
            listData[j] = listData[j].Insert(index, listReplacement[count + 1]);
            RemoveReplacement(dublicates);
        }
        static void IsNotNull(List<int> dublicates, int j)
        {
            index = listData[j].IndexOf(listReplacement[count]);
            listData[j] = listData[j].Remove(index, listReplacement[count].Length);
            RemoveReplacement(dublicates);
        }
        static void RemoveReplacement(List<int> dublicates)
        {
            if (dublicates.Count > 1)
            {
                count = dublicates.Skip(1).Take(1).First();
                dublicates.RemoveAt(0);
            }
        }
        static void ReplaceDigitalReplacement()
        {
            for (int i = 0; i < listData.Count && listDigitalReplacement.Count != 0; i++)
            {
                listData[i] = listData[i].Replace(listDigitalReplacement[count], listDigitalReplacement[count + 1]);
                if (i == listData.Count - 1)
                    listDigitalReplacement.RemoveRange(0, 2);
            }
        }
        static void ReadFile()
        {
            dynamic arrayReplacement = JsonConvert.DeserializeObject(File.ReadAllText("replacement.json"));
            foreach (var item in arrayReplacement)
            {
                if (item.source == null)
                    item.source = "null";
                listReplacement.Add(Convert.ToString(item.replacement));
                listReplacement.Add(Convert.ToString(item.source));
            }
            try
            {
                using (var webClient = new WebClient())
                using (var reader = webClient.OpenRead("https://raw.githubusercontent.com/thewhitesoft/student-2022-assignment/main/data.json"))
                using (var readStream = new StreamReader(reader))
                    while (!readStream.EndOfStream)
                        listData.Add(readStream.ReadLine());
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
        static void WriteFile()
        {
            var js = JsonConvert.DeserializeObject(string.Join("", listData));
            using (StreamWriter st = new StreamWriter("result.json", false))
                st.WriteLine(js);
        }
    }
}
