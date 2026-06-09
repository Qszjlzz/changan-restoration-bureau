#!/usr/bin/env python3
"""Validate generated 2D assets before Unity promotion or release."""

from __future__ import annotations

import hashlib
import pathlib
import struct
import sys


ROOT = pathlib.Path(__file__).resolve().parents[1]
PRODUCTION = ROOT / "assets" / "generated" / "production"
UNITY_PRODUCTION = (
    ROOT
    / "unity"
    / "ChanganRestorationBureau"
    / "Assets"
    / "Art"
    / "Generated"
    / "Production"
)

EXPECTED = {
    "background_open_map.png": (3072, 1792),
    "landmark_bureau.png": (1024, 1024),
    "landmark_stele_yard.png": (1024, 1024),
    "landmark_market.png": (1024, 1024),
    "landmark_relic_yard.png": (1024, 1024),
    "character_keeper_idle.png": (512, 512),
    "character_keeper_down.png": (512, 512),
    "character_keeper_up.png": (512, 512),
    "character_keeper_left.png": (512, 512),
    "character_keeper_right.png": (512, 512),
    "prop_grass_patch.png": (512, 512),
    "prop_cleared_grass.png": (512, 512),
    "prop_rubble_stones.png": (512, 512),
    "prop_relic_pile.png": (512, 512),
}

ARTIFACT_BASES = (
    "artifact_terracotta_fragment",
    "artifact_roof_tile",
    "artifact_bronze_mirror",
    "artifact_rubbing",
    "artifact_tangsancai",
    "artifact_bamboo_slip",
)
for base in ARTIFACT_BASES:
    EXPECTED[f"{base}_damaged.png"] = (256, 256)
    EXPECTED[f"{base}_repaired.png"] = (256, 256)


def png_size(path: pathlib.Path) -> tuple[int, int]:
    with path.open("rb") as handle:
        signature = handle.read(24)
    if len(signature) != 24 or signature[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError("not a valid PNG")
    return struct.unpack(">II", signature[16:24])


def digest(path: pathlib.Path) -> str:
    value = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            value.update(chunk)
    return value.hexdigest()


def find_source(name: str) -> pathlib.Path | None:
    matches = list(PRODUCTION.rglob(name))
    return matches[0] if len(matches) == 1 else None


def main() -> int:
    errors: list[str] = []
    warnings: list[str] = []

    for name, expected_size in sorted(EXPECTED.items()):
        source = find_source(name)
        unity = UNITY_PRODUCTION / name

        if source is None:
            errors.append(f"missing or ambiguous production asset: {name}")
            continue
        if not unity.exists():
            errors.append(f"missing Unity production copy: {name}")
            continue

        try:
            actual_size = png_size(source)
        except ValueError as exc:
            errors.append(f"{name}: {exc}")
            continue

        if actual_size != expected_size:
            warnings.append(
                f"{name}: expected {expected_size[0]}x{expected_size[1]}, "
                f"found {actual_size[0]}x{actual_size[1]}"
            )
        if digest(source) != digest(unity):
            errors.append(f"production and Unity copies differ: {name}")

        meta = pathlib.Path(f"{unity}.meta")
        if not meta.exists():
            errors.append(f"missing Unity meta: {name}.meta")
            continue
        meta_text = meta.read_text(encoding="utf-8", errors="replace")
        if "spritePixelsToUnits: 100" not in meta_text:
            errors.append(f"unexpected PPU in Unity meta: {name}")

    print("Asset contract report")
    print(f"  checked: {len(EXPECTED)}")
    for warning in warnings:
        print(f"  WARN: {warning}")
    for error in errors:
        print(f"  ERROR: {error}")

    if errors:
        print(f"FAILED with {len(errors)} blocking error(s)")
        return 1
    print(f"PASSED with {len(warnings)} warning(s)")
    return 0


if __name__ == "__main__":
    sys.exit(main())
