#!/usr/bin/env python3
"""
scripts/render_3d.py
====================
Cinematic 3D Animation Renderer for GANESHAATHI46 GitHub Profile.
Renders real 3D geometry with perspective projection, orbital mechanics,
specular shading, and glowing particles.
"""

import os
import sys
import subprocess

def main():
    repo_root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    cs_engine = os.path.join(repo_root, "scripts", "Engine3D.cs")
    temp_frames = os.path.join(repo_root, "scripts", "temp_frames")

    print("[*] Ganesh A — Cinematic 3D Rendering Pipeline")
    print(f"[*] Engine source: {cs_engine}")
    print(f"[*] Output frame cache: {temp_frames}")

    # Invoke C# engine via PowerShell Add-Type / .NET JIT compiler
    ps_cmd = f"""
    $code = Get-Content -Raw '{cs_engine}'
    Add-Type -TypeDefinition $code -ReferencedAssemblies System.Drawing, System.Numerics
    [Engine3D]::Main(@('all', '{temp_frames}'))
    """
    
    res = subprocess.run(["powershell", "-NoProfile", "-Command", ps_cmd], capture_output=True, text=True)
    if res.returncode != 0:
        print(f"[!] Error executing 3D engine:\n{res.stderr}")
        sys.exit(res.returncode)

    print("[+] All 6 3D animation scenes rendered successfully into frame cache.")

if __name__ == "__main__":
    main()

