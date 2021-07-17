#!/usr/bin/env python3

import datetime
import inspect
import os
import platform
import re
import subprocess
import sys
import tempfile

SCRIPT_NAME = "GitUnityProjectLauncher"
SCRIPT_VERSION = 1.0
SUPPORTED_PLATFORMS = ["Linux"]
LOCAL_BRANCH_NAME = "master"
REMOTE_BRANCH_NAME = f"origin/{LOCAL_BRANCH_NAME}"
PROJECT_VERSION_FILE = os.path.join("ProjectSettings", "ProjectVersion.txt")
UNITY_EDITOR_PATH = os.path.join(os.path.expanduser("~"), ".local", "share", "unity-editor")

GIT_EXEC_PATH = ""
PROJECT_DIRECTORY = ""
LOG_DIRECTORY = ""
LOGGER = None


class SimpleLogger(object):
    """Simple logging class"""
    log_to_console = True
    log_to_file = True
    log_file_path = None

    __first_message_logged = False

    def __init__(self):
        """Initialize logger variables"""
        self.log_file_path = os.path.join(LOG_DIRECTORY, get_log_file_name(SCRIPT_NAME))

    def __write_to_log(self, log_level, message):
        """Writes the given message to the log file as well as the console"""
        date_stamp = self.__get_date_stamp()
        class_path = self.__get_class_path()
        log_msg = f"[{date_stamp}] [{log_level}] {class_path}:\t{message}"

        if not self.__first_message_logged:
            self.__first_message_logged = True
            with open(self.log_file_path, "a") as log_file:
                log_file.write("\n\n")
            self.info_msg(f"Log file initialized at: '{self.log_file_path}'")

        if self.log_to_file:
            with open(self.log_file_path, "a") as log_file:
                log_file.write(log_msg)
                log_file.write("\n")

        if self.log_to_console:
            print(log_msg)

    @staticmethod
    def __get_class_path(skip=2):
        """
        Get a name of a caller in the format module.class.method

        `skip` specifies how many levels of stack to skip while getting caller
        name. skip=1 means "who calls me", skip=2 "who calls my caller" etc.

        An empty string is returned if skipped levels exceed stack height

        Source: https://gist.github.com/techtonik/2151727
        """

        def stack_(frame):
            framelist = []
            while frame:
                framelist.append(frame)
                frame = frame.f_back
            return framelist

        stack = stack_(sys._getframe(1))
        start = 0 + skip
        if len(stack) < start + 1:
            return ''
        parentframe = stack[start]

        name = []
        module = inspect.getmodule(parentframe)
        # `modname` can be None when frame is executed directly in console
        if module:
            name.append(module.__name__)
        # detect classname
        if 'self' in parentframe.f_locals:
            # I don't know any way to detect call from the object method
            # XXX: there seems to be no way to detect static method callit will
            #      be just a function call
            name.append(parentframe.f_locals['self'].__class__.__name__)
        codename = parentframe.f_code.co_name
        if codename != '<module>':  # top level usually
            name.append(codename)  # function or a method
        del parentframe

        if "__main__" in name:
            if len(name) > 1:
                name.remove("__main__")
            else:
                name[name.index("__main__")] = "main"

        if "__write_to_log" in name:
            name.remove("__write_to_log")

        return ".".join(name)

    @staticmethod
    def __get_date_stamp():
        """Returns a date stamp string"""
        date = datetime.datetime.now()
        return date.strftime("%Y-%m-%d %H:%M:%S:%f")

    def info_msg(self, message):
        """Logs an INFO message"""
        self.__write_to_log("INFO", message)

    def error_msg(self, message):
        """Logs an ERROR message"""
        self.__write_to_log("ERROR", message)

    def fatal_msg(self, message, exit_code=1):
        """Logs an FATAL message"""
        self.__write_to_log("FATAL", message)
        exit(exit_code)


