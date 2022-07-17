using System;
using System.Collections.Generic;
using CalibrationToolLibrary.Model;

namespace CalibrationToolLibrary.Equation
{
    public abstract class EquationFactory
    {
        public abstract void Savefile(string filepath, List<Equation_Model> em);
    }
}