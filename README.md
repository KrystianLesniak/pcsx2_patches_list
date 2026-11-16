# PCSX2 Patches List

![Last commit](https://img.shields.io/github/last-commit/KrystianLesniak/pcsx2_patches_list)

**Does my game have a widescreen patch in PCSX2?** This site answers that in a few seconds.

PCSX2 ships with thousands of game patches: widescreen fixes, no-interlacing, 50/60 FPS unlocks and more.
They power options like "Enable Widescreen Patches" and "Enable No-Interlacing Patches", but the emulator
doesn't show which games they cover. This list does.

## [🔎 Open the patches list](https://krystianlesniak.github.io/pcsx2_patches_list/markdown_patches.html)

- **Search** by game title, game code (e.g. `SLUS-21269`), CRC or patch description
- **Filter** by patch type (Widescreen, No-Interlace, 50/60 FPS) and region (NTSC-U, PAL, NTSC-J, NTSC-K, Asia)
- **Sort** any column, and **share** a search: filters are kept in the link
- **Download** the `.pnach` file of any patch with one click
- Works on phones too

## How it works

A GitHub Actions workflow runs every day. It pulls the latest
[PCSX2/pcsx2_patches](https://github.com/PCSX2/pcsx2_patches), reads every `.pnach` file with a small .NET app
(`src/`) and regenerates [`markdown_patches.md`](markdown_patches.md). GitHub Pages then publishes it, so the list
is never more than a day behind PCSX2.

## Run it locally

Regenerate the list (needs the .NET 10 SDK):

```bash
git submodule update --init pcsx2_patches
dotnet run --project src/PatchesList.ConsoleApp
```

Preview the site (needs Docker), then open http://localhost:4000/markdown_patches.html:

```bash
docker run --rm -it -p 4000:4000 -v "${PWD}:/srv/site" -w /srv/site ruby:3.3 bash -c "gem install github-pages webrick --no-document && echo 'plugins: [jekyll-remote-theme, jekyll-optional-front-matter, jekyll-seo-tag, jekyll-feed]' > /tmp/local.yml && jekyll serve --host 0.0.0.0 --config _config.yml,/tmp/local.yml"
```

## Credits

All patches come from the PCSX2 team and the patch authors credited in
[PCSX2/pcsx2_patches](https://github.com/PCSX2/pcsx2_patches). This repo only lists them.