def active_local_changes():
    """
    Checks if there are any pending local changes tracked by Git
    TODO: This might not be a 100% accurate way to check this sort of thing
    :return: True if there are any changes, False if not
    """
    git_status = subprocess.check_output([GIT_EXEC_PATH, "status", "--short"], cwd=PROJECT_DIRECTORY).decode().strip()

    if len(git_status) > 0:
        return True
    else:
        return False


def dir_is_writable(directory_path):
    """
    Checks to see if the given path is writable.
    :param directory_path: Path to the directory we want to check
    :return: True if successfully wrote testing file to directory, False if not
    """
    test_file_path = os.path.join(directory_path, "DIR_WRITE_TEST.TESTING_123")

    try:
        with open(test_file_path, "w") as test:
            test.write("test")
    except IOError:
        return False
    finally:
        if os.path.isfile(test_file_path):
            os.remove(test_file_path)
    return True


def execute_git_command(command_args):
    """
    Executes a single Git command
    :param command_args: List of git command arguments
    :return: Void
    """
    cmd = [GIT_EXEC_PATH]
    cmd.extend(command_args)

    LOGGER.info_msg(f"Executing Git command: '{' '.join(command_args)}'")

    cmd_process = subprocess.Popen(cmd, cwd=PROJECT_DIRECTORY, stdout=subprocess.PIPE, stderr=subprocess.PIPE)

    for line in cmd_process.stdout.readlines():
        LOGGER.info_msg(f"git {' '.join(command_args)}: {line.decode().strip()}")

    for line in cmd_process.stderr.readlines():
        LOGGER.error_msg(f"git {' '.join(command_args)}: {line.decode().strip()}")

    if cmd_process.returncode:
        LOGGER.fatal_msg(f"'{' '.join(cmd)}' returned a non-zero exit code: {cmd_process.returncode}")


def get_branch_latest_commit_id(branch_name):
    """
    Gets the latest Git commit ID from the given branch
    :param branch_name: Name of the Git branch we want to get the commit ID for
    :return: Commit ID as a string
    """
    commit_regex = r"(commit ([a-zA-Z0-9]{40}))"
    git_output = subprocess.check_output([GIT_EXEC_PATH, "show", branch_name], cwd=PROJECT_DIRECTORY).decode()
    commit_info = re.search(commit_regex, git_output)

    if len(commit_info.groups()) == 2:
        return commit_info.group(2)

    return "Unknown"


def get_branch_commit_id_list(branch_name):
    """
    Lists all commit IDs in the given Git branch
    :param branch_name: Name of the Git branch we want to list commit IDs for
    :return: List of commit ID strings
    """
    commit_id_list = []
    git_output = subprocess.check_output([GIT_EXEC_PATH, "rev-list", "--remotes", branch_name],
                                         cwd=PROJECT_DIRECTORY).decode().split("\n")

    for commit in git_output:
        if commit not in commit_id_list:
            commit_id_list.append(commit)

    return commit_id_list


def get_log_file_name(base_file_name):
    """
    Generates a file name for a log file
    :param base_file_name: Desired base log file name
    :return: Formatted log file name as a string
    """
    return f"{base_file_name}.{datetime.datetime.now().strftime('%Y-%m-%d_%H:%M:%S')}.log"


def get_project_unity_version():
    """
    Parses the Unity version from the project's version file
    :return: Project's Unity version as a string
    """
    unity_version_key = "m_EditorVersion: "

    with open(os.path.join(PROJECT_DIRECTORY, PROJECT_VERSION_FILE), "r") as version_file:
        for line in version_file.readlines():
            if unity_version_key in line:
                return line.split(unity_version_key)[1].strip()

    LOGGER.fatal_msg("Unable to determine project's Unity version", exit_code=103)


