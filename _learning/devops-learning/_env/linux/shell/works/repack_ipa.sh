#########################################################################
# File Name: repack_ipa.sh
# Author: etond
# mail: monkey_tv@126.com
# Created Time: Tue Mar 22 19:39:28 2016
#########################################################################
#!/bin/bash


export IPA_PATH=$PWD/Payload/test.ipa
export APP_PATH=$PWD/Payload/test.app

export CERT_ID=CB0B54E03B8334D18E8B70EEF226C407B3C4CC18

## unlock the login.keychain
security unlock-keychain -p 123456 login.keychain

## code sign
/usr/bin/codesign --force --sign $CERT_ID --entitlements test.app.xcent --timestamp=none $APP_PATH

## package the ipa
xcrun -sdk iphoneos PackageApplication -v $APP_PATH -o $IPA_PATH
