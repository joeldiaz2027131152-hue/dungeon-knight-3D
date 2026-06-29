# Agent Instructions

This project has a Graphify knowledge graph in `graphify-out/`.

Before answering questions about architecture, code relationships, project structure, data flow, dependencies, or "how does X connect to Y?", consult Graphify first.

Use these commands from the project root:

```bash
graphify query "<question>"
graphify explain "<node>"
graphify path "<source>" "<target>"
```

After code changes, update the graph:

```bash
graphify update .
```

Do not rebuild or overwrite the graph unnecessarily. Prefer `graphify query` for exploration and `graphify update .` only after relevant files change.

## Safety checkpoint rule

Before making any project change, create a safe checkpoint first:

- Check `git status`.
- If there are user changes, do not overwrite them.
- Commit and push the current safe state to GitHub before starting new edits whenever possible.
- For Unity scene/editor changes, save the scene or create a scene/checkpoint backup before moving, deleting, parenting, resizing, or regenerating objects.
- After completing changes, commit and push again so the latest work is recoverable.
