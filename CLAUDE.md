# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

All project instructions live in `AGENTS.md` (shared with other coding agents) and are imported below. Add or change project rules there; keep this file for Claude Code specifics only.

@AGENTS.md

## Claude Code specifics

- A graphify `PreToolUse` hook on Bash/Grep/Read/Glob enforces the "graphify first" policy from `AGENTS.md`. It is installed per user (`~/.claude/settings.json`), not in this repository, so it may be absent on other machines — the policy applies either way. Follow the hook's reminder rather than working around it.
- The `## graphify` section below is managed by `graphify claude install` / `uninstall` — do not edit it by hand.

## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- For codebase questions, first run `graphify query "<question>"` when graphify-out/graph.json exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).
