ios_version=$1
xcode_deviceimg_root=/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/DeviceSupport/${ios_version}/
ideviceimagemounter ${xcode_deviceimg}/DeveloperDiskImage.dmg ${xcode_deviceimg}/DeveloperDiskImage.dmg.signature

