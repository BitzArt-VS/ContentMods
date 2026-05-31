# WncResin Codex Instructions

Use these instructions for work inside `WncResin/`.

## Mod Purpose

WncResin is a Vintage Story code/content mod for producing resin chemically from pine or acacia wood.

The intended player-facing flow is documented separately in `.codex/resin-flow.md`. Read that file before changing recipes, item definitions, pot behavior, spoilage behavior, localization, or assets related to resin production.

## Project Layout

- `WncResin.slnx` is the solution file for this mod.
- `WncResin/WncResin.csproj` is the C# project.
- `modinfo/bitzartwncresin/modinfo.json` contains the mod metadata.
- `modinfo/bitzartwncresin/modicon.png` is packaged when present.
- `modinfo/mod-description.html` is linked into package output as root-level `mod-description.html` when present.
- `assets/**` is copied to build and publish output by the project file.

Keep mod assets under `assets/bitzartwncresin/` unless there is a deliberate reason to use another domain.

## Project Facts

- Mod id: `bitzartwncresin`.
- Display name: `We Need to Cook (Resin)`.
- Root namespace: `BitzArt.Wnc.Resin`.
- Target framework: `net10.0`.
- Package output is created by the `Package` target after `dotnet publish`.

## Editing Rules

- Treat `.codex/resin-flow.md` as the design source for the resin production pipeline.
- Keep current implementation details separate from planned design notes when documenting unfinished behavior.
- Preserve the batch shape of the intended process unless the user explicitly changes the design.
- Do not copy shared Vintage Story libraries into this mod folder. References should continue to point at `../../resources/lib/`.
- When adding assets, recipes, itemtypes, or localization, keep names and domains aligned with `bitzartwncresin`.
- Prefer vanilla content patterns from `F:/Vintagestory/assets` when working on itemtypes, cooking recipes, pot behavior, texture mappings, or handbook text.

## Resin Implementation Notes

- The resin process uses vanilla cooking recipes under `assets/bitzartwncresin/recipes/cooking/`.
- The boiling batch shape is one cooking-pot slot of aqua vitae and three cooking-pot slots of pine or acacia boards. Matching stack sizes from 1 through 6 produce the same number of resin-solution portions. At maximum input, 6 aqua vitae plus 18 boards produces 6 portions.
- Aqua vitae is `game:alcoholportion`. Pine boards are `game:plank-pine`; acacia boards are `game:plank-acacia`.
- Resin solution items should behave like vanilla liquid portions, not ordinary items. Use `class: "ItemLiquidPortion"`, `matterState: "liquid"`, `shape: { "base": "item/liquid" }`, `attributes.waterTightContainerProps`, and `itemsPerLitre: 100`.
- Cooking recipe liquid outputs use vanilla liquid units. One visible portion is `quantity: 100`; six portions are `quantity: 600`.
- Non-food `cooksInto` recipes can dirty the pot when `isFood`/`IsFood` is omitted or false. Vanilla glue (`survival/recipes/cooking/glue.json`) is the closest reference for a dirty-pot, non-food liquid output.
- Dirty cooked pot visuals depend on `survival/blocktypes/clay/fired/dirtypot.json`. Add or merge texture mappings for resin-solution contents rather than assuming item textures alone will render in the pot.
- Cooking recipes do not reliably appear on ingredient handbook pages like grid recipes. Explain the process on the custom resin solution handbook pages through `attributes.handbook.extraSections` and the shared `item-handbooktitle-resin-solution` / `item-handbooktext-resin-solution` localization keys.

## Build And Package

Use the project-local solution or project file when building this mod.

The project copies `../assets/**`, `../modinfo/bitzartwncresin/modinfo.json`, `../modinfo/bitzartwncresin/modicon.png`, and linked `../modinfo/mod-description.html` into output and publish directories. `dotnet publish` also creates a `WncResin.zip` package through the `Package` target. The project intentionally sets `CopyBuildOutputToPublishDirectory=false` so the content-only package does not include an empty `WncResin.dll`.
