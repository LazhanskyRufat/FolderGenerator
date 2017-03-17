import os, shutil, subprocess, platform
from subprocess import Popen

PROJECT_FOLDER = "D:/Unity Projects/Lantern/.git/hooks"

def openProjectFolder(path):
    if platform.system() == "Windows":
        os.startfile(path)
    elif platform.system() == "Darwin":
        Popen(["open", path])
    else:
        Popen(["xdg-open", path])

openProjectFolder(PROJECT_FOLDER)