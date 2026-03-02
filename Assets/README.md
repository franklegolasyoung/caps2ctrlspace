# App Icon

Place your `app.ico` file here.

You can create one at:
- https://icon.kitchen (generate from text/emoji)
- https://realfavicongenerator.net

Recommended: Use a simple "⌨" or "Caps" design with 16x16, 32x32, 48x48, 256x256 sizes.

For quick testing, you can generate one with ImageMagick:
```bash
magick -size 256x256 xc:"#0078D4" -fill white -font Arial-Bold -pointsize 120 -gravity center -annotate 0 "C" app.ico
```
