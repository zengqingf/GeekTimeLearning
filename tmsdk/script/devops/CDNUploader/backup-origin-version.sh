#########################################################################
# File Name: featch-origin.sh
# Author: etond
# mail: monkey_tv@126.com
# Created Time: Thu Mar  1 22:48:29 2018
#########################################################################
#!/bin/bash
VERSION_NAME=version.json

for url in `cat ./urls`
do
    #http://static.aldzn.xyimg.net/hardcore/vivo/asset/
    systime=`date +%Y-%m-%d-%H-%M-%S`"_version_backup"
    echo "热更新日期 ："${systime}

    localdir=./${systime}/origin-version/${url/http:\/\/static.aldzn.xyimg.net\//./}
    
    echo $localdir

    mkdir -p $localdir
			
    echo 123456 | sudo -S curl $url$VERSION_NAME -o $localdir/$VERSION_NAME

    echo -----------------${url/http:\/\/static.aldzn.xyimg.net\//./}------------------

    cat $localdir/$VERSION_NAME

    echo ------------------------------------------------------------------------------

done
