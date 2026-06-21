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
