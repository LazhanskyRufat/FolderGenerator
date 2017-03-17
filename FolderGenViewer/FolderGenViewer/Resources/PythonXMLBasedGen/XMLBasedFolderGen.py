###########################################
# XML-file based project folder generator #
# Author: Rufat Lazhansky                 #
###########################################

import os, shutil, subprocess, platform
from subprocess import Popen
from xml.etree import ElementTree

XML_FILE_PATH = './XMLFolderStructure.xml'
XML_NAME_ATTRIB = 'name'
XML_REF_PATH_ATTRIB = 'path'
XML_TEMPLATE_TOKEN_ATTRIB = 'token'
XML_OUTPUT_DIR_ATTRIB = 'outputDir'
XML_FILE_TAG = 'file'
XML_FOLDER_TAG = 'folder'
XML_REFERENCE_TAG = 'reference'
XML_DUMMY_FILE_NAME = '.dummy'

def openProjectFolder(path):
    if platform.system() == "Windows":
        os.startfile(path)
    elif platform.system() == "Darwin":
        Popen(["open", path])
    else:
        Popen(["xdg-open", path])

def createEmptyFile(filePath):
    fileDescriptor = os.open(filePath, os.O_RDWR|os.O_CREAT)
    os.close(fileDescriptor)

def createSimpleFile(filePath, content):
    fileDescriptor = os.open(filePath, os.O_RDWR|os.O_CREAT)
    os.write(fileDescriptor, bytes(content + '\n', 'utf-8'))
    os.close(fileDescriptor)

def walkChildren(node, parentFolderPath, token, projectNumber):
    children = node.getchildren()
    if(not children):
        createEmptyFile(os.path.join(parentFolderPath, XML_DUMMY_FILE_NAME))
    else:
        for child in children:
            childPath = os.path.join(parentFolderPath, str(child.attrib.get(XML_NAME_ATTRIB)).replace(token, projectNumber))
            print(childPath)
            if(child.tag == XML_FOLDER_TAG):
                os.makedirs(childPath)
                walkChildren(child, childPath, token, projectNumber)
            if(child.tag == XML_REFERENCE_TAG):
                referencePath = child.attrib.get(XML_REF_PATH_ATTRIB)
                if(os.path.exists(referencePath)):
                    resultPath = shutil.copytree(referencePath, childPath)
                else:
                    print('reference: {0} not found. Creating empty folder instead.'.format(os.path.abspath(referencePath)))
                    os.makedirs(childPath)
                    createEmptyFile(os.path.join(childPath, XML_DUMMY_FILE_NAME))
            if(child.tag == XML_FILE_TAG):
                content = child.text
                if(not content):
                    createEmptyFile(childPath)
                else:
                    createSimpleFile(childPath, content)

def createUnityProject():
    xmlTree = ElementTree.parse(XML_FILE_PATH).getroot()
    projectFolderName = str(xmlTree.attrib.get(XML_NAME_ATTRIB))
    projectTemplateToken = str(xmlTree.attrib.get(XML_TEMPLATE_TOKEN_ATTRIB))
    while True:
        try:
            formatString ='{0:0' + str(len(projectTemplateToken)) + 'd}'
            projectNumber = formatString.format(int(input('Input episode number or type Ctrl+Z to exit: ')))
            resultProjectFolderName = projectFolderName.replace(projectTemplateToken, projectNumber)
            outputFolderPath = str(xmlTree.attrib.get(XML_OUTPUT_DIR_ATTRIB))
            if(not outputFolderPath):
                projectFolderPath = os.path.join(os.path.curdir, resultProjectFolderName)
            else:
                projectFolderPath = os.path.join(outputFolderPath, resultProjectFolderName)
            try:
                os.makedirs(projectFolderPath)
                walkChildren(xmlTree, projectFolderPath, projectTemplateToken, projectNumber)
                return os.path.abspath(projectFolderPath)
            except OSError:
                print(projectFolderPath + ' already exists, try another one')
        except EOFError:
            input('Exiting now! Press Enter to continue. ')
            quit()
        except ValueError:
            print('Input ONLY intger number, please. Try again')

projectFolderPath = createUnityProject()
openProjectFolder(projectFolderPath)

os.chdir('../../')
Popen("make_symlinks.bat")
