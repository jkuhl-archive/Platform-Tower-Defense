# Development Environment Setup Instructions

## Download and install Manjaro or another Arch based Linux distro
https://manjaro.org/downloads/official/gnome/

## Install the basics
```shell
sudo pacman -Syu vlc qbittorrent gimp blender inkscape signal-desktop audacity steam git base-devel
```

## Install yay for easy access to the AUR
```shell
git clone https://aur.archlinux.org/yay.git ~/Downloads/yay && cd ~/Downloads/yay
makepkg -si
```

## Install some stuff from AUR
```shell
yay -S librewolf-bin unityhub jetbrains-toolbox jitsi-meet-desktop-bin visual-studio-code-bin
```

## Configure git
```shell
git config --global user.name "YOUR GITHUB USERNAME HERE"
git config --global user.email "YOUR GITHUB EMAIL ADDRESS HERE"
```

## Install the project's Unity version
1. Launch Unity Hub and sign in using your Unity ID
2. Install Unity 2020.3.14f1 via Unity Hub. Make sure to select build support for any systems you want to build for such as Windows or macOS:
![image](https://user-images.githubusercontent.com/46010615/125982174-fb05fda8-93fb-4d26-a430-4d2c2b9e7258.png)

## Setup Github authentication (SSH keys)
https://yewtu.be/watch?v=Z3ELWci34cM

## Pull down the project from Github
```shell
mkdir -p ~/Unity-Projects && git clone https://github.com/virtual-meme-machine/Platform-Tower-Defense.git ~/Unity-Projects/Platform-Tower-Defense
```

## Open the project in Unity
```shell
~/Unity-Projects/Platform-Tower-Defense/Open_In_Unity.py
```
