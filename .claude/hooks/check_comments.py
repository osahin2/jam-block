#!/usr/bin/env python3
"""Checks the comments of C# files in Assets/Game and the Core package (Docs/Conventions/CodeStyle.md).

Claude Code hook: reads the hook payload from stdin and checks the edited .cs file. It reports the comments that
break a rule and every comment the edit added, so each new comment gets a second look; then it exits 2, so Claude
sees the report. Run by hand: check_comments.py <file.cs> [...]  (rules only; exit 1 when any file breaks one).

Line based: full-line `//` comments only.
"""
import json
import os
import re
import subprocess
import sys

CHECKED_ROOTS = ("Assets/Game/", "Packages/com.base-setup.core/")
MAX_BLOCK_LINES = 2
COMMENT = re.compile(r"^\s*//")
XML_DOC = re.compile(r"^\s*///")
ATTRIBUTE = re.compile(r"^\s*\[")
TYPE_DECLARATION = re.compile(
    r"^\s*((public|internal|private|protected|static|sealed|abstract|partial|readonly)\s+)*"
    r"(class|struct|enum|record)\b")


def comment_blocks(lines):
    """Yields (first line index, line count) for each run of full-line comments."""
    i = 0
    while i < len(lines):
        if COMMENT.match(lines[i]):
            start = i
            while i < len(lines) and COMMENT.match(lines[i]):
                i += 1
            yield start, i - start
        else:
            i += 1


def next_code_line(lines, index):
    while index < len(lines) and (not lines[index].strip() or ATTRIBUTE.match(lines[index])):
        index += 1
    return lines[index] if index < len(lines) else ""


def rule_problems(lines):
    problems = []
    for start, count in comment_blocks(lines):
        where = f"line {start + 1}"
        if XML_DOC.match(lines[start]):
            problems.append(f"{where}: XML doc comment; names and signatures document the code")
        if count > MAX_BLOCK_LINES:
            problems.append(f"{where}: {count}-line comment; a reason fits in {MAX_BLOCK_LINES} lines")
        if TYPE_DECLARATION.match(next_code_line(lines, start + count)):
            problems.append(f"{where}: comment on a type; the type's name and its system note say what it is")
    return problems


def comments(text):
    return [line.strip() for line in text.splitlines() if COMMENT.match(line)]


def committed_text(path):
    result = subprocess.run(["git", "show", f"HEAD:{path}"], capture_output=True, text=True,
                            cwd=os.environ.get("CLAUDE_PROJECT_DIR", "."))
    return result.stdout if result.returncode == 0 else ""


def added_comments(tool_name, tool_input, relative_path):
    if tool_name == "Write":
        before = set(comments(committed_text(relative_path)))
        after = comments(tool_input.get("content", ""))
    else:
        edits = tool_input.get("edits") or [tool_input]
        before = set()
        after = []
        for edit in edits:
            before.update(comments(edit.get("old_string", "")))
            after.extend(comments(edit.get("new_string", "")))
    return [line for line in after if line not in before]


def relative_to_project(path):
    root = os.environ.get("CLAUDE_PROJECT_DIR", os.getcwd())
    return os.path.relpath(os.path.abspath(path), root).replace(os.sep, "/")


def is_checked(relative_path):
    return relative_path.endswith(".cs") and relative_path.startswith(CHECKED_ROOTS)


def read_lines(path):
    with open(path, encoding="utf-8-sig") as file:
        return file.read().splitlines()


def run_hook():
    payload = json.load(sys.stdin)
    tool_input = payload.get("tool_input", {})
    path = tool_input.get("file_path", "")
    relative_path = relative_to_project(path)
    if not is_checked(relative_path) or not os.path.exists(path):
        return 0
    problems = rule_problems(read_lines(path))
    added = added_comments(payload.get("tool_name", ""), tool_input, relative_path)
    if not problems and not added:
        return 0
    report = [f"Comments in {relative_path}:"]
    report += [f"  {problem}" for problem in problems]
    if added:
        report.append("  Added by this edit; keep each only if it states a reason, an invariant or an outside "
                      "fact the code cannot say, delete it otherwise:")
        report += [f"    {line}" for line in added]
    print("\n".join(report), file=sys.stderr)
    return 2


def run_by_hand(paths):
    failed = False
    for path in paths:
        problems = rule_problems(read_lines(path))
        for problem in problems:
            print(f"{path}: {problem}")
        failed = failed or bool(problems)
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(run_by_hand(sys.argv[1:]) if len(sys.argv) > 1 else run_hook())
