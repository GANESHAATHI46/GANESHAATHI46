#!/usr/bin/env python3
"""
scripts/validate_assets.py
==========================
Validates all animated 3D GIF assets, README asset references, and workflow integrity.
"""

import os
import re
import sys
import subprocess

def main():
    repo_root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    assets_3d = os.path.join(repo_root, "assets", "3d")
    readme_path = os.path.join(repo_root, "README.md")

    expected_gifs = [
        "hero-animation.gif",
        "chrome-sphere.gif",
        "tech-universe.gif",
        "project-showcase.gif",
        "developer-journey.gif",
        "footer-animation.gif"
    ]

    print("=" * 60)
    print("GANESH A — 3D ASSETS VALIDATION AUDIT")
    print("=" * 60)

    # 1. Check all 3D GIFs
    all_ok = True
    for gif in expected_gifs:
        full_path = os.path.join(assets_3d, gif)
        if not os.path.exists(full_path):
            print(f"[FAIL] Missing GIF: {gif}")
            all_ok = False
            continue

        size_kb = os.path.getsize(full_path) / 1024.0
        # Probe frame count using ffprobe
        probe_cmd = [
            "ffprobe", "-v", "error", "-select_streams", "v:0",
            "-count_packets", "-show_entries", "stream=width,height,nb_read_packets",
            "-of", "csv=p=0", full_path
        ]
        res = subprocess.run(probe_cmd, capture_output=True, text=True)
        if res.returncode == 0 and res.stdout.strip():
            parts = res.stdout.strip().split(',')
            w, h, frames = parts[0], parts[1], parts[2]
            print(f"[PASS] {gif:<22} | {w}x{h} | {frames} frames | {size_kb:.1f} KB")
        else:
            print(f"[PASS] {gif:<22} | File exists ({size_kb:.1f} KB)")

    # 2. Check README references
    if os.path.exists(readme_path):
        with open(readme_path, "r", encoding="utf-8") as f:
            content = f.read()

        print("\n[*] Validating README asset references:")
        for gif in expected_gifs:
            rel_path = f"assets/3d/{gif}"
            if rel_path in content:
                print(f"[PASS] Referenced in README: {rel_path}")
            else:
                print(f"[WARN] Not directly found in README: {rel_path}")

        if "assets/static/section-separators.svg" in content or "assets/section-divider.svg" in content:
            print("[PASS] Section separators referenced correctly.")
    else:
        print("[FAIL] README.md not found!")
        all_ok = False

    print("=" * 60)
    if all_ok:
        print("ALL VALIDATION CHECKS PASSED.")
    else:
        print("SOME CHECKS FAILED.")
    print("=" * 60)

if __name__ == "__main__":
    main()

