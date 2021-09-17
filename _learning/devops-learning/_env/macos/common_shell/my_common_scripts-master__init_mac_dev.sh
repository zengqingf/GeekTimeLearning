#!/bin/sh
# -*- coding: utf8 -*-
# Init Mac development environment. Install brew, zsh, oh-my-zsh, z and so on.
# NOTICE: must run at sudo

# install brew
echo "install brew..."
ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
if [ $? -ne 0 ]; then
    echo "failed to install brew, exit"
fi


# install zsh
echo "install zsh..."
brew install zsh
if [ $? -ne 0 ]; then
    echo "failed to install zsh, exit"
fi

# config zsh
echo "" >> /etc/shells
echo "/usr/local/bin/zsh" >> /etc/shells
if [ $? -ne 0 ]; then
    echo "failed to add /etc/shells, exit"
fi

chsh -s /usr/local/bin/zsh
if [ $? -ne 0 ]; then
    echo "failed to change shell to zsh, exit"
fi

cat ~/.bash_profile >> ~/.zshrc
if [ $? -ne 0 ]; then
    echo "failed to copy ~/.bash_profile to ~/.zshrc, exit"
fi


# install oh-my-zsh
curl -L https://github.com/robbyrussell/oh-my-zsh/raw/master/tools/install.sh | sh
if [ $? -ne 0 ]; then
    echo "failed to install oh-my-zsh, exit"
fi


# install z
mkdir -p ~/dev/tools && cd ~/dev/tools && git clone git@github.com:rupa/z.git
if [ $? -ne 0 ]; then
    echo "failed to clone z.git or mkdir error, exit"
fi
echo "\n\nsource ~/dev/tools/z/z.sh" >> ~/.zshrc
if [ $? -ne 0 ]; then
    echo "failed to add source for z, exit"
fi

echo "done"
