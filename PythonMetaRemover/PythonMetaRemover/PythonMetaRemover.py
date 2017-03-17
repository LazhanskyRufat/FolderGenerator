import os
from pathlib import Path

META_EXTENTION = '.meta'

def removeUnnecessaryMetafiles(path):
    for root, dirs, files in os.walk(path):
        for file in files:
            extention = Path(file).suffix
            fileName = Path(file).name
            fileName = fileName.replace(extention, '')
            if(extention == META_EXTENTION):
                dirOrFileFound = False
                for dir in dirs:
                    dirName =  Path(dir).name
                    if(dirName == fileName):
                        dirOrFileFound = True
                        break
                for otherFile in files:
                    otherFileExtention = Path(otherFile).suffix
                    if(not (otherFileExtention == META_EXTENTION)):
                        otherFileName = Path(otherFile).name
                        if(otherFileName == fileName):
                            dirOrFileFound = True
                            break
                if(not dirOrFileFound):
                    os.remove(os.path.join(root, file))

removeUnnecessaryMetafiles('.')