
import os

def checkdir(dir):
    if not os.path.exists(dir):
        os.makedirs(dir)