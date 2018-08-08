using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COThermoSocket.Core;

namespace COThermoSocket.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            COThermo co = COThermo.GetCOThermo();

            Console.WriteLine("------------ Property Package Managers------------------");
            foreach (var propPkgMgr in co.PropPkgMgrs)
            {
                Console.WriteLine("------------ Property Package Manager Information------------------");
                Console.WriteLine("Name: " + propPkgMgr.Name);
                Console.WriteLine("CapeVersion: " + propPkgMgr.CapeVersion);
                Console.WriteLine("VenderURL: " + propPkgMgr.VenderURL);
                Console.WriteLine("Description: " + propPkgMgr.Description);
                Console.WriteLine("ComponentVersion: " + propPkgMgr.ComponentVersion);
                Console.WriteLine("ProgID: " + propPkgMgr.ProgID);
            }

            co.PropPkgMgr = co.PropPkgMgrs[0];

            Console.WriteLine("------------ Property Packages------------------");
            foreach (var pkg in co.PropPkgNames)
            {
                Console.WriteLine(pkg);
            }

            co.PropPkgName = co.PropPkgNames[3];

            var mo = co.MaterialObject;

            #region ICapeThermoCompounds
            Console.WriteLine("------------ GetCompoundList------------------");
            Console.WriteLine("{0,-10}{1,-10}{2,-10}{3,-10}{4,-10}{5,-10}", "compid", "formulate", "name", "boiltemps", "molwts", "casnos");
            object compids, formulae, names, boilTemps, molwts, casnos;
            compids = formulae = names = boilTemps = molwts = casnos = null;
            mo.GetCompoundList(ref compids, ref formulae, ref names, ref boilTemps, ref molwts, ref casnos);
            for (int i = 0; i < (compids as string[]).Length; i++)
            {
                Console.WriteLine("{0,-10}{1,-10}{2,-10}{3,-10}{4,-10}{5,-10}", (compids as string[])[i], (formulae as string[])[i], (names as string[])[i], (boilTemps as double[])[i], (molwts as double[])[i], (casnos as string[])[i]);

            }
            #endregion
            var props = mo.GetConstPropList();
            Console.WriteLine("------------ GetConstPropList------------------");
            foreach (var prop in props)
            {
                Console.WriteLine(prop);
            }

            //foreach (var phase in co.PhaseLabels)
            //{
            //    Console.WriteLine(phase);
            //}


            //Console.WriteLine("------------ PureProperty------------------");
            //foreach (var prop in Enum.GetValues(typeof(Properties)))
            //{
            //    double[] result = null;
            //    if (((Properties)prop).ToPropertyCalc().PropType.Equals("CompoundConstant"))
            //    {
            //        result = co.PureProperty(co.CompIDs, (Properties)prop);
            //    }

            //    else if (((Properties)prop).ToPropertyCalc().PropType.Equals("TDependentProperty"))
            //    {
            //        result = co.PureProperty(co.CompIDs, (Properties)prop, temp: 200 + 273.15);
            //    }
            //    else if (((Properties)prop).ToPropertyCalc().PropType.Equals("SinglePhaseProp"))
            //    {
            //        result = co.PureProperty(co.CompIDs, (Properties)prop, 200 + 273.15, 101.325 * 1000.0, "Vapor");
            //    }
            //    Console.Write("{0,-20}", ((Properties)prop).ToPropertyCalc().PropName);
            //    if (result != null)
            //    {
            //        foreach (var val in result)
            //        {
            //            Console.Write("{0,-20:0.000}", val);
            //        }
            //    }
            //    Console.WriteLine();
            //}
            //Console.WriteLine(co.Test());


            #region Exposed Property Methods

            Console.WriteLine("------------ GetMolWeights------------------");
            foreach (var mw in mo.GetMolWeights())
            {
                Console.WriteLine(mw);
            }

            Console.WriteLine("------------ GetMolWeight------------------");
            Console.WriteLine(mo.GetMolWeight(new[] { 0.2, 0.2, 0.2, 0.2, 0.2 }));

            Console.WriteLine("------------ GetMolarEnthalpy------------------");
            Console.WriteLine(mo.MixtureProperty(30 + 273.15, 101325.0, new[] { 0.2, 0.2, 0.2, 0.2, 0.2 }, "density", Phases.Vapor));


            mo.Flash(FlashTypes.TP, 30 + 273.15, 101325.0, new[] { 0.2, 0.2, 0.2, 0.2, 0.2 });

            Console.WriteLine(mo.Vf);
            #endregion



            Console.ReadKey();
        }


        public bool ValidResults(double[] baseline, double[] result)
        {
            if (baseline.Length != result.Length) return false;
            for (int i = 0; i < baseline.Length; i++)
            {
                if (Math.Abs(baseline[i] - result[i]) > 1e-6)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
