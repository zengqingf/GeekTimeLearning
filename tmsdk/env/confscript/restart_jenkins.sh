

echo 卸载中
sudo launchctl unload /Library/LaunchDaemons/org.jenkins-ci.plist

sleep 3

echo 启动中

sudo launchctl load /Library/LaunchDaemons/org.jenkins-ci.plist
echo 启动完成

