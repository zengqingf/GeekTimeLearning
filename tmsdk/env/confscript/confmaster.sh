# !!!!!!!!!!!
# python3环境
# !!!!!!!!!!!

#自动忽略ssh第一次登陆确认
echo "StrictHostKeyChecking no" >~/.ssh/config

pip3 -V
if [ $? != "0" ]; then
    echo ************************
    echo NO PIP3
    echo ************************

    exit $?
fi

echo 开始配置clush

pip3 install ClusterShell

VER=$(python3 -V | grep -oE "\d\.\d+\.")
VER=${VER%.*}
ETC_PATH="/Library/Frameworks/Python.framework/Versions/$VER/etc"

sudo cp -r $ETC_PATH/clustershell /etc

echo clush初始配置完成
