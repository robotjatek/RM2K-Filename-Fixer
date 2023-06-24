# RM2K Filename Fixer

Ezzel a tool-lal az RPG Maker 2000-rel magyar nyelvű operációrendszeren készült játékok fájneveit lehet megkísérelni megjavítani.

Sajnos gyakori probléma nem magyar nyelvű operációs rendszereken, hogy a játék Ő karakter helyett Õ-t keres a fájlnevekben. Egy másik előjövő probléma kitömörítésnél jelentkezik, ekkor az Ő-kből rendre Ï karakterek lesznek, miközben a játék továbbra is Õ-t keres. Elvileg magyar nyelvű rendszerken az **Õ => Ő**  egyeztetéssel nem szokott probléma lenni. Ez a tool egy konfigurációs tábla alapján megkeresi a hibás fájlneveket és kísérletet tesz a javításukra.

Használata: Az `exe` és `json` fájt a játék főkönyvtárába másolva az `RMFileNameFixer.exe`-t elindítani.

## Konfiguráció

A tool json fájl segítségével konfigurálható.

A `locale` kulcsban megadható, hogy adott OS nyelvhez milyen karaktercseréket hajtson végre a program.

```json
"locale": {
    "en-US": {
      "ï": "õ",
      "ő": "õ",
      "Ï": "Õ",
      "Ő": "Õ"
    },
    "hu-HU": {
    }
  }
```

A fenti példa jelentése: `en-US` nyelvi beállításoknál az Ï és Ő karaktereket cserélje Õ-re.

A `fallback_locale` kulcsban megadható, hogy mi legyen az alapértelmezetten használt konfiguráció, ha az operációs rendszer olyan nyelvi beállításokat használ, ami nincs definiálva a konfigurációban.

```json
"fallback_locale": "en-US",
```

Az `rm_folders` kulcsban megadható, hogy melyik mappákban keressen a program.

```json
"rm_folders": [
    "Backdrop",
    "Battle",
    "CharSet",
    "ChipSet",
    "FaceSet",
    "GameOver",
    "Monster",
    "Movie",
    "Music",
    "Panorama",
    "Picture",
    "Sound",
    "System",
    "Title"
  ]
```

## Visszaállítás funkció

A program megjegyzi a megváltoztatott fájlneveket, hogy egy esetleges sikertelen javítás után a fájlok visszaállíthatóak legyenek. Ehhez konzolból indítva a programot a `restore` paraméter hozzáadásával lehet helyreállítani az eredeti állapotot.

```
C:\Games\RPG_GAME> RMFileNameFixer.exe restore
```

## Követelmények

- dotNET7
- Windows