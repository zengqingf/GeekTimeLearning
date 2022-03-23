# coding=utf-8
# -------------------------------------------------------------------------
#
#  Part of the CodeChecker project, under the Apache License v2.0 with
#  LLVM Exceptions. See LICENSE for license information.
#  SPDX-License-Identifier: Apache-2.0 WITH LLVM-exception
#
# -------------------------------------------------------------------------
""" Setup for the test package tu_collector. """

import os
import shutil
import tempfile


# Test workspace should be initialized in this module.
TEST_WORKSPACE = None


def get_workspace(test_id='test'):
    """ Return a temporary workspace for the tests. """
    workspace_root = os.environ.get("TU_COLLECTOR_TEST_WORKSPACE_ROOT")
    if not workspace_root:
        # if no external workspace is set create under the build dir
        workspace_root = os.path.join(os.environ['REPO_ROOT'], 'build',
                                      'workspace')

    if not os.path.exists(workspace_root):
        os.makedirs(workspace_root)

    if test_id:
        return tempfile.mkdtemp(prefix=test_id + "-", dir=workspace_root)
    else:
        return workspace_root


def setup_package():
    """ Setup the environment for the tests. """

    global TEST_WORKSPACE
    TEST_WORKSPACE = get_workspace('tu_collector')

    print(TEST_WORKSPACE)
    os.environ['TEST_WORKSPACE'] = TEST_WORKSPACE


def teardown_package():
    """ Delete the workspace associated with this test. """

    # TODO: If environment variable is set keep the workspace
    # and print out the path.
    global TEST_WORKSPACE

    print("Removing: " + TEST_WORKSPACE)
    shutil.rmtree(TEST_WORKSPACE)
