# Platform-Tower-Defense
Tower Defense style game developed in Unity

![image](https://user-images.githubusercontent.com/46010615/125985845-ddf13e32-8c38-4aab-93d1-f764ead2b8b1.png)


# Building
## Dependencies
- [Visual Studio](https://visualstudio.microsoft.com) / [VS Code](https://code.visualstudio.com/) / [Rider](https://www.jetbrains.com/rider/) / Other C# IDE
- Git
- Unity Hub
- Unity 2020.3.14f1

## Development Environment Setup
1. Install Unity Hub via the [Unity website](https://unity3d.com/get-unity/download) or for based Arch users via the [AUR](https://aur.archlinux.org/packages/unityhub/)
2. Sign in to Unity Hub using your Unity ID
3. Install Unity 2020.3.14f1 via Unity Hub. Make sure to select build support for any systems you want to build for such as Windows or macOS:
![image](https://user-images.githubusercontent.com/46010615/125982174-fb05fda8-93fb-4d26-a430-4d2c2b9e7258.png)
4. Pull down this repo via Git:
```bash
mkdir -p ~/Unity-Projects && git clone https://github.com/virtual-meme-machine/Platform-Tower-Defense.git ~/Unity-Projects/Platform-Tower-Defense
```
5. In Unity Hub, open the 'Projects' tab and then click the 'ADD' button at the top:
![image](https://user-images.githubusercontent.com/46010615/125983299-70f2bbe1-4e64-4c0d-97d7-9457074a1d89.png)
6. Browse to the Platform-Tower-Defense directory, this should be ```~/Unity-Projects/Platform-Tower-Defense``` if you have been following along, then click 'Open':
![image](https://user-images.githubusercontent.com/46010615/125983992-447add47-47c4-44a6-ae95-b8d92bf73efc.png)
7. Click on the 'Platform-Tower-Defense' project in the project list to open the project in the Unity editor:
![image](https://user-images.githubusercontent.com/46010615/125984283-580fa187-12b0-4c99-bfe2-bfae345f320c.png)

# Syncing the Project via Git
**This should be done every time before you open the project in Unity**
```bash
cd ~/Unity-Projects/Platform-Tower-Defense && git pull
```
