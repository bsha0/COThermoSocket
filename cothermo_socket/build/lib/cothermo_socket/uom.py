import json
from enum import IntEnum
import os


class UOMs(IntEnum):
    # make sure string values are the same with UOMSet.json.
    Unitless = 0
    Length = 1
    Temperature = 2
    Pressure = 3
    MolarDensity = 4
    MassDensity = 5
    MolarEnthalpy = 6
    MassEnthalpy = 7


class UOM(object):
    def __init__(self, uom: UOMs.Unitless, value: float = None, unit: str = None):
        path = os.path.join(os.path.dirname(os.path.realpath(__file__)), 'UOMSet.json')
        with open(path, encoding='utf-8') as f:
            uom_set = json.load(f)
            if uom.name in uom_set:
                self._uom = uom_set[uom.name]
            else:
                raise Exception(f'Invalid uom name: {uom.name}')
        if unit is None:
            if value is not None:
                self.set_value(value)
        else:
            if unit in self.units:
                if value is not None:
                    self.set_value(value, unit)
            else:
                raise Exception(f'{unit} is not a valid unit of {name}')

    @property
    def units(self):
        units = []
        for unit in self._uom:
            units.append(unit['ShortName'])
        return tuple(units)

    def get_value(self, unit=None):
        if unit is None:
            return (self._value, None)
        else:
            if unit in self.units:
                unit_class = [u for u in self._uom if u['ShortName'] == unit][0]
                if 'FactorB' in unit_class:
                    return ((self._value - unit_class['FactorB']) / unit_class['FactorA'], unit)
                else:
                    return (self._value / unit_class['FactorA'], unit)
            else:
                raise Exception(f'Invalid unit: {unit}')

    def set_value(self, value: float, unit=None):
        self._unit = unit
        if unit is None:
            self._value = value
        else:
            if unit in self.units:
                unit_class = [u for u in self._uom if u['ShortName'] == unit][0]
                if 'FactorB' in unit_class:
                    self._value = unit_class['FactorA'] * value + unit_class['FactorB']
                else:
                    self._value = unit_class['FactorA'] * value
            else:
                raise Exception(f'Invalid unit: {unit}')

    def __str__(self):
        return str(self.get_value(self._unit))


if __name__ == '__main__':
    pass
