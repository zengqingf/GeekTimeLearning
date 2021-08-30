#########################################################################
# File Name: register-device.sh
# Author: etond
# mail: monkey_tv@126.com
# Created Time: Thu Jun  6 18:38:33 2019
#########################################################################
#!/bin/bash

PROFILE_NAME=$1
PROFILE_UDID=$2

if [ "" == ${PROFILE_NAME} ]; then
    echo sample ./register-device.sh TM_xxxx 00000xxxxxxxxx
    exit 1;
fi

if [ "" == "${PROFILE_UDID}" ]; then
    PROFILE_UDID=`idevice_id -l`
    echo got the plugin device udid : ${PROFILE_UDID}
fi

if [ "" == "${PROFILE_UDID}" ]; then
    echo plugin in the iOS device set the auth
    echo sample ./register-device.sh TM_xxxx
    exit 1
fi

DATETIME=`date '+%Y%m%d-%H%M%S'`

PROFILE_FILENAME="TM-V2-Profile-${DATETIME}.mobileprovision"
UDID_FILENAME="${PROFILE_FILENAME}.UUID"

#CONFIG_FILE="./BuildTools/config.json"

echo "#####################################"

echo device name: $PROFILE_NAME udid: $PROFILE_UDID
echo profile filename $PROFILE_FILENAME, udidfilename: $UDID

ruby ./add-device-and-generate-profile.rb $PROFILE_NAME $PROFILE_UDID $PROFILE_FILENAME $UDID_FILENAME

echo 0

SVN_KEY_PATH="../../sec/"
PROVISION_PATH="../../sec/key/ios/a8_debug_dev.mobileprovision"
if [ -f ${UDID_FILENAME} ] && [ -f ${PROFILE_FILENAME} ]; then
    if [ ! -d ${SVN_KEY_PATH} ]; then
        echo svn checkout "svn://192.168.2.177/sdk/sec" to ${SVN_KEY_PATH}
        svn co "svn://192.168.2.177/sdk/sec" ${SVN_KEY_PATH}
    else
        svn revert ${SVN_KEY_PATH}
        svn up ${SVN_KEY_PATH}
    fi
    UUID=`cat ${UDID_FILENAME}`
    echo uuid: ${UUID}
    cp ${PROFILE_FILENAME} ${PROVISION_PATH}
    svn commit ${SVN_KEY_PATH} -m "device(): add device for ${PROFILE_NAME}"
else
    echo missing ${PROFILE_FILENAME} and ${UDID_FILENAME}
    exit 1;
fi

exit 0
if [ -f ${UDID_FILENAME} ] && [ -f ${PROFILE_FILENAME} ]; then

    open ${PROFILE_FILENAME}
    scp ${PROFILE_FILENAME} tengmu@192.168.2.146:~/

    ssh tengmu@192.168.2.146 "open ~/${PROFILE_FILENAME}"

    UUID=`cat ${UDID_FILENAME}`
    echo uuid: ${UUID}

    svn revert ${CONFIG_FILE}
    svn up ${CONFIG_FILE}

    sed -i "" 's/"provision": ".*"/"provision": "'${UUID}'"/g' ${CONFIG_FILE}

    svn diff ${CONFIG_FILE}
    svn commit ${CONFIG_FILE} -m "device(): add device for ${PROFILE_NAME}"
else
    echo missing ${PROFILE_FILENAME} and ${UDID_FILENAME}
    exit 1;
fi
