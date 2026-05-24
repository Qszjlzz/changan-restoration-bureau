from pathlib import Path
from PIL import Image


ROOT = Path(__file__).resolve().parent.parent
STAGING = ROOT / "assets" / "generated" / "staging"
PRODUCTION = ROOT / "assets" / "generated" / "production"
UNITY_PRODUCTION = ROOT / "unity" / "ChanganRestorationBureau" / "Assets" / "Art" / "Generated" / "Production"


def ensure_dirs() -> None:
    for path in [
        PRODUCTION / "backgrounds",
        PRODUCTION / "landmarks",
        PRODUCTION / "characters",
        PRODUCTION / "artifacts",
        PRODUCTION / "props",
        PRODUCTION / "ui",
        UNITY_PRODUCTION,
    ]:
        path.mkdir(parents=True, exist_ok=True)


def remove_green(image: Image.Image, tolerance: int = 70) -> Image.Image:
    rgba = image.convert("RGBA")
    out = []
    for r, g, b, a in rgba.getdata():
        is_green = g > 150 and r < 90 and b < 90 and (g - r) > tolerance and (g - b) > tolerance
        if is_green:
            out.append((r, g, b, 0))
        else:
            if g > r and g > b:
                g = min(g, max(r, b) + 10)
            out.append((r, g, b, a))
    rgba.putdata(out)
    return rgba


def remove_magenta(image: Image.Image, tolerance: int = 70) -> Image.Image:
    rgba = image.convert("RGBA")
    out = []
    for r, g, b, a in rgba.getdata():
        is_magenta = r > 170 and b > 170 and g < 90 and (r - g) > tolerance and (b - g) > tolerance
        is_fringe = r > 115 and b > 115 and g < 155 and abs(r - b) < 95 and (r - g) > 35 and (b - g) > 35
        if is_magenta or is_fringe:
            out.append((r, g, b, 0))
        else:
            if r > 140 and b > 140 and g < 150:
                r = min(r, max(g + 25, 95))
                b = min(b, max(g + 25, 95))
            out.append((r, g, b, a))
    rgba.putdata(out)
    return rgba


def crop_alpha_bounds(image: Image.Image, padding: int = 16) -> Image.Image:
    rgba = image.convert("RGBA")
    alpha = rgba.getchannel("A")
    bbox = alpha.getbbox()
    if bbox is None:
        return rgba
    left, top, right, bottom = bbox
    left = max(0, left - padding)
    top = max(0, top - padding)
    right = min(rgba.width, right + padding)
    bottom = min(rgba.height, bottom + padding)
    return rgba.crop((left, top, right, bottom))


