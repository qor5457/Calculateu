using System.ComponentModel.DataAnnotations;
using System.Windows.Markup;

namespace CalibrationToolLibrary.Model
{
    /// <summary>
    /// n차 방정식 모델
    /// </summary>
    public class Equation_Model
    {
        public string Eqiation_Num { get; set; }
        public string Formula { get; set; }
        public string[] Value { get; set; }
        
        public string ValueTostring()
        {
            int index = 2;
            var strEquation = $"{Value[Value.Length-2]}x + {Value[Value.Length-1]}"; 
            
            for (int i = Value.Length-3 ; i >= 0; i--)
            {
                strEquation = $"{Value[i]}x^{index++} + {strEquation}";
            }

            strEquation = $"y = {strEquation}";

            return strEquation;
        }
    }
}