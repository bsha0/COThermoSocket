using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CAPEOPEN110;
namespace COThermoSocket.Core
{
    [ComVisible(true)]
    [Guid("8B52ACAE-0D91-4DF4-AF5E-371E844BC3BB")]

    public class MaterialObjet
        //CAPE-OPEN 1.1
        : ICapeThermoMaterial, ICapeThermoCompounds, ICapeThermoPhases, ICapeThermoUniversalConstant, ICapeThermoPropertyRoutine, ICapeThermoEquilibriumRoutine, ICapeThermoMaterialContext
    {
        private dynamic _pp;
        public MaterialObjet(object pp)
        {
            _pp = pp;
            SetMaterial(this);
        }

        // storage values:
        // 1. phase.property.basis ——> single phase property value
        // 3. phaselabel           ——> phasestatus
        private Dictionary<string, object> _propVals = new Dictionary<string, object>();


        #region ICapeThermoMaterial not implemented
        public void ClearAllProps()
        {
            _propVals.Clear();
        }

        public void CopyFromMaterial(ref object source)
        {
            throw new NotImplementedException();
        }

        public dynamic CreateMaterial()
        {
            throw new NotImplementedException();
        }

        public void GetOverallProp(string property, string basis, ref object results)
        {
            if (basis == null) basis = Bases.Undefined.ToString();
            string key = string.Join(".", Phases.Overall.ToString().ToLower(), property.ToLower(), basis.ToLower());
            results = _propVals[key];
        }

        public void GetOverallTPFraction(ref double temperature, ref double pressure, ref object composition)
        {
            GetTPFraction(Phases.Overall.ToString(), ref temperature, ref pressure, ref composition);
        }

        public void GetPresentPhases(ref object phaseLabels, ref object phaseStatus)
        {
            string[] phasenames = Enum.GetNames(typeof(Phases));
            phaseLabels = phasenames;
            eCapePhaseStatus[] stats = new eCapePhaseStatus[phasenames.Length];
            for (int i = 0; i < phasenames.Length; i++)
            {

                if (_propVals.ContainsKey(phasenames[i]))
                {
                    stats[i] = (eCapePhaseStatus)_propVals[phasenames[i]];
                }
                else
                {
                    stats[i] = eCapePhaseStatus.CAPE_UNKNOWNPHASESTATUS;
                    _propVals.Add(phasenames[i], stats[i]);
                }
            }
            phaseStatus = stats;
        }

        public void GetSinglePhaseProp(string property, string phaseLabel, string basis, ref object results)
        {
            string key = string.Join(".", phaseLabel.ToLower(), property.ToLower(), basis.ToLower());
            results = _propVals[key];
        }

        public void GetTPFraction(string phaseLabel, ref double temperature, ref double pressure, ref object composition)
        {
            object vals = null;
            GetSinglePhaseProp("temperature", phaseLabel, Bases.Undefined.ToString(), ref vals);
            temperature = (vals as double[])[0];

            vals = null;
            GetSinglePhaseProp("pressure", phaseLabel, Bases.Undefined.ToString(), ref vals);
            pressure = (vals as double[])[0];

            vals = null;
            GetSinglePhaseProp("fraction", phaseLabel, Bases.Mole.ToString(), ref vals);
            composition = vals;
        }

        public void GetTwoPhaseProp(string property, object phaseLabels, string basis, ref object results)
        {
            throw new NotImplementedException();
        }

        public void SetOverallProp(string property, string basis, object values)
        {
            string key = string.Join(".", Phases.Overall.ToString().ToLower(), property.ToLower(), basis.ToLower());
            if (_propVals.ContainsKey(key))
            {
                _propVals[key] = values;
            }
            else
            {
                _propVals.Add(key, values);
            }
        }

        public void SetPresentPhases(object phaseLabels, object phaseStatus)
        {
            var phasenames = phaseLabels as string[];
            var stats = phaseStatus as eCapePhaseStatus[];

            for (int i = 0; i < phasenames.Length; i++)
            {
                string key = phasenames[i];
                if (_propVals.ContainsKey(key))
                {
                    _propVals[key] = stats[i];
                }
                else
                {
                    _propVals.Add(key, stats[i]);
                }
            }
        }

        public void SetSinglePhaseProp(string property, string phaseLabel, string basis, object values)
        {
            if (basis == null) basis = Bases.Undefined.ToString();
            string key = string.Join(".", phaseLabel.ToLower(), property.ToLower(), basis.ToLower());
            if (_propVals.ContainsKey(key))
            {
                _propVals[key] = values;
            }
            else
            {
                _propVals.Add(key, values);
            }
        }

        public void SetTwoPhaseProp(string property, object phaseLabels, string basis, object values)
        {
            //string key = string.Join(".", phaseLabel.ToLower(), property.ToLower(), basis.ToLower());
            //if (_propVals.ContainsKey(key))
            //{
            //    _propVals[key] = values;
            //}
            //else
            //{
            //    _propVals.Add(key, values);
            //}
        }
        #endregion

        #region ICapeThermoCompounds

        /// <summary>
        /// Returns the values of constant Physical Properties for the specified Compounds.
        /// </summary>
        /// <param name="props">CapeArrayString </param>
        /// <param name="compIds">CapeArrayString </param>
        /// <returns></returns>
        public dynamic GetCompoundConstant(object props, object compIds)
        {
            try
            {
                return (_pp as ICapeThermoCompounds).GetCompoundConstant(props, compIds);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Returns the list of all Compounds. This includes the Compound identifiers recognised and extra information that can be used to further identify the Compounds.
        /// </summary>
        /// <param name="compIds">CapeArrayString </param>
        /// <param name="formulae">CapeArrayString </param>
        /// <param name="names">CapeArrayString </param>
        /// <param name="boilTemps">CapeArrayDouble </param>
        /// <param name="molwts">CapeArrayDouble </param>
        /// <param name="casnos">CapeArrayString </param>
        public void GetCompoundList(ref object compIds, ref object formulae, ref object names, ref object boilTemps, ref object molwts, ref object casnos)
        {
            try
            {
                (_pp as ICapeThermoCompounds).GetCompoundList(ref compIds, ref formulae, ref names, ref boilTemps, ref molwts, ref casnos);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Returns the list of supported constant Physical Properties.
        /// </summary>
        /// <returns>CapeArrayString </returns>
        public dynamic GetConstPropList()
        {
            try
            {
                return (_pp as ICapeThermoCompounds).GetConstPropList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public int GetNumCompounds()
        {
            try
            {
                return (_pp as ICapeThermoCompounds).GetNumCompounds();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void GetPDependentProperty(object props, double pressure, object compIds, ref object propVals)
        {
            try
            {
                (_pp as ICapeThermoCompounds).GetPDependentProperty(props, pressure, compIds, propVals);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public dynamic GetPDependentPropList()
        {
            try
            {
                return (_pp as ICapeThermoCompounds).GetPDependentPropList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void GetTDependentProperty(object props, double temperature, object compIds, ref object propVals)
        {
            try
            {
                (_pp as ICapeThermoCompounds).GetTDependentProperty(props, temperature, compIds, ref propVals);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public dynamic GetTDependentPropList()
        {
            try
            {
                return (_pp as ICapeThermoCompounds).GetTDependentPropList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #endregion

        #region ICapeThermoPhases
        public int GetNumPhases()
        {
            // (_pp as icapether)
            throw new NotImplementedException();
        }

        public dynamic GetPhaseInfo(string phaseLabel, string phaseAttribute)
        {
            throw new NotImplementedException();
        }

        public void GetPhaseList(ref object phaseLabels, ref object stateOfAggregation, ref object keyCompoundId)
        {
            try
            {
                (_pp as ICapeThermoPhases).GetPhaseList(phaseLabels, stateOfAggregation, keyCompoundId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region ICapeThermoUniversalConstant
        public dynamic GetUniversalConstant(string constantId)
        {
            throw new NotImplementedException();
        }

        public dynamic GetUniversalConstantList()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ICapeThermoPropertyRoutine
        public void CalcAndGetLnPhi(string phaseLabel, double temperature, double pressure, object moleNumbers, int fFlags, ref object lnPhi, ref object lnPhiDT, ref object lnPhiDP, ref object lnPhiDn)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// CalcSinglePhaseProp is used to calculate properties and property derivatives of a mixture in a single Phase at the current 
        /// values of temperature, pressure and composition set in the Material Object. 
        /// CalcSinglePhaseProp does not perform phase Equilibrium Calculations. 
        /// </summary>
        /// <param name="props">CapeArrayString </param>
        /// <param name="phaseLabel">CapeString </param>
        public void CalcSinglePhaseProp(object props, string phaseLabel)
        {
            try
            {
                //bool b = CheckSinglePhasePropSpec("enthalpyF", phaseLabel);
                (_pp as ICapeThermoPropertyRoutine).CalcSinglePhaseProp(props, phaseLabel);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void CalcTwoPhaseProp(object props, object phaseLabels)
        {
            try
            {
                (_pp as ICapeThermoPropertyRoutine).CalcTwoPhaseProp(props, phaseLabels);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool CheckSinglePhasePropSpec(string property, string phaseLabel)
        {
            try
            {
                return (_pp as ICapeThermoPropertyRoutine).CheckSinglePhasePropSpec(property, phaseLabel);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool CheckTwoPhasePropSpec(string property, object phaseLabels)
        {
            try
            {
                return (_pp as ICapeThermoPropertyRoutine).CheckTwoPhasePropSpec(property, phaseLabels);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public dynamic GetSinglePhasePropList()
        {
            try
            {
                return (_pp as ICapeThermoPropertyRoutine).GetSinglePhasePropList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public dynamic GetTwoPhasePropList()
        {
            try
            {
                return (_pp as ICapeThermoPropertyRoutine).GetTwoPhasePropList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        #endregion

        #region ICapeThermoEquilibriumRoutine

        /// <summary>
        /// CalcEquilibrium is used to calculate the amounts and compositions of Phases at equilibrium.
        /// CalcEquilibrium will calculate temperature and/or pressure if these are not among the two
        /// specifications that are mandatory for each Equilibrium Calculation considered.
        /// </summary>
        /// <param name="specification1"></param>
        /// <param name="specification2"></param>
        /// <param name="solutionType"></param>
        public void CalcEquilibrium(object specification1, object specification2, string solutionType)
        {
            try
            {
                (_pp as ICapeThermoEquilibriumRoutine).CalcEquilibrium(specification1, specification2, solutionType);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool CheckEquilibriumSpec(object specification1, object specification2, string solutionType)
        {
            try
            {
                return (_pp as ICapeThermoEquilibriumRoutine).CheckEquilibriumSpec(specification1, specification2, solutionType);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #endregion

        #region ICapeThermoMaterialContext

        // SetMaterial will calls material.GetCompoundList(o, o, o, o, o, o) to check compounds can be indentified by the package.
        public void SetMaterial(object material)
        {
            try
            {
                (_pp as ICapeThermoMaterialContext).SetMaterial(material);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);

            }
        }

        public void UnsetMaterial()
        {
            try
            {
                (_pp as ICapeThermoMaterialContext).UnsetMaterial();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        #endregion


        #region Exposed Property Methods

        private string[] components;
        public string[] Components
        {
            get
            {
                if (components == null)
                {
                    object compids, formulae, names, boilTemps, molwts, casnos;
                    compids = formulae = names = boilTemps = molwts = casnos = null;
                    GetCompoundList(ref compids, ref formulae, ref names, ref boilTemps, ref molwts, ref casnos);
                    components = compids as string[];
                }
                return components;
            }
        }

        /// <summary>
        /// get average molar weight.
        /// </summary>
        /// <returns></returns>
        public double GetMolWeight(double[] z)
        {
            double[] mws = GetMolWeights();
            double mw = 0;
            for (int i = 0; i < mws.Length; i++)
            {
                mw += mws[i] * z[i];
            }
            return mw;
        }

        /// <summary>
        /// get component molar weights.
        /// </summary>
        /// <returns></returns>
        public double[] GetMolWeights()
        {
            object[] mws = GetCompoundConstant(new[] { "molecularWeight" }, Components);
            return Array.ConvertAll(mws, o => (double)o);
        }

        public double MixtureProperty(double t, double p, double[] z, string property, Phases phase)
        {
            T = t;
            P = p;
            Z = z;

            object vals = null;
            string phaseLabel = phase.ToString();
            if (phase == Phases.Overall)
            {
                throw new Exception("'overall' is not a valid phase.");
            }
            else
            {
                SetSinglePhaseProp("temperature", phaseLabel, Bases.Undefined.ToString(), new[] { t });
                SetSinglePhaseProp("pressure", phaseLabel, Bases.Undefined.ToString(), new[] { p });
                SetSinglePhaseProp("fraction", phaseLabel, Bases.Mole.ToString(), z);
                CalcSinglePhaseProp(new[] { property }, phaseLabel);
                GetSinglePhaseProp(property, phaseLabel, Bases.Mole.ToString(), ref vals);
            }
            return (vals as double[])[0];
        }

        public void Flash(FlashTypes flashType, double parm1, double parm2, double[] z)
        {
            switch (flashType)
            {
                case FlashTypes.TP:
                    FlashTP(parm1, parm2, z);
                    break;
                case FlashTypes.PH:
                    FlashPH(parm1, parm2, z);
                    break;
                case FlashTypes.TH:
                    FlashTH(parm1, parm2, z);
                    break;
                case FlashTypes.TVf:
                    FlashTVf(parm1, parm2, z);
                    break;
                case FlashTypes.PVf:
                    FlashPVf(parm1, parm2, z);
                    break;
                default:
                    throw new Exception("Invalid flash type.");
                    break;
            }
        }
        private void FlashTP(double t, double p, double[] z)
        {
            T = t;
            P = p;
            Z = z;

            SetOverallProp("temperature", Bases.Undefined.ToString(), new[] { t });
            SetOverallProp("pressure", Bases.Undefined.ToString(), new[] { p });
            SetOverallProp("fraction", Bases.Mole.ToString(), z);

            CalcEquilibrium(new[] { "temperature" }, new[] { "pressure" }, "Normal");

            object vals = null;
            GetSinglePhaseProp("fraction", Phases.Liquid.ToString(), Bases.Mole.ToString(), ref vals);
            X = vals as double[];

            vals = null;
            GetSinglePhaseProp("fraction", Phases.Vapor.ToString(), Bases.Mole.ToString(), ref vals);
            Y = vals as double[];

            vals = null;
            GetSinglePhaseProp("phasefraction", Phases.Vapor.ToString(), Bases.Mole.ToString(), ref vals);
            Vf = (vals as double[])[0];
        }

        private void FlashPH(double p, double h, double[] z)
        {
            P = p;
            Z = z;

            SetOverallProp("pressure", Bases.Undefined.ToString(), new[] { p });
            SetOverallProp("enthalpy", Bases.Undefined.ToString(), new[] { h });
            SetOverallProp("fraction", Bases.Mole.ToString(), z);

            CalcEquilibrium(new[] { "pressure" }, new[] { "enthalpy" }, "Normal");

            object vals = null;
            GetSinglePhaseProp("fraction", Phases.Liquid.ToString(), Bases.Mole.ToString(), ref vals);
            X = vals as double[];

            vals = null;
            GetSinglePhaseProp("fraction", Phases.Vapor.ToString(), Bases.Mole.ToString(), ref vals);
            Y = vals as double[];

            vals = null;
            GetSinglePhaseProp("phasefraction", Phases.Vapor.ToString(), Bases.Mole.ToString(), ref vals);
            Vf = (vals as double[])[0];

            vals = null;
            GetOverallProp("temperature", Bases.Undefined.ToString(), ref vals);
            T = (vals as double[])[0];
        }

        private void FlashTH(double t, double h, double[] z)
        {
            T = t;
            Z = z;

            SetOverallProp("temperature", Bases.Undefined.ToString(), new[] { t });
            SetOverallProp("enthalpy", Bases.Undefined.ToString(), new[] { h });
            SetOverallProp("fraction", Bases.Mole.ToString(), z);

            CalcEquilibrium(new[] { "temperature" }, new[] { "enthalpy" }, "Normal");

            object vals = null;
            GetSinglePhaseProp("fraction", Phases.Liquid.ToString(), Bases.Mole.ToString(), ref vals);
            X = vals as double[];

            vals = null;
            GetSinglePhaseProp("fraction", Phases.Vapor.ToString(), Bases.Mole.ToString(), ref vals);
            Y = vals as double[];

            vals = null;
            GetSinglePhaseProp("phasefraction", Phases.Vapor.ToString(), Bases.Mole.ToString(), ref vals);
            Vf = (vals as double[])[0];

            vals = null;
            GetOverallProp("pressure", Bases.Undefined.ToString(), ref vals);
            P = (vals as double[])[0];
        }

        private void FlashTVf(double t, double vf, double[] z)
        {
            T = t;
            Vf = vf;
            Z = z;

            SetOverallProp("temperature", Bases.Undefined.ToString(), new[] { t });
            SetOverallProp("phasefraction", Bases.Mole.ToString(), new[] { vf });
            SetOverallProp("fraction", Bases.Mole.ToString(), z);

            CalcEquilibrium(new[] { "temperature" }, new[] { "phasefraction" }, "Normal");

            object vals = null;
            GetSinglePhaseProp("fraction", Phases.Liquid.ToString(), Bases.Mole.ToString(), ref vals);
            X = vals as double[];

            vals = null;
            GetSinglePhaseProp("fraction", Phases.Vapor.ToString(), Bases.Mole.ToString(), ref vals);
            Y = vals as double[];

            vals = null;
            GetOverallProp("pressure", Bases.Undefined.ToString(), ref vals);
            P = (vals as double[])[0];
        }

        private void FlashPVf(double p, double vf, double[] z)
        {
            P = p;
            Vf = vf;
            Z = z;

            SetOverallProp("pressure", Bases.Undefined.ToString(), new[] { p });
            SetOverallProp("phasefraction", Bases.Mole.ToString(), new[] { vf });
            SetOverallProp("fraction", Bases.Mole.ToString(), z);

            CalcEquilibrium(new[] { "pressure" }, new[] { "phasefraction" }, "Normal");

            object vals = null;
            GetSinglePhaseProp("fraction", Phases.Liquid.ToString(), Bases.Mole.ToString(), ref vals);
            X = vals as double[];

            vals = null;
            GetSinglePhaseProp("fraction", Phases.Vapor.ToString(), Bases.Mole.ToString(), ref vals);
            Y = vals as double[];

            vals = null;
            GetOverallProp("temperature", Bases.Undefined.ToString(), ref vals);
            T = (vals as double[])[0];
        }


        public double T, P, Vf;
        public double[] X, Y, Z;



        #endregion
    }

    public enum Phases
    {
        Vapor,
        Liquid,
        Liquid2,
        Solid,
        Overall
    }

    public enum Bases
    {
        Mole,
        Mass,
        Undefined,
    }

    public enum FlashTypes
    {
        TP,
        PH,
        TH,
        TVf,
        PVf
    }

}
