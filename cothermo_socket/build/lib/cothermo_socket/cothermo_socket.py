# import com dll
import clr
import platform
import sys
import os
if platform.architecture()[0] == '64bit':
    sys.path.append(os.path.join(os.path.dirname(os.path.realpath(__file__)), '64bit'))
else:
    sys.path.append(os.path.join(os.path.dirname(os.path.realpath(__file__)), '32bit'))
clr.FindAssembly(r"COThermoSocket.Core.dll")
from COThermoSocket.Core import *
from enum import IntEnum
sys.path.append(os.path.dirname(os.path.realpath(__file__)))
from uom import *

class Phases(IntEnum):
    Vapor = 0
    Liquid = 1
    Liquid2 = 2
    Solid = 3
    Overall = 4


class Bases(IntEnum):
    Mole = 0
    Mass = 1
    Undefined = 2


class Properties(IntEnum):
    Density = 0
    Enthalpy = 1
    Volume = 2


class FlashTypes(IntEnum):
    TP = 0
    PH = 1
    TH = 2
    TVf = 3
    PVf = 4


class COThermo_Socket(object):

    def __init__(self):
        self.__COThermo = COThermo.GetCOThermo()

    @property
    def pkgmgrs(self):
        '''Retrieves the names of Package Manager components.'''
        return [pkgmgr.Name for pkgmgr in self.__COThermo.PropPkgMgrs]

    @property
    def pkgs(self):
        '''Retrieves the names of the Property Packages being managed by a Property Package Manager component.'''
        return [pkg for pkg in self.__COThermo.PropPkgNames]

    @property
    def pkgmgr(self):
        return self._pkgmgr

    @pkgmgr.setter
    def pkgmgr(self, value):
        pkgmgrs = self.pkgmgrs
        if value not in pkgmgrs:
            raise Exception(f'{value} is not a valid name of property package manager component.' + 'please use method get_pkgmgrs to retrieve names.')
        else:
            self.__COThermo.PropPkgMgr = self.__COThermo.PropPkgMgrs[pkgmgrs.index(value)]
            self._pkgmgr = value

    @property
    def pkg(self):
        return self._pkg

    @pkg.setter
    def pkg(self, value):
        pkgs = self.pkgs
        if value not in pkgs:
            raise Exception(f'{value} is not a valid name of property package.' + 'please use method get_pkgs to retrieve names.')
        else:
            self.__COThermo.PropPkgName = value
            self._pkg = value
            self._mo = self.__COThermo.MaterialObject

    @property
    def components(self):
        '''get components in the property package'''
        return tuple(self._mo.Components)

    def get_MolWeight(self, z):
        '''get average molar weight'''
        return self._mo.GetMolWeight(z)

    def get_MolWeights(self):
        '''get component molar weights'''
        return tuple(self._mo.GetMolWeights())

    def get_PropertyUom(self, prop, basis):
        # density
        if prop == Properties.Density and basis == Bases.Mole:
            return dict(uom=UOMs.MolarDensity, unit='mol/cum', ismultiply=True)
        elif prop == Properties.Density and basis == Bases.Mass:
            return dict(uom=UOMs.MassDensity, unit='g/cum', ismultiply=True)
        # Enthalpy
        elif prop == Properties.Enthalpy and basis == Bases.Mole:
            return dict(uom=UOMs.MolarEnthalpy, unit='J/mol', ismultiply=True)
        elif prop == Properties.Enthalpy and basis == Bases.Mass:
            return dict(uom=UOMs.MassEnthalpy, unit='J/g', ismultiply=False)
        # Volume
        elif prop == Properties.Volume and basis == Bases.Mole:
            return dict(uom=UOMs.MolarVolume, unit='cum/mol', ismultiply=True)
        elif prop == Properties.Volume and basis == Bases.Mass:
            return dict(uom=UOMs.MassVolume, unit='cum/g', ismultiply=False)
        # Unitless
        else:
            return dict(uom=UOMs.Unitless, unit=None, ismultiply=True)

    def get_MixtureProperty(self, t, p, z, prop, phase, basis):
        '''get mixture property(density/enthalpy) in single phase.'''
        multiplier = 1
        if basis == Bases.Mass:
            multiplier = self.get_MolWeight(z)
        prop_uom = self.get_PropertyUom(prop, basis)
        if prop_uom['ismultiply']:
            value = self._mo.MixtureProperty(t, p, z, prop.name, phase.value) * multiplier
        else:
            value = self._mo.MixtureProperty(t, p, z, prop.name, phase.value) / multiplier

        return UOM(prop_uom['uom'], value, prop_uom['unit'])

    def flash(self, flashtype, parm1, parm2, z):
        '''flash'''
        if isinstance(parm1, (float, int)):
            pass
        # elif isinstance(parm1, UOM):
        elif hasattr(parm1, 'get_value'):
            parm1 = parm1.get_value()[0]
        else:
            raise Exception(f'invalid argument parm1 {parm1}')

        if isinstance(parm2, (float, int)):
            pass
        # elif isinstance(parm2, UOM):
        elif hasattr(parm2, 'get_value'):
            parm2 = parm2.get_value()[0]
        else:
            raise Exception(f'invalid argument parm2 {parm2}')

        self._mo.Flash(flashtype, parm1, parm2, z)
        return (self._mo.T, self._mo.P, self._mo.Vf, tuple(self._mo.X), tuple(self._mo.Y))


if __name__ == "__main__":
    pass
