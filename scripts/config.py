
import os

repo_root_url = "https://github.com/XGameSolutions"

repo_defs = {
    "XClientLua":           ("../../XClientLua",            repo_root_url + "/XGame-XClientLua.git"),
    "XConfig":              ("../../XConfig",               repo_root_url + "/XGame-XConfig.git"),
    "XCommon":              ("../../XCommon",               repo_root_url + "/XGame-XCommon.git"),
    "XDriver":              ("../../XDriver",               repo_root_url + "/XGame-XDriver.git"),
    "XServer":              ("../../XServer",               repo_root_url + "/XGame-XServer.git"),
    "ProjectSettings":      ("ProjectSettings",             repo_root_url + "/XGame-XClient-ProjectSettings.git"),
    "Packages":             ("Packages",                    repo_root_url + "/XGame-XClient-Packages.git"),
    "StandaloneWindow64":   ("ResAB/StandaloneWindow64",    repo_root_url + "/XGame-XClient-StandaloneWindow64.git"),
    "StandaloneOSX":        ("ResAB/StandaloneOSX",         repo_root_url + "/XGame-XClient-StandaloneOSX.git"),
    "iOS":                  ("ResAB/iOS",                   repo_root_url + "/XGame-XClient-iOS.git"),
    "Android":              ("ResAB/Android",               repo_root_url + "/XGame-XClient-Android.git"),
    "Editors":              ("Assets/Editors",              repo_root_url + "/XGame-XClient-Editors.git"),
    "Plugins":              ("Assets/Plugins",              repo_root_url + "/XGame-XClient-Plugins.git"),
    "ResData":              ("Assets/ResData",              repo_root_url + "/XGame-XClient-ResData.git"),
    "ResPublic":             ("Assets/ResPublic",             repo_root_url + "/XGame-XClient-ResPublic.git"),
    "ResEffect":            ("Assets/ResEffect",            repo_root_url + "/XGame-XClient-ResEffect.git"),
    "ResModel":             ("Assets/ResModel",             repo_root_url + "/XGame-XClient-ResModel.git"),
    "ResShader":            ("Assets/ResShader",            repo_root_url + "/XGame-XClient-ResShader.git"),
    "ResScene":             ("Assets/ResScene",             repo_root_url + "/XGame-XClient-ResScene.git"),
    "ResSceneModel":        ("Assets/ResSceneModel",        repo_root_url + "/XGame-XClient-ResSceneModel.git"),
    "ResUI":                ("Assets/ResUI",                repo_root_url + "/XGame-XClient-ResUI.git"),
    "Runtime":              ("Assets/Runtime",              repo_root_url + "/XGame-XClient-Runtime.git"),
}

repo_basic = ("ProjectSettings", "Packages",
              "Editors", "Plugins", "Runtime")
repo_all_res = ("ResPublic", "ResEffect", "ResModel",
                "ResShader", "ResScene", "ResSceneModel", "ResUI")
repo_all_lua = ("XClientLua", "XConfig", "XCommon", "XServer")

repo_groups = {
    "developer": repo_basic + repo_all_res + repo_all_lua,
    "qa": repo_basic + repo_all_lua,
}


def get_repo_relative_path(repo_name):
    return repo_defs[repo_name][0]


def get_repo_url(repo_name):
    return repo_defs[repo_name][1]
