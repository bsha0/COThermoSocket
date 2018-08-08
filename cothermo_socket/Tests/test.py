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

print(thermo.components)
print(thermo.get_MolWeights())
print(thermo.get_MolWeight(z))
(t, p, vf, x, y) = thermo.flash(FlashTypes.TP, t, p, z)
for name, member in Properties.__members__.items():
    print(thermo.get_MixtureProperty(t, p, y, member, Phases.Vapor, Bases.Mole))
    print(thermo.get_MixtureProperty(t, p, y, member, Phases.Vapor, Bases.Mass))

val = thermo.get_MixtureProperty(t, p, y, Properties.Density, Phases.Vapor, Bases.Mole)
print(val.units)
print(val.get_value('kmol/cum'))
