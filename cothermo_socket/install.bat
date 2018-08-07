pip uninstall cothermo_socket --y
REM del template files
REM python setup.py bdist_wheel --plat-name win_amd64
python setup.py bdist_wheel
pushd dist
pip install cothermo_socket-1.0.0-py3-none-any.whl
popd

REM publish package to PYPI
REM twine upload  dist/*