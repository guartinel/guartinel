# Csomagok

Csomagok a rendszer alapvető építő elemei. Magukba foglalják a figyelendő objektumot és annak a tulajdonságait.

 
## Csomag típusok
+ Weboldal  - Egy weboldal elérhetőségét, betöltési sebességét, tanúsítvány érvényességét és egy keresett szó tartalmazását vizsgálhatja.
+ Hoszt  -  Egy szerver elérhetőségét (ICMP ping) vagy egy port nyitott állapotát ellenőrizheti (TCP)
+ Hardver - Páratartalmat, hőmérsékletet, feszültséget, átfolyó áramot vagy víz jelenlétet érzékelhet.
+ Email - Egy email szerver levél küldési és fogadási képességét tudja ellenőrizni.
+ Alkalmazás - Egy szerver belső állapotáról tudunk képet alkotni a CLI alkalmazásunk segítségével.

## Weboldal felügyelő

    mkdocs.yml    # The configuration file.
    docs/
        index.md  # The documentation homepage.
        ...       # Other markdown pages, images and other files.
