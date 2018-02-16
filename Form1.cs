using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ResourceRenamer
{
    public partial class Form1 : Form
    {
        static char[] lineseparator = { ' ', '\t' };
        static string[] separator = { @"\\" };

        public Form1()
        {
            InitializeComponent();

            textBox2.AppendText("Przed użyciem dodaj do projektu pliki png z domyslnie wygenerowanymi nazwami" + Environment.NewLine);
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "resource files (*.rc)|*.rc";

            var result = ofd.ShowDialog();
            if(result == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string rcDir = textBox1.Text;
            string resDir = Path.GetDirectoryName(textBox1.Text) + "\\resource.h";
            
            List<string> rcFile = new List<string>();
            List<string> resFile = new List<string>();

            ReadFileToList(rcFile, rcDir);
            ReadFileToList(resFile, resDir);

            for(int i = 0; i < rcFile.Count; i++)
            {
                string pngIdString = rcFile[i];

                if (pngIdString.StartsWith("IDB_PNG_")) continue;
                if (pngIdString.StartsWith("IDB_PNG"))
                {
                    var tempString = pngIdString.Split(lineseparator, StringSplitOptions.RemoveEmptyEntries);

                    if (tempString[1] != "PNG" & tempString.Count() != 3) continue;

                    string fileNameString = tempString[2].Trim('"');
                    string idNameString = fileNameString.Split(separator, StringSplitOptions.RemoveEmptyEntries).Last().ToUpperInvariant();

                    if (idNameString.EndsWith(".PNG"))
                    {
                        idNameString = "IDB_PNG_" + idNameString.Substring(0, idNameString.Count() - 4);
                        rcFile[i] = rcFile[i].Replace(tempString[0], idNameString);

                        for (int ii = 0; ii < resFile.Count; ii++)
                        {
                            if (resFile[ii].StartsWith($"#define {tempString[0]}"))
                            {
                                resFile[ii] = resFile[ii].Replace($"#define {tempString[0]}", $"#define {idNameString}");
                                break;
                            }
                        }

                        textBox2.AppendText($"{tempString[0]} --> {idNameString}" + Environment.NewLine);
                    }
                    
                }
            }

            WriteListToFile(rcFile, rcDir);
            WriteListToFile(resFile, resDir);
        }

        private void ReadFileToList(List<string> list, string filename)
        {
            list.Clear();

            string lineToWrite = null;
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    lineToWrite = reader.ReadLine();
                    list.Add(lineToWrite);
                }
            }
        }

        private void WriteListToFile(List<string> list, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename, false))
            {
                foreach (string line in list)
                {
                    writer.WriteLine(line);
                }
            }
        }

    }
}
