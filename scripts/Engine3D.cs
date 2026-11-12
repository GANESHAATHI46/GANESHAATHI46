using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using System.Collections.Generic;

public class Engine3D {

    public static void Main(string[] args) {
        string target = args.Length > 0 ? args[0] : "all";
        string baseDir = args.Length > 1 ? args[1] : Path.Combine(Directory.GetCurrentDirectory(), "temp_frames");

        if (target == "all" || target == "hero") RenderHero(Path.Combine(baseDir, "hero"));
        if (target == "all" || target == "sphere") RenderChromeSphere(Path.Combine(baseDir, "sphere"));
        if (target == "all" || target == "tech") RenderTechUniverse(Path.Combine(baseDir, "tech"));
        if (target == "all" || target == "projects") RenderProjectShowcase(Path.Combine(baseDir, "projects"));
        if (target == "all" || target == "journey") RenderDeveloperJourney(Path.Combine(baseDir, "journey"));
        if (target == "all" || target == "footer") RenderFooterAnimation(Path.Combine(baseDir, "footer"));
    }

    // -------------------------------------------------------------
    // SCENE 1: HERO ANIMATION (36 frames, 840x360)
    // -------------------------------------------------------------
    public static void RenderHero(string outDir) {
        Directory.CreateDirectory(outDir);
        int w = 840, h = 360;
        int totalFrames = 36;

        // Precompute stable particle positions
        Random rnd = new Random(42);
        List<Vector3> particles = new List<Vector3>();
        for (int i = 0; i < 90; i++) {
            particles.Add(new Vector3(
                (float)(rnd.NextDouble() * 800 - 400),
                (float)(rnd.NextDouble() * 320 - 160),
                (float)(rnd.NextDouble() * 400 + 100)
            ));
        }

        for (int frame = 0; frame < totalFrames; frame++) {
            float progress = (float)frame / totalFrames;
            float angle = progress * (float)Math.PI * 2f;

            using (Bitmap bmp = new Bitmap(w, h))
            using (Graphics g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.Clear(Color.FromArgb(5, 5, 7));

                // 1. Central crimson nebula glow
                using (GraphicsPath p = new GraphicsPath()) {
                    p.AddEllipse(w / 2 - 280, h / 2 - 140, 560, 280);
                    using (PathGradientBrush pgb = new PathGradientBrush(p)) {
                        pgb.CenterPoint = new PointF(w / 2, h / 2);
                        pgb.CenterColor = Color.FromArgb(45, 255, 32, 32);
                        pgb.SurroundColors = new Color[] { Color.FromArgb(0, 5, 5, 7) };
                        g.FillPath(pgb, p);
                    }
                }

                // 2. 3D Stars & Floating Red Particles
                foreach (var pt in particles) {
                    float orbitAngle = angle + pt.X * 0.01f;
                    float px = pt.X * (float)Math.Cos(orbitAngle * 0.2f) - pt.Z * 0.1f * (float)Math.Sin(orbitAngle * 0.2f);
                    float py = pt.Y + (float)Math.Sin(angle + pt.X * 0.05f) * 12f;
                    float pz = pt.Z + (float)Math.Cos(orbitAngle * 0.2f) * 40f;

                    float scale = 450f / (450f + pz);
                    float sx = w / 2 + px * scale;
                    float sy = h / 2 + py * scale;
                    float size = Math.Max(1.5f, 5f * scale);

                    int alpha = Math.Min(255, (int)(180 * scale));
                    using (Brush pb = new SolidBrush(Color.FromArgb(alpha, 255, 60, 60))) {
                        g.FillEllipse(pb, sx - size / 2, sy - size / 2, size, size);
                    }
                }

                // 3. 3D Rotating Orbital Rings (Back part, z < 0)
                Draw3DRing(g, w / 2, h / 2, 320, 75, angle * 1.0f, 28f, false);
                Draw3DRing(g, w / 2, h / 2, 270, 60, -angle * 0.8f, -22f, false);

                // 4. Central 3D Metallic Typography "GANESH A"
                float sweepX = -200 + (progress * (w + 400)); // light sweep position
                DrawMetallicTitle(g, "GANESH A", w / 2, h / 2 - 45, sweepX);

                // Subtitle: Software Developer
                using (Font subFont = new Font("Segoe UI", 13, FontStyle.Bold))
                using (Brush subBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255))) {
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString("SOFTWARE DEVELOPER  |  FULL STACK  |  CLOUD & DEVOPS", subFont, subBrush, w / 2, h / 2 + 30, sf);
                }

                // Motto with Glowing Dots
                using (Font mottoFont = new Font("JetBrains Mono", 10, FontStyle.Regular))
                using (Brush mottoBrush = new SolidBrush(Color.FromArgb(180, 255, 50, 50))) {
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString("BUILD  •  LEARN  •  IMPROVE  •  REPEAT", mottoFont, mottoBrush, w / 2, h / 2 + 58, sf);
                }

                // 5. 3D Rotating Orbital Rings (Front part, z >= 0)
                Draw3DRing(g, w / 2, h / 2, 320, 75, angle * 1.0f, 28f, true);
                Draw3DRing(g, w / 2, h / 2, 270, 60, -angle * 0.8f, -22f, true);

                // Outer border glow
                using (Pen borderPen = new Pen(Color.FromArgb(50, 255, 32, 32), 1.5f)) {
                    g.DrawRectangle(borderPen, 2, 2, w - 4, h - 4);
                }

                string filePath = Path.Combine(outDir, string.Format("frame_{0:D4}.png", frame));
                bmp.Save(filePath, ImageFormat.Png);
            }
        }
    }

    private static void DrawMetallicTitle(Graphics g, string text, float cx, float cy, float sweepX) {
        using (Font font = new Font("Segoe UI", 52, FontStyle.Bold)) {
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };

            // 3D Extrusion Shadows
            for (int i = 6; i >= 1; i--) {
                using (Brush shadowBrush = new SolidBrush(Color.FromArgb(25 + i * 15, 139, 0, 0))) {
                    g.DrawString(text, font, shadowBrush, cx + i * 1.2f, cy + i * 1.2f, sf);
                }
            }

            // Metallic Face
            RectangleF textRect = new RectangleF(cx - 300, cy, 600, 80);
            using (LinearGradientBrush lgb = new LinearGradientBrush(
                new PointF(cx, cy), new PointF(cx, cy + 70),
                Color.FromArgb(255, 255, 255), Color.FromArgb(140, 140, 150))) {
                g.DrawString(text, font, lgb, cx, cy, sf);
            }

            // Red Light Flare Sweep
            if (sweepX > cx - 250 && sweepX < cx + 250) {
                float dist = Math.Abs(sweepX - cx);
                int flareAlpha = (int)Math.Max(0, 180 - dist * 0.7f);
                using (Brush flareBrush = new SolidBrush(Color.FromArgb(flareAlpha, 255, 40, 40))) {
                    g.DrawString(text, font, flareBrush, cx, cy, sf);
                }
            }

            // Crisp Rim Outline
            using (Pen rimPen = new Pen(Color.FromArgb(180, 255, 32, 32), 1.2f))
            using (GraphicsPath gp = new GraphicsPath()) {
                gp.AddString(text, font.FontFamily, (int)font.Style, font.Size, new PointF(cx, cy), sf);
                g.DrawPath(rimPen, gp);
            }
        }
    }

    private static void Draw3DRing(Graphics g, float cx, float cy, float radiusX, float radiusY, float angleRad, float tiltDeg, bool frontOnly) {
        float tiltRad = tiltDeg * (float)Math.PI / 180f;
        int steps = 120;

        for (int i = 0; i < steps; i++) {
            float t = (float)i / steps * (float)Math.PI * 2f;
            float z = radiusY * (float)Math.Sin(t + angleRad);

            if (frontOnly && z < 0) continue;
            if (!frontOnly && z >= 0) continue;

            float rx = radiusX * (float)Math.Cos(t + angleRad);
            float ry = radiusY * (float)Math.Sin(t + angleRad);

            // Apply 3D tilt
            float px = cx + rx * (float)Math.Cos(tiltRad) - ry * (float)Math.Sin(tiltRad);
            float py = cy + rx * (float)Math.Sin(tiltRad) + ry * (float)Math.Cos(tiltRad);

            float zNorm = (z + radiusY) / (radiusY * 2f); // 0 to 1
            int alpha = (int)(70 + zNorm * 185);
            float size = 1.2f + zNorm * 2.5f;

            Color col = zNorm > 0.6f ? Color.FromArgb(alpha, 255, 40, 40) : Color.FromArgb(alpha, 140, 0, 0);
            using (Brush b = new SolidBrush(col)) {
                g.FillEllipse(b, px - size / 2, py - size / 2, size, size);
            }
        }
    }

    // -------------------------------------------------------------
    // SCENE 2: ROTATING CHROME SPHERE (36 frames, 500x480)
    // -------------------------------------------------------------
    public static void RenderChromeSphere(string outDir) {
        Directory.CreateDirectory(outDir);
        int w = 500, h = 480;
        int totalFrames = 36;
        float cx = w / 2f, cy = h / 2f;
        float sphereRadius = 90f;

        for (int frame = 0; frame < totalFrames; frame++) {
            float progress = (float)frame / totalFrames;
            float angle = progress * (float)Math.PI * 2f;

            using (Bitmap bmp = new Bitmap(w, h))
            using (Graphics g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.FromArgb(5, 5, 7));

                // Ambient core aura
                using (GraphicsPath p = new GraphicsPath()) {
                    p.AddEllipse(cx - 180, cy - 180, 360, 360);
                    using (PathGradientBrush pgb = new PathGradientBrush(p)) {
                        pgb.CenterPoint = new PointF(cx, cy);
                        pgb.CenterColor = Color.FromArgb(50, 255, 32, 32);
                        pgb.SurroundColors = new Color[] { Color.FromArgb(0, 5, 5, 7) };
                        g.FillPath(pgb, p);
                    }
                }

                // 3D Orbital Rings (Back half, z < 0)
                DrawSphereRing(g, cx, cy, 170, 50, angle, 40f, false);
                DrawSphereRing(g, cx, cy, 190, 60, -angle * 1.2f, -35f, false);
                DrawSphereRing(g, cx, cy, 140, 130, angle * 0.7f, 75f, false);

                // 3D Metallic Chrome Sphere
                // Dynamic light position rotating around the sphere
                float lightX = cx + (float)Math.Cos(angle) * 35f - 20f;
                float lightY = cy + (float)Math.Sin(angle * 0.5f) * 25f - 30f;

                using (GraphicsPath sp = new GraphicsPath()) {
                    sp.AddEllipse(cx - sphereRadius, cy - sphereRadius, sphereRadius * 2, sphereRadius * 2);

                    using (PathGradientBrush pgb = new PathGradientBrush(sp)) {
                        pgb.CenterPoint = new PointF(lightX, lightY);
                        pgb.CenterColor = Color.FromArgb(255, 255, 255);
                        pgb.SurroundColors = new Color[] { Color.FromArgb(15, 2, 4) };
                        g.FillPath(pgb, sp);
                    }

                    // Crimson Rim Glow on Sphere
                    using (PathGradientBrush rimPgb = new PathGradientBrush(sp)) {
                        rimPgb.CenterPoint = new PointF(cx, cy);
                        rimPgb.CenterColor = Color.FromArgb(0, 0, 0, 0);
                        rimPgb.SurroundColors = new Color[] { Color.FromArgb(200, 255, 32, 32) };
                        g.FillPath(rimPgb, sp);
                    }

                    // Specular Hotspot
                    using (GraphicsPath specPath = new GraphicsPath()) {
                        specPath.AddEllipse(lightX - 18, lightY - 14, 36, 28);
                        using (PathGradientBrush sPgb = new PathGradientBrush(specPath)) {
                            sPgb.CenterPoint = new PointF(lightX, lightY);
                            sPgb.CenterColor = Color.FromArgb(240, 255, 255, 255);
                            sPgb.SurroundColors = new Color[] { Color.FromArgb(0, 255, 255, 255) };
                            g.FillPath(sPgb, specPath);
                        }
                    }

                    using (Pen rimPen = new Pen(Color.FromArgb(220, 255, 40, 40), 1.5f)) {
                        g.DrawPath(rimPen, sp);
                    }
                }

                // 3D Orbital Rings (Front half, z >= 0)
                DrawSphereRing(g, cx, cy, 170, 50, angle, 40f, true);
                DrawSphereRing(g, cx, cy, 190, 60, -angle * 1.2f, -35f, true);
                DrawSphereRing(g, cx, cy, 140, 130, angle * 0.7f, 75f, true);

                // Orbiting Chrome Beads
                for (int b = 0; b < 3; b++) {
                    float bAngle = angle * 1.5f + b * (float)Math.PI * 2f / 3f;
                    float bx = cx + (float)Math.Cos(bAngle) * 165f;
                    float by = cy + (float)Math.Sin(bAngle) * 55f;
                    using (Brush beadBrush = new SolidBrush(Color.FromArgb(255, 255, 80, 80))) {
                        g.FillEllipse(beadBrush, bx - 5, by - 5, 10, 10);
                    }
                    using (Brush coreBrush = new SolidBrush(Color.White)) {
                        g.FillEllipse(coreBrush, bx - 2, by - 2, 4, 4);
                    }
                }

                string filePath = Path.Combine(outDir, string.Format("frame_{0:D4}.png", frame));
                bmp.Save(filePath, ImageFormat.Png);
            }
        }
    }

    private static void DrawSphereRing(Graphics g, float cx, float cy, float rx, float ry, float angleRad, float tiltDeg, bool frontOnly) {
        float tiltRad = tiltDeg * (float)Math.PI / 180f;
        int steps = 140;

        for (int i = 0; i < steps; i++) {
            float t = (float)i / steps * (float)Math.PI * 2f;
            float z = ry * (float)Math.Sin(t + angleRad);

            if (frontOnly && z < 0) continue;
            if (!frontOnly && z >= 0) continue;

            float x0 = rx * (float)Math.Cos(t + angleRad);
            float y0 = ry * (float)Math.Sin(t + angleRad);

            float px = cx + x0 * (float)Math.Cos(tiltRad) - y0 * (float)Math.Sin(tiltRad);
            float py = cy + x0 * (float)Math.Sin(tiltRad) + y0 * (float)Math.Cos(tiltRad);

            float zNorm = (z + ry) / (ry * 2f);
            int alpha = (int)(60 + zNorm * 195);
            float size = 1.2f + zNorm * 2.8f;

            Color col = zNorm > 0.5f ? Color.FromArgb(alpha, 255, 45, 45) : Color.FromArgb(alpha, 120, 0, 0);
            using (Brush b = new SolidBrush(col)) {
                g.FillEllipse(b, px - size / 2, py - size / 2, size, size);
            }
        }
    }

    // -------------------------------------------------------------
    // SCENE 3: FLOATING TECHNOLOGY UNIVERSE (36 frames, 840x380)
    // -------------------------------------------------------------
    struct TechNode {
        public string Name;
        public Color ThemeColor;
        public float BaseAngle;
    }

    public static void RenderTechUniverse(string outDir) {
        Directory.CreateDirectory(outDir);
        int w = 840, h = 380;
        int totalFrames = 36;
        float cx = w / 2f, cy = h / 2f;

        TechNode[] nodes = new TechNode[] {
            new TechNode { Name = "React", ThemeColor = Color.FromArgb(0, 216, 255), BaseAngle = 0 },
            new TechNode { Name = "Next.js", ThemeColor = Color.FromArgb(255, 255, 255), BaseAngle = 40 },
            new TechNode { Name = "Python", ThemeColor = Color.FromArgb(255, 212, 59), BaseAngle = 80 },
            new TechNode { Name = "Laravel", ThemeColor = Color.FromArgb(255, 45, 32), BaseAngle = 120 },
            new TechNode { Name = "Node.js", ThemeColor = Color.FromArgb(104, 160, 99), BaseAngle = 160 },
            new TechNode { Name = "AWS", ThemeColor = Color.FromArgb(255, 153, 0), BaseAngle = 200 },
            new TechNode { Name = "Docker", ThemeColor = Color.FromArgb(36, 150, 237), BaseAngle = 240 },
            new TechNode { Name = "Kubernetes", ThemeColor = Color.FromArgb(50, 108, 229), BaseAngle = 280 },
            new TechNode { Name = "PostgreSQL", ThemeColor = Color.FromArgb(51, 103, 145), BaseAngle = 320 }
        };

        for (int frame = 0; frame < totalFrames; frame++) {
            float progress = (float)frame / totalFrames;
            float rot = progress * (float)Math.PI * 2f;

            using (Bitmap bmp = new Bitmap(w, h))
            using (Graphics g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.Clear(Color.FromArgb(5, 5, 7));

                // 3D Perspective Grid at bottom
                DrawPerspectiveGrid(g, w, h);

                // Central red energy core
                using (GraphicsPath cp = new GraphicsPath()) {
                    cp.AddEllipse(cx - 35, cy - 35, 70, 70);
                    using (PathGradientBrush cpgb = new PathGradientBrush(cp)) {
                        cpgb.CenterPoint = new PointF(cx, cy);
                        cpgb.CenterColor = Color.FromArgb(220, 255, 32, 32);
                        cpgb.SurroundColors = new Color[] { Color.FromArgb(0, 5, 5, 7) };
                        g.FillPath(cpgb, cp);
                    }
                }
                using (Brush coreB = new SolidBrush(Color.White)) {
                    g.FillEllipse(coreB, cx - 6, cy - 6, 12, 12);
                }

                // Compute 3D positions for all nodes
                List<Tuple<float, Action>> drawList = new List<Tuple<float, Action>>();

                for (int i = 0; i < nodes.Length; i++) {
                    var n = nodes[i];
                    float nodeAngle = rot + n.BaseAngle * (float)Math.PI / 180f;

                    float radiusX = 320f;
                    float radiusZ = 120f;

                    float x3d = radiusX * (float)Math.Cos(nodeAngle);
                    float z3d = radiusZ * (float)Math.Sin(nodeAngle);
                    float y3d = (float)Math.Sin(nodeAngle * 2f) * 25f;

                    // Camera perspective projection
                    float cameraDist = 450f;
                    float scale = cameraDist / (cameraDist + z3d);
                    float screenX = cx + x3d * scale;
                    float screenY = cy + y3d * scale - 15f;

                    // Connect laser beam to core
                    using (Pen laserPen = new Pen(Color.FromArgb(Math.Max(20, (int)(70 * scale)), 255, 32, 32), 1f)) {
                        g.DrawLine(laserPen, cx, cy, screenX, screenY);
                    }

                    // Store draw action sorted by z3d (painter's algorithm)
                    TechNode capturedNode = n;
                    float capturedScale = scale;
                    float capturedSX = screenX;
                    float capturedSY = screenY;

                    drawList.Add(new Tuple<float, Action>(z3d, () => {
                        DrawTechNodeCard(g, capturedNode.Name, capturedNode.ThemeColor, capturedSX, capturedSY, capturedScale);
                    }));
                }

                // Sort back to front (highest z3d is furthest back)
                drawList.Sort((a, b) => b.Item1.CompareTo(a.Item1));
                foreach (var item in drawList) {
                    item.Item2();
                }

                string filePath = Path.Combine(outDir, string.Format("frame_{0:D4}.png", frame));
                bmp.Save(filePath, ImageFormat.Png);
            }
        }
    }

    private static void DrawTechNodeCard(Graphics g, string name, Color theme, float x, float y, float scale) {
        float cardW = 90f * scale;
        float cardH = 46f * scale;

        RectangleF rect = new RectangleF(x - cardW / 2, y - cardH / 2, cardW, cardH);

        // 3D Metallic Card Body
        using (LinearGradientBrush lgb = new LinearGradientBrush(
            rect, Color.FromArgb(220, 22, 22, 30), Color.FromArgb(240, 10, 10, 14), 90f)) {
            g.FillRectangle(lgb, rect);
        }

        // Glowing Tech Border
        int borderAlpha = Math.Min(255, (int)(200 * scale));
        using (Pen p = new Pen(Color.FromArgb(borderAlpha, theme), 1.5f * scale)) {
            g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
        }

        // Top Accent Neon Bar
        using (Brush ab = new SolidBrush(Color.FromArgb(255, theme))) {
            g.FillRectangle(ab, rect.X, rect.Y, rect.Width, 2.5f * scale);
        }

        // Text
        float fontSize = Math.Max(8f, 11f * scale);
        using (Font f = new Font("Segoe UI", fontSize, FontStyle.Bold))
        using (Brush tb = new SolidBrush(Color.White)) {
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(name, f, tb, rect, sf);
        }
    }

    private static void DrawPerspectiveGrid(Graphics g, int w, int h) {
        float horizonY = h / 2f + 50f;
        using (Pen gridPen = new Pen(Color.FromArgb(30, 255, 32, 32), 1f)) {
            // Horizontal perspective lines
            for (int i = 1; i <= 6; i++) {
                float y = horizonY + i * i * 3.5f;
                if (y < h) g.DrawLine(gridPen, 0, y, w, y);
            }
            // Vanishing lines to center
            for (int x = -400; x <= w + 400; x += 80) {
                g.DrawLine(gridPen, w / 2, horizonY, x, h);
            }
        }
    }

    // -------------------------------------------------------------
    // SCENE 4: 3D PROJECT SHOWCASE (36 frames, 840x400)
    // -------------------------------------------------------------
    public static void RenderProjectShowcase(string outDir) {
        Directory.CreateDirectory(outDir);
        int w = 840, h = 400;
        int totalFrames = 36;
        float cx = w / 2f, cy = h / 2f - 20f;

        string[] projectTitles = new string[] {
            "01. Image Scraper Pro",
            "02. Focus Tourism Eventz",
            "03. Mei Alai",
            "04. Namma Kanakku",
            "05. Leo AI Assistant",
            "06. ChatG",
            "07. GT Holidays Extractor",
            "08. Client Hunter",
            "09. PaperCollect",
            "10. Namma Ooru Quest"
        };

        string[] projectTags = new string[] {
            "React • Python • FastAPI",
            "Vite • Django • AWS",
            "React • Laravel • MySQL",
            "React Native • SQLite",
            "Python • Ollama • FastAPI",
            "React Native • NestJS • Redis",
            "FastAPI • Scraper • React",
            "Python • Streamlit • Pandas",
            "Expo • TypeScript • SQLite",
            "Unity • C# • 3D Runner"
        };

        int count = projectTitles.Length;

        for (int frame = 0; frame < totalFrames; frame++) {
            float progress = (float)frame / totalFrames;
            float rot = progress * (float)Math.PI * 2f;

            using (Bitmap bmp = new Bitmap(w, h))
            using (Graphics g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.Clear(Color.FromArgb(5, 5, 7));

                // 3D Reflection Floor
                DrawPerspectiveGrid(g, w, h);

                // Compute 3D cylinder positions for all 10 panels
                List<Tuple<float, Action>> panels = new List<Tuple<float, Action>>();

                for (int i = 0; i < count; i++) {
                    float panelAngle = rot + i * ((float)Math.PI * 2f / count);

                    float radiusX = 330f;
                    float radiusZ = 160f;

                    float px = radiusX * (float)Math.Sin(panelAngle);
                    float pz = radiusZ * (float)Math.Cos(panelAngle);

                    float cameraDist = 500f;
                    float scale = cameraDist / (cameraDist + pz);
                    float sx = cx + px * scale;
                    float sy = cy;

                    int capturedIdx = i;
                    float capturedScale = scale;
                    float capturedSX = sx;
                    float capturedSY = sy;
                    float capturedZ = pz;

                    panels.Add(new Tuple<float, Action>(pz, () => {
                        DrawProjectCard3D(g, projectTitles[capturedIdx], projectTags[capturedIdx], capturedSX, capturedSY, capturedScale, capturedZ < 0);
                    }));
                }

                // Sort back to front (highest pz is furthest away)
                panels.Sort((a, b) => b.Item1.CompareTo(a.Item1));
                foreach (var p in panels) {
                    p.Item2();
                }

                // Top Banner Indicator
                using (Font hFont = new Font("JetBrains Mono", 10, FontStyle.Bold))
                using (Brush hBrush = new SolidBrush(Color.FromArgb(255, 255, 32, 32))) {
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString("3D FEATURED PROJECTS CAROUSEL", hFont, hBrush, cx, 15, sf);
                }

                string filePath = Path.Combine(outDir, string.Format("frame_{0:D4}.png", frame));
                bmp.Save(filePath, ImageFormat.Png);
            }
        }
    }

    private static void DrawProjectCard3D(Graphics g, string title, string tag, float sx, float sy, float scale, bool isFront) {
        float cardW = 190f * scale;
        float cardH = 110f * scale;

        RectangleF rect = new RectangleF(sx - cardW / 2, sy - cardH / 2, cardW, cardH);

        // Card Fill
        int alpha = (int)(160 + (isFront ? 95 : 20));
        using (LinearGradientBrush lgb = new LinearGradientBrush(
            rect, Color.FromArgb(alpha, 25, 25, 35), Color.FromArgb(alpha, 10, 10, 15), 90f)) {
            g.FillRectangle(lgb, rect);
        }

        // Glowing Border (Brighter in front)
        Color borderColor = isFront ? Color.FromArgb(255, 255, 32, 32) : Color.FromArgb(100, 139, 0, 0);
        using (Pen p = new Pen(borderColor, (isFront ? 2f : 1f) * scale)) {
            g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
        }

        // Header strip
        using (Brush stripB = new SolidBrush(isFront ? Color.FromArgb(240, 255, 32, 32) : Color.FromArgb(120, 139, 0, 0))) {
            g.FillRectangle(stripB, rect.X, rect.Y, rect.Width, 3f * scale);
        }

        // Title text
        float titleSize = Math.Max(7.5f, 11f * scale);
        using (Font tf = new Font("Segoe UI", titleSize, FontStyle.Bold))
        using (Brush tb = new SolidBrush(isFront ? Color.White : Color.FromArgb(180, 200, 200, 200))) {
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            RectangleF tRect = new RectangleF(rect.X + 4, rect.Y + 15 * scale, rect.Width - 8, 40 * scale);
            g.DrawString(title, tf, tb, tRect, sf);
        }

        // Tag text
        float tagSize = Math.Max(6f, 8.5f * scale);
        using (Font tagF = new Font("JetBrains Mono", tagSize, FontStyle.Regular))
        using (Brush tagB = new SolidBrush(isFront ? Color.FromArgb(255, 120, 120) : Color.FromArgb(120, 150, 150, 150))) {
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            RectangleF tgRect = new RectangleF(rect.X + 4, rect.Y + 65 * scale, rect.Width - 8, 30 * scale);
            g.DrawString(tag, tagF, tagB, tgRect, sf);
        }
    }

    // -------------------------------------------------------------
    // SCENE 5: DEVELOPER JOURNEY (36 frames, 840x340)
    // -------------------------------------------------------------
    public static void RenderDeveloperJourney(string outDir) {
        Directory.CreateDirectory(outDir);
        int w = 840, h = 340;
        int totalFrames = 36;

        string[] steps = new string[] {
            "IDEA", "DESIGN", "DEVELOPMENT", "DATABASE", "DOCKER", "AWS", "DEPLOY"
        };
        int count = steps.Length;

        for (int frame = 0; frame < totalFrames; frame++) {
            float progress = (float)frame / totalFrames;

            using (Bitmap bmp = new Bitmap(w, h))
            using (Graphics g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.Clear(Color.FromArgb(5, 5, 7));

                // Background pipeline rail
                float startX = 60f;
                float endX = w - 60f;
                float stepW = (endX - startX) / (count - 1);

                PointF[] points = new PointF[count];
                for (int i = 0; i < count; i++) {
                    float px = startX + i * stepW;
                    float py = h / 2f + (float)Math.Sin(i * 0.9f + progress * Math.PI * 2f) * 20f;
                    points[i] = new PointF(px, py);
                }

                // Laser Backbone Line
                using (Pen laserGlow = new Pen(Color.FromArgb(80, 255, 32, 32), 6f)) {
                    g.DrawCurve(laserGlow, points, 0.4f);
                }
                using (Pen laserCore = new Pen(Color.FromArgb(255, 255, 255, 255), 1.5f)) {
                    g.DrawCurve(laserCore, points, 0.4f);
                }

                // Moving Energy Pulse traveling across pipeline
                float pulseIndex = (progress * (count - 1));
                int baseIdx = (int)Math.Floor(pulseIndex);
                int nextIdx = Math.Min(count - 1, baseIdx + 1);
                float frac = pulseIndex - baseIdx;

                float pulseX = points[baseIdx].X + (points[nextIdx].X - points[baseIdx].X) * frac;
                float pulseY = points[baseIdx].Y + (points[nextIdx].Y - points[baseIdx].Y) * frac;

                // Draw pulse glow
                using (Brush pulseB = new SolidBrush(Color.FromArgb(255, 255, 32, 32))) {
                    g.FillEllipse(pulseB, pulseX - 12, pulseY - 12, 24, 24);
                }
                using (Brush pulseCore = new SolidBrush(Color.White)) {
                    g.FillEllipse(pulseCore, pulseX - 5, pulseY - 5, 10, 10);
                }

                // Draw 3D Milestone Nodes
                for (int i = 0; i < count; i++) {
                    PointF pt = points[i];
                    bool isLit = Math.Abs(pulseIndex - i) < 0.8f;

                    // 3D Hexagonal / Rounded Node Pill
                    float nodeW = 90f;
                    float nodeH = 50f;
                    RectangleF nr = new RectangleF(pt.X - nodeW / 2, pt.Y - nodeH / 2, nodeW, nodeH);

                    using (LinearGradientBrush ngb = new LinearGradientBrush(
                        nr, Color.FromArgb(240, 20, 20, 26), Color.FromArgb(240, 10, 10, 14), 90f)) {
                        g.FillRectangle(ngb, nr);
                    }

                    Color bCol = isLit ? Color.FromArgb(255, 255, 32, 32) : Color.FromArgb(140, 139, 0, 0);
                    using (Pen np = new Pen(bCol, isLit ? 2f : 1.2f)) {
                        g.DrawRectangle(np, nr.X, nr.Y, nr.Width, nr.Height);
                    }

                    // Top strip
                    using (Brush topB = new SolidBrush(isLit ? Color.FromArgb(255, 255, 40, 40) : Color.FromArgb(180, 139, 0, 0))) {
                        g.FillRectangle(topB, nr.X, nr.Y, nr.Width, 3f);
                    }

                    // Number badge
                    using (Font numF = new Font("JetBrains Mono", 8, FontStyle.Bold))
                    using (Brush numB = new SolidBrush(Color.FromArgb(255, 255, 50, 50))) {
                        g.DrawString(string.Format("0{0}", i + 1), numF, numB, nr.X + 6, nr.Y + 6);
                    }

                    // Title
                    using (Font stFont = new Font("Segoe UI", 8.5f, FontStyle.Bold))
                    using (Brush stBrush = new SolidBrush(Color.White)) {
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        RectangleF textR = new RectangleF(nr.X, nr.Y + 12, nr.Width, nr.Height - 12);
                        g.DrawString(steps[i], stFont, stBrush, textR, sf);
                    }
                }

                // Title
                using (Font hf = new Font("JetBrains Mono", 10, FontStyle.Bold))
                using (Brush hb = new SolidBrush(Color.FromArgb(255, 255, 32, 32))) {
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString("3D DEVELOPER PIPELINE PROGRESSION", hf, hb, w / 2, 18, sf);
                }

                string filePath = Path.Combine(outDir, string.Format("frame_{0:D4}.png", frame));
                bmp.Save(filePath, ImageFormat.Png);
            }
        }
    }

    // -------------------------------------------------------------
    // SCENE 6: CINEMATIC FOOTER (36 frames, 840x260)
    // -------------------------------------------------------------
    public static void RenderFooterAnimation(string outDir) {
        Directory.CreateDirectory(outDir);
        int w = 840, h = 260;
        int totalFrames = 36;
        float planetCx = w / 2f;
        float planetCy = h + 130f;
        float planetR = 250f;

        for (int frame = 0; frame < totalFrames; frame++) {
            float progress = (float)frame / totalFrames;
            float rot = progress * (float)Math.PI * 2f;

            using (Bitmap bmp = new Bitmap(w, h))
            using (Graphics g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.Clear(Color.FromArgb(5, 5, 7));

                // 1. Horizon Red Atmosphere Glow
                using (GraphicsPath hp = new GraphicsPath()) {
                    hp.AddEllipse(planetCx - planetR - 80, planetCy - planetR - 80, (planetR + 80) * 2, (planetR + 80) * 2);
                    using (PathGradientBrush hpgb = new PathGradientBrush(hp)) {
                        hpgb.CenterPoint = new PointF(planetCx, planetCy);
                        hpgb.CenterColor = Color.FromArgb(70, 255, 32, 32);
                        hpgb.SurroundColors = new Color[] { Color.FromArgb(0, 5, 5, 7) };
                        g.FillPath(hpgb, hp);
                    }
                }

                // 2. 3D Planet Body
                using (GraphicsPath pp = new GraphicsPath()) {
                    pp.AddEllipse(planetCx - planetR, planetCy - planetR, planetR * 2, planetR * 2);
                    using (LinearGradientBrush plgb = new LinearGradientBrush(
                        new PointF(planetCx, planetCy - planetR), new PointF(planetCx, planetCy),
                        Color.FromArgb(255, 139, 0, 0), Color.FromArgb(255, 8, 8, 12))) {
                        g.FillPath(plgb, pp);
                    }
                }

                // 3. Rotating 3D Latitude/Longitude Wireframe Lines
                using (Pen wirePen = new Pen(Color.FromArgb(70, 255, 80, 80), 1f)) {
                    for (int longIdx = 0; longIdx < 8; longIdx++) {
                        float lAngle = rot + longIdx * ((float)Math.PI / 8f);
                        float spanW = planetR * (float)Math.Cos(lAngle);
                        if (Math.Abs(spanW) > 5) {
                            g.DrawEllipse(wirePen, planetCx - Math.Abs(spanW), planetCy - planetR, Math.Abs(spanW) * 2, planetR * 2);
                        }
                    }
                }

                // 4. Glowing Horizon Laser Rim
                using (Pen rimPen = new Pen(Color.FromArgb(255, 255, 50, 50), 2.5f)) {
                    g.DrawArc(rimPen, planetCx - planetR, planetCy - planetR, planetR * 2, planetR * 2, 195, 150);
                }

                // 5. Rising Red Embers
                Random emberRnd = new Random(100);
                for (int e = 0; e < 35; e++) {
                    float ex = (float)(emberRnd.NextDouble() * (w - 100) + 50);
                    float ey = (float)(emberRnd.NextDouble() * h);
                    float eOffset = (progress + (float)emberRnd.NextDouble()) % 1.0f;
                    float curY = h - eOffset * 200f;
                    using (Brush eb = new SolidBrush(Color.FromArgb((int)(180 * (1.0f - eOffset)), 255, 40, 40))) {
                        g.FillEllipse(eb, ex, curY, 3, 3);
                    }
                }

                // 6. Central Brand Ethos
                using (Font mainF = new Font("Segoe UI", 18, FontStyle.Bold))
                using (Brush mainB = new SolidBrush(Color.White)) {
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString("ENGINEERING A BETTER TOMORROW", mainF, mainB, w / 2, 45, sf);
                }

                using (Font mottoF = new Font("JetBrains Mono", 12, FontStyle.Bold))
                using (Brush mottoB = new SolidBrush(Color.FromArgb(255, 255, 32, 32))) {
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString("BUILD  •  LEARN  •  IMPROVE  •  REPEAT", mottoF, mottoB, w / 2, 85, sf);
                }

                using (Font subF = new Font("Segoe UI", 11, FontStyle.Italic))
                using (Brush subB = new SolidBrush(Color.FromArgb(160, 200, 200, 200))) {
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString("\"Turning Ideas Into Real Products\"", subF, subB, w / 2, 115, sf);
                }

                string filePath = Path.Combine(outDir, string.Format("frame_{0:D4}.png", frame));
                bmp.Save(filePath, ImageFormat.Png);
            }
        }
    }
}
