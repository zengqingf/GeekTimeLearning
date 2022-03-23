

a="FAIL "

for dep in `cat pythondep` ;
do
    pip3 install $dep
    if [ $? != "0" ]; then
        a="${a},$dep"
    fi
done


if [ $a == "FAIL " ]; then
    echo "安装失败的库 $a"
else
    echo "安装成功"
fi