def get_unity_editor_exec(unity_version):
    """
    Gets the path to the Unity Editor for the given Unity version
    :param unity_version: Unity version we want to get the Editor exec for
    :return: Path to the Unity Editor exec as a string
    """

    editor_exec = os.path.join(UNITY_EDITOR_PATH, unity_version, "Editor", "Unity")

    if os.path.isfile(editor_exec):
        return editor_exec

    LOGGER.fatal_msg(f"Unable to locate Unity Editor installation for project's Unity version '{unity_version}'",
                     exit_code=105)


def open_project_in_editor():
    """
    Launches the project in the Unity Editor
    :return: Void
    """
    editor_exec = get_unity_editor_exec(get_project_unity_version())
    editor_log_file = os.path.join(LOG_DIRECTORY, get_log_file_name(f"UnityEditor.v{get_project_unity_version()}"))
    cmd = [editor_exec, "-projectPath", PROJECT_DIRECTORY, "-logFile", editor_log_file]

    LOGGER.info_msg(f"Opening project in Unity editor. Editor log file: '{editor_log_file}'")
    subprocess.Popen(cmd, cwd=PROJECT_DIRECTORY)


def platform_check():
    """
    Verifies this operating system is capable of executing this script.
    Exits the script immediately if host OS is unsupported.
    :return: Void
    """
    if platform.system() not in SUPPORTED_PLATFORMS:
        print(f"Platform '{platform.system()}' is not supported by this script")
        exit(102)


def print_heads_up_info():
    """
    Prints important script variables
    :return: Void
    """
    LOGGER.info_msg(f"\n"
                    f"Host Operating System           '{platform.system()}'\n"
                    f"Git Executable Path             '{GIT_EXEC_PATH}'\n"
                    f"Project Directory               '{PROJECT_DIRECTORY}'\n"
                    f"Project Unity Version           '{get_project_unity_version()}'\n"
                    f"Unity Editor Executable Path    '{get_unity_editor_exec(get_project_unity_version())}'\n"
                    f"Current Project Commit          '{get_branch_latest_commit_id(LOCAL_BRANCH_NAME)}'\n"
                    f"Local Branch Name               '{LOCAL_BRANCH_NAME}'\n"
                    f"Remote Branch Name              '{REMOTE_BRANCH_NAME}'\n"
                    f"Script Log File                 '{LOGGER.log_file_path}'\n"
                    f"Script Version                  '{SCRIPT_VERSION}'")


def prompt_for_user_input(prompt_message):
    """
    Prompts the user with a yes or no question and waits for valid yes or no input
    :param prompt_message: Message to be displayed to the user
    :return: True if the user inputs a 'yes' message, False if the user inputs a 'no' message
    """
    yes = ["y", "ye", "yes", "yeah", "sure"]
    no = ["n", "no", "nah"]
    prompt_message = f"{prompt_message} [y/n]: "

    LOGGER.info_msg(f"Prompting user for input: '{prompt_message}'")

    while True:
        user_input = input(prompt_message).lower()

        if user_input in yes:
            LOGGER.info_msg("User selected 'yes' option")
            return True
        elif user_input in no:
            LOGGER.info_msg("User selected 'no' option")
            return False


def set_git_exec_path():
    """
    Locates the git executable on the system and stores it's location as a constant.
    If Git is found, set the GIT_EXEC_PATH. If not exit unsuccessfully.
    :return: Void
    """
    git_exec_path = subprocess.check_output(["/usr/bin/which", "git"]).decode().strip()
    if os.path.exists(os.path.abspath(git_exec_path)):
        global GIT_EXEC_PATH
        GIT_EXEC_PATH = os.path.abspath(git_exec_path)

        return

    print("Git is not installed on this system or could not be located, aborting.")
    exit(100)


