#!/usr/bin/env python3
"""Checks the member order of C# classes and structs (Docs/Conventions/CodeStyle.md).

Claude Code hook: reads the hook payload from stdin and checks the edited .cs file. When the order is
wrong it prints the problems to stderr and exits 2, so Claude sees them and fixes the file.
Run by hand: check_member_order.py <file.cs> [...]  (exit 1 when any file is out of order).

Line based and heuristic: it understands ordinary declarations, not the whole C# grammar.
"""
import json
import os
import re
import sys

GROUPS = {
    1: "constants and static members",
    2: "events",
    3: "serialized fields",
    4: "other fields",
    5: "properties",
    6: "constructors, [Inject] Construct and Init",
    7: "Unity messages",
    8: "public and protected methods",
    9: "private methods",
}
NESTED_TYPE = 10  # nested types may stand anywhere; they are not checked

UNITY_LIFECYCLE = ["Awake", "OnEnable", "Reset", "Start", "FixedUpdate", "Update", "LateUpdate",
                   "OnDisable", "OnDestroy"]
UNITY_OTHER = {"OnValidate", "OnApplicationPause", "OnApplicationFocus", "OnApplicationQuit", "OnGUI",
               "OnDrawGizmos", "OnDrawGizmosSelected", "OnBecameVisible", "OnBecameInvisible",
               "OnTriggerEnter", "OnTriggerStay", "OnTriggerExit", "OnTriggerEnter2D", "OnTriggerStay2D",
               "OnTriggerExit2D", "OnCollisionEnter", "OnCollisionStay", "OnCollisionExit",
               "OnCollisionEnter2D", "OnCollisionStay2D", "OnCollisionExit2D", "OnMouseDown", "OnMouseUp",
               "OnRectTransformDimensionsChange", "OnTransformParentChanged",
               # EditorWindow messages
               "CreateGUI", "OnFocus", "OnLostFocus", "OnSelectionChange", "OnInspectorUpdate",
               "OnHierarchyChange", "OnProjectChange"}
UNITY_MESSAGES = set(UNITY_LIFECYCLE) | UNITY_OTHER

MODIFIERS = {"public", "private", "protected", "internal", "static", "readonly", "const", "virtual",
             "override", "abstract", "sealed", "async", "extern", "new", "partial", "unsafe", "volatile",
             "event", "required"}
TYPE_KEYWORD = re.compile(r"\b(class|struct|interface|enum|record|delegate)\b")
DELEGATE_TYPE = re.compile(r"^(System\.)?(Action|Func|UnityEvent|UnityAction|Predicate)\b")


def strip_code(text):
    """Blanks comments, strings and char literals (keeping newlines) and drops preprocessor lines."""
    out = []
    i, n = 0, len(text)
    while i < n:
        c = text[i]
        nxt = text[i + 1] if i + 1 < n else ""
        if c == "/" and nxt == "/":
            while i < n and text[i] != "\n":
                i += 1
        elif c == "/" and nxt == "*":
            end = text.find("*/", i + 2)
            end = n if end < 0 else end + 2
            out.append(re.sub(r"[^\n]", " ", text[i:end]))
            i = end
        elif c == '"' or (c in "@$" and nxt == '"') or (c in "@$" and nxt in "@$" and text[i + 2:i + 3] == '"'):
            start = i
            verbatim = False
            while text[i] != '"':
                verbatim = verbatim or text[i] == "@"
                i += 1
            if text[i:i + 3] == '"""':
                end = text.find('"""', i + 3)
                i = n if end < 0 else end + 3
            else:
                i += 1
                while i < n:
                    if verbatim and text[i] == '"' and text[i + 1:i + 2] == '"':
                        i += 2
                    elif not verbatim and text[i] == "\\":
                        i += 2
                    elif text[i] == '"':
                        i += 1
                        break
                    else:
                        i += 1
            out.append('""' + re.sub(r"[^\n]", "", text[start:i]))
        elif c == "'":
            end = i + 1
            while end < n and text[end] != "'":
                end += 2 if text[end] == "\\" else 1
            out.append("' '")
            i = end + 1
        else:
            out.append(c)
            i += 1
    lines = "".join(out).split("\n")
    return "\n".join("" if line.lstrip().startswith("#") else line for line in lines)


def outside_parentheses(text):
    """The text with every parenthesized part removed, so default parameter values do not look like initializers."""
    out, depth = [], 0
    for ch in text:
        depth += ch == "("
        if depth == 0:
            out.append(ch)
        depth -= ch == ")" and depth > 0
    return "".join(out)


def parameter_paren(text):
    """Index of the '(' that opens a parameter list, skipping tuples inside generic arguments."""
    angle = 0
    for i, ch in enumerate(text):
        angle += ch == "<"
        angle -= ch == ">" and angle > 0
        if ch == "(" and angle == 0:
            return i
    return -1


def split_attributes(header):
    attributes = []
    header = header.lstrip()
    while header.startswith("["):
        depth = 0
        for k, ch in enumerate(header):
            depth += ch == "["
            depth -= ch == "]"
            if depth == 0:
                attributes.append(header[1:k])
                header = header[k + 1:].lstrip()
                break
        else:
            break
    return attributes, header


