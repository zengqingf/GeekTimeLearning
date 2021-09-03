#########################################################################
# File Name: featch-origin.sh
# Author: etond
# mail: monkey_tv@126.com
# Created Time: Thu Mar  1 22:48:29 2018
#########################################################################
#!/bin/bash

currDate=`date +%Y-%m-%d`

for url in `cat ./${currDate}_HotfixRecord.txt | grep ".txt"`
do		
    echo -----------------check : ${url}------------------

    curl $url

    res=$?

    echo $res

    if [ ${res} != 0 ]; then

        echo XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

    else

        echo ------------------------------------------------------------------------------

    fi

done
