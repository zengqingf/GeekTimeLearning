# -------------------------------------------------------------------------
#
#  Part of the CodeChecker project, under the Apache License v2.0 with
#  LLVM Exceptions. See LICENSE for license information.
#  SPDX-License-Identifier: Apache-2.0 WITH LLVM-exception
#
# -------------------------------------------------------------------------
"""
Contains functions to format and pretty-print data from two-dimensional arrays.
"""


import json
from operator import itemgetter


def to_str(format_name, keys, rows,
           sort_by_column_number=None, rev=False,
           separate_footer=False):
    """
    Converts the given two-dimensional array (with the specified keys)
    to the given format.
    """
    if sort_by_column_number is not None:
        rows.sort(key=itemgetter(sort_by_column_number), reverse=rev)

    all_rows = rows
    if keys:
        all_rows = [keys] + list(rows)

    if format_name == 'rows':
        return __to_rows(rows)
    elif format_name == 'table' or format_name == 'plaintext':
        # TODO: 'plaintext' for now to support the 'CodeChecker cmd' interface.
        return __to_table(all_rows, True, separate_footer)
    elif format_name == 'csv':
        return __to_csv(all_rows)
    elif format_name == 'dictlist':
        return __to_dictlist(keys, rows)
    elif format_name == 'json':
        return json.dumps(__to_dictlist(keys, rows))
    else:
        raise ValueError("Unsupported format")


def __to_rows(lines):
    """
    Prints the given rows with minimal formatting.
    """

    str_parts = []

    lines = [['' if e is None else e for e in line] for line in lines]

    # Count the column width.
    widths = []
    for line in lines:
        for i, size in enumerate([len(str(x)) for x in line]):
            while i >= len(widths):
                widths.append(0)
            if size > widths[i]:
                widths[i] = size

    # Generate the format string to pad the columns.
    print_string = " "
    for i, width in enumerate(widths):
        if i == 0 or i == len(widths) - 1 or width == 0:
            print_string += "{" + str(i) + "} "
        else:
            print_string += "{" + str(i) + ":" + str(width) + "} "
    if not print_string:
        return
    print_string = print_string[:-1]

    # Print the actual data.
    for i, line in enumerate(lines):
        try:
            str_parts.append(print_string.format(*line))
        except IndexError:
            raise TypeError("One of the rows have a different number of "
                            "columns than the others")

    return '\n'.join(str_parts)


def __to_table(lines, separate_head=True, separate_footer=False):
    """
    Pretty-prints the given two-dimensional array's lines.
    """

    str_parts = []

    # It is possible that one of the item in the line is None which will
    # raise an exception when passed to the format function below. So this is
    # the reason why we need to convert None values to valid strings here.
    lines = [['' if e is None else e for e in line] for line in lines]

    # Count the column width.
    widths = []
    for line in lines:
        for i, size in enumerate([len(str(x)) for x in line]):
            while i >= len(widths):
                widths.append(0)
            if size > widths[i]:
                widths[i] = size

    # Generate the format string to pad the columns.
    print_string = ""
    for i, width in enumerate(widths):
        print_string += "{" + str(i) + ":" + str(width) + "} | "
    if not print_string:
        return
    print_string = print_string[:-3]

    # Print the actual data.
    str_parts.append("-" * (sum(widths) + 3 * (len(widths) - 1)))
    for i, line in enumerate(lines):
        try:
            str_parts.append(print_string.format(*line))
        except IndexError:
            raise TypeError("One of the rows have a different number of "
                            "columns than the others")
        if i == 0 and separate_head:
            str_parts.append("-" * (sum(widths) + 3 * (len(widths) - 1)))
        if separate_footer and i == len(lines) - 2:
            str_parts.append("-" * (sum(widths) + 3 * (len(widths) - 1)))

    str_parts.append("-" * (sum(widths) + 3 * (len(widths) - 1)))

    return '\n'.join(str_parts)


def __to_csv(lines):
    """
    Pretty-print the given two-dimensional array's lines in CSV format.
    """

    str_parts = []

    lines = [['' if e is None else e for e in line] for line in lines]

    # Count the columns.
    columns = 0
    for line in lines:
        if len(line) > columns:
            columns = len(line)

    print_string = ""
    for i in range(columns):
        print_string += "{" + str(i) + "},"

    if not print_string:
        return
    print_string = print_string[:-1]

    # Print the actual data.
    for line in lines:
        try:
            str_parts.append(print_string.format(*line))
        except IndexError:
            raise TypeError("One of the rows have a different number of "
                            "columns than the others")

    return '\n'.join(str_parts)


def __to_dictlist(key_list, lines):
    """
    Pretty-print the given two-dimensional array's lines into a JSON
    object list. The key_list acts as the "header" of the table, specifying the
    keys to use in the resulting object.

    This function expects values to be the same number as the length of
    key_list, and that the order of values in a line corresponds to the order
    of keys.
    """

    res = []
    for line in lines:
        res.append({key: value for (key, value) in zip(key_list, line)})

    return res
