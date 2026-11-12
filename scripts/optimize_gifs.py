#!/usr/bin/env python3
"""
scripts/optimize_gifs.py
========================
Optimizes 3D rendered frames into cinematic GIF animations using FFmpeg palettegen & paletteuse.
"""

import os
import sys
import subprocess

def main():
    repo_root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    temp_frames = os.path.join(repo_root, "scripts", "temp_frames")
    assets_3d = os.path.join(repo_root, "assets", "3d")

    scenes = {
        "hero": "hero-animation.gif",
        "sphere": "chrome-sphere.gif",
        "tech": "tech-universe.gif",
        "projects": "project-showcase.gif",
        "journey": "developer-journey.gif",
        "footer": "footer-animation.gif"
    }

    print("[*] Ganesh A — GIF Palette Optimizer")

    for key, gif_name in scenes.items():
        frame_pattern = os.path.join(temp_frames, key, "frame_%04d.png")
        out_gif = os.path.join(assets_3d, gif_name)

        if not os.path.exists(os.path.join(temp_frames, key)):
            print(f"[-] Skipping {key}: frame cache not found")
            continue

        cmd = [
            "ffmpeg", "-framerate", "18",
            "-i", frame_pattern,
            "-filter_complex", "[0:v] split [a][b];[a] palettegen=max_colors=128:reserve_transparent=0 [p];[b][p] paletteuse=dither=bayer:bayer_scale=3",
            "-y", out_gif
        ]

        print(f"[*] Compiling {gif_name}...")
        res = subprocess.run(cmd, capture_output=True, text=True)
        if res.returncode == 0:
            size_kb = os.path.getsize(out_gif) / 1024.0
            print(f"[+] Generated {gif_name}: {size_kb:.1f} KB")
        else:
            print(f"[!] Error compiling {gif_name}: {res.stderr}")

if __name__ == "__main__":
    main()

