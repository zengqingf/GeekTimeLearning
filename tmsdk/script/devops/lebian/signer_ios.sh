mkfolder(){
    if [ ! -d ${1} ]; then
        mkdir "${1}"
    fi
}
rmfile(){
    if [ -f ${1} ]; then
        rm "${1}"
    fi
}
rmtree(){
    if [ -d ${1} ]; then
        rm -rf "${1}"
    fi
}
APKROOT="../apkroot"
IPA_NAME='68523_33_com.cheng.kdyzapp.hx_5b10b967eb58adb50bad0048bfbf2e5b.ipa'
BUNDLE_ID='com.cheng.kdyzapp.hx'
PROVISION_PATH='/Users/user/Desktop/signer/si/_ios_appstore/channels/dycc/dev/Cheng_dev.mobileprovision'
CODESING_STR='iPhone Developer: wei su (ST5X8TW62A)'


IPA_PATH=$1
BUNDLE_ID=$2
PROVISION_PATH=$3
CODESING_STR=$4

APKROOT=`dirname ${IPA_PATH}`
IPA_NAME=`basename ${IPA_PATH}`

UNZIO_IPA_FOLDER_NAME=${IPA_NAME%.ipa}
SIGNED_IPA_NAME="${UNZIO_IPA_FOLDER_NAME}_signed.ipa"
UNZIP_PATH="${APKROOT}/${UNZIO_IPA_FOLDER_NAME}"


mkfolder "${APKROOT}"
mkfolder "${UNZIP_PATH}"
echo "${UNZIP_PATH}"
echo "${IPA_PATH}"

echo '开始解压'
rmtree "${UNZIP_PATH}"
unzip -q "${IPA_PATH}" -d "${UNZIP_PATH}"
echo '解压完成'

PAYLOAD_PATH="${UNZIP_PATH}/Payload"

for AN in `ls "${PAYLOAD_PATH}"`
do
    APP_PATH="${PAYLOAD_PATH}/${AN}"
done
APP_PROVISION_PATH="${APP_PATH}/embedded.mobileprovision"

# ------clear start-----------
rmfile "${APP_PROVISION_PATH}"
# Plugins和Watch是Extention，个人签名无法签名
# rm -rf "${APP_PATH}/PlugIns"
# rm -rf "${APP_PATH}/Watch"


# ------clear end-----------

# ------modify start-----------
# 改bundleid
OLD_BUNDLE_ID=`/usr/libexec/PlistBuddy -c "Print :CFBundleIdentifier" "${APP_PATH}/Info.plist"`
echo "[bundleid] ${OLD_BUNDLE_ID}---->${BUNDLE_ID}"
/usr/libexec/PlistBuddy -c "Set :CFBundleIdentifier ${BUNDLE_ID}" "${APP_PATH}/Info.plist"

#拷贝embedded.mobileprovision
cp "${PROVISION_PATH}" "${APP_PROVISION_PATH}"

# 过滤entitlements
ENTITLEMENTS_TEMP_PATH="entitlements_temp.plist"
ENTITLEMENTS_PATH="entitlements.plist"
security cms -D -i "${PROVISION_PATH}" > "${ENTITLEMENTS_TEMP_PATH}"
/usr/libexec/PlistBuddy -x -c "Print :Entitlements" "${ENTITLEMENTS_TEMP_PATH}" > "${ENTITLEMENTS_PATH}"
rmfile "${ENTITLEMENTS_TEMP_PATH}"



echo "${APP_PATH}"
FRAMEWORK_PATH="${APP_PATH}/Frameworks"

# 解锁钥匙串
LOGIN_KEYCHAIN_PATH=`security login-keychain`
security unlock-keychain -p "123456" "${LOGIN_KEYCHAIN_PATH}"
# 先签内部动态库
if [ -d  "${FRAMEWORK_PATH}" ]; then
	for FRAMEWORK in "${FRAMEWORK_PATH}/"*
	do
		echo "[sign] ${FRAMEWORK}"
		codesign -fs "${CODESING_STR}" "${FRAMEWORK}"
	done
fi
# 再签整包
echo "[sign] ${APP_PATH}"
codesign -f -s "${CODESING_STR}" --no-strict --entitlements="${ENTITLEMENTS_PATH}" "${APP_PATH}"

rmfile "${ENTITLEMENTS_PATH}"
# 重新压缩
echo '重新组包'
rmfile "${APKROOT}/${SIGNED_IPA_NAME}"
cd "${UNZIP_PATH}" && zip -rq "../${SIGNED_IPA_NAME}" "Payload"
echo '组包完成'
# ------modify end-----------
rmtree "${UNZIP_PATH}"
