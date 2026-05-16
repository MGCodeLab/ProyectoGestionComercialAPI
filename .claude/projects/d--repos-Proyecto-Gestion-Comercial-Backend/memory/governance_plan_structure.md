---
name: governance_plan_structure
description: Sprint plans are segregated by sprint (one file = one sprint), not monolithic by phase
metadata:
  type: feedback
---

# Plan Governance: One Sprint = One File

**Why:** Monolithic roadmap files containing multiple sprints prevent moving completed sprints to `completed/` folder independently. Segregation allows each sprint to be treated as a self-contained planning unit (like a Jira card or Azure DevOps board).

**How to apply:** 
- Create individual plan file per sprint: `YYYY-MM-DD_catalogo-sprintN-{nombre}.md`
- Each file: Detailed but specific (executable, not abstract)
- When sprint completes: Move file to `plans/completed/` with status updated
- Macro vision stays in `.claude/PROYECTO_VISION_COMPLETA.md` (references to active plans, not full details)
- See `IA_Docs/PLAN_GOVERNANCE_BY_SPRINT.md` for structure template

**Details:** Implemented 2026-05-16 after Sprint 2 completion; eliminates previous monolithic roadmap pattern.
