import sys
import os
import subprocess
import shutil
import re
import json
import time
import datetime
import config
import util

try:
    import git
except ImportError:
    os.system("pip3 install --user gitpython")
    import git


def pull(repopath):
    if os.path.exists(repopath):
        print("============Pulling " + repopath)
        repo = git.Repo(repopath)
        print(repo.git.pull())
        print("")


def pull_clone_def(projectpath, repo_def):
    relative_path = config.get_repo_relative_path(repo_def)
    repo_path = os.path.join(projectpath, relative_path)
    if os.path.exists(repo_path):
        print("============Pulling " + relative_path)
        repo = git.Repo(repo_path)
        print(repo.git.pull())
        print("")
    else:
        print("============Cloning " + relative_path)
        repo_url = config.get_repo_url(repo_def)
        print(git.Repo.clone_from(repo_url, repo_path))
        print("")


def pull_clone_group(projectpath, buildtarget, repo_group):
    util.checkdir(projectpath)
    repos = config.repo_groups[repo_group]
    pull_clone_def(projectpath, buildtarget)
    for repo in repos:
        pull_clone_def(projectpath, repo)
