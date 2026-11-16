# Work log

> Summary: One line per session: date, approximate hours, what was done, what an LLM tool did. The README's work time and LLM sections are written from this file.

| Date | Hours | Done | LLM use |
|---|---|---|---|
| 2026-11-16 | | Setup: Unity 2022.3.62f2, project, Core copied from the base template and tested, conventions, plan | Claude Code: installed the editor, created the project, ported Core, wrote the docs and plan |
| 2026-11-16 | | Phase 0 finished: Core ported to 2022.3 (63 EditMode tests green), packages trimmed, samples removed, Android portrait settings, batch-mode skill, git `main` and `develop` | Claude Code: ported Core, ran the batch compile and tests, wrote the setup editor script, the skill and the status |
| 2026-11-16/17 | | Phase 1 start: Case.md checked against the PDF, reference game studied on an Android device through five levels, open decisions settled | Claude Code: read the PDF, drove the device over adb (screenshots, swipes), wrote Reference.md and the decisions; the developer chose |
| 2026-11-17 | | Phase 1 finished and phase 2 started: supplied assets imported and reviewed, game and test assemblies, level format and move rules with tests; level validation moved out of the board into `LevelValidator`, which runs when a level is made; a blocked step returns a result instead of throwing | Claude Code: wrote the import and model-report editor scripts, measured and rendered the models, wrote the asset review, assemblies, rules, tests and notes |
