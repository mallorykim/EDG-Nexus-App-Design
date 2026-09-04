# Project Agora & Nexus — Context Handoff

*Prepared by Mallory for continuing this work in a new Claude account.*

---

## Who I am / my role

Design lead at Dell on a consumer PC experience team, leading UX strategy and an AI-first design-to-code workflow across two connected initiatives: **Project Agora** and **Nexus**.

---

## Project Agora

Long-term consolidation of multiple legacy Dell PC apps (Updates, Support, Device Optimization, Peripherals & Displays) into a single unified WinUI3 experience.

- I own the `agora-context` repo — the GitHub-based system of record: design system, XAML components/layouts, feature specs, accessibility rules, design philosophy, technical constraints, and IA.
- I am the **sole merge gatekeeper** for this repo.
- Framing for the effort: **"orchestration vs. architecture."** Near-term unification is *sequencing* existing apps (one Dell prompt at a time, fixed priority order). Future state is a single app with a unified shell.

## Nexus

A shorter-timeline pre-project that re-platforms an existing peripheral and display management tool (DDPM), bringing it in-house with updated code and a new design system.

- Must run as a **standalone app on non-Dell devices** — its nav structure is permanently app-specific and will never fully merge into Agora's shell.
- Currently on a **one-week deadline** for the coded app.

## Shared deliverable & workflow

- Mandated deliverable: a **coded WinUI3 prototype** — fully styled, navigable, real interaction states — built via AI-assisted coding (**Windsurf** is the primary coding agent).
- Workflow is **"Figma first, then code,"** with a deliberate plan to progressively phase Figma out, component family by component family, as each stabilizes in code.
- **Ether** is the underlying design system/component library — built in Figma, coded in WinUI3, packaged as a NuGet package, MVVM-compatible. Figma file ID: `ursRC201v8IiVeafliI45F`.

---

## Related deliverable: First-Run Experience (OOBE) presentation

A separate but connected Agora workstream: a leadership-facing presentation (internally sometimes called "Dell First Experience") mapping three states of the first-run PC onboarding flow — **current** (fragmented, multi-app), **near-term** (orchestrated using existing apps), **future** (fully unified in Agora). Uses the same "orchestration vs. architecture" framing as the rest of Agora.

- **Repo**: `https://github.com/mallorykim/otb-first-experience`. Goal is to enable **GitHub Pages** so it renders live (deploy from `main` branch root, filename `index.html`) rather than requiring download.
- **Format**: a single self-contained HTML file (images embedded as base64, no assets folder needed) with three tabbed flows, an "at a glance" strip per tab for a five-minute leadership read, detailed phase flows below a divider, a collapsible legend, a Figma/FigJam embed slot, and a click-to-zoom lightbox.
- **Content decisions already locked in:**
  - Future-state checklist order (right-side panel): **Updates** (conditional, non-numbered, disappears when nothing to report) → **Account** (step 1, required before Migrate) → **Migrate** (step 2, starts background transfer) → **Purchases** (step 3) → **Recommended setup** (step 4). **Rewards** shown as a persistent balance widget *outside* the numbered list.
  - Near-term Phase 4 includes: silent automatic updates with restart-only surfacing; a re-engagement node for skipped tasks capped at 3× with a "never show again" option; an "exclusive software deals" notification capped at 3×/15 days; a Pro Support awareness notification capped at 2×/30 days; an always-available Essentials section fallback in Optimizer's home page.
  - Future state also includes an ever-present Shop section and a My Account Pro-plan badge concept.
  - Grounded in real platform research: OEMs can pin ≤3 apps to the taskbar (enabling badges), but Start menu pins carry no Windows 11 badge; Windows enforces a ~1-hour Focus/DND quiet period post-OOBE suppressing toasts and badges; Dell Optimizer v6 already absorbed MyDell and Digital Delivery; the "what's this PC for" personalization screen was cut because Microsoft's newer OOBE already collects that signal — Agora just reads it.
- Figma token pulls for this file hit an access error (Ether Figma file needs *edit* permission, not just view, for the variable API) — worked around with a manually maintained "ETHER TOKEN MAP" comment block at the top of the CSS.

---

## Known documentation gaps flagged for `agora-context`

Identified as things that would materially improve AI-agent output quality/consistency, not yet confirmed as done:

