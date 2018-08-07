import setuptools

setuptools.setup(
    name='cothermo_socket',
    version='1.0.0',
    description='cape-open thermo python package.',
    license="MIT Licence",
    author="bshao",
    author_email="bshao@163.com",
    packages=setuptools.find_packages(),
    classifiers=['Programming Language :: Python :: 3'],
    include_package_data=True,
    install_requires=['pythonnet'],
)
