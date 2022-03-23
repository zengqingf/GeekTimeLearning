

MIRROR="[global]
index-url = https://mirrors.aliyun.com/pypi/simple/
[install]
trusted-host = mirrors.aliyun.com"


# installer -pkg XXX -target /

if [ -d ~/.pip ]; then
    if [ -f ~/.pip/pip.conf ]; then
        cat ~/.pip/pip.conf
        echo '~/.pip/pip.conf EXSISTS'
    else
        echo "${MIRROR}" > ~/.pip/pip.conf
    fi
else
    mkdir ~/.pip
    echo "${MIRROR}" > ~/.pip/pip.conf
fi

# /Library/Frameworks/Mono.framework/Versions/6.0.0
# /Library/Frameworks/Xamarin.iOS.framework
# /Library/Frameworks/PluginManager.framework
# /Library/Frameworks/NyxAudioAnalysis.framework