- **Component manifest/index** — names, props, variants, and intended use cases for each component.
- **Naming alignment** — Figma layer names ↔ XAML component names need a consistent mapping.
- **Token/variable mapping** — Figma variables → XAML resource keys.
- **Interaction-state documentation** — hover/pressed/disabled/loading/error/empty states, responsive/resize behavior, and nav/frame transition rules (static Figma screens don't capture these).
- **MVVM conventions + mock data patterns**, documented explicitly, plus explicit anti-patterns to avoid.
- **Icon-sourcing rule** — when to use Segoe Fluent icons vs. custom assets.
- **Copy clarity** — a way to mark whether Figma text is final copy or placeholder.
- Reinforces the existing principle that 1–2 finished **golden reference pages** in code are high-leverage for the agent to pattern-match against.

Also worth carrying forward: periodically **refresh the sandbox from `agora-context`** (start of a session, or right after merging someone else's context update) to prevent silent drift between the two.

---

## Key people

| Person | Role |
|---|---|
| **Yiqi** | Core collaborator on Agora/Nexus prep; co-contributor to the context repo |
| Three category-owner designers (Nexus) | Each owns a peripheral category; not experienced with AI workflows or context repo contribution; input providers/consumers only, not repo contributors |
| One researcher | Also on Nexus |
| Development director | Mandated the coded prototype deliverable |

---

## Current architecture & state

- **Nexus repo structure**: a separate class library project, `Nexus.Components`, lives within the Nexus repo and takes a `PackageReference` to the Ether NuGet package. App-specific composite components (e.g., an audio equalizer built from Ether sliders) and shell patterns live here under a `/Shell` folder. Nav structure lives in the Nexus app project itself.
- **Shell architecture**: the chrome layer (Frame hosting, nav service, transition logic) is a candidate for eventual extraction into `agora-context` — but the decision is to **build Nexus first, extract after**, to avoid premature abstraction under deadline pressure.
- **Repo split**: `agora-context` (reference/system of record) and `agora-app` (code output — intentionally minimal, Views and Assets only) are kept as two separate repos. Designers reference `agora-context` explicitly in every Windsurf prompt.
- **Sandbox WinUI3 app**: a separate sandbox repo is the active working environment for visually testing/iterating on components. Finalized components graduate back to `agora-context` via AI-assisted "graduation" prompts (not manual copy-paste), done **by logical component family, not all at once**.
- **Category-owner designers' workflow** (Nexus): (1) requirements markdown pass first → (2) UX debt resolution → (3) reskin in Figma and surface component gaps. They draw missing controls on canvas as proposals but **do not** publish them as Figma library components — component creation stays in the code loop to avoid competing sources of truth.
- **Figma MCP + agora-context** guide AI agent code generation. The agent can create missing components, flag them for review, and write them back to the library.

---

## On the horizon

- Extract nav-agnostic chrome pieces from Nexus into `agora-context` after Nexus ships (move, not rewrite).
- Continue progressively phasing out Figma by component family as each stabilizes in code.
- Scale the AI-assisted workflow to accommodate multiple designers feeding screens into the pipeline.
- Enable GitHub Pages on `https://github.com/mallorykim/otb-first-experience` for live rendering of the first-run experience presentation.

---

## Key learnings & principles (hard-won — don't relitigate these)

- **Composition over re-templating**: Nexus composites must be built by composing Ether controls via XAML, not by copying/locally overriding ControlTemplates. Composition inherits upstream changes; local re-templating freezes them.
- **Don't abstract prematurely under deadline**: build first, extract shared patterns afterward as a move, not a rewrite.
- **The bottleneck shifts, it doesn't disappear**: the constraint moves from upfront library build to my review throughput as the agent generates new components. Investing in component-creation conventions in the repo context keeps review fast.
- **One source of truth for components**: component creation happens in the code loop only. Designers propose on canvas but never publish to the Figma library — prevents two competing sources of truth.
- **Graduate by component family**: avoid batching all fixes before updating `agora-context` (review-quality risk, hard to isolate, blocks others) *and* avoid graduating one tiny change at a time. Family-level batching is the right granularity.
- **XAML-first constraint applies throughout** the workflow.
- **Golden reference pages are high-leverage**: one or two fully finished pages in code give the agent strong structural patterns to match against.
- **Interaction states and nav rules must be documented**: static Figma screens don't capture hover/pressed/disabled/loading/error/empty states, responsive behavior, or frame transition rules — undocumented gaps reduce agent output quality.

---

## Working patterns & preferences

- I'm the sole merge gatekeeper for `agora-context`; complexity is kept minimal for a non-developer design team.
- Designers are input providers/consumers of the repo, not contributors.
- Prompting approach: designers reference `agora-context` explicitly in every Windsurf prompt (a multi-root workspace setup was considered and rejected as too complex).
- Screen ID convention for stable Figma-to-spec linking, e.g. `NEXUS-DISPLAY-001`.
- Per-device-type requirements files use a markdown skeleton template.
- I prefer **iterative, targeted edits** over full rewrites.
- I push back clearly when advice doesn't match actual constraints — expect the assistant to adjust rather than repeat the same suggestion.
- Executive-facing artifacts should favor cut-and-dry framing, generous visual breathing room, and five-minute readability at the leadership level.

---

## Tools & resources

- **Windsurf** — primary AI coding agent
- **WinUI3 / XAML** — core UI framework; MVVM pattern throughout
- **Figma + Figma MCP** — design source; MCP pulls design tokens and searches the design system. *Note: the Figma account needs edit permissions, not just view, for variable API access.*
- **Ether Design System** — NuGet-packaged WinUI3 component library (Figma file ID `ursRC201v8IiVeafliI45F`)
- **GitHub** — `agora-context` (reference), `agora-app` (code output), `mallorykim/otb-first-experience` (first-run experience presentation)
- **Visual Studio** — WinUI3 dev environment. *Note: corporate App Control for Business policy has previously blocked debug builds — has an IT escalation path.*
- **Node.js / docx npm package** — used to programmatically generate Word docs when file-creation tools fail; PDF conversion via `soffice.py` + `pdftoppm`.

---

## How to use this doc

Paste this whole file into the first message of the new Claude conversation, with a note on what you want to pick up next (e.g., "we're mid-way through the Nexus shell extraction plan" or "help me draft the graduation prompt for the equalizer component family"). It's a snapshot as of the handoff date — flag anything below that's since changed so the new thread doesn't work off stale assumptions.