def fit_canvas(image: Image.Image, size: tuple[int, int]) -> Image.Image:
    rgba = image.convert("RGBA")
    target_w, target_h = size
    scale = min(target_w / rgba.width, target_h / rgba.height)
    new_size = (max(1, int(rgba.width * scale)), max(1, int(rgba.height * scale)))
    resized = rgba.resize(new_size, Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", size, (0, 0, 0, 0))
    x = (target_w - new_size[0]) // 2
    y = target_h - new_size[1]
    canvas.alpha_composite(resized, (x, y))
    return canvas


def fit_canvas_center(image: Image.Image, size: tuple[int, int]) -> Image.Image:
    rgba = image.convert("RGBA")
    target_w, target_h = size
    scale = min(target_w / rgba.width, target_h / rgba.height)
    new_size = (max(1, int(rgba.width * scale)), max(1, int(rgba.height * scale)))
    resized = rgba.resize(new_size, Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", size, (0, 0, 0, 0))
    x = (target_w - new_size[0]) // 2
    y = (target_h - new_size[1]) // 2
    canvas.alpha_composite(resized, (x, y))
    return canvas


def copy_to_unity(src: Path, unity_name: str) -> None:
    target = UNITY_PRODUCTION / unity_name
    target.write_bytes(src.read_bytes())


def process_background() -> None:
    raw = Image.open(STAGING / "backgrounds" / "background_open_map_raw.png").convert("RGB")
    resized = raw.resize((3072, 1728), Image.Resampling.LANCZOS)
    out = PRODUCTION / "backgrounds" / "background_open_map.png"
    resized.save(out)
    copy_to_unity(out, "background_open_map.png")


def process_landmarks() -> None:
    raw = Image.open(STAGING / "landmarks" / "landmark_sheet_raw.png")
    crops = {
        "landmark_bureau.png": (0, 0, 768, 512),
        "landmark_stele_yard.png": (768, 0, 1536, 512),
        "landmark_market.png": (0, 512, 768, 1024),
        "landmark_relic_yard.png": (768, 512, 1536, 1024),
    }
    for name, box in crops.items():
        cutout = crop_alpha_bounds(remove_green(raw.crop(box)), 8)
        fitted = fit_canvas(cutout, (1024, 1024))
        out = PRODUCTION / "landmarks" / name
        fitted.save(out)
        copy_to_unity(out, name)


def process_character() -> None:
    raw = Image.open(STAGING / "characters" / "character_keeper_idle_raw.png")
    cutout = crop_alpha_bounds(remove_green(raw), 24)
    fitted = fit_canvas(cutout, (512, 512))
    out = PRODUCTION / "characters" / "character_keeper_idle.png"
    fitted.save(out)
    copy_to_unity(out, "character_keeper_idle.png")

    sheet_path = STAGING / "characters" / "character_keeper_4dir_raw.png"
    if not sheet_path.exists():
        return

    sheet = Image.open(sheet_path)
    w, h = sheet.size
    crops = {
        "character_keeper_down.png": (0, 0, w // 2, h // 2),
        "character_keeper_up.png": (w // 2, 0, w, h // 2),
        "character_keeper_left.png": (0, h // 2, w // 2, h),
        "character_keeper_right.png": (w // 2, h // 2, w, h),
    }
    for name, box in crops.items():
        cutout = crop_alpha_bounds(remove_green(sheet.crop(box)), 24)
        fitted = fit_canvas(cutout, (512, 512))
        out = PRODUCTION / "characters" / name
        fitted.save(out)
        copy_to_unity(out, name)


def process_ui() -> None:
    raw = Image.open(STAGING / "ui" / "ui_kit_raw.png")
    crops = {
        "ui_detail_panel.png": (70, 40, 670, 970),
        "ui_repair_button.png": (730, 120, 1200, 270),
        "ui_display_button.png": (730, 310, 1200, 460),
    }
    for name, box in crops.items():
        cutout = crop_alpha_bounds(remove_green(raw.crop(box)), 8)
        if name == "ui_detail_panel.png":
            fitted = fit_canvas(cutout, (512, 768))
        else:
            fitted = fit_canvas(cutout, (384, 128))
        out = PRODUCTION / "ui" / name
        fitted.save(out)
        copy_to_unity(out, name)


def process_artifacts_and_props() -> None:
    sheet_path = STAGING / "artifacts" / "relic_prop_sheet_alpha.png"
    if not sheet_path.exists():
        sheet_path = STAGING / "artifacts" / "relic_prop_sheet_raw.png"
        if not sheet_path.exists():
            return

    raw = Image.open(sheet_path)
    w, h = raw.size
    cell_w = w // 4
    cell_h = h // 4
    cells = [
        ("artifacts", "artifact_terracotta_fragment_damaged.png", 0, 0, (256, 256), "center"),
        ("artifacts", "artifact_terracotta_fragment_repaired.png", 1, 0, (256, 256), "center"),
        ("artifacts", "artifact_roof_tile_damaged.png", 2, 0, (256, 256), "center"),
        ("artifacts", "artifact_roof_tile_repaired.png", 3, 0, (256, 256), "center"),
        ("artifacts", "artifact_bronze_mirror_damaged.png", 0, 1, (256, 256), "center"),
        ("artifacts", "artifact_bronze_mirror_repaired.png", 1, 1, (256, 256), "center"),
        ("artifacts", "artifact_rubbing_damaged.png", 2, 1, (256, 256), "center"),
        ("artifacts", "artifact_rubbing_repaired.png", 3, 1, (256, 256), "center"),
        ("artifacts", "artifact_tangsancai_damaged.png", 0, 2, (256, 256), "center"),
        ("artifacts", "artifact_tangsancai_repaired.png", 1, 2, (256, 256), "center"),
        ("artifacts", "artifact_bamboo_slip_damaged.png", 2, 2, (256, 256), "center"),
        ("artifacts", "artifact_bamboo_slip_repaired.png", 3, 2, (256, 256), "center"),
        ("props", "prop_grass_patch.png", 0, 3, (512, 512), "bottom"),
        ("props", "prop_cleared_grass.png", 1, 3, (512, 512), "bottom"),
        ("props", "prop_rubble_stones.png", 2, 3, (512, 512), "bottom"),
        ("props", "prop_relic_pile.png", 3, 3, (512, 512), "bottom"),
    ]

    for folder, name, col, row, size, anchor in cells:
        box = (col * cell_w, row * cell_h, (col + 1) * cell_w, (row + 1) * cell_h)
        cell = raw.crop(box)
        if cell.mode == "RGBA" and cell.getchannel("A").getbbox() is not None:
            cutout = crop_alpha_bounds(cell, 10)
        else:
            cutout = crop_alpha_bounds(remove_magenta(cell), 10)
        fitted = fit_canvas(cutout, size) if anchor == "bottom" else fit_canvas_center(cutout, size)
        out = PRODUCTION / folder / name
        fitted.save(out)
        copy_to_unity(out, name)


def main() -> None:
    ensure_dirs()
    process_background()
    process_landmarks()
    process_character()
    process_ui()
    process_artifacts_and_props()


if __name__ == "__main__":
    main()
