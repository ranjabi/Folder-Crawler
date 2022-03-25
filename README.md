# Folder Crawling with BFS and DFS

## i. Description
Tugas Besar 2 IF2211 Strategi Algoritma. BFS and DFS Algorithm Implementation on Folder Crawling. 

## ii. Requirement
Install MSAGL modules from NuGet Package Manager. Choose **Tools > NuGet Package Manager > Package Manager Console** on Visual Studio and run the command below.
```
Install-Package AutomaticGraphLayout -Version 1.1.11
Install-Package AutomaticGraphLayout.Drawing -Version 1.1.11
Install-Package AutomaticGraphLayout.WpfGraphControl -Version 1.1.11
Install-Package AutomaticGraphLayout.GraphViewerGDI -Version 1.1.11
```
You also need to install the MaterialSkin via NuGet. Follow these steps.

1. Right-click on the project in the solution explorer
2. Select `Manage NuGet Packages`
3. Go to Browse tab and search for `MaterialSkin`
4. Install it and the Material Controls should automatically be added to the Control Toolbox

> If something goes wrongly on installing the MaterialSkin, you could check it in https://ourcodeworld.com/articles/read/441/how-to-use-material-design-controls-with-c-in-your-winforms-application or another resources

## iii. How to Compile
Open `Folder-Crawling-BFS-DFS/src/FolderCrawling.sln` on Visual Studio. Choose **Build -> Build Solution (Ctrl + Shift + B)**. Press **Start Without Debugging Button (Ctrl + F5)**.

## iv. Running the Program
1. Open `Folder-Crawling-BFS-DFS/bin/FolderCrawling.exe`
2. Choose starting directory by pressing **CHOOSE YOUR STARTING FOLDER** button
3. Input Folder/File Name
4. Select "Find All Occurances" to find all folder/file with same name.
5. Choose searching method (BFS/DFS)
6. Set Visualizer Speed
7. Click **Search** button

## v. Author
| Name          | NIM           |
| ------------- |:-------------:|
|Muhammad Fikri Ranjabi| 13520002 |
|Azka Syauqy Irsyad    | 13520107 |
|M. Syahrul Surya Putra| 13520161 |
