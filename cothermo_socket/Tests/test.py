from cothermo_socket import *

thermo = COThermo_Socket()
print(thermo.pkgmgrs)
thermo.pkgmgr = thermo.pkgmgrs[0]
print(thermo.pkgs)
thermo.pkg = thermo.pkgs[3]

#t = 30 + 273.15
t = UOM(UOMs.Temperature, 30, 'C')
p = 101325.0
z = (0.2, 0.2, 0.2, 0.2, 0.2)

print(isinstance(t,UOM))
print(thermo.components)
print(thermo.get_MolWeights())
print(thermo.get_MolWeight(z))
(t, p, vf, x, y, z) = thermo.flash(FlashType.TP, t, p, z)
for name, member in PropName.__members__.items():
    print(thermo.get_MixtureProperty(t, p, y, member, Phase.Vapor, Basis.Mole))
    print(thermo.get_MixtureProperty(t, p, y, member, Phase.Vapor, Basis.Mass))

val = thermo.get_MixtureProperty(t, p, y, PropName.Density, Phase.Vapor, Basis.Mole)
print(val.units)
print(val.get_value('kmol/cum'))
