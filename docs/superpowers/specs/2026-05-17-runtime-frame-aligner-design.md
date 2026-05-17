# Runtime Frame Aligner Design

## Goal

Build a small local browser tool for manually correcting 1-2 pixel animation drift after pixel snap and 256x256 runtime normalization.

The tool should let the user load a small frame set, inspect all frames against shared visual guides, nudge individual frames by exact pixel offsets, preview the loop, and export corrected frames or a corrected runtime sheet.

## Primary Use Case

The current hero idle frames are already processed into 256x256 transparent PNGs:

- `DragonKnight/Assets/Art/Hero Sprite/Idle Frames Processed/Final 256/hero_idle_frame_01_256.png`
- `DragonKnight/Assets/Art/Hero Sprite/Idle Frames Processed/Final 256/hero_idle_frame_02_256.png`
- `DragonKnight/Assets/Art/Hero Sprite/Idle Frames Processed/Final 256/hero_idle_frame_03_256.png`
- `DragonKnight/Assets/Art/Hero Sprite/Idle Frames Processed/Final 256/hero_idle_frame_04_256.png`

Even with shared canvas size and foot-baseline normalization, the apparent character position can still drift. The tool exists to fix that final visual alignment manually.

## UI Layout

Use a single-page HTML/Canvas interface.

Main stage:

- Large canvas preview area.
- Dark neutral background.
- Selected frame drawn at integer pixel coordinates.
- Optional ghost/onion-skin layers for neighboring or selected frames.
- Blue vertical center line.
- Yellow horizontal reference line for eyes or upper-body comparison.
- Green foot baseline.

Right panel:

- Frame buttons, numbered with zero padding.
- Ghost layer toggles per frame.
- Per-frame `dx`, `dy` offset table.
- Selected frame indicator.

Bottom controls:

- `Prev` and `Next`.
- Arrow nudge buttons.
- `Reset Frame`.
- `Reset All`.
- `Ghost Selected`.
- `Ghost Neighbors`.
- `Clear Ghosts`.
- Opacity slider for ghost layers.
- `Play/Pause` loop preview.
- `Export PNGs`.
- `Export Sheet`.

## Interaction

Keyboard:

- Arrow keys nudge selected frame by 1 pixel.
- Shift + arrow keys nudge selected frame by 5 pixels.
- `[` and `]` move to previous/next frame.
- Space toggles loop preview.
- `0` resets selected frame.

Mouse:

- Clicking a frame button selects that frame.
- Ghost checkboxes toggle onion-skin layers.
- Buttons mirror the keyboard actions for discoverability.

Nudging must update only the selected frame's offset. The underlying source image is not destructively edited until export.

## Data Model

Keep all source frames and offsets in memory:

```json
{
  "frameWidth": 256,
  "frameHeight": 256,
  "baselineY": 224,
  "frames": [
    { "name": "hero_idle_frame_01_256.png", "dx": 0, "dy": 0 },
    { "name": "hero_idle_frame_02_256.png", "dx": 0, "dy": 0 },
    { "name": "hero_idle_frame_03_256.png", "dx": -1, "dy": 0 },
    { "name": "hero_idle_frame_04_256.png", "dx": 0, "dy": 0 }
  ]
}
```

Offsets are applied at render and export time. Positive `dx` moves right. Positive `dy` moves down.

## Input

Initial version can be project-specific:

- Load the four current hero idle `256x256` PNGs from the `Final 256` folder.
- Assume all input frames are same-size transparent PNGs.

Later versions can add file picker support for arbitrary frame sets or sprite sheets.

## Export

Export should create non-destructive outputs:

- Corrected individual PNGs in a sibling folder such as `Final 256 Aligned/`.
- A horizontal runtime sheet such as `hero_idle_frames_256_aligned_strip.png`.
- A small JSON sidecar such as `hero_idle_frames_256_offsets.json` with the chosen offsets.

All exports must use nearest-neighbor semantics by drawing source pixels directly to canvas with smoothing disabled.

## Validation

Before calling the tool complete:

- Load the current four hero idle frames.
- Verify all frames render at native 256x256 size.
- Verify arrow-key nudging changes only the selected frame.
- Verify ghost opacity and ghost toggles work.
- Verify loop preview uses the current offsets.
- Export corrected PNGs and sheet.
- Confirm output dimensions remain `256x256` per frame and `1024x256` for a four-frame horizontal sheet.

## Out Of Scope

- AI regeneration of bad frames.
- Pixel-snapper integration.
- Palette editing.
- Automatic pose analysis.
- Unity import setting changes.

Those steps can remain separate pipeline stages.
