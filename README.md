# Wisp Tracker

Overlay that guesses Wildwood wisp counts from monster stats
and draws `juice (stat%)` on empowered monsters.

## Calibration

### Purple (area)

Projectiles are shown but untracked so far.

- 701 purple = 41% area
- 1k purple = 45% area
- 2k purple = 60% area
- 4k purple = 90% area
- 5k purple = 105% area
- 8652 purple = 140% area

### Yellow (velocity)

Attack speed is shown but untracked so far.

- 2k yellow = 45% velocity
- 3553 yellow = 60% velocity
- 4k yellow = 65% velocity
- 4307 yellow = 68% velocity
- 7876 yellow = 103% movement velocity

### Blue

Not shown on the client side.

## Rules

- Wildwood: every monster empowered with a type gets the full amount
  (white mob = 68%).
- Scarab of Wisps: 2k added to a type. Multiple types = 2k each.
  Can roll 2k/2k/2k. Cannot roll 6k/0k/0k.
- 2x Scarab of Wisps: 4k added to a type. Multiple types = 4k each.
  Can roll 4k/4k/4k. Cannot roll 12k/0k/0k.
