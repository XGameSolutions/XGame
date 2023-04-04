
import sys
import os
import config
import util
import util_git

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print(
            "Usage: python3 gittool.py <project path> <build target> [repo group]")
        exit(1)
    projectpath = sys.argv[1]
    buildtarget = sys.argv[2]
    repo_group = "develop"
    if len(sys.argv) > 3:
        repo_group = sys.argv[3]
    util_git.pull_clone_group(projectpath, buildtarget, repo_group)
