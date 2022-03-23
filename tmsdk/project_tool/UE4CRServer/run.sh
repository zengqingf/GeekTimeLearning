VIRTUALENV_ERR(){
    echo virtualenv not installed
}
MAKE_VENV(){
    virtualenv venv
    source venv/bin/activate
    pip3 install -r requirements.txt
    venv/bin/deactivate
}
virtualenv -h > /dev/null 2>&1
if [ "$?" != "0" ] ; then
    VIRTUALENV_ERR
fi

if [ ! -d "venv" ] ; then
    MAKE_VENV
fi

source venv/bin/activate
#  ------------
#  run or pak
#  ------------
nohup python3 run.py > server.log 2>&1 &
deactivate

