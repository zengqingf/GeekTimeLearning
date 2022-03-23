python3 -V
statecode=$?
echo statecode${statecode}
if [ "${statecode}" == "0" ]; then
    echo "************************"
    echo NOT NEED INSTALL PYTHON3
    echo "************************"
    exit 1
fi

echo 安装python-3.7.7
echo 123456 | sudo -S installer -pkg python-3.7.7-macosx10.9.pkg -target /

echo 添加环境变量
echo "export PATH=/Library/Frameworks/Python.framework/Versions/3.7/bin:$PATH" >>~/.bash_profile
source ~/.bash_profile

echo 设置pip镜像
sh pipmirrors.sh
echo 安装需要的python库
sh dep.sh