brew uninstall --ignore-dependencies libimobiledevice
brew uninstall --ignore-dependencies usbmuxd
brew uninstall --ignore-dependencies ideviceinstaller
brew install --HEAD usbmuxd
brew unlink usbmuxd && brew link usbmuxd
brew install --HEAD libimobiledevice
brew unlink libimobiledevice
brew link --overwrite libimobiledevice
brew install ideviceinstaller
brew unlink ideviceinstaller
brew link --overwrite ideviceinstaller