def set_log_directory():
    """
    Determines the folder to store log files in and sets the LOG_DIRECTORY constant.
    Attempts to use the 'Logs' folder in the Unity project directory, defaults to the temp folder if unable to.
    :return: Void
    """
    log_dir = os.path.join(PROJECT_DIRECTORY, "Logs")

    # Attempt to create the log directory if it does not exist
    if not os.path.exists(log_dir):
        try:
            os.makedirs(log_dir)
        except:
            pass

    # If the log directory is not a directory or is not writable, use the temp folder instead
    if not os.path.isdir(log_dir) or not dir_is_writable(log_dir):
        log_dir = tempfile.gettempdir()

    # Set LOG_DIRECTORY constant
    global LOG_DIRECTORY
    LOG_DIRECTORY = log_dir


def set_project_directory():
    """
    Verifies this script is executing from within a valid Unity project directory.
    If this is a valid Unity project directory, sets the PROJECT_DIRECTORY constant.
    Otherwise exits the script unsuccessfully.
    :return: Void
    """
    script_folder = os.path.abspath(os.path.dirname(__file__))

    if os.path.isfile(os.path.join(script_folder, PROJECT_VERSION_FILE)) and dir_is_writable(script_folder):
        global PROJECT_DIRECTORY
        PROJECT_DIRECTORY = os.path.abspath(script_folder)

        return

    print("This script does not seem to be located in a valid Unity project directory, aborting.")
    exit(101)


def verify_project_is_git_repo():
    """
    Verifies the project directory is a valid Git repo
    :return: Void
    """
    git_status_check = -1

    try:
        git_status_check = subprocess.check_call([GIT_EXEC_PATH, "status"],
                                                 cwd=PROJECT_DIRECTORY,
                                                 stdout=subprocess.DEVNULL)
    except:
        pass

    if os.path.isdir(os.path.join(PROJECT_DIRECTORY, ".git")) and git_status_check == 0:
        return

    LOGGER.fatal_msg("This Unity project does not seem to be tracked via Git, aborting.", exit_code=104)


if __name__ == "__main__":
    # Verify script is running on a supported system, then set constants
    platform_check()
    set_git_exec_path()
    set_project_directory()
    set_log_directory()
    LOGGER = SimpleLogger()

    # Verify project is a Git repo and that we have a compatible Unity Editor installed
    verify_project_is_git_repo()
    get_unity_editor_exec(get_project_unity_version())

    # Print heads up info
    print_heads_up_info()

    # Print message if there are un-committed changes
    if active_local_changes():
        LOGGER.info_msg(f"Project directory has local changes that have not been committed")

    # Pull down latest changes from remote
    LOGGER.info_msg(f"Downloading latest changes from '{REMOTE_BRANCH_NAME}'...")
    execute_git_command(["remote", "update"])

    # Pull changes into local branch
    LOGGER.info_msg(f"Attempting to pull latest changes into local branch '{LOCAL_BRANCH_NAME}'...")
    if get_branch_latest_commit_id(LOCAL_BRANCH_NAME) not in get_branch_commit_id_list(
            REMOTE_BRANCH_NAME) or active_local_changes():
        LOGGER.info_msg("Theres a good chance the pull will require manual review and / or merging, be prepared")
    execute_git_command(["pull"])

    # Check if we need to push local commits to remote
    if get_branch_latest_commit_id(LOCAL_BRANCH_NAME) not in get_branch_commit_id_list(REMOTE_BRANCH_NAME):
        LOGGER.info_msg(f"Commit '{get_branch_latest_commit_id(LOCAL_BRANCH_NAME)}'"
                        f" is not in '{REMOTE_BRANCH_NAME}', likely needs to be pushed")
        user_input = prompt_for_user_input(f"Push local commit '{get_branch_latest_commit_id(LOCAL_BRANCH_NAME)}'"
                                           f" to '{REMOTE_BRANCH_NAME}'?")
        if user_input:
            LOGGER.info_msg(f"Pushing '{get_branch_latest_commit_id(LOCAL_BRANCH_NAME)}' to '{REMOTE_BRANCH_NAME}'...")
            execute_git_command(["push"])

    # Open the project in the Unity editor
    open_project_in_editor()
    exit(0)