def classify(header, terminator):
    """Returns (group, name) for a member declaration header, or None when it is not a member."""
    attributes, h = split_attributes(header)
    h = " ".join(h.split())
    if not h or not re.match(r"[A-Za-z_~(]", h):
        return None
    attribute_text = " ".join(attributes)

    keyword = TYPE_KEYWORD.search(h)
    paren = parameter_paren(h)
    if keyword and (paren < 0 or keyword.start() < paren):
        name = re.search(r"\b(?:class|struct|interface|enum|record|delegate)\b\s+(?:[\w<>,\s]+?\s+)?(\w+)", h)
        return NESTED_TYPE, name.group(1) if name else keyword.group(1)

    arrow = h.find("=>")
    assign = re.search(r"(?<![=!<>])=(?![=>])", h)
    cut = min([p for p in (arrow, assign.start() if assign else -1) if p >= 0], default=len(h))
    declaration = h[:cut].strip()
    tokens = declaration.split(" ")
    modifiers = [t for t in tokens if t in MODIFIERS]

    paren = parameter_paren(declaration)
    if paren > 0:
        before = declaration[:paren].strip()
        name = re.sub(r"<.*>$", "", before.split(" ")[-1]).split(".")[-1]
        rest = [t for t in before.split(" ")[:-1] if t not in MODIFIERS]
        if "operator" in before:
            return 8, before.split(" ")[-1]
        if not rest or name.startswith("~") or name in ("Construct", "Init") or re.search(r"\bInject\b", attribute_text):
            return 6, name
        if name in UNITY_MESSAGES:
            return 7, name
        if "private" in modifiers or not ({"public", "protected", "internal"} & set(modifiers)):
            return 9, name
        return 8, name

    name = tokens[-1] if tokens else "?"
    if "event" in modifiers:
        return 2, name
    if "const" in modifiers or "static" in modifiers:
        return 1, name
    is_property = (terminator == "{" and assign is None) or arrow >= 0
    if is_property:
        if re.search(r"\bfield\s*:\s*SerializeField\b", attribute_text):
            return 3, name
        return 5, name
    type_tokens = [t for t in tokens[:-1] if t not in MODIFIERS]
    if type_tokens and DELEGATE_TYPE.match(type_tokens[0]) and "public" in modifiers:
        return 2, name  # a public delegate field is an event in all but name
    if re.search(r"\bSerializeField\b|\bSerializeReference\b", attribute_text):
        return 3, name
    return 4, name


def members_by_type(code):
    """Yields (type_line, [(line, group, name), ...]) for every class and struct body."""
    results = []
    stack = []  # entries: dict(kind, members, line, member_open)
    header = []
    header_line = None
    recorded = False
    line = 1

    def at_type():
        return stack and stack[-1]["kind"] == "type"

    for ch in code:
        if ch == "\n":
            line += 1
        if ch == "{":
            text = "".join(header)
            if at_type() and not recorded:
                kind = classify(text, "{")
                if kind:
                    stack[-1]["members"].append((header_line, kind[0], kind[1]))
            is_type = bool(TYPE_KEYWORD.search(text)) and re.search(r"\b(class|struct|record)\b", text) and (
                text.find("(") < 0 or re.search(r"\b(class|struct|record)\b", text).start() < text.find("("))
            if is_type and not re.search(r"=>|=", text.split("(")[0]):
                stack.append({"kind": "type", "members": [], "line": header_line})
            elif re.search(r"\b(interface|enum)\b", text) and not recorded and (not stack or at_type()):
                stack.append({"kind": "skip", "line": header_line})
            else:
                stack.append({"kind": "body", "expression": recorded or bool(
                    re.search(r"=>|(?<![=!<>])=(?![=>])", outside_parentheses(text)))})
            header, header_line, recorded = [], None, False
        elif ch == "}":
            if stack:
                closed = stack.pop()
                if closed["kind"] == "type":
                    results.append((closed["line"], closed["members"]))
                if at_type() and closed["kind"] == "body" and closed.get("expression"):
                    recorded = True  # an initializer or lambda: the statement continues to its ';'
                    continue
            header, header_line, recorded = [], None, False
        elif ch == ";":
            if at_type() and not recorded:
                kind = classify("".join(header), ";")
                if kind:
                    stack[-1]["members"].append((header_line, kind[0], kind[1]))
            header, header_line, recorded = [], None, False
        else:
            if header_line is None and not ch.isspace():
                header_line = line
            header.append(ch)
    return results


def check(path):
    with open(path, encoding="utf-8-sig") as f:
        code = strip_code(f.read())
    problems = []
    for _, members in members_by_type(code):
        highest = (0, None)
        lifecycle_seen = -1
        for line, group, name in members:
            if group == NESTED_TYPE:
                continue
            if group < highest[0]:
                problems.append(f"{path}:{line}: {name} ({GROUPS[group]}) comes after "
                                f"{highest[1]} ({GROUPS[highest[0]]}); move it up.")
            else:
                highest = (group, name)
            if group == 7 and name in UNITY_LIFECYCLE:
                index = UNITY_LIFECYCLE.index(name)
                if index < lifecycle_seen:
                    problems.append(f"{path}:{line}: {name} is out of lifecycle order "
                                    f"({', '.join(UNITY_LIFECYCLE)}).")
                lifecycle_seen = max(lifecycle_seen, index)
    return problems


def expected_order():
    return ("Expected order: " + "; ".join(f"{k}. {v}" for k, v in GROUPS.items())
            + ". Nested types may stand anywhere.")


def main():
    if len(sys.argv) > 1:
        problems = [p for path in sys.argv[1:] for p in check(path)]
        print("\n".join(problems))
        return 1 if problems else 0

    try:
        payload = json.load(sys.stdin)
    except ValueError:
        return 0
    path = (payload.get("tool_input") or {}).get("file_path", "")
    if not path.endswith(".cs") or not os.path.isfile(path):
        return 0
    if re.search(r"/(Library|Temp|obj|Logs|PackageCache)/", path):
        return 0
    problems = check(path)
    if not problems:
        return 0
    print("Member order does not follow Docs/Conventions/CodeStyle.md:", file=sys.stderr)
    print("\n".join(problems), file=sys.stderr)
    print(expected_order(), file=sys.stderr)
    return 2


if __name__ == "__main__":
    sys.exit(main())
