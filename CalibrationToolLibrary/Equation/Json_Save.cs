using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using CalibrationToolLibrary.Model;

namespace CalibrationToolLibrary.Equation
{



    public class Json_Save : EquationFactory
    {
        public List<Equation_Model> Openfile(string filepath)
        {
            var em = new List<Equation_Model>();
            using (var fs = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                var readValue = Encoding.Default.GetString(bytes).TrimEnd('\0');
                em = JsonSerializer.Deserialize<List<Equation_Model>>(readValue);
            }
            return em;
        }
        public override void Savefile(string filepath,  List<Equation_Model> Em)
        {
            string jsonString = JsonSerializer.Serialize(Em);
            using ( FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(jsonString);
                sw.Close();
            }
        }
    }
}