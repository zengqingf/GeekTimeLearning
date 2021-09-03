password="123456"
security unlock-keychain -p ${password} ~/Library/Keychains/login.keychain

uuid=$(idevice_id -l | head -n1)

xcodebuild -project /Users/hegu/_ExtendTools/STF/WebDriverAgent/WebDriverAgent.xcodeproj -scheme WebDriverAgentRunner -destination "id=$uuid" test

