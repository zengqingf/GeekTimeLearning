# Python 配置

* 问题

  * 问题1

    Q：

    ``` text
    > pip2
    Fatal error in launcher: Unable to create process using '"c:\python27\python.exe"  "C:\Python27\Scripts\pip2.exe" ': ???????????
    > pip3
    Fatal error in launcher: Unable to create process using '"c:\python38\python3.exe"  "C:\Python38\Scripts\pip3.exe" ': ???????????
    
    ```

    A：

    ``` text
    Use 010 editor, edit C: \ Python27 \ Scripts \ pip2.exe,
    
    Amended as follows catalog program name
    ```

    ![](E:\ws\mjx\scripts\python\问题1-1.png)

    ![](E:\ws\mjx\scripts\python\问题1-2.png)