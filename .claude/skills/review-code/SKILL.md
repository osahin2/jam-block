---
name: review-code
description: Run an independent general code review of the case code in a separate agent - architecture deviations, separation of concerns, SRP and encapsulation, duplication, code smells, interface contracts (Liskov), dispatch on kinds that change (open-closed), per-frame allocations, abstractions without a reason - then let the developer choose what to fix. Use when a feature, a phase step or a set of fixes is finished, or when the developer asks for a review.
---

# Reviewing the code

When a feature, a phase step or a set of fixes is finished, ask the developer whether to review it before moving on (not after every small edit). The review runs in a fresh agent, so it looks at the code without the reasoning that wrote it. The checklist and the guard against overengineering are in the agent definition `.claude/agents/code-reviewer.md`; this skill is the workflow around it.

## 1. Scope
- Work on a branch: everything since it left `develop`, `git diff develop...HEAD` plus uncommitted changes.
- Otherwise the commits since the last review (the work log names it), or the folders the developer names.
- Write the scope as a commit range or a list of folders, and one sentence on what the work was for.

A large scope can be split into disjoint parts (by assembly or feature), each reviewed by its own agent in parallel; each agent reads across its boundary but reports only on its own part, and the findings are merged and deduplicated in step 3.

## 2. Start the reviewer
Start an agent of type `code-reviewer` with the scope and that sentence; not a fork, which would carry this conversation's reasoning into the review. It runs in the background; carry on with other work or wait for it. If the type is not available yet (agent definitions load when a session starts), start a `general-purpose` agent and tell it to follow `.claude/agents/code-reviewer.md` and change nothing.

## 3. Check its findings
The report is a claim, not proof. Open the code for each finding; drop the ones that are wrong or that a recorded decision already covers, and say which were dropped and why.

## 4. The developer chooses
Show the findings to the developer ranked, each with its cost and the proposed fix, plus your own recommendation (which to fix now, which belong to a later phase). Fix nothing before the developer has chosen.

## 5. Fix and record
- Fix the chosen findings, then compile and run the EditMode tests (skill `unity-editor`).
- Update the system notes the fixes touch; a choice a reviewer could ask about goes to `Docs/Game/Decisions.md`.
- Findings left for later go into the phase they belong to in `Docs/Game/Plan.md`.
- Name the review in the work log line of the session, so the next review knows where to start.